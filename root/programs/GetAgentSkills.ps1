#Requires -Version 5.1
<#
.SYNOPSIS
    スキル リポジトリの main から src/skills を取得し、.claude/skills へ配置する。

.DESCRIPTION
    スキルの本体は OpenTouryoCodingAgentAssets にある。**こちらは複製である。**

      https://github.com/OpenTouryoProject/OpenTouryoCodingAgentAssets

    向こうの install.ps1 は**フレームワークの利用者（アプリ開発）向け**で、
    AGENTS.md や CLAUDE.md も書き換える。
    こちらはフレームワーク本体のリポジトリで、**独自の AGENTS.md を持つ**ため、
    スキルだけを取りに行く。

    **配置先は .gitignore の対象である。** 複製をコミットすると、
    向こうが更新されたときに古くなり、どちらが正か分からなくなる。
    使う前にこのスクリプトを実行すること。

    既定で除外するスキル（フレームワーク本体の開発には合わない）。

      opentouryo-project-setup*     アプリの新規構築手順
      opentouryo-project-policy     プロジェクト方針。**本体は AGENTS.md が正**
      opentouryo-project-transform  既存資産の移行。本体側では別の話
      opentouryo-comment-convention コメント規約。**本体は CODING.md が正**
      opentouryo-base2-customize    利用者による基底クラスの改造

.PARAMETER Ref
    取得するブランチまたはタグ。既定は main。

.PARAMETER Skill
    取得するスキル名。省略時は除外分を引いた全件。

.PARAMETER Exclude
    除外するスキル名。ワイルドカード可。既定は上記の 3 種。

.PARAMETER Destination
    配置先。既定はリポジトリ直下の .claude/skills。

.PARAMETER List
    取得せず、対象になるスキル名を一覧表示して終わる。

.EXAMPLE
    .\GetAgentSkills.ps1

.EXAMPLE
    .\GetAgentSkills.ps1 -List

.EXAMPLE
    .\GetAgentSkills.ps1 -Skill opentouryo-layer-d,opentouryo-layer-b

.NOTES
    作成者          ：玄人 幸道
    更新履歴        ：
     日時        更新者            内容
     ----------  ----------------  -------------------------------------------------
     2026/08/21  玄人 幸道         新規作成（#577）
#>
[CmdletBinding(SupportsShouldProcess)]
param(
    [string]$Ref = "main",
    [string[]]$Skill,
    [string[]]$Exclude = @(
        "opentouryo-project-setup*",
        "opentouryo-project-policy",
        "opentouryo-project-transform",
        "opentouryo-comment-convention",
        "opentouryo-base2-customize"
    ),
    [string]$Destination,
    [switch]$List
)

$ErrorActionPreference = "Stop"

$Repo = "OpenTouryoProject/OpenTouryoCodingAgentAssets"
$repoRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)

if (-not $Destination) { $Destination = Join-Path $repoRoot ".claude\skills" }

# Windows PowerShell 5.1 は既定で TLS 1.0 を使うことがあり、GitHub に繋がらない。
[Net.ServicePointManager]::SecurityProtocol =
    [Net.ServicePointManager]::SecurityProtocol -bor [Net.SecurityProtocolType]::Tls12

$work = Join-Path ([IO.Path]::GetTempPath()) ("OpenTouryoSkills_" + [Guid]::NewGuid().ToString("N"))
$zip  = Join-Path $work "skills.zip"

try
{
    New-Item -ItemType Directory -Path $work -Force | Out-Null

    $url = "https://codeload.github.com/$Repo/zip/refs/heads/$Ref"
    Write-Host ("=== 取得 : {0} ({1}) ===" -f $Repo, $Ref) -ForegroundColor Cyan

    # -UseBasicParsing は 5.1 で必要（IE エンジンに依存しない）。
    Invoke-WebRequest -Uri $url -OutFile $zip -UseBasicParsing

    Expand-Archive -Path $zip -DestinationPath $work -Force

    $src = Get-ChildItem -Path $work -Directory |
           ForEach-Object { Join-Path $_.FullName "src\skills" } |
           Where-Object { Test-Path $_ } |
           Select-Object -First 1

    if (-not $src) { throw "src/skills が見つかりません（$url）" }

    # SKILL.md を持つものだけがスキルである。
    $all = @(Get-ChildItem -Path $src -Directory |
             Where-Object { Test-Path (Join-Path $_.FullName "SKILL.md") })

    if ($all.Count -eq 0) { throw "スキルが 1 件も見つかりません : $src" }

    $targets = @($all)

    if ($Skill)
    {
        $unknown = @($Skill | Where-Object { $n = $_; -not ($all | Where-Object { $_.Name -eq $n }) })
        if ($unknown.Count -gt 0)
        {
            Write-Host ("  **不明なスキル : {0}**" -f ($unknown -join ", ")) -ForegroundColor Red
            Write-Host  "  -List で一覧を出せます。" -ForegroundColor Yellow
            exit 1
        }
        $targets = @($targets | Where-Object { $Skill -contains $_.Name })
    }
    else
    {
        foreach ($pat in $Exclude)
        {
            $targets = @($targets | Where-Object { $_.Name -notlike $pat })
        }
    }

    if ($targets.Count -eq 0)
    {
        Write-Host "  **対象が 0 件です。**" -ForegroundColor Red
        exit 1
    }

    if ($List)
    {
        Write-Host ("=== 対象のスキル（全 {0} 件中 {1} 件）===" -f $all.Count, $targets.Count) -ForegroundColor Cyan
        foreach ($t in ($targets | Sort-Object Name)) { Write-Host ("  " + $t.Name) }

        $skipped = @($all | Where-Object { $n = $_.Name; -not ($targets | Where-Object { $_.Name -eq $n }) })
        if ($skipped.Count -gt 0)
        {
            Write-Host ("=== 除外 {0} 件 ===" -f $skipped.Count) -ForegroundColor Yellow
            foreach ($s in ($skipped | Sort-Object Name)) { Write-Host ("  " + $s.Name) }
        }
        exit 0
    }

    New-Item -ItemType Directory -Path $Destination -Force | Out-Null

    $n = 0
    foreach ($t in ($targets | Sort-Object Name))
    {
        $dest = Join-Path $Destination $t.Name

        if ($PSCmdlet.ShouldProcess($dest, "配置"))
        {
            # **毎回入れ替える。** 差分を追うより、向こうの状態に揃えるほうが確実。
            if (Test-Path $dest) { Remove-Item -Path $dest -Recurse -Force }
            Copy-Item -Path $t.FullName -Destination $dest -Recurse -Force
            $n++
        }
    }

    # **除外に回ったものが残っていたら消す。**
    #   除外を増やしたときに、前回配置したものが取り残される。
    #   消すのは「向こうに在るスキル名」だけにする（無関係な物は触らない）。
    $pruned = 0

    if (-not $Skill)
    {
        $keep = @($targets | ForEach-Object { $_.Name })

        foreach ($known in $all)
        {
            if ($keep -contains $known.Name) { continue }

            $stale = Join-Path $Destination $known.Name
            if (-not (Test-Path $stale)) { continue }

            if ($PSCmdlet.ShouldProcess($stale, "除外分を削除"))
            {
                Remove-Item -Path $stale -Recurse -Force
                $pruned++
            }
        }
    }

    Write-Host ""
    Write-Host ("  配置 : {0} 件 → {1}" -f $n, $Destination) -ForegroundColor Green
    if ($Skill)
    {
        Write-Host ("  ※ -Skill 指定のため、他の {0} 件は取得していない" -f ($all.Count - $targets.Count))
    }
    else
    {
        Write-Host ("  除外 : {0} 件" -f ($all.Count - $targets.Count))
    }
    if ($pruned -gt 0)
    {
        Write-Host ("  削除 : {0} 件（除外に回ったもの）" -f $pruned) -ForegroundColor Yellow
    }

    Write-Host  "  **ここは .gitignore の対象。コミットしないこと。**"
}
finally
{
    if (Test-Path $work) { Remove-Item -Path $work -Recurse -Force -ErrorAction SilentlyContinue }
}

exit 0
