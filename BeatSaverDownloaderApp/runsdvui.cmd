cd /d "C:\Users\Tyler\RiderProjects\BeatSaverDownloaderApp\BeatSaverDownloaderApp" &msbuild "BeatSaverDownloaderApp.csproj" /t:sdvViewer /p:configuration="Debug" /p:platform=Any CPU
exit %errorlevel% 