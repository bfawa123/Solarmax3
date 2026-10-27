param(
  [Parameter(Mandatory = $true)][string]$Apk,
  [Parameter(Mandatory = $true)][string]$EntriesCsv
)

Add-Type -AssemblyName System.IO.Compression.FileSystem

$entries = $EntriesCsv.Split(',') | ForEach-Object { $_.Trim() } | Where-Object { $_.Length -gt 0 }

$zip = [System.IO.Compression.ZipFile]::OpenRead($Apk)
try {
  foreach ($p in $entries) {
    $e = $zip.GetEntry($p)
    if ($null -eq $e) {
      Write-Host ("MISSING {0}" -f $p)
    }
    else {
      Write-Host ("OK      {0} ({1} bytes)" -f $p, $e.Length)
    }
  }
}
finally {
  $zip.Dispose()
}
