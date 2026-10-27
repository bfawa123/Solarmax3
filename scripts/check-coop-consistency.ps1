param(
  [Parameter(Mandatory = $true)][string]$ChapterXml,
  [Parameter(Mandatory = $true)][string]$CooChapterXml,
  [Parameter(Mandatory = $true)][string]$LevelXml,
  [Parameter(Mandatory = $true)][string]$MaplistXml
)

[xml]$chap = Get-Content -Raw -Encoding UTF8 $ChapterXml
[xml]$coo = Get-Content -Raw -Encoding UTF8 $CooChapterXml
[xml]$lvl = Get-Content -Raw -Encoding UTF8 $LevelXml
[xml]$maps = Get-Content -Raw -Encoding UTF8 $MaplistXml

$chapterIds = @{}
foreach ($c in $chap.chapters.chapter) { $chapterIds[$c.GetAttribute('id')] = $true }

$levelIds = @{}
$levelMaps = @{}
foreach ($l in $lvl.levels.level) {
  $id = $l.GetAttribute('id')
  $m = $l.GetAttribute('map')
  if ($id) { $levelIds[$id] = $true }
  if ($m) { $levelMaps[$m] = $true }
}

$mapNames = @{}
foreach ($m in $maps.maps.map) { $mapNames[$m.GetAttribute('name')] = $true }

$missingCooChapters = New-Object System.Collections.Generic.List[string]
$missingCooLevels = New-Object System.Collections.Generic.List[string]

foreach ($c in $coo.coos.coo) {
  $cid = $c.GetAttribute('chapter')
  if ($cid -and -not $chapterIds.ContainsKey($cid)) { $missingCooChapters.Add($cid) | Out-Null }

  foreach ($k in @('levelId1','levelId2','levelId3','levelId4')) {
    $lid = $c.GetAttribute($k)
    if ($lid -and -not $levelIds.ContainsKey($lid)) { $missingCooLevels.Add($lid) | Out-Null }
  }
}

$missingLevelMaps = New-Object System.Collections.Generic.List[string]
foreach ($m in $levelMaps.Keys) {
  if (-not $mapNames.ContainsKey($m)) { $missingLevelMaps.Add($m) | Out-Null }
}

Write-Host ("coochapter.xml coo count={0}" -f @($coo.coos.coo).Count)
Write-Host ("missing coo chapter ids={0}" -f ($missingCooChapters | Sort-Object -Unique).Count)
($missingCooChapters | Sort-Object -Unique | Select-Object -First 30) | ForEach-Object { "  " + $_ }

Write-Host ("missing coo level ids={0}" -f ($missingCooLevels | Sort-Object -Unique).Count)
($missingCooLevels | Sort-Object -Unique | Select-Object -First 30) | ForEach-Object { "  " + $_ }

Write-Host ("level.xml level count={0}" -f @($lvl.levels.level).Count)
Write-Host ("unique level maps={0}" -f $levelMaps.Count)
Write-Host ("missing level maps in maplist={0}" -f ($missingLevelMaps | Sort-Object -Unique).Count)
($missingLevelMaps | Sort-Object -Unique | Select-Object -First 50) | ForEach-Object { "  " + $_ }
