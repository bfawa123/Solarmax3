$ErrorActionPreference = "Stop"

$root = "D:\Solarmax3\Solarmax3vt3_killer\assets\res\data"

[xml]$chapter = Get-Content -Raw -Encoding UTF8 (Join-Path $root "chapter.xml")
[xml]$coo = Get-Content -Raw -Encoding UTF8 (Join-Path $root "coochapter.xml")
[xml]$level = Get-Content -Raw -Encoding UTF8 (Join-Path $root "level.xml")
[xml]$maplist = Get-Content -Raw -Encoding UTF8 (Join-Path $root "maplist.xml")

$chapterIds = [System.Collections.Generic.HashSet[string]]::new()
$chapter.SelectNodes("//chapter[@id]") | ForEach-Object { $null = $chapterIds.Add([string]$_.id) }

$levelIds = [System.Collections.Generic.HashSet[string]]::new()
$level.SelectNodes("//level[@id]") | ForEach-Object { $null = $levelIds.Add([string]$_.id) }

$mapNames = [System.Collections.Generic.HashSet[string]]::new()
$maplist.SelectNodes("//map[@name]") | ForEach-Object { $null = $mapNames.Add([string]$_.name) }

function Print-Top([string]$title, $items, [int]$max = 30) {
  Write-Host ""
  Write-Host $title
  if (-not $items -or $items.Count -eq 0) {
    Write-Host "NONE"
    return
  }
  $items | Select-Object -First $max | ForEach-Object { Write-Host $_ }
  if ($items.Count -gt $max) { Write-Host ("... total: " + $items.Count) }
}

$missingChapterRefs = New-Object System.Collections.Generic.List[string]
$coo.SelectNodes("//coo[@chapter]") | ForEach-Object {
  $c = [string]$_.chapter
  if (-not $chapterIds.Contains($c)) { $missingChapterRefs.Add($c) }
}
Print-Top "coochapter.xml missing chapter refs:" ($missingChapterRefs | Sort-Object -Unique)

$missingCooLevelRefs = New-Object System.Collections.Generic.List[string]
$coo.SelectNodes("//coo") | ForEach-Object {
  foreach ($k in @("levelId1","levelId2","levelId3","levelId4")) {
    $v = [string]$_.GetAttribute($k)
    if ($v -and -not $levelIds.Contains($v)) { $missingCooLevelRefs.Add($v) }
  }
}
Print-Top "coochapter.xml missing levelId refs:" ($missingCooLevelRefs | Sort-Object -Unique)

$missingChapterPointLevels = New-Object System.Collections.Generic.List[string]
$chapter.SelectNodes("//*[@level]") | ForEach-Object {
  $v = [string]$_.level
  if ($v -and $v -ne "0" -and -not $levelIds.Contains($v)) { $missingChapterPointLevels.Add($v) }
}
Print-Top "chapter.xml point level missing in level.xml:" ($missingChapterPointLevels | Sort-Object -Unique)

$missingLevelMaps = New-Object System.Collections.Generic.List[string]
$level.SelectNodes("//level[@map]") | ForEach-Object {
  $m = [string]$_.map
  if ($m -and -not $mapNames.Contains($m)) { $missingLevelMaps.Add($m) }
}
Print-Top "level.xml map missing in maplist.xml:" ($missingLevelMaps | Sort-Object -Unique)

$mapCfgRoot = Join-Path $root "mapconfig"
$missingMapCfgFiles = New-Object System.Collections.Generic.List[string]
foreach ($m in ($level.SelectNodes("//level[@map]") | ForEach-Object { [string]$_.map } | Sort-Object -Unique)) {
  if ([string]::IsNullOrEmpty($m)) { continue }
  $p = Join-Path $mapCfgRoot ($m + ".xml")
  if (-not (Test-Path $p)) { $missingMapCfgFiles.Add($m) }
}
Print-Top "level.xml mapconfig missing files:" $missingMapCfgFiles

