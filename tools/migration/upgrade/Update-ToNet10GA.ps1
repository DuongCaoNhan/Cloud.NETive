# Upgrade solution to .NET 10 GA
Get-ChildItem -Recurse -Filter '*.csproj' | ForEach-Object { (Get-Content $_) -replace 'net9.0', 'net10.0' | Set-Content $_ }
