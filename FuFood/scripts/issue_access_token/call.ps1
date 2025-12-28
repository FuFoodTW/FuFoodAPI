param(
  [string]$UserId = "U78d1bb86f8410a52d375306295c06503",
  [switch]$ForceCompile = $false
)

Set-Location $PSScriptRoot

if ($IsWindows)
{
  # On Windows, pre-compile the binary
  $binaryPath = "app.exe"
  if ($ForceCompile -or (-not (Test-Path $binaryPath)))
  {
    go build -o $binaryPath .
  }
  $value = & $binaryPath $UserId
} else
{
  $value = go run . $UserId
}

Write-Host $value
$value | Set-Clipboard
