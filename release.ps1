#!/usr/bin/env pwsh

param(
    [Parameter(Mandatory=$true)]
    [string]$Version,
    [switch]$DryRun
)

# Validate version format
if ($Version -notmatch '^(\d+)\.(\d+)\.(\d+)(-[a-zA-Z0-9\-\.]+)?$') {
    Write-Host "❌ Invalid version format. Use semantic versioning (e.g., 1.0.0, 1.0.0-beta.1)" -ForegroundColor Red
    exit 1
}

$tag = "v$Version"

Write-Host "🚀 Preparing release $tag" -ForegroundColor Green

# Check if we're on main branch
$currentBranch = git branch --show-current
if ($currentBranch -ne "main") {
    Write-Host "⚠️  Warning: You're not on the main branch (current: $currentBranch)" -ForegroundColor Yellow
    $continue = Read-Host "Continue anyway? (y/N)"
    if ($continue -ne "y" -and $continue -ne "Y") {
        Write-Host "❌ Release cancelled" -ForegroundColor Red
        exit 1
    }
}

# Check for uncommitted changes
$status = git status --porcelain
if ($status) {
    Write-Host "❌ You have uncommitted changes. Please commit or stash them first." -ForegroundColor Red
    git status --short
    exit 1
}

# Check if tag already exists
$existingTag = git tag -l $tag
if ($existingTag) {
    Write-Host "❌ Tag $tag already exists" -ForegroundColor Red
    exit 1
}

# Fetch latest changes
Write-Host "📡 Fetching latest changes..." -ForegroundColor Yellow
git fetch origin

# Check if local main is up to date
$localCommit = git rev-parse HEAD
$remoteCommit = git rev-parse origin/main
if ($localCommit -ne $remoteCommit) {
    Write-Host "❌ Your local main branch is not up to date with origin/main" -ForegroundColor Red
    Write-Host "Please run: git pull origin main" -ForegroundColor Yellow
    exit 1
}

# Run local CI checks
Write-Host "🧪 Running local CI checks..." -ForegroundColor Yellow
$env:PACKAGE_VERSION = $Version
./build.ps1
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Local CI checks failed. Fix issues before releasing." -ForegroundColor Red
    exit 1
}

if ($DryRun) {
    Write-Host "🔍 DRY RUN: Would create and push tag $tag" -ForegroundColor Cyan
    Write-Host "🔍 DRY RUN: This would trigger the release workflow on GitHub" -ForegroundColor Cyan
    exit 0
}

# Create and push tag
Write-Host "🏷️  Creating tag $tag..." -ForegroundColor Yellow
git tag -a $tag -m "Release $Version"

Write-Host "📤 Pushing tag to origin..." -ForegroundColor Yellow
git push origin $tag

Write-Host "✅ Release $tag has been created and pushed!" -ForegroundColor Green
Write-Host "🔗 Monitor the release workflow at: https://github.com/Saraphite/CloakId/actions" -ForegroundColor Cyan
Write-Host "📦 Packages will be available at: https://github.com/Saraphite/CloakId/releases/tag/$tag" -ForegroundColor Cyan
