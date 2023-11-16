param (
    [switch]$Drop
)

$ConfigurationFile = (Get-Content -Path "../appsettings.json" -Raw | ConvertFrom-Json).ConnectionStrings
$Username = $ConfigurationFile.Username
$Database = $ConfigurationFile.Database
$CreateDb = "CREATE DATABASE $Database;"
$DropDb = "DROP DATABASE IF EXISTS $Database;"
$env:PGPASSWORD=$configurationFile.Password

# Drop database if -Drop is present
if ($Drop) {
    Invoke-Expression -Command "psql -U $Username -c `'$DropDb`'"
}

# Create database
Invoke-Expression -Command "psql -U $Username -c `'$CreateDb`'"

$PathToDbFile = Join-Path -Path $PSScriptRoot -ChildPath '.\db.sql'

# Create tables
$PsqlCommand = "psql.exe -U $Username -d $Database -a -f `'$PathToDbFile`'"

Invoke-Expression -Command $PsqlCommand