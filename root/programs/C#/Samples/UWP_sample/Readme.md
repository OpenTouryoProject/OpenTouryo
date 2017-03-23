# Open Touryo UWP App Sample
## Overview
This folder contains the UWP app samples using Open Touryo.

## Solution structure
- UWP_Sample.Html  
UWP app sample with HTML/JavaScript.
- UWP_Sample.Xaml  
UWP app sample with XAML/C#.
- SPA_sample  
Single Page Application and Web API sample. UWP app samples call the web api(s) in this project.

## Usage
### Enable developer mode for Windows 10
To develop UWP app, developers need to use *developer features*.

1. Open *Update & security*.
1. Select *For developers*, and choose *Developer Mode*.

For details, please refer [the Microsoft webpage](https://docs.microsoft.com/en-us/windows/uwp/get-started/enable-your-device-for-development).

### Sign an app
1. Open *package.appxmanifest* file in UWP_Sample.Html project or UWP_Sample.Xaml project.
1. Select *Packaging*, and click *Choose Certificate* button.
1. Select certificate for signing. You may create test certificate.

### Disable forms authentication
By default, forms authentication of SPA_sample is enabled. To be able to call web api(s) from UWP app, disable forms authentication.

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

### Debug UWP app
1. Right-click UWP_Sample.Html project or UWP_Sample.Xaml project, and choose *Set as StartUp Project*.
1. To debug UWP app, press *F5* key. (IIS Express launchs and SPA_Sample starts automatically. If starting without debugging, IIS Express does not launch automatically.)
