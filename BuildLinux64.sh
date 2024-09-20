mkdir Build
dotnet publish -p:PublishSingleFile=true --self-contained true
cp -f "./steam-dl-daemon/bin/Release/net8.0/linux-x64/publish/steam-dl-daemon" "./Build/steam-dl-daemon"
