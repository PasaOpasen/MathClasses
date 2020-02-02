del /s /q Computator.NET\bin\Release
@RD /S /Q Computator.NET\bin\Release
del /s /q AppPackages\PackageFiles
@RD AppPackages\PackageFiles
nuget restore Computator.NET.sln
msbuild Computator.NET\Computator.NET.csproj /p:Configuration=Release /t:Clean;Build
xcopy /s Computator.NET\bin\Release AppPackages\PackageFiles\*
xcopy build-uwp\AppxManifest.xml AppPackages\PackageFiles\*
xcopy build-uwp\Registry.dat AppPackages\PackageFiles\*
xcopy /s Computator.NET.Core\Special\windows-x64 AppPackages\PackageFiles\*
xcopy /s Graphics\Assets AppPackages\PackageFiles\Assets\*
xcopy /s "Computator.NET.Core\TSL Examples" "AppPackages\PackageFiles\VFS\Users\ContainerAdministrator\Documents\Computator.NET\TSL Examples\*"
makepri createconfig /cf AppPackages\PackageFiles\priconfig.xml /dq en-US
makepri new /pr AppPackages\PackageFiles /cf AppPackages\PackageFiles\priconfig.xml
move /y .\*.pri AppPackages\PackageFiles
makeappx pack -d AppPackages\PackageFiles -p AppPackages\Computator.NET.appx
signtool.exe sign -f build-uwp\Computator.NET_TemporaryKey.pfx -fd SHA256 -v AppPackages\Computator.NET.appx