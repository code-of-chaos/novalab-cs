# -----------------------------------------------------------------------------------------------------------------
# Tweakable variables
# -----------------------------------------------------------------------------------------------------------------
$Language = "CSharp"
$NameSpace = "NovaLab.ApiClient"
$OutputFolder = "./src/lib/NovaLab.ApiClient/"
$ClassName = "NovaLabApiClient"
$OpenApiFile = "https://localhost:7145/swagger/v1/swagger.json"

$CsprojPath = "./src/lib/NovaLab.ApiClient/NovaLab.ApiClient.csproj"
$TempCsprojPath = "./.temp/NovaLab.ApiClient.csproj"

# -----------------------------------------------------------------------------------------------------------------
# Code
# -----------------------------------------------------------------------------------------------------------------
function Copy-Csproj {
  if (Test-Path -Path $CsprojPath) {
    Copy-Item -Path $CsprojPath -Destination $TempCsprojPath
  }
}

function Restore-Csproj {
  cd "../../../"
  if (-not (Test-Path -Path $CsprojPath) -and (Test-Path -Path $TempCsprojPath)) {
    Move-Item -Path $TempCsprojPath -Destination $CsprojPath
  }
}

function Test-OpenApiFile {
  if ($OpenApiFile -match '^(http|https)://') {
    try {
      $response = Invoke-WebRequest -Uri $OpenApiFile -UseBasicParsing
      return $response.StatusCode -eq "200" -or 200
    } catch {
      echo "Error accessing $OpenApiFile"
      return $false
    }
  } else {
    return Test-Path -Path $OpenApiFile
  }
}

function main {
  #  Before we run anything, check if the OpenApiFile actually exists
  echo "Checking if $OpenApiFile is accessible..."
  if (-not (Test-OpenApiFile)) {
    echo "Failed to access $OpenApiFile. Aborting."
    exit 
  }
  
  echo "Copying .csproj file to temporary location..."
  Copy-Csproj

  echo "Generating OpenAPI client with Kiota..."
  cd $OutputFolder
  kiota generate `
    --openapi $OpenApiFile `
    --language $Language `
    --namespace-name $NameSpace `
    --backing-store false `
    --class-name $ClassName `
    --output ./ `
    --exclude-backward-compatible `
    --clean-output `
    --clear-cache
  
  echo "Restoring .csproj file if it no longer exists..."
  Restore-Csproj
  echo "Finished"
}

main