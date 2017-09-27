..\..\nuget.exe pack Public.nuspec -OutputDirectory out
..\..\nuget.exe pack Framework.nuspec -OutputDirectory out
..\..\nuget.exe pack Framework.RichClient.nuspec -OutputDirectory out
..\..\nuget.exe pack DamManagedOdp.nuspec -OutputDirectory out
..\..\nuget.exe pack DamPstGrS.nuspec -OutputDirectory out
..\..\nuget.exe pack DamMySQL.nuspec -OutputDirectory out

pause
