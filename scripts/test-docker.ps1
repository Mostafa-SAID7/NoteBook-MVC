# Docker Verification Test Script
# This script tests the complete Docker build, push, pull workflow

param(
    [switch]$SkipBuild,
    [switch]$SkipPush,
    [switch]$SkipPull,
    [string]$Tag = "test"
)

$ErrorActionPreference = "Stop"
$IMAGE = "msaid356/notebook"
$VERSION = $Tag

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Docker Workflow Verification" -ForegroundColor Cyan
Write-Host "Image: ${IMAGE}:${VERSION}" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Helper function for checks
function Test-Step {
    param([string]$Name, [scriptblock]$Action)
    Write-Host "Testing: $Name..." -ForegroundColor Yellow
    try {
        & $Action
        Write-Host "✓ PASS: $Name" -ForegroundColor Green
        return $true
    } catch {
        Write-Host "✗ FAIL: $Name" -ForegroundColor Red
        Write-Host "Error: $_" -ForegroundColor Red
        return $false
    }
}

$results = @()

# Test 1: Docker is running
$results += Test-Step "Docker is running" {
    docker ps | Out-Null
    if ($LASTEXITCODE -ne 0) { throw "Docker is not running" }
}

# Test 2: Docker login
$results += Test-Step "Docker Hub authentication" {
    $info = docker info 2>&1 | Select-String "Username"
    if (-not $info) {
        Write-Host "Not logged in. Please run: docker login" -ForegroundColor Yellow
        throw "Not authenticated"
    }
}

# Test 3: Build image
if (-not $SkipBuild) {
    $results += Test-Step "Build Docker image" {
        Write-Host "Building ${IMAGE}:${VERSION}..." -ForegroundColor Gray
        docker build -t "${IMAGE}:${VERSION}" . | Out-Null
        if ($LASTEXITCODE -ne 0) { throw "Build failed" }
    }
    
    # Test 4: Image exists
    $results += Test-Step "Verify image exists" {
        $image = docker images "${IMAGE}:${VERSION}" --format "{{.Repository}}:{{.Tag}}"
        if ($image -ne "${IMAGE}:${VERSION}") { throw "Image not found" }
        Write-Host "Image ID: $(docker images ${IMAGE}:${VERSION} --format '{{.ID}}')" -ForegroundColor Gray
        Write-Host "Size: $(docker images ${IMAGE}:${VERSION} --format '{{.Size}}')" -ForegroundColor Gray
    }
    
    # Test 5: Run container test
    $results += Test-Step "Container startup test" {
        Write-Host "Starting test container..." -ForegroundColor Gray
        docker run -d --name test-notebook-verify -p 9090:8080 "${IMAGE}:${VERSION}" | Out-Null
        if ($LASTEXITCODE -ne 0) { throw "Container failed to start" }
        
        Start-Sleep -Seconds 5
        
        $status = docker ps --filter "name=test-notebook-verify" --format "{{.Status}}"
        Write-Host "Container status: $status" -ForegroundColor Gray
        
        # Cleanup
        docker stop test-notebook-verify 2>&1 | Out-Null
        docker rm test-notebook-verify 2>&1 | Out-Null
    }
} else {
    Write-Host "Skipping build tests..." -ForegroundColor Yellow
}

# Test 6: Push to Docker Hub
if (-not $SkipPush) {
    $results += Test-Step "Push to Docker Hub" {
        Write-Host "Pushing ${IMAGE}:${VERSION}..." -ForegroundColor Gray
        docker push "${IMAGE}:${VERSION}" | Out-Null
        if ($LASTEXITCODE -ne 0) { throw "Push failed" }
        Write-Host "Successfully pushed to Docker Hub" -ForegroundColor Gray
    }
} else {
    Write-Host "Skipping push test..." -ForegroundColor Yellow
}

# Test 7: Pull from Docker Hub
if (-not $SkipPull) {
    $results += Test-Step "Pull from Docker Hub" {
        Write-Host "Removing local image..." -ForegroundColor Gray
        docker rmi "${IMAGE}:${VERSION}" 2>&1 | Out-Null
        
        Write-Host "Pulling ${IMAGE}:${VERSION}..." -ForegroundColor Gray
        docker pull "${IMAGE}:${VERSION}" | Out-Null
        if ($LASTEXITCODE -ne 0) { throw "Pull failed" }
        
        $pulled = docker images "${IMAGE}:${VERSION}" --format "{{.Repository}}:{{.Tag}}"
        if ($pulled -ne "${IMAGE}:${VERSION}") { throw "Image not found after pull" }
    }
} else {
    Write-Host "Skipping pull test..." -ForegroundColor Yellow
}

# Test 8: Dockerfile validation
$results += Test-Step "Dockerfile syntax" {
    if (-not (Test-Path "Dockerfile")) { throw "Dockerfile not found" }
    $content = Get-Content "Dockerfile" -Raw
    if ($content -notmatch "FROM.*dotnet.*10\.0") { throw "Dockerfile not using .NET 10" }
    if ($content -notmatch "EXPOSE 8080") { throw "Port 8080 not exposed" }
}

# Test 9: docker-compose validation
$results += Test-Step "docker-compose.yml validation" {
    if (-not (Test-Path "docker-compose.yml")) { throw "docker-compose.yml not found" }
    $content = Get-Content "docker-compose.yml" -Raw
    if ($content -notmatch "msaid356/notebook") { throw "docker-compose not using correct image" }
}

# Test 10: .dockerignore exists
$results += Test-Step ".dockerignore exists" {
    if (-not (Test-Path ".dockerignore")) { throw ".dockerignore not found" }
}

# Test 11: GitHub workflows exist
$results += Test-Step "GitHub Actions workflows" {
    if (-not (Test-Path ".github/workflows/build.yml")) { throw "build.yml not found" }
    if (-not (Test-Path ".github/workflows/docker-publish.yml")) { throw "docker-publish.yml not found" }
}

# Summary
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Test Results Summary" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

$passed = ($results | Where-Object { $_ -eq $true }).Count
$failed = ($results | Where-Object { $_ -eq $false }).Count
$total = $results.Count

Write-Host "Total Tests: $total" -ForegroundColor White
Write-Host "Passed: $passed" -ForegroundColor Green
Write-Host "Failed: $failed" -ForegroundColor $(if ($failed -gt 0) { "Red" } else { "Green" })

Write-Host ""
if ($failed -eq 0) {
    Write-Host "✓ All tests passed! Docker setup is working correctly." -ForegroundColor Green
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Yellow
    Write-Host "1. Tag for production: docker tag ${IMAGE}:${VERSION} ${IMAGE}:latest" -ForegroundColor White
    Write-Host "2. Push to Docker Hub: docker push ${IMAGE}:latest" -ForegroundColor White
    Write-Host "3. Run with docker-compose: docker-compose up -d" -ForegroundColor White
    Write-Host "4. Test health: curl http://localhost:5000/health" -ForegroundColor White
} else {
    Write-Host "✗ Some tests failed. Please review errors above." -ForegroundColor Red
    Write-Host "See DOCKER_CHECKLIST.md for detailed troubleshooting." -ForegroundColor Yellow
    exit 1
}

Write-Host ""
