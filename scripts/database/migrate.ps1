# Run EF Core migrations
$services = @('CloudNative.Data')
foreach ($svc in $services) {
    dotnet ef database update --project $svc
}
