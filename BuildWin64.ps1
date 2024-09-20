mkdir Build -Force
dotnet publish -p:PublishSingleFile=true --self-contained true
cp -Force ".\steam-dl-daemon\bin\Release\net8.0\win-x64\publish\steam-dl-daemon.exe" ".\Build\steam-dl-daemon.exe"
