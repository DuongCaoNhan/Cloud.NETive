# Overall solution package health check
Get-ChildItem -Recurse -Filter '*.csproj' | ForEach-Object { dotnet list $_ package --outdated }
