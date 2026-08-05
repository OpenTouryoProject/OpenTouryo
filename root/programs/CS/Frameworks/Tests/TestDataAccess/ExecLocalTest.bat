# net48（SQL Server / Oracle / MySQL）
.\net48\bin\Debug\TestDataAccessFx.exe /MODE LOCAL > Result48.txt

# .NET 10（＋ PostgreSQL）
cd .\core100\bin\Debug\net10.0
dotnet TestDataAccessCore.dll -- /MODE LOCAL > ..\..\..\..\ResultCore100.txt