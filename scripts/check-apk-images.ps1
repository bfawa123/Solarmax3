param(
  [Parameter(Mandatory = $true)][string]$Apk
)

Add-Type -AssemblyName System.IO.Compression.FileSystem

$paths = @(
  'assets/res/data/image/bg57.jpg',
  'assets/res/data/image/bg58.jpg',
  'assets/res/data/image/bg59.jpg',
  'assets/res/data/image/bg60.jpg',
  'assets/res/data/image/Btn_CopyUI_57.jpg',
  'assets/res/data/image/Btn_CopyUI_58.jpg',
  'assets/res/data/image/Btn_CopyUI_59.jpg',
  'assets/res/data/image/Btn_CopyUI_60.jpg'
)

$zip = [System.IO.Compression.ZipFile]::OpenRead($Apk)
try {
  foreach ($p in $paths) {
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
