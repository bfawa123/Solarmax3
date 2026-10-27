param(
  [Parameter(Mandatory = $true)][string]$Apk,
  [Parameter(Mandatory = $true)][string]$Pattern
)

Add-Type -AssemblyName System.IO.Compression.FileSystem

$zip = [System.IO.Compression.ZipFile]::OpenRead($Apk)
try {
  $e = $zip.GetEntry('assets/res/filelist.xml')
  if ($null -eq $e) {
    Write-Host 'filelist.xml missing'
    exit 0
  }
  $r = [System.IO.StreamReader]::new($e.Open())
  try {
    $t = $r.ReadToEnd()
    $t.Split([char]10) | Select-String -Pattern $Pattern | ForEach-Object { $_.Line }
  }
  finally {
    $r.Dispose()
  }
}
finally {
  $zip.Dispose()
}
