..\..\packages\opencover.4.6.519\OpenCover.Console.exe -target:"..\..\packages\NUnit.Runners.2.6.3\tools\nunit-console.exe" -targetargs:"..\bin\Release\Public.Test.dll /noshadow" -register:user -filter:"+[Public]*" -output:Public.xml -mergebyhash
..\..\packages\ReportGenerator_2.4.4.0\bin\reportgenerator.exe -reports:Public.xml -targetdir:coverage
