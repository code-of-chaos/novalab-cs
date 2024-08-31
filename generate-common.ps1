# ENV variables required by the various docker systems
#       No-external ones are set to a default one
$envVariables = @{
    "SqlServerSAPassword"             = "pa55w0rd!"
    "SslCertPassword"                 = "pa55w0rd!" # If you change this one, then you have to change it in generate-certs.ps1 as well
    "TwitchClientId"                  = $null
    "TwitchClientSecret"              = $null
}
# -----------------------------------------------------------------------------------------------------------------
# Code
# -----------------------------------------------------------------------------------------------------------------
function getBoolResponse ($question) {
    do {
        $response = (Read-Host -Prompt $question).ToUpper()[0]
    }
    while ($response -ne 'Y' -and $response -ne 'N')

    return $response
}

function main(){
    $useDefaults = getBoolResponse "Automatically use all defaults? (Yes/No)"

    $envVariablesCopy = $envVariables.Clone()
    foreach ($name in $envVariablesCopy.Keys) {
        if(($envVariablesCopy[$name] -eq $null) -or ($useDefaults -eq 'N')){
            $value = Read-Host -Prompt "Set value for $name :"
            $envVariables[$name] = $value
        }
    }

    # Name of the .env file
    $envFileName = ".env"
    if (Test-Path $envFileName) {
        Remove-Item -Path $envFileName
    }

    # Loop through each variable in the HashTable and add to .env file
    foreach ($entry in $envVariables.GetEnumerator()) {
        Add-Content -Path $envFileName -Value ("{0}={1}" -f $entry.Key, $entry.Value)
    }

    Write-Output "Successfully created .env files, with the following variables: $($envVariables.Keys -join ', ')"

    # Generate the Development certificates for the various systems which need it
    #      The default password is also used in the launch configurations
    echo "Creating NovaLab.Servers.Blazor.pfx ..."
    dotnet dev-certs https --trust -ep $env:USERPROFILE/.aspnet/https/NovaLab.Servers.Blazor.pfx -p $envVariables["SslCertPassword"]

    echo "Creating NovaLab.Servers.API.pfx ..."
    dotnet dev-certs https --trust -ep $env:USERPROFILE/.aspnet/https/NovaLab.Servers.API.pfx -p $envVariables["SslCertPassword"]
    
    echo "Successfully created .pfx files"
}

# Run the main
main 