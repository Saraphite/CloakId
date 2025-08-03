#!/usr/bin/env pwsh

param(
    [switch]$SkipTests,
    [switch]$SkipPack,
    [string]$Configuration = "Release",
    [string]$Version
)

# Set console encoding to UTF-8 for proper emoji display
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$PSDefaultParameterValues['Out-File:Encoding'] = 'utf8'

# Set version environment variable if provided
if ($Version) {
    $env:PACKAGE_VERSION = $Version
}

Write-Host "==> CloakId Local CI Script" -ForegroundColor Green
Write-Host "Configuration: $Configuration" -ForegroundColor Blue

# Restore dependencies
Write-Host "[1/5] Restoring dependencies..." -ForegroundColor Yellow
dotnet restore CloakId.slnx
if ($LASTEXITCODE -ne 0) { 
    Write-Host "ERROR: Restore failed" -ForegroundColor Red
    exit $LASTEXITCODE 
}

# Build solution
Write-Host "[2/5] Building solution..." -ForegroundColor Yellow
$buildArgs = @("build", "CloakId.slnx", "--no-restore", "--configuration", $Configuration)
if ($Version) {
    $buildArgs += "-p:VersionPrefix=$Version"
}
& dotnet @buildArgs
if ($LASTEXITCODE -ne 0) { 
    Write-Host "ERROR: Build failed" -ForegroundColor Red
    exit $LASTEXITCODE 
}

# Check formatting
Write-Host "[3/5] Checking code formatting..." -ForegroundColor Yellow
dotnet format CloakId.slnx --verify-no-changes --verbosity minimal
if ($LASTEXITCODE -ne 0) { 
    Write-Host "ERROR: Code formatting check failed. Run 'dotnet format' to fix." -ForegroundColor Red
    exit $LASTEXITCODE 
}

# Run tests
if (-not $SkipTests) {
    Write-Host "[4/5] Running tests..." -ForegroundColor Yellow
    dotnet test CloakId.slnx --no-build --configuration $Configuration --verbosity minimal
    if ($LASTEXITCODE -ne 0) { 
        Write-Host "ERROR: Tests failed" -ForegroundColor Red
        exit $LASTEXITCODE 
    }
}

# Pack NuGet packages
if (-not $SkipPack) {
    Write-Host "[5/5] Creating NuGet packages..." -ForegroundColor Yellow
    
    # Create packages directory
    if (Test-Path "./packages") {
        Remove-Item "./packages" -Recurse -Force
    }
    New-Item -ItemType Directory -Path "./packages" | Out-Null
    
    # Get version for local builds (default if not specified)
    $PackageVersion = if ($env:PACKAGE_VERSION) { $env:PACKAGE_VERSION } else { "1.0.0-dev" }
    Write-Host "Package Version: $PackageVersion" -ForegroundColor Cyan
    
    # Pack each project
    dotnet pack src/CloakId/CloakId.csproj --no-build --configuration $Configuration --output ./packages -p:PackageVersion=$PackageVersion
    dotnet pack src/CloakId.AspNetCore/CloakId.AspNetCore.csproj --no-build --configuration $Configuration --output ./packages -p:PackageVersion=$PackageVersion
    dotnet pack src/CloakId.Sqids/CloakId.Sqids.csproj --no-build --configuration $Configuration --output ./packages -p:PackageVersion=$PackageVersion
    
    if ($LASTEXITCODE -ne 0) { 
        Write-Host "ERROR: Packaging failed" -ForegroundColor Red
        exit $LASTEXITCODE 
    }
    
    Write-Host "SUCCESS: Packages created in ./packages/" -ForegroundColor Green
    if (Test-Path "./packages/*.nupkg") {
        Get-ChildItem "./packages/*.nupkg" | ForEach-Object { 
            Write-Host "  * $($_.Name)" -ForegroundColor Cyan
        }
    }
}

Write-Host "SUCCESS: All checks passed!" -ForegroundColor Green
