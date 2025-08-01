#!/usr/bin/env pwsh

# CloakId Benchmarks Runner
# Usage: .\run-benchmarks.ps1 [filter] [--dry] [--help]
# Examples:
#   .\run-benchmarks.ps1                           # Run all benchmarks
#   .\run-benchmarks.ps1 "*Encode*"                # Run only encoding benchmarks
#   .\run-benchmarks.ps1 "*Json*"                  # Run only JSON benchmarks
#   .\run-benchmarks.ps1 "*HappyPath*"             # Run only happy path tests
#   .\run-benchmarks.ps1 "*SadPath*"               # Run only sad path tests
#   .\run-benchmarks.ps1 "*" --dry                 # Dry run all benchmarks

param(
    [string]$Filter = "*",
    [switch]$Dry,
    [switch]$Help
)

if ($Help) {
    Write-Host "CloakId Benchmarks Runner"
    Write-Host ""
    Write-Host "Usage: .\run-benchmarks.ps1 [filter] [--dry] [--help]"
    Write-Host ""
    Write-Host "Parameters:"
    Write-Host "  filter    Filter benchmarks by name pattern (default: '*' for all)"
    Write-Host "  --dry     Run a quick dry run instead of full benchmarks"
    Write-Host "  --help    Show this help message"
    Write-Host ""
    Write-Host "Examples:"
    Write-Host "  .\run-benchmarks.ps1                           # Run all benchmarks"
    Write-Host "  .\run-benchmarks.ps1 '*Encode*'                # Run only encoding benchmarks"
    Write-Host "  .\run-benchmarks.ps1 '*Json*'                  # Run only JSON benchmarks"
    Write-Host "  .\run-benchmarks.ps1 '*HappyPath*'             # Run only happy path tests"
    Write-Host "  .\run-benchmarks.ps1 '*SadPath*'               # Run only sad path tests"
    Write-Host "  .\run-benchmarks.ps1 '*' --dry                 # Dry run all benchmarks"
    Write-Host ""
    Write-Host "Available benchmark categories:"
    Write-Host "  SqidsCodecBenchmarks        - Core encoding/decoding operations"
    Write-Host "  JsonSerializationBenchmarks - JSON serialization with CloakId"
    Write-Host ""
    Write-Host "Happy Path Tests:"
    Write-Host "  - EncodeInt32_HappyPath, EncodeUInt32_HappyPath, EncodeLong_HappyPath"
    Write-Host "  - DecodeInt32_HappyPath, EncodeDecodeRoundTrip_Int32"
    Write-Host "  - SerializeModel_HappyPath, DeserializeModel_HappyPath"
    Write-Host ""
    Write-Host "Sad Path Tests:"
    Write-Host "  - TryEncodeUnsupportedType_SadPath, TryDecodeInvalidValue_SadPath"
    Write-Host "  - TryDeserializeInvalidJson_SadPath, TryDeserializeMalformedJson_SadPath"
    return
}

$ProjectPath = "benchmarks/CloakId.Benchmarks/CloakId.Benchmarks.csproj"

# Check if project exists
if (-not (Test-Path $ProjectPath)) {
    Write-Error "Benchmark project not found at: $ProjectPath"
    Write-Error "Make sure you're running this script from the repository root."
    exit 1
}

Write-Host "🚀 Running CloakId Benchmarks" -ForegroundColor Green
Write-Host "Filter: $Filter" -ForegroundColor Cyan

# Build arguments
$dotnetArgs = @(
    "run", 
    "--project", $ProjectPath, 
    "--configuration", "Release"
)

if ($Filter -ne "*") {
    $dotnetArgs += "--"
    $dotnetArgs += "--filter"
    $dotnetArgs += $Filter
}

if ($Dry) {
    if ($Filter -eq "*") {
        $dotnetArgs += "--"
    }
    $dotnetArgs += "--job"
    $dotnetArgs += "dry"
    Write-Host "Running in dry mode (quick validation)" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Executing: dotnet $($dotnetArgs -join ' ')" -ForegroundColor Gray
Write-Host ""

# Run the benchmarks
& dotnet @dotnetArgs

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "✅ Benchmarks completed successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "📊 Results have been saved to:" -ForegroundColor Cyan
    Write-Host "  - BenchmarkDotNet.Artifacts/results/" -ForegroundColor Gray
    Write-Host ""
    Write-Host "📈 View detailed results:" -ForegroundColor Cyan
    Write-Host "  - Open .html files for interactive charts" -ForegroundColor Gray
    Write-Host "  - Check .md files for GitHub-friendly reports" -ForegroundColor Gray
    Write-Host "  - Use .csv files for data analysis" -ForegroundColor Gray
} else {
    Write-Host ""
    Write-Host "❌ Benchmarks failed with exit code: $LASTEXITCODE" -ForegroundColor Red
    exit $LASTEXITCODE
}
