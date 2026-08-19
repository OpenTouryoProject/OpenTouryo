# Open Touryo

## Outline
*Open Touryo* is an application framework based on .NET Framework and .NET Core.

Click [here](Readme.ja.md) for the Japanese version of this file.

## How to run the programs for developing Open Touryo itself
- The following are the steps to run Open Touryo itself and the sample applications bundled with it.
- For the setup to develop an application *using* Open Touryo, refer to [this repository](https://github.com/OpenTouryoProject/OpenTouryoCodingAgentAssets/).
- When developing Open Touryo itself with a coding agent, read [AGENTS.md](../AGENTS.md) first.

### Install prerequisites
- Install Visual Studio beforehand (or Visual Studio Code with the .NET SDK and extensions).  

- Also, prepare the DBMS to be used out of the ones supported by Open Touryo. [LocalServicesOnDocker](https://github.com/NetDevInfraWGinOSSConsortium/LocalServicesOnDocker) is useful for this.

- The supported data providers can be checked in [Touryo.Infrastructure.Public.Db](https://github.com/OpenTouryoProject/OpenTouryo/tree/develop/root/programs/CS/Frameworks/Infrastructure/Public/Db).

- The officially supported data providers are *SQL / OLE / ODBC / ODP / MCN / NPG*.

- *DB2 / HiRDB / OracleClient* are currently excluded, but their templates remain, so you can support them yourself by making use of agents and so on.

- The abbreviations and the namespaces of the ADO.NET data providers
  - SQL: Microsoft.Data.SqlClient
  - OLE: System.Data.Odbc
  - ODBC: System.Data.OleDb
  - ODP: Oracle.ManagedDataAccess.Client
  - MCN: MySql.Data.MySqlClient
  - NPG: Npgsql
  - DB2: IBM.Data.DB2
  - HiRDB: Hitachi.HiRDB
  - OracleClient: System.Data.OracleClient

### Deploy Open Touryo
Copy the *root* folder to just under the C drive. If you copy it elsewhere, the build may fail because of the Windows maximum path length limitation.

### Set up the sample database

#### Make use of [LocalServicesOnDocker](https://github.com/NetDevInfraWGinOSSConsortium/LocalServicesOnDocker)

#### SQL Server  
The sample applications require the *Northwind* database.
Download the setup script of the *Northwind* database from the following Microsoft site and install it.  

- Download: NorthWind and pubs Sample Databases for SQL Server 2000 - Microsoft Download Center  
  http://www.microsoft.com/download/en/details.aspx?displaylang=en&id=23654

When the installation succeeds, the *SQL Server 2000 Sample Databases* folder is created just under the C drive.  
When using SQL Server 2012 or later, open the *instnwnd.sql* file in that folder with an editor and comment out the following code. (Because the *sp_dboption* system stored procedure does not exist in SQL Server 2012 or later)

```sql
exec sp_dboption 'Northwind','trunc. log on chkpt.','true'
exec sp_dboption 'Northwind','select into/bulkcopy','true'
```

Execute the following command at a command prompt.  
(In the following command, the folder path of *SQLCMD.EXE* differs depending on the version of SQL Server. Confirm the folder path in your environment before executing the command)
```bat
"C:\Program Files\Microsoft SQL Server\100\Tools\Binn\SQLCMD.EXE" -S localhost\SQLExpress -E -i "C:\SQL Server 2000 Sample Databases\instnwnd.sql"
```

#### DBMSs other than SQL Server
- Create an empty database in each DBMS.
- Create the tables for testing by running C:\root\files\resource\Sql\\[DBMS name]\TestTable.txt.

### Build the programs
- To build the programs of Open Touryo, run the build batch files.  

- The build batch files are stored in the following folders.
  - C:\root\programs\
  - C:\root\programs\CS  
  - C:\root\programs\VB

#### Structure of the build batch files

**The number at the beginning of a file name indicates the build order.**
The build proceeds from the smaller numbers, stacking up in the order of
infrastructure, tools, and then sample applications.

| Number | Role |
|---|---|
| `0_` | Batch execution (`0_ExecAllBat.bat` calls the following in order) |
| `1_` | Clean up (deleting `bin` / `obj` / `packages` and so on) |
| `2_` | Framework itself (the assemblies to be packed into NuGet packages) |
| `3_` | Business layer (the base classes and templates for business code) |
| `4_` | Copying assemblies for reference, and the bundled tools |
| `5_` `6_` `8_` `10_` | Sample applications |
| `7_` | Service interface for web services (on the framework side) |
| `9_` | (Not used on the C# side. Used for the WPF client on the VB side) |
| `y_` | Test code |
| `z_` | Common processing (called from the beginning of each batch file. Not run by itself) |

- **By executing `0_ExecAllBat.bat`, everything from the infrastructure to the
  sample applications is built.** The individual batch files are for rebuilding only a part.
- **`y_` (the test code) is not included in `0_ExecAllBat.bat`.** To run the unit tests,
  use `2_RunAllTests.ps1` described below, or run the `y_` batch files individually.
- The file names containing `Core`, or `netcore100`, target .NET 10.0.
  The others (`net48`) target .NET Framework 4.8.
- **The clean up of `1_` is executed repeatedly**, because the infrastructure and the
  sample applications are rebuilt at different timings. Therefore, when the whole build
  has finished, no intermediate files remain except for the ones built last.

#### Common processing (z_Common.bat)

- Each batch file calls `z_Common.bat` at its beginning, which provides the following:
  - **Resolving the build tool** ... MSBuild is located by `vswhere` (independent of the edition)
  - **Build configuration** ... `BUILD_CONFIG` (Debug / Release) and `DEBUG_TYPE`
  - **NuGet settings** ... the proxy, and the MSBuild to be used on restore

- `z_Common2.bat` is the **devenv version** of the same. It remains for the cases where  
  a build does not pass with MSBuild but passes with devenv. Normally it is not used.

- The libraries used by Open Touryo are downloaded via NuGet. Therefore, the NuGet libraries might not be downloaded normally under a proxy environment. When using a proxy environment, define the *http_proxy* environment variable as follows.
    - Open C:\root\programs\CS\z_Common.bat and C:\root\programs\VB\z_Common.bat with a text editor.
    - By default, the part that defines the *http_proxy* environment variable is commented out, so remove "@rem" to uncomment it.
    - Set your proxy information in the *http_proxy* environment variable.

#### Verification after the build

Confirming that the build passes, running the unit tests, and checking that the sample
applications work can be done with the scripts in `C:\root\programs\`.
**All of them report the result by their exit code.**

```powershell
cd C:\root\programs
.\0_RunAll.ps1          # Runs the following three together
```

| Script | Description |
|---|---|
| `1_BuildAll.ps1` | Builds everything (equivalent to `0_ExecAllBat.bat`), aggregating errors and warnings |
| `2_RunAllTests.ps1` | Runs the unit tests and compares the results with the previous ones |
| `3_SmokeTest.ps1` | Starts the sample applications and checks that they work |

For the procedures and the criteria, refer to
[`BUILDING.md`](programs/BUILDING.md) / [`TESTING.md`](programs/TESTING.md) /
[`SMOKETEST.md`](programs/SMOKETEST.md) in the same folder.
The whole release procedure is summarized in [`RELEASE.md`](programs/RELEASE.md).

### Start the ASP.NET state service
Open a command prompt as an administrator and execute the following commands.  
```bat
   sc config aspnet_state start= auto
   net start aspnet_state
```

### Run the sample applications
- Open the following files.
- Open web.config or app.config (appsettings.json in the case of .NET Core), and  
modify the values of the connectionString section according to your actual database environment.
- Run the sample application.  
If a login screen appears, enter arbitrary alphanumeric characters. (Password authentication is not performed by default)  
   
#### Web applications:
- ASP.NET Web Forms  
  - C:\root\programs\CS\Samples\WebApp_sample\WebForms_Sample\WebForms_Sample.sln
  - C:\root\programs\VB\Samples\WebApp_sample\WebForms_Sample\WebForms_Sample.sln
- ASP.NET MVC  
  - C:\root\programs\CS\Samples\WebApp_sample\MVC_Sample\MVC_Sample.sln
  - C:\root\programs\VB\Samples\WebApp_sample\MVC_Sample\MVC_Sample.sln

#### Two-tier client server applications:
- Windows Forms  
  - C:\root\programs\CS\Samples\2CS_sample\2CSClientWin_sample\2CSClientWin_sample.sln
  - C:\root\programs\VB\Samples\2CS_sample\2CSClientWin_sample\2CSClientWin_sample.sln
- WPF  
  - C:\root\programs\CS\Samples\2CS_sample\2CSClientWPF_sample\2CSClientWPF_sample.sln
  - C:\root\programs\VB\Samples\2CS_sample\2CSClientWPF_sample\2CSClientWPF_sample.sln

#### Three-tier client server applications:
- Windows Forms  
  - Ordinary Windows Forms application
    - C:\root\programs\CS\Samples\WS_sample\WSClient_sample\WSClientWin_sample\WSClientWin_sample.sln
    - C:\root\programs\VB\Samples\WS_sample\WSClient_sample\WSClientWin_sample\WSClientWin_sample.sln
  - ClickOnce application  
C:\root\programs\CS\Samples\WS_sample\WSClient_sample\WSClientWinCone_sample\WSClientWinCone_sample.sln
- WPF
  - C:\root\programs\CS\Samples\WS_sample\WSClient_sample\WSClientWPF_sample\WSClientWPF_sample.sln
  - C:\root\programs\VB\Samples\WS_sample\WSClient_sample\WSClientWPF_sample\WSClientWPF_sample.sln

### .NET Core applications

**There is currently no plan to provide a .NET Core version for VB.** All of the following are for C# only.

#### Infrastructure:
- C:\root\programs\CS\Frameworks\Infrastructure

#### Tools:
- C:\root\programs\CS\Frameworks\Tools

#### Sample applications:
- C:\root\programs\CS\Samples4NetCore


## Other items of note

### Copyright and license
Refer to the [License](https://github.com/OpenTouryoProject/OpenTouryo/tree/master/license) directory.

### Bug fix
If you find a bug while using Open Touryo, report it as an [issue](https://github.com/OpenTouryoProject/OpenTouryo/issues).  
The community will check the content and deal with it appropriately.

### Obtaining libraries, export control procedures, and attaching to the license
- The libraries that can be obtained from a package manager such as NuGet or npm are not bundled with Open Touryo, so they do not need export control.
- The other libraries, that is, the ones that cannot be obtained from a package manager, need to be obtained, bundled, and exported by yourself as necessary. In that case, the licenses of the libraries you use need to be attached to the license of Open Touryo.

### Reference
The documents in the OpenTouryoDocuments repository are available for using Open Touryo.
- [Introduction](https://github.com/OpenTouryoProject/OpenTouryoDocuments/tree/master/documents/0_Introduction)  
The overview documents of Open Touryo (PowerPoint slides and so on).
- [User Guide](https://github.com/OpenTouryoProject/OpenTouryoDocuments/tree/master/documents/1_User_Guide)  
The mechanism of Open Touryo and the specifications of each feature.
- [Tutorial](https://github.com/OpenTouryoProject/OpenTouryoDocuments/tree/master/documents/2_Tutorial)  
The first step guide of Open Touryo.
