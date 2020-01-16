
set version=%1
set key=%2

cd %~dp0
dotnet build magic.io.contracts/magic.io.contracts.csproj --configuration Release --source https://api.nuget.org/v3/index.json
dotnet nuget push magic.io.contracts/bin/Release/magic.io.contracts.%version%.nupkg -k %key% -s https://api.nuget.org/v3/index.json

cd %~dp0
dotnet build magic.io.services/magic.io.services.csproj --configuration Release --source https://api.nuget.org/v3/index.json
dotnet nuget push magic.io.services/bin/Release/magic.io.services.%version%.nupkg -k %key% -s https://api.nuget.org/v3/index.json

cd %~dp0
dotnet build magic.io.controller/magic.io.controller.csproj --configuration Release --source https://api.nuget.org/v3/index.json
dotnet nuget push magic.io.controller/bin/Release/magic.io.%version%.nupkg -k %key% -s https://api.nuget.org/v3/index.json
