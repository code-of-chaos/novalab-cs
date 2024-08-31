# -----------------------------------------------------------------------------------------------------------------
# Tweakable variables
# -----------------------------------------------------------------------------------------------------------------
$Language = "CSharp"
$NameSpace = "NovaLab.ApiClient"
$OutputFolder = "./src/lib/NovaLab.ApiClient/"
$ClassName = "NovaLabApiClient"

# -----------------------------------------------------------------------------------------------------------------
# Code
# -----------------------------------------------------------------------------------------------------------------
function main {
  echo "Generating OpenAPI client ..."
  cd "./src/lib/NovaLab.ApiClient/"
  kiota generate `
    --openapi ..\..\server\api\NovaLab.Server.API\swagger.json `
    --language $Language `
    --namespace-name $NameSpace `
    --backing-store false `
    --class-name $ClassName `
    --output ./
  
  echo "Finished"
}

main