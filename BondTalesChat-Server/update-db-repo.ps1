# Get the folder where the script is located
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
Set-Location $scriptDir

$repoUrl = "https://github.com/AKASH-kce/BondTalesChat-Database"
$targetDir = "BondTalesChat-Database"

if (Test-Path $targetDir) {
    Write-Output "Repo folder exists. Deleting..."
    Remove-Item -Recurse -Force $targetDir
    Write-Output "Existing folder deleted."
}

Write-Output "Cloning repo..."
$process = Start-Process git -ArgumentList "clone", $repoUrl, $targetDir -NoNewWindow -Wait -PassThru

if ($process.ExitCode -ne 0) {
    Write-Output "Failed to clone repo. Git exited with code $($process.ExitCode)"
    exit 1
} else {
    Write-Output "Repo cloned successfully."
}
