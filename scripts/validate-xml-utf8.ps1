param(
  [Parameter(Mandatory = $true)][string]$Path
)

Add-Type -AssemblyName System.Xml.Linq

Write-Host ("XML: {0}" -f $Path)

try {
  $text = Get-Content -Raw -Encoding UTF8 $Path
  [System.Xml.Linq.XDocument]::Parse($text) | Out-Null
  Write-Host "Parse: OK"
}
catch [System.Xml.XmlException] {
  $ex = $_.Exception
  Write-Host "Parse: FAIL"
  Write-Host ("Line={0} Pos={1}" -f $ex.LineNumber, $ex.LinePosition)
  Write-Host $ex.Message
  $lines = Get-Content -Encoding UTF8 $Path
  if ($ex.LineNumber -ge 1 -and $ex.LineNumber -le $lines.Count) {
    Write-Host ("BadLine: {0}" -f $lines[$ex.LineNumber - 1])
  }
}
catch {
  Write-Host "Parse: FAIL"
  Write-Host $_.Exception.Message
}
