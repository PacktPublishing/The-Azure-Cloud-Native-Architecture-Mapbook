This code is for illustration purposes only.

Note: the Dapr example was developed using Dapr's runtime version 0.11.3 and CLI v0.11.0. As you know, the cloud is a moving target and Dapr even more. If you want to run the sample in the same conditions as the book, you should make sure to download the latest dapr CLI and initialize the installation the following way:
    
    dapr init --runtime-version 0.11.3


Since the CLI also has some breaking changes, you should NOT invoke the sample commands as explained in the book:


    dapr run --app-id order --components-path ./components --app-port 5002 --port 3503 dotnet run

But uou should rather get rid of the --port flag which does not exist anymore. Dapr will find a port by itself. So, the above command should simply be:

    dapr run --app-id order --components-path ./components --app-port 5002 dotnet run

For the order query service, you should start it this way:

    dapr run --app-id orderquery --components-path ./components --app-port 5000 dotnet run

If you want to use the latest runtime, you'll have to update the provided code as Dapr made many breaking changes. Because it is in constant movement, we recommend you to follow directly the official Dapr repo and direct guidance for .NET: https://github.com/dapr/dotnet-sdk
