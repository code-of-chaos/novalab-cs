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
  echo "Generating OpenAPI client with Kiota ..."
  cd "./src/lib/NovaLab.ApiClient/"
  kiota generate `
    --openapi ..\..\servers\api\NovaLab.Servers.API\swagger.json `
    --language $Language `
    --namespace-name $NameSpace `
    --backing-store false `
    --class-name $ClassName `
    --output ./
  
  echo "Finished"
}

main