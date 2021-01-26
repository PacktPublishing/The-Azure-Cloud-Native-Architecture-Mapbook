This code is for illustration purposes only.

Note: the Dapr example was developed using Dapr 0.11.3 but as you know, the cloud is a moving target and Dapr even more. If you use a more recent version of Dapr such as:

CLI version: 1.0.0-rc.3
Runtime version: 0.11.3

You should upgrade the nuget package references in the solution to also use the last version.

The CLI also has some breaking changes. In the book, you will see a series of dapr run commands, like the following:

    dapr run --app-id order --components-path ./components --app-port 5002 --port 3503 dotnet run

You should get rid of the --port flag which does not exist anymore. Dapr will find a port by itself. So, the above command should simply be:

    dapr run --app-id order --components-path ./components --app-port 5002 dotnet run
