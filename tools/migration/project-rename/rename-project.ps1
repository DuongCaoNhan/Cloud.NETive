# Rename project namespace across solution
param([string]$OldName, [string]$NewName)
Get-ChildItem -Recurse -File | ForEach-Object { (Get-Content $_) -replace $OldName, $NewName | Set-Content $_ }
