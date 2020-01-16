cd %~dp0
dotnet build magic.io.contracts/magic.io.contracts.csproj --configuration Release
dotnet build magic.io.services/magic.io.services.csproj --configuration Release
dotnet build magic.io.controller/magic.io.controller.csproj --configuration Release
