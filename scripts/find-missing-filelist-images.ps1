param(
  [Parameter(Mandatory = $true)][string]$ChapterXml,
  [Parameter(Mandatory = $true)][string]$FilelistXml
)

[xml]$chap = Get-Content -Raw -Encoding UTF8 $ChapterXml
$imgs = @()

foreach ($c in $chap.chapters.chapter) {
  $bg = $c.GetAttribute('chapterBg')
  $sc = $c.GetAttribute('starchart')
  if ($bg -like 'data/image/*.jpg') { $imgs += $bg.Substring('data/image/'.Length) }
  if ($sc -like 'data/image/*.jpg') { $imgs += $sc.Substring('data/image/'.Length) }
}

$imgs = $imgs | Sort-Object -Unique
Write-Host ("chapter.xml referenced images count={0}" -f $imgs.Count)

$filelistText = Get-Content -Raw -Encoding UTF8 $FilelistXml
$missing = @()
foreach ($img in $imgs) {
  $id = 'assets/res/data/image/' + [System.IO.Path]::GetFileNameWithoutExtension($img)
  if ($filelistText -notmatch [regex]::Escape('id="' + $id + '"')) {
    $missing += $img
  }
}

Write-Host ("missing in filelist count={0}" -f $missing.Count)
$missing
