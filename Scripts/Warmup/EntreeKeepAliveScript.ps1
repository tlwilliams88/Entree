#
# Script to keep the Entree services active and awake to reduce warmup times
#
# Parameters:
#  
# -InternalService <string> - Set the internal service URL to ping
# -WebApi <string>          - Set the WebApi URL to ping
# -Foundation <string>      - Set the Foundation URL to ping
#
# Optionally you can set the defaults for these values
#
# Example:
# powerhsell.exe
# ^ Task to run in scheduler
#
# -File "c:\path\to\script\EntreeKeepAliveScript.ps1" -InternalService http://internalserviceurl:port/ETLService.svc -WebApi http://webapiurl:port/profile -Foundation http://foundationserviceurl:port/Foundation/FoundationService.svc
# ^ Parameter/Argument examples to use in scheduler
#
# powershell.exe -File "c:\path\to\script\EntreeKeepAliveScript.ps1" -InternalService http://internalserviceurl:port/ETLService.svc -WebApi http://webapiurl:port/profile -Foundation http://foundationserviceurl:port/Foundation/FoundationService.svc
# ^ full example of executed command
#


# Accept the parameters passed in
param([String]$InternalService="http://localhost:1317/ETLService.svc",
      [String]$WebApi="http://localhost:60606/profile",
      [String]$Foundation="http://localhost:1000/Foundation/FoundationService.svc")



# Keep internal service awake
Invoke-WebRequest -Method GET $InternalService

# Keep WebApi awake
Invoke-WebRequest -Method GET $WebApi

# Keep Foundation service awake
Invoke-WebRequest -Method GET $Foundation
