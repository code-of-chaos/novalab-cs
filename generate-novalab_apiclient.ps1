# Define variables
$generatorLanguage = "csharp"
$specFilePath = "src\NovaLab.API\swagger.json"
$outputFolder = "..\0005-01-cs-novalab-api_client"
$packageName = "NovaLab.ApiClient"

$netCoreProjectFile = "true"
$targetFramework = "net8.0"
$nullableReferenceTypes = "true"

# Run command
function main {
  echo "Generating OpenAPI client ..."
  openapi-generator-cli generate `
    -g $generatorLanguage `
    -i $specFilePath `
    -o $outputFolder `
    --additional-properties=packageName=$packageName `
    --additional-properties=netCoreProjectFile=$netCoreProjectFile `
    --additional-properties=targetFramework=$targetFramework `
    --additional-properties=nullableReferenceTypes=$nullableReferenceTypes

  # Define variables
  $sourceFolder = "..\0005-01-cs-novalab-api_client\src\NovaLab.ApiClient"
  $destinationFolder = "src\NovaLab.ApiClient"

  # Remove all items in the destination folder
  echo "Cleaning destination Folder ..."
  Remove-Item -Path "$destinationFolder\*" -Recurse -Force

  # Copy files
  echo "Copying to original folder ..."
  Copy-Item -Path "$sourceFolder\*" -Destination $destinationFolder -Recurse -Force


  echo "Finished"
}

main