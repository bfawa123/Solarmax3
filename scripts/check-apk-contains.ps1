$ErrorActionPreference = "Stop"
$ApkPath = "D:\Solarmax3\build\Solarmax3vt3_killersigned-aligned.apk"

Add-Type -AssemblyName System.IO.Compression.FileSystem

$zip = [System.IO.Compression.ZipFile]::OpenRead($ApkPath)
try {
  $set = [System.Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
  foreach ($e in $zip.Entries) {
    $null = $set.Add($e.FullName)
  }

  $targets = @(
    "assets/res/data/chapter.xml",
    "assets/res/data/image/bg57.jpg",
    "assets/res/data/image/bg62.jpg",
    "assets/res/data/image/Btn_CopyUI_62.jpg"
  )

  foreach ($t in $targets) {
    if ($set.Contains($t)) {
      Write-Host ("FOUND " + $t)
    } else {
      Write-Host ("MISSING " + $t)
    }
  }

  $entry = $zip.GetEntry("assets/res/data/chapter.xml")
  if ($entry -ne $null) {
    $sr = New-Object System.IO.StreamReader($entry.Open())
    try {
      $text = $sr.ReadToEnd()
      $xml = New-Object System.Xml.XmlDocument
      $xml.LoadXml($text)
      $node = $xml.SelectSingleNode("//chapter[@id='4017']")
      if ($node -ne $null) {
        Write-Host ""
        Write-Host "chapter.xml id=4017 attributes:"
        Write-Host ("starchart=" + $node.GetAttribute("starchart"))
        Write-Host ("chapterBg=" + $node.GetAttribute("chapterBg"))
      }
    }
    finally {
      $sr.Dispose()
    }
  }
}
finally {
  $zip.Dispose()
}
