$ErrorActionPreference = "Stop"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Resolve-Path (Join-Path $scriptDir "..\..")
$apiProject = Join-Path $repoRoot "src\WebApiCoreSeed.Api\WebApiCoreSeed.Api.csproj"

if (-not (Test-Path $apiProject)) {
    throw "API project not found: $apiProject"
}

$userSecretsId = dotnet msbuild $apiProject -getProperty:UserSecretsId
if ([string]::IsNullOrWhiteSpace($userSecretsId)) {
    throw "UserSecretsId is not configured for the API project. No new ID was generated."
}

function ConvertTo-PlainText {
    param([Parameter(Mandatory = $true)][System.Security.SecureString]$SecureValue)

    $bstr = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($SecureValue)
    try {
        [Runtime.InteropServices.Marshal]::PtrToStringBSTR($bstr)
    }
    finally {
        [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($bstr)
    }
}

function Read-RequiredSecret {
    param([Parameter(Mandatory = $true)][string]$Prompt)

    $secureValue = Read-Host -Prompt $Prompt -AsSecureString
    $plainValue = ConvertTo-PlainText $secureValue
    if ([string]::IsNullOrWhiteSpace($plainValue)) {
        throw "Value cannot be empty."
    }

    $plainValue
}

Write-Host "Configuring User Secrets for src/WebApiCoreSeed.Api/WebApiCoreSeed.Api.csproj"
$connectionString = Read-RequiredSecret "ConnectionStrings:DefaultConnection"
$jwtSecret = Read-RequiredSecret "AppSettings:Secret"
$developmentSeedPassword = Read-RequiredSecret "DevelopmentSeed:User:Password"

dotnet user-secrets set "ConnectionStrings:DefaultConnection" $connectionString --project $apiProject | Out-Null
dotnet user-secrets set "AppSettings:Secret" $jwtSecret --project $apiProject | Out-Null
dotnet user-secrets set "DevelopmentSeed:User:Password" $developmentSeedPassword --project $apiProject | Out-Null

Write-Host "User Secrets configured without printing secret values."
