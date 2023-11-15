$ConfigurationFile = (Get-Content -Path "../appsettings.json" -Raw | ConvertFrom-Json).ConnectionStrings
$Username = $ConfigurationFile.Username
$Database = $ConfigurationFile.Database
$CreateDb = "CREATE DATABASE $Database;"
$env:PGPASSWORD=$configurationFile.Password

# Create database
Invoke-Expression -Command "psql -U $Username -c `'$CreateDb`'"

$PathToDbFile = Join-Path -Path $PSScriptRoot -ChildPath '.\db.sql'

# Create tables
$PsqlCommand = "psql.exe -U $Username -d $Database -a -f `'$PathToDbFile`'"

Invoke-Expression -Command $PsqlCommand