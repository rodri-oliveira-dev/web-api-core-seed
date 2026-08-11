[CmdletBinding()]
param(
    [switch]$Check,
    [switch]$Force
)

$ErrorActionPreference = "Stop"

$expectedHooksPath = ".githooks"
$requiredHook = "pre-push"

function Invoke-Git {
    param([Parameter(Mandatory = $true)][string[]]$Arguments)

    $output = & git @Arguments 2>$null
    return @{
        ExitCode = $LASTEXITCODE
        Output = ($output -join "`n")
    }
}

$insideWorkTree = Invoke-Git -Arguments @("rev-parse", "--is-inside-work-tree")
if ($insideWorkTree.ExitCode -ne 0) {
    [Console]::Error.WriteLine("Error: current directory is not inside a Git repository.")
    exit 1
}

$repoRootResult = Invoke-Git -Arguments @("rev-parse", "--show-toplevel")
if ($repoRootResult.ExitCode -ne 0 -or [string]::IsNullOrWhiteSpace($repoRootResult.Output)) {
    [Console]::Error.WriteLine("Error: could not identify repository root.")
    exit 1
}

$repoRoot = $repoRootResult.Output.Trim()
$hooksDir = Join-Path $repoRoot $expectedHooksPath
$hookPath = Join-Path $hooksDir $requiredHook
$currentResult = Invoke-Git -Arguments @("-C", $repoRoot, "config", "--local", "--get", "core.hooksPath")
$currentHooksPath = if ($currentResult.ExitCode -eq 0) { $currentResult.Output.Trim() } else { "" }

function Test-HookFile {
    if (-not (Test-Path -LiteralPath $hooksDir -PathType Container)) {
        [Console]::Error.WriteLine("Error: hooks directory not found: $expectedHooksPath.")
        return $false
    }

    if (-not (Test-Path -LiteralPath $hookPath -PathType Leaf)) {
        [Console]::Error.WriteLine("Error: required hook not found: $expectedHooksPath/$requiredHook.")
        return $false
    }

    return $true
}

if ($Check) {
    $status = 0

    if ($currentHooksPath -eq $expectedHooksPath) {
        Write-Host "OK: core.hooksPath is configured as $expectedHooksPath."
    }
    else {
        $actual = if ([string]::IsNullOrEmpty($currentHooksPath)) { "<unset>" } else { $currentHooksPath }
        [Console]::Error.WriteLine("Error: core.hooksPath is '$actual', expected '$expectedHooksPath'.")
        $status = 1
    }

    if (-not (Test-HookFile)) {
        $status = 1
    }

    exit $status
}

if (-not (Test-HookFile)) {
    exit 1
}

if ([string]::IsNullOrEmpty($currentHooksPath)) {
    & git -C $repoRoot config --local core.hooksPath $expectedHooksPath
    if ($LASTEXITCODE -ne 0) {
        [Console]::Error.WriteLine("Error: failed to configure local core.hooksPath.")
        exit 1
    }

    Write-Host "Configured local core.hooksPath=$expectedHooksPath."
    Write-Host "Remove with: git config --local --unset core.hooksPath"
    exit 0
}

if ($currentHooksPath -eq $expectedHooksPath) {
    Write-Host "Local core.hooksPath is already configured as $expectedHooksPath."
    Write-Host "Remove with: git config --local --unset core.hooksPath"
    exit 0
}

if (-not $Force) {
    [Console]::Error.WriteLine("Error: local core.hooksPath already points to '$currentHooksPath'.")
    [Console]::Error.WriteLine("Re-run with -Force only if you want to replace it with '$expectedHooksPath'.")
    exit 1
}

& git -C $repoRoot config --local core.hooksPath $expectedHooksPath
if ($LASTEXITCODE -ne 0) {
    [Console]::Error.WriteLine("Error: failed to configure local core.hooksPath.")
    exit 1
}

Write-Host "Replaced local core.hooksPath."
Write-Host "Previous value: $currentHooksPath"
Write-Host "New value: $expectedHooksPath"
Write-Host "Remove with: git config --local --unset core.hooksPath"
