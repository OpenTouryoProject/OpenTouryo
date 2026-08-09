<#
.SYNOPSIS
    Open 棟梁のリリース バージョンを、関係する全ファイルに一括反映する。

.DESCRIPTION
    バージョン番号の定義箇所は 2 系統に分かれており、手作業では追随を忘れやすい（#531）。

      (1) CS\Frameworks\Infrastructure\Directory.Build.props の OpenTouryoVersion
          … SDK 形式 csproj（*_netcore100.csproj）が <Version> で参照する。
      (2) 各 net48 プロジェクトの Properties\AssemblyInfo.cs の AssemblyVersion
          … 旧形式 csproj は Microsoft.Common.props を通らないため
            Directory.Build.props が効かない。別管理になっている。

    (2) の追随を忘れると、同じ NuGet パッケージの中で net48 と net10.0 の
    アセンブリの版が食い違う。**公開後には直せない。**

    _NuGetPack.bat はパッケージ化の前にこの一致を検査して停止するが、
    本スクリプトは、そもそもずれた状態を作らないためのもの。

.PARAMETER Version
    設定するバージョン。3 桁（例: 3.1.0）で指定する。
    AssemblyVersion には 4 桁目に 0 を補って書き込む（例: 3.1.0.0）。

.PARAMETER WhatIf
    書き換えずに、変更内容だけを表示する。

.EXAMPLE
    .\0_SetVersion.ps1 -Version 3.1.0

.EXAMPLE
    .\0_SetVersion.ps1 -Version 3.1.0 -WhatIf

.NOTES
    ・net48 側の対象は「NuGet パッケージに入る 6 本」のみ。
      DamPstGrS は net48 が無い。Business は非パッケージかつ意図的に
      別系統（1.0.0）のため、対象にしない。
    ・書き換え後は 0_Release4Nuget.bat でのリビルドが必要。
      OpenTouryoVersion はアセンブリに焼き込まれるため、
      先にパッケージ化すると古いアセンブリに新しい版番号が付く。
    ・手順の全体は RELEASE.md フェーズ 0 を参照。
#>
[CmdletBinding(SupportsShouldProcess = $true)]
param(
    [Parameter(Mandatory = $true)]
    [string]$Version
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# --------------------------------------------------
# コンソールのコード ページと、PowerShell の出力エンコードは別物（ANALYSIS.md 8.5）。
# 判定を分けること。
# --------------------------------------------------
if ((cmd /c chcp) -notmatch '65001')
{
    cmd /c chcp 65001 | Out-Null
}

if ([Console]::OutputEncoding.CodePage -ne 65001)
{
    [Console]::OutputEncoding = New-Object Text.UTF8Encoding $false
}

# --------------------------------------------------
# 引数の検証
# --------------------------------------------------
if ($Version -notmatch '^[0-9]+\.[0-9]+\.[0-9]+$')
{
    Write-Host "[ERROR] Version は 3 桁で指定してください（例: 3.1.0）。指定値: $Version" -ForegroundColor Red
    exit 1
}

$assemblyVersion = "$Version.0"

# --------------------------------------------------
# 対象ファイル
#
# net48 側は「NuGet パッケージに入る 6 本」のみ。
# _NuGetPack.bat の検査対象と一致させること。
# --------------------------------------------------
$infra = Join-Path $PSScriptRoot "CS\Frameworks\Infrastructure"
$props = Join-Path $infra "Directory.Build.props"

$net48Projects = @(
    "Public",
    "Public\Security",
    "Framework",
    "Framework\RichClient",
    "Public\Db\DamManagedOdp",
    "Public\Db\DamMySQL"
)

# --------------------------------------------------
# 読み書きのヘルパ
#
# BOM の有無を保って書き戻す。5.1 の Get-Content / Set-Content は
# 既定のエンコードが ANSI のため使わない（ANALYSIS.md 8.5）。
# --------------------------------------------------
function Read-TextFile
{
    param([string]$Path)

    $bytes = [IO.File]::ReadAllBytes($Path)
    $hasBom = ($bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF)

    if ($hasBom)
    {
        $text = [Text.Encoding]::UTF8.GetString($bytes, 3, $bytes.Length - 3)
    }
    else
    {
        $text = [Text.Encoding]::UTF8.GetString($bytes)
    }

    return [pscustomobject]@{ Text = $text; HasBom = $hasBom }
}

function Write-TextFile
{
    param([string]$Path, [string]$Text, [bool]$HasBom)

    [IO.File]::WriteAllText($Path, $Text, (New-Object Text.UTF8Encoding $HasBom))
}

# --------------------------------------------------
# 置換
#
# 戻り値は、変更があったかどうか。
# 変更前の値が見つからない場合はエラーで停止する（黙って素通りさせない）。
# --------------------------------------------------
$results = @()
$failed = $false

function Update-VersionInFile
{
    param(
        [string]$Path,
        [string]$Label,
        [string]$Pattern,
        [string]$Replacement,
        [string]$NewValue
    )

    if (-not (Test-Path $Path))
    {
        $script:results += [pscustomobject]@{ 対象 = $Label; 変更前 = "-"; 変更後 = "-"; 結果 = "NG (ファイルが無い)" }
        $script:failed = $true
        return
    }

    $file = Read-TextFile -Path $Path
    $found = [regex]::Matches($file.Text, $Pattern)

    if ($found.Count -eq 0)
    {
        $script:results += [pscustomobject]@{ 対象 = $Label; 変更前 = "-"; 変更後 = "-"; 結果 = "NG (該当行が無い)" }
        $script:failed = $true
        return
    }

    # グループ 1 は置換時に残す前置き、グループ 2 が現在のバージョン。
    # 全パターンでこの並びに揃えてある。
    $before = $found[0].Groups[2].Value

    if ($before -eq $NewValue)
    {
        $script:results += [pscustomobject]@{ 対象 = $Label; 変更前 = $before; 変更後 = $NewValue; 結果 = "変更なし" }
        return
    }

    $updated = [regex]::Replace($file.Text, $Pattern, $Replacement)

    if ($PSCmdlet.ShouldProcess($Path, "$before -> $NewValue"))
    {
        Write-TextFile -Path $Path -Text $updated -HasBom $file.HasBom
        $script:results += [pscustomobject]@{ 対象 = $Label; 変更前 = $before; 変更後 = $NewValue; 結果 = "更新" }
    }
    else
    {
        $script:results += [pscustomobject]@{ 対象 = $Label; 変更前 = $before; 変更後 = $NewValue; 結果 = "更新（WhatIf）" }
    }
}

Write-Host ""
Write-Host "--------------------------------------------------"
Write-Host " OpenTouryoVersion = $Version   AssemblyVersion = $assemblyVersion"
Write-Host "--------------------------------------------------"

# --------------------------------------------------
# (1) Directory.Build.props
# --------------------------------------------------
Update-VersionInFile `
    -Path $props `
    -Label "Directory.Build.props" `
    -Pattern '(<OpenTouryoVersion>)([^<]*)(?=</OpenTouryoVersion>)' `
    -Replacement "`${1}$Version" `
    -NewValue $Version

# --------------------------------------------------
# (2) net48 の AssemblyInfo.cs
#
# コメント アウトされた行（例: // [assembly: AssemblyVersion("1.0.*")]）は
# 対象外にする。行頭から属性までの間に // が無いことを条件にしている。
# AssemblyFileVersion は、在れば一緒に更新する。
# --------------------------------------------------
foreach ($project in $net48Projects)
{
    $path = Join-Path $infra (Join-Path $project "Properties\AssemblyInfo.cs")

    Update-VersionInFile `
        -Path $path `
        -Label "$project (AssemblyVersion)" `
        -Pattern '(?m)^(?!\s*//)(\s*\[assembly:\s*AssemblyVersion\(")([0-9][^"]*)(?="\)\])' `
        -Replacement "`${1}$assemblyVersion" `
        -NewValue $assemblyVersion

    $file = Read-TextFile -Path $path

    if ($file.Text -match '(?m)^(?!\s*//)\s*\[assembly:\s*AssemblyFileVersion\("')
    {
        Update-VersionInFile `
            -Path $path `
            -Label "$project (AssemblyFileVersion)" `
            -Pattern '(?m)^(?!\s*//)(\s*\[assembly:\s*AssemblyFileVersion\(")([0-9][^"]*)(?="\)\])' `
            -Replacement "`${1}$assemblyVersion" `
            -NewValue $assemblyVersion
    }
}

# --------------------------------------------------
# 結果
# --------------------------------------------------
Write-Host ""
$results | Format-Table -AutoSize | Out-String -Width 200 | Write-Host

if ($failed)
{
    Write-Host "[ERROR] 更新できなかったファイルがあります。上の NG を確認してください。" -ForegroundColor Red
    exit 1
}

Write-Host "次に行うこと" -ForegroundColor Yellow
Write-Host "  1. Business 系が 1.0.0 のままであることを確認する（意図的に別系統）"
Write-Host "  2. コミットして push する（Source Link はこのコミットに固定される）"
Write-Host "  3. CS\0_Release4Nuget.bat でリビルドする（版はアセンブリに焼き込まれる）"
Write-Host "  4. CS\NuGet\_NuGetPack.bat でパッケージ化する（版の一致が再検査される）"
Write-Host ""
Write-Host "  詳細は RELEASE.md フェーズ 0 を参照。"
Write-Host ""

exit 0
