# Open Touryo UWP Sample
## Overview
This folder contains the UWP samples using Open Touryo.

## Solution structure
- UWP_Sample.Html  
UWP sample with HTML/JavaScript.
- UWP_Sample.Xaml  
UWP sample with XAML/C#.
- SPA_sample  
Single Page Application and Web API sample. UWP samples call the web api(s) in this project.

## Usage
### Disable forms authentication
By default, forms authentication of SPA_sample is enabled. To be able to call web api(s) from UWP, disable forms authentication.

Open *Web.config* and comment `<deny users="?" />`.

```xml
<authorization>
  <!-- 全ユーザーへの許可 -->
  <!--<allow users="*" />-->
  <!-- 匿名ユーザーの禁止 -->
  <!--<deny users="?" />-->
  <!--  
    <allow  users="[ユーザーのコンマ区切り一覧]"
        roles="[ロールのコンマ区切り一覧]"/>
    <deny  users="[ユーザーのコンマ区切り一覧]"
        roles="[ロールのコンマ区切り一覧]"/>
  -->
</authorization>
```

### Configure startup project
UWP samples require web api(s) in SPA_sample. So, both of UWP sample and SPA_sample should be launched.

1. In *Solution Explorer*, right-click the solution and select *Property*.
1. Select *Common Properties*, and choose *Startup Project*.
1. Select the *Multiple Startup Project* option.
1. Configure startup project.
  - In case of debugging UWP_Sample.Html, configure as follows:
    - SPA_Sample: *Start*
    - UWP_Sample.Html: *Start*
    - UWP_Sample.Xaml: *None*
  - In case of debugging UWP_Sample.Xaml, configure as follows:
    - SPA_Sample: *Start*
    - UWP_Sample.Html: *None*
    - UWP_Sample.Xaml: *Start*
1. Click *OK*.

### Run UWP and SPA samples
1. Start ASP.NET State Service.
1. On the menu bar, choose *Build*, *Deploy Solution*.
1. Click *Start Debugging* button and run UWP and SPA samples.
