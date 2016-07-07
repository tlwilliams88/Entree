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
