# Scan packages for known vulnerabilities
Get-ChildItem -Recurse -Filter '*.csproj' | ForEach-Object { dotnet list $_ package --vulnerable }
