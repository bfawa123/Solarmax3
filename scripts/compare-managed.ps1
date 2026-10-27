param(
  [Parameter(Mandatory = $true)][string]$OldApk,
  [Parameter(Mandatory = $true)][string]$NewApk,
  [Parameter(Mandatory = $true)][string]$CurrentManagedDir
)

Add-Type -AssemblyName System.IO.Compression.FileSystem

function Get-ManagedEntries([string]$apk) {
  $zip = [System.IO.Compression.ZipFile]::OpenRead($apk)
  try {
    return $zip.Entries |
      Where-Object { $_.FullName -like 'assets/bin/Data/Managed/*' -and -not $_.FullName.EndsWith('/') } |
      ForEach-Object {
        [pscustomobject]@{
          Name   = $_.FullName.Substring('assets/bin/Data/Managed/'.Length)
          Length = $_.Length
        }
      }
  }
  finally {
    $zip.Dispose()
  }
}

$old = Get-ManagedEntries $OldApk
$new = Get-ManagedEntries $NewApk

$oldMap = @{}
foreach ($e in $old) { $oldMap[$e.Name] = $e.Length }
$newMap = @{}
foreach ($e in $new) { $newMap[$e.Name] = $e.Length }

$allNames = @($oldMap.Keys + $newMap.Keys) | Sort-Object -Unique
$diff = @()
foreach ($n in $allNames) {
  $inOld = $oldMap.ContainsKey($n)
  $inNew = $newMap.ContainsKey($n)
  if (-not $inOld) { $diff += [pscustomobject]@{ Name = $n; OldLength = $null; NewLength = $newMap[$n]; Note = 'only-in-new-apk' }; continue }
  if (-not $inNew) { $diff += [pscustomobject]@{ Name = $n; OldLength = $oldMap[$n]; NewLength = $null; Note = 'only-in-old-apk' }; continue }
  if ($oldMap[$n] -ne $newMap[$n]) { $diff += [pscustomobject]@{ Name = $n; OldLength = $oldMap[$n]; NewLength = $newMap[$n]; Note = 'size-diff' } }
}

Write-Host ("old apk managed files: {0}" -f $old.Count)
Write-Host ("new apk managed files: {0}" -f $new.Count)
Write-Host ("diff count: {0}" -f $diff.Count)
$diff | Sort-Object Note, Name | Select-Object -First 80 | Format-Table -AutoSize

Write-Host '--- current Managed folder vs new apk (missing/size diff) ---'
$cur = Get-ChildItem $CurrentManagedDir -File | Select-Object Name, Length
$curMap = @{}
foreach ($f in $cur) { $curMap[$f.Name] = $f.Length }
$curDiff = @()
foreach ($n in $newMap.Keys) {
  if ($curMap.ContainsKey($n)) {
    if ($curMap[$n] -ne $newMap[$n]) { $curDiff += [pscustomobject]@{ Name = $n; CurLength = $curMap[$n]; ApkLength = $newMap[$n]; Note = 'size-diff' } }
  }
  else {
    $curDiff += [pscustomobject]@{ Name = $n; CurLength = $null; ApkLength = $newMap[$n]; Note = 'missing-in-folder' }
  }
}
$curDiff | Sort-Object Note, Name | Select-Object -First 80 | Format-Table -AutoSize

function Get-ZipEntryMd5([string]$apk, [string]$entryPath) {
  $zip = [System.IO.Compression.ZipFile]::OpenRead($apk)
  try {
    $entry = $zip.GetEntry($entryPath)
    if ($null -eq $entry) { return $null }
    $md5 = [System.Security.Cryptography.MD5]::Create()
    try {
      $stream = $entry.Open()
      try {
        $hashBytes = $md5.ComputeHash($stream)
        return ([System.BitConverter]::ToString($hashBytes)).Replace('-', '')
      }
      finally {
        $stream.Dispose()
      }
    }
    finally {
      $md5.Dispose()
    }
  }
  finally {
    $zip.Dispose()
  }
}

Write-Host '--- spot-check md5 (Managed/Assembly-CSharp.dll) ---'
$entryName = 'assets/bin/Data/Managed/Assembly-CSharp.dll'
$oldMd5 = Get-ZipEntryMd5 $OldApk $entryName
$newMd5 = Get-ZipEntryMd5 $NewApk $entryName
$curFile = Join-Path $CurrentManagedDir 'Assembly-CSharp.dll'
$curMd5 = $null
if (Test-Path $curFile) {
  $curMd5 = (Get-FileHash $curFile -Algorithm MD5).Hash
}
[pscustomobject]@{
  OldApkMd5 = $oldMd5
  NewApkMd5 = $newMd5
  CurrentFolderMd5 = $curMd5
} | Format-List
