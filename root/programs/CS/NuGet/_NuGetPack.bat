setlocal
@echo off

@rem --------------------------------------------------
@rem Read the version number from Directory.Build.props.
@rem The nuspec files keep the $version$ token, so the version is
@rem defined in exactly one place.
@rem
@rem NOTE: keep this file pure ASCII (#532).
@rem --------------------------------------------------
set OT_INFRA=..\Frameworks\Infrastructure
set OT_PROPS=%OT_INFRA%\Directory.Build.props
set OT_VERSION=

for /f "usebackq delims=" %%v in (
  `powershell -NoProfile -ExecutionPolicy Bypass -Command "([xml](Get-Content '%OT_PROPS%' -Raw)).Project.PropertyGroup.OpenTouryoVersion"`
) do set OT_VERSION=%%v

if not defined OT_VERSION (
  echo [ERROR] Failed to read OpenTouryoVersion from %OT_PROPS%.
  pause
  exit /b 1
)

@rem --------------------------------------------------
@rem Verify that the net48 assemblies carry the same version (#531).
@rem
@rem Directory.Build.props is imported through Microsoft.Common.props, so it
@rem only reaches the SDK-style projects (*_netcore100.csproj). The old-style
@rem projects (*_net48.csproj) do not import it and keep their version in
@rem Properties\AssemblyInfo.cs instead.
@rem
@rem Raising OpenTouryoVersion without updating those files ships a package
@rem whose net48 and net10.0 assemblies carry DIFFERENT versions, and that
@rem cannot be corrected once published. Stop here instead.
@rem
@rem The 6 projects checked are the ones that go into the NuGet packages.
@rem DamPstGrS has no net48. Business is deliberately on a separate version
@rem line (1.0.0.0) and is not packaged, so neither is checked.
@rem
@rem AssemblyVersion has four components (3.0.0.0) and OpenTouryoVersion has
@rem three (3.0.0), so only the first three are compared.
@rem
@rem OpenTouryoVersion may carry a prerelease suffix (3.3.0-alpha1). The
@rem suffix never reaches the assemblies: the SDK assigns VersionPrefix to
@rem AssemblyVersion / FileVersion and the whole string to
@rem InformationalVersion. It is therefore stripped before comparing.
@rem
@rem _T_NuGetPack.bat does NOT need this check. The test package takes its
@rem version from the command line and never reads OpenTouryoVersion, so the
@rem version only names the package and is not written into any assembly.
@rem There is nothing for the assemblies to disagree with.
@rem --------------------------------------------------
for /f "delims=- tokens=1" %%x in ("%OT_VERSION%") do set OT_VERPREFIX=%%x

echo --------------------------------------------------
echo Checking the net48 AssemblyVersion against %OT_VERPREFIX%.
echo --------------------------------------------------

powershell -NoProfile -ExecutionPolicy Bypass -Command "$v='%OT_VERPREFIX%'; $ok=$true; foreach($p in 'Public','Public\Security','Framework','Framework\RichClient','Public\Db\DamManagedOdp','Public\Db\DamMySQL'){ $f=Join-Path '%OT_INFRA%' ($p+'\Properties\AssemblyInfo.cs'); if(-not (Test-Path $f)){ Write-Host ('  NG  '+$p+' : AssemblyInfo.cs not found'); $ok=$false; continue }; $m=Select-String -Path $f -Pattern 'AssemblyVersion\(.([0-9]+\.[0-9]+\.[0-9]+)' | Select-Object -First 1; if(-not $m){ Write-Host ('  NG  '+$p+' : AssemblyVersion not found'); $ok=$false; continue }; $a=$m.Matches[0].Groups[1].Value; if($a -eq $v){ Write-Host ('  OK  '+$p+' : '+$a) } else { Write-Host ('  NG  '+$p+' : '+$a+'  expected '+$v); $ok=$false } }; if(-not $ok){ exit 1 }"

if errorlevel 1 goto VersionMismatch

@rem --------------------------------------------------
@rem Read the commit hash (#531).
@rem It is passed to <repository commit="$commit$"> in the nuspec so that
@rem nuget.org shows which commit the package was built from.
@rem
@rem Source Link itself works from the information inside the PDB, so
@rem source debugging still works when this is empty. It only helps
@rem people navigate back to the source.
@rem --------------------------------------------------
set OT_COMMIT=

for /f "usebackq delims=" %%c in (`git rev-parse HEAD 2^>nul`) do set OT_COMMIT=%%c

if not defined OT_COMMIT (
  echo [WARN] Could not read the commit hash from git. Continuing with an empty value.
  set OT_COMMIT=
)

echo --------------------------------------------------
echo OpenTouryoVersion = %OT_VERSION%
echo commit            = %OT_COMMIT%
echo --------------------------------------------------

@rem --------------------------------------------------
@rem Clear the working folders before packing (#531).
@rem
@rem Only this batch's own output is removed. The test packages built by
@rem _T_NuGetPack.bat are left alone; their push uses a different wildcard.
@rem The reasons are in _Cleanup.bat, which can also be run on its own.
@rem --------------------------------------------------
call "%~dp0_Cleanup.bat" Touryo.Infrastructure /NOPAUSE

xcopy /E /Y "%OT_INFRA%\Build_net48" "in\net48"
xcopy /E /Y "%OT_INFRA%\Build_netcore100\net10.0" "in\net10.0"
xcopy /E /Y "%OT_INFRA%\Build_netcore100\net10.0-windows7.0" "in\net10.0-windows"

@rem --------------------------------------------------
@rem Verify that the copied assemblies were actually rebuilt (#531).
@rem
@rem The check above compares two SOURCE files with each other
@rem (Directory.Build.props and AssemblyInfo.cs). It cannot notice that
@rem 0_Release4Nuget.bat was skipped: Build_* would still hold the previous
@rem version, _Cleanup.bat would refresh in\ from it, and a package carrying
@rem old assemblies would be published under the new version number.
@rem That cannot be corrected once published.
@rem
@rem The list of assemblies is taken from the nuspec files themselves, so a
@rem new package is covered without editing this batch. Satellite
@rem assemblies are skipped; they carry the same version anyway.
@rem OpenTouryo.Business.dll is not listed in any nuspec and is therefore
@rem not checked - it is deliberately on a separate version line (1.0.0.0).
@rem
@rem symbol_template.nuspec is excluded. Windows globbing is case
@rem insensitive, so Symbol_*.nuspec would otherwise match the template and
@rem report its placeholder (OpenTouryo.xxxx.dll) as missing.
@rem --------------------------------------------------
echo --------------------------------------------------
echo Checking the packaged assemblies against %OT_VERPREFIX%.
echo --------------------------------------------------

powershell -NoProfile -ExecutionPolicy Bypass -Command "$v='%OT_VERPREFIX%'; $ok=$true; $n=0; foreach($f in Get-ChildItem -Path 'Symbol_*.nuspec' -Exclude 'symbol_template.nuspec'){ $x=[xml](Get-Content $f.FullName -Raw); foreach($e in $x.package.files.file){ $s=$e.src; if($s -notlike '*.dll'){continue}; if($s -like '*ja-JP*'){continue}; if(-not (Test-Path $s)){ Write-Host ('  NG  '+$s+' : not found'); $ok=$false; continue }; $a=[Reflection.AssemblyName]::GetAssemblyName((Resolve-Path $s)).Version; $a3=''+$a.Major+'.'+$a.Minor+'.'+$a.Build; $n++; if($a3 -eq $v){ Write-Host ('  OK  '+$s+' : '+$a) } else { Write-Host ('  NG  '+$s+' : '+$a+'  expected '+$v+'.x'); $ok=$false } } }; if($n -eq 0){ Write-Host '  NG  no assemblies were checked'; $ok=$false }; if(-not $ok){ exit 1 }"

if errorlevel 1 goto AssemblyMismatch

"..\..\nuget.exe" pack Symbol_Public.nuspec -Properties version=%OT_VERSION%;commit=%OT_COMMIT% -OutputDirectory "out\sp" -Symbols -SymbolPackageFormat snupkg
"..\..\nuget.exe" pack Symbol_Public.Security.nuspec -Properties version=%OT_VERSION%;commit=%OT_COMMIT% -OutputDirectory "out\sp" -Symbols -SymbolPackageFormat snupkg
"..\..\nuget.exe" pack Symbol_Framework.nuspec -Properties version=%OT_VERSION%;commit=%OT_COMMIT% -OutputDirectory "out\sp" -Symbols -SymbolPackageFormat snupkg
"..\..\nuget.exe" pack Symbol_Framework.RichClient.nuspec -Properties version=%OT_VERSION%;commit=%OT_COMMIT% -OutputDirectory "out\sp" -Symbols -SymbolPackageFormat snupkg
"..\..\nuget.exe" pack Symbol_DamManagedOdp.nuspec -Properties version=%OT_VERSION%;commit=%OT_COMMIT% -OutputDirectory "out\sp" -Symbols -SymbolPackageFormat snupkg
"..\..\nuget.exe" pack Symbol_DamPstGrS.nuspec -Properties version=%OT_VERSION%;commit=%OT_COMMIT% -OutputDirectory "out\sp" -Symbols -SymbolPackageFormat snupkg
"..\..\nuget.exe" pack Symbol_DamMySQL.nuspec -Properties version=%OT_VERSION%;commit=%OT_COMMIT% -OutputDirectory "out\sp" -Symbols -SymbolPackageFormat snupkg

pause
exit /b 0

:VersionMismatch
echo.
echo [ERROR] The net48 AssemblyVersion does not match %OT_VERPREFIX%
echo         ^(OpenTouryoVersion = %OT_VERSION%^)
echo         Update Properties\AssemblyInfo.cs in the projects marked NG above,
echo         rebuild with 0_Release4Nuget.bat, then run this again.
echo         See RELEASE.md phase 0.
pause
exit /b 1

:AssemblyMismatch
echo.
echo [ERROR] The packaged assemblies do not carry %OT_VERPREFIX%
echo         ^(OpenTouryoVersion = %OT_VERSION%^)
echo         The sources say the right version but the binaries do not,
echo         so the rebuild was skipped. Run CS\0_Release4Nuget.bat,
echo         then run this again.
echo         See RELEASE.md phase 0 and phase 3.
pause
exit /b 1