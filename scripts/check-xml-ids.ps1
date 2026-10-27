$ErrorActionPreference = "Stop"

$root = "D:\Solarmax3\Solarmax3vt3_killer\assets\res\data"
$targets = @(
  @{ name = "chapter.xml"; path = (Join-Path $root "chapter.xml") },
  @{ name = "coochapter.xml"; path = (Join-Path $root "coochapter.xml") },
  @{ name = "dictionary.xml"; path = (Join-Path $root "dictionary.xml") },
  @{ name = "maplist.xml"; path = (Join-Path $root "maplist.xml") },
  @{ name = "level.xml"; path = (Join-Path $root "level.xml") }
)

function Get-Duplicates([string[]]$values) {
  return $values | Group-Object | Where-Object { $_.Count -gt 1 } | Sort-Object Count -Descending | Select-Object Name, Count
}

foreach ($t in $targets) {
  $name = $t.name
  $path = $t.path

  Write-Host ""
  Write-Host ("==== " + $name + " ====")

  if (-not (Test-Path $path)) {
    Write-Host ("MISSING: " + $path)
    continue
  }

  [xml]$x = Get-Content -Raw -Encoding UTF8 $path

  if ($name -eq "chapter.xml") {
    $ids = @($x.SelectNodes("//chapter[@id]") | ForEach-Object { [string]$_.id })
    $dups = Get-Duplicates $ids
    Write-Host "chapter.id duplicates:"
    if ($dups.Count -eq 0) { Write-Host "NONE" } else { $dups | Format-Table -AutoSize | Out-String | Write-Host }

    $zero = @($x.SelectNodes("//*[@level='0']")).Count
    Write-Host ("point level=0 count: " + $zero)
  }
  elseif ($name -eq "coochapter.xml") {
    $ids = @($x.SelectNodes("//coo[@id]") | ForEach-Object { [string]$_.id })
    $dups = Get-Duplicates $ids
    Write-Host "coo.id duplicates:"
    if ($dups.Count -eq 0) { Write-Host "NONE" } else { $dups | Format-Table -AutoSize | Out-String | Write-Host }
  }
  elseif ($name -eq "dictionary.xml") {
    $ids = @($x.SelectNodes("//language[@id]") | ForEach-Object { [string]$_.id })
    $dups = Get-Duplicates $ids
    Write-Host "language.id duplicates:"
    if ($dups.Count -eq 0) {
      Write-Host "NONE"
    } else {
      $dups | Select-Object -First 40 | Format-Table -AutoSize | Out-String | Write-Host
      Write-Host ("... total dup groups: " + $dups.Count)
    }
  }
  elseif ($name -eq "maplist.xml") {
    $ids = @($x.SelectNodes("//map[@name]") | ForEach-Object { [string]$_.name })
    $dups = Get-Duplicates $ids
    Write-Host "map.name duplicates:"
    if ($dups.Count -eq 0) { Write-Host "NONE" } else { $dups | Format-Table -AutoSize | Out-String | Write-Host }
  }
  elseif ($name -eq "level.xml") {
    $ids = @($x.SelectNodes("//level[@id]") | ForEach-Object { [string]$_.id })
    $dups = Get-Duplicates $ids
    Write-Host "level.id duplicates:"
    if ($dups.Count -eq 0) {
      Write-Host "NONE"
    } else {
      $dups | Select-Object -First 40 | Format-Table -AutoSize | Out-String | Write-Host
      Write-Host ("... total dup groups: " + $dups.Count)
    }
  }
}
