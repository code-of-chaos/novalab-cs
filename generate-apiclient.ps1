# -----------------------------------------------------------------------------------------------------------------
# Tweakable variables
# -----------------------------------------------------------------------------------------------------------------
$Language = "CSharp"
$NameSpace = "NovaLab.ApiClient"
$OutputFolder = "./src/NovaLab.ApiClient/"
$ClassName = "NovaLabApiClient"

# -----------------------------------------------------------------------------------------------------------------
# Code
# -----------------------------------------------------------------------------------------------------------------
function main {
  echo "Generating OpenAPI client ..."
  cd "./src/NovaLab.ApiClient/"
  kiota generate `
    --openapi ..\NovaLab.API\swagger.json `
    --language $Language `
    --namespace-name $NameSpace `
    --backing-store false `
    --class-name $ClassName `
    --output ./
  
  echo "Finished"
}

main