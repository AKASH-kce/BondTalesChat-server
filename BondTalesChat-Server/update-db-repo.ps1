$repoUrl = "https://github.com/AKASH-kce/BondTalesChat-Database"
$targetDir = "BondTalesChat-Database"

if (Test-Path $targetDir) {
    Write-Output "Repo exists. Pulling latest changes..."
    Set-Location $targetDir
    git pull origin main
} else {
    Write-Output "Cloning repo..."
    git clone $repoUrl $targetDir
}
