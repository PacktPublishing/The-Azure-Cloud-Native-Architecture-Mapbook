using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace PacktOrchestrationDemo
{  
    public static class Orchestration
    {
[FunctionName("StartOrchestration")]
public static async Task<HttpResponseMessage> StartOrchestration(
    [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestMessage req,
    [DurableClient] IDurableOrchestrationClient starter,
    ILogger log)
{
    //real world ==> https & API/Function Key or through API gateway with JWT validation for callback URL                
    string instanceId = await starter.StartNewAsync("MainOrchestrator",
        JsonConvert.DeserializeObject<OrchestrationConfiguration>(
        await req.Content.ReadAsStringAsync()));
    return starter.CreateCheckStatusResponse(req, instanceId);
}

[FunctionName("MainOrchestrator")]
public static async Task MainOrchestrator(
    [OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
{
           
    OrchestrationConfiguration process = context.GetInput<OrchestrationConfiguration>();
    //copy callback URL when demoing this   
    
    foreach (OrchestrationStep step in process.steps)
    {
                
        var retryOptions = new RetryOptions(
            firstRetryInterval: TimeSpan.FromSeconds(5),
            maxNumberOfAttempts: (step.maxRetryCount>0)? step.maxRetryCount+1:1);
               
        try
        {
            step.timeOut = (step.timeOut == 0) ? process.defaultStepTimeOut : step.timeOut;
            var result = await context.CallSubOrchestratorWithRetryAsync<bool>(
                nameof(SingleStepOrchestration),
                retryOptions, step);
        }
        catch
        {
            if (!context.IsReplaying)
            {
                log.LogWarning("-------step {0} failed-----------------", step.stepName);
                if (step.stopFlowOnFailure)
                    break;
            }
        }
    }

    log.LogInformation("------------------ORCHESTRATION IS OVER-----------------");      
}

[FunctionName(nameof(SingleStepOrchestration))]
public static async Task<bool> SingleStepOrchestration(
    [OrchestrationTrigger] IDurableOrchestrationContext context,
    ILogger log)
{   
    try
    {
        var step = context.GetInput<OrchestrationStep>();              
               
        log.LogInformation("------------------CREATING ACI for {0} on instance {1} -----------------", 
            step.stepName, context.InstanceId);
        log.LogInformation("------------------CALLBACK URL http://localhost:7071/api/stepResultCallback?instanceId={0}&eventName={1}&status= -----------------",
            context.InstanceId, step.stepName);
        log.LogInformation("------------------WAITING FOR EVENT {0} on instance {1}-----------------", step.stepName, context.InstanceId);               
               
        var state = await context.WaitForExternalEvent<string>(
        step.stepName, TimeSpan.FromMinutes(step.timeOut));
        //catch timeout
        log.LogInformation(
            "------------------STEP {0} INSTANCE IS {1} STATE IS {2}-----------------",
            step.stepName, context.InstanceId, state);

        if (state != "ok")
        {
            log.LogInformation("------------------state NOK-------------------");
            throw new ApplicationException();
        }
                    
    }
    catch(TimeoutException)
    {
        log.LogInformation("------------------TIMEOUT-------------------");
        throw;
    }
    finally
    {                
        log.LogInformation("------------------deleting ACI-------------------");                
    }
    return true;
}       
        
        //this one could also be bound to a queue where the ACI would report its state.
        //real world: function would be behind an API gateway
        [FunctionName("stepResultCallback")]
        public static async Task stepResultCallback(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient cli,
            ILogger log)
        {
            string eventName = req.RequestUri.ParseQueryString().Get("eventName");
            string status = req.RequestUri.ParseQueryString().Get("status");
            log.LogInformation($"RECEIVED FEEDBACK FROM {eventName} WITH STATUS {status}");
            await cli.RaiseEventAsync(req.RequestUri.ParseQueryString().Get("instanceId"), eventName, status);
        }


    }
public class OrchestrationConfiguration
{
    public bool isRewindable { get; set; }
    public int defaultStepTimeOut { get; set; }
    public List<OrchestrationStep> steps { get; set; }
}

public class OrchestrationStep
{        
    internal string instanceId { get; set; }
    public string stepName { get; set; }
    public int maxRetryCount { get; set; }
    public bool stopFlowOnFailure { get; set; }
    public int timeOut { get; set; }        
}
}