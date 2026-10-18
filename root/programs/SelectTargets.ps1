<#
.SYNOPSIS
    検証の済んだコミットからの変更を調べ、回す必要のあるビルド・単体テスト・疎通を割り出す。

.DESCRIPTION
    通し（0_RunAll.ps1）は長い（#571）。変更が末端（サンプル・ツール・テスト）だけなら、
    その対象だけを回せば足りる（#576）。本スクリプトは、その判断を差分から機械的に行う（#610）。

    ＜何を基点にするか＞
      「検証の済んだコミット」を .verified-commit に記録しておき、そこからの差分を見る。
        ・-Record          … いまの HEAD を記録する（通しが通った後に使う）
        ・-Run が全部 OK    … 自動で記録する
      記録が無いとき、あるいは別の基点で見たいときは -Since で指定する。

      **差分にはワーキング ツリーの変更（未コミット・未追跡）も含める。**
      エージェントは作業結果をコミットせずに残すため（AGENTS.md）。

    ＜どう割り出すか＞
      **対象名とバッチの対応は、各スクリプトの -List -PassThru から取る。**
      ここに写すと二重管理になる。本スクリプトが持つのは
      「ファイル → バッチ」の対応の引き方だけで、それもバッチと .sln から読む。

        1. バッチ（CS\*.bat / VB\*.bat）が建てる .sln / .csproj / .vbproj を読み、
           そのフォルダ（.sln が束ねるプロジェクトのフォルダを含む）をバッチに対応付ける
        2. 変更されたファイルを、その下にあるバッチに対応付ける
        3. バッチから、1_BuildAll / 2_RunAllTests / 3_SmokeTest の対象名を引く

      **基盤（Frameworks\Infrastructure）に触れたら通し。** 下は全部やり直しになるため。
      **対応が引けないファイルも通し。** 迷ったら通し（AGENTS.md）。

    ＜Linux（#632）＞
      Public・Public.Security（と、それを Linux で回すテスト）に触れたら、
      2_RunAllTests.ps1 に -Linux を付ける（net10.0 の出力を Linux のコンテナで回す）。
      OS で実装が分かれる（CNG / OpenSSL、パスの区切り）のは、この 2 つのため。
      **docker が要る。** 無ければ 2 が NG になる（黙って省かない）。fixing では後回しにする。

    ＜検証スクリプト自身の変更＞
      .ps1 は表示だけの変更でも実行する（CODING.md 5 節）。
      変えたスクリプトが他の対象で回らないときは、軽い対象を 1 件だけ選んで回す。

    ＜段階（-Stage）＞
      コミットの段階で回す量を変える（#610）。施主のコミットは
      「fixing #N (phX)」（途中）と「fixed #N」（完了）に分かれる。
        fixed  … 割り出した範囲を全部回す（DB を使う対象を含む）。全部 OK なら記録を進める
        fixing … 重いものを後回しにする。**記録は進めない**（後回しにした分が残るため）
                 ・DB を使う対象（NeedDb）を外す
                 ・基盤に触れて通しになった場合、ビルドは基盤だけ、疎通は後回し
                   （単体テストは基盤の DLL だけを見るので、それで回せる）
                 ・VB の通しも後回し
      後回しにしたものは一覧で出す。**fixed で必ず回すこと。**

.PARAMETER Since
    差分の基点（コミット、ブランチ、HEAD~3 など）。省略時は .verified-commit の記録。
.PARAMETER Run
    割り出した範囲で 1 → 2 → 3 を実行する。全部 OK なら、HEAD を記録する。
.PARAMETER Record
    いまの HEAD を「検証の済んだコミット」として記録して終わる。
.PARAMETER IgnoreErrors
    1_BuildAll.ps1 に渡す既知のエラー。既定は 0_RunAll.ps1 と同じ。
.PARAMETER Stage
    fixed（既定）/ fixing。上の＜段階＞を参照。

.EXAMPLE
    .\SelectTargets.ps1                     # 割り出すだけ（何も実行しない）

.EXAMPLE
    .\SelectTargets.ps1 -Stage fixing -Run  # 途中のコミットの前。重いものは後回し

.EXAMPLE
    .\SelectTargets.ps1 -Run                # 割り出した範囲で実行する

.EXAMPLE
    .\SelectTargets.ps1 -Since develop      # develop からの変更で割り出す

.EXAMPLE
    .\SelectTargets.ps1 -Record             # 通しが通ったので、いまの HEAD を記録する

.NOTES
    作成者          ：玄人 幸道
    更新履歴        ：
     日時        更新者            内容
     ----------  ----------------  -------------------------------------------------
     2026/10/15  玄人 幸道         新規作成（#610）
     2026/10/15  玄人 幸道         -Stage（fixing / fixed）を追加（#610）
     2026/10/19  玄人 幸道         Public・Public.Security に触れたら、2_RunAllTests.ps1 に -Linux を付けるようにした（#632）
#>
[CmdletBinding()]
param(
    [string]$Since,
    [switch]$Run,
    [switch]$Record,
    [ValidateSet("fixed", "fixing")]
    [string]$Stage = "fixed",
    [string[]]$IgnoreErrors = @('error MSB(3482|3325|3321):.*WSClientWinCone_sample\.csproj')
)

$ErrorActionPreference = "Continue"

$progRoot   = $PSScriptRoot
$repoRoot   = [IO.Path]::GetFullPath((& git -C $progRoot rev-parse --show-toplevel))
# 検証の済んだコミットの記録。Git 管理外（.gitignore）。
$recordFile = Join-Path $progRoot ".verified-commit"

. (Join-Path $PSScriptRoot "SummaryTable.ps1")

# ------------------------------------------------------------------
# 記録の読み書き
# ------------------------------------------------------------------
function Get-RecordedCommit
{
    if (-not (Test-Path $recordFile)) { return $null }
    $line = @(Get-Content $recordFile -Encoding UTF8 | Where-Object { $_ -match '^[0-9a-f]{40}' }) | Select-Object -First 1
    if ($line) { return ($line -split '\s+')[0] }
    return $null
}

function Save-RecordedCommit
{
    $head  = (& git -C $repoRoot rev-parse HEAD)
    $dirty = @(& git -C $repoRoot status --porcelain).Count -gt 0

    # 1 行目がコミット、2 行目は人が読むための注記。
    $note = "# {0} に記録（{1}）" -f (Get-Date -Format "yyyy/MM/dd HH:mm"), `
        $(if ($dirty) { "未コミットの変更あり" } else { "ワーキング ツリーはクリーン" })
    Set-Content -Path $recordFile -Value @($head, $note) -Encoding UTF8

    Write-Host ("  検証の済んだコミットとして記録しました : {0}" -f $head) -ForegroundColor Green

    # **未コミットの変更は、次回も対象に入る。**
    #   基点は HEAD なので、HEAD との差分（＝未コミットの変更）は消えない。
    #   検証済みのものをもう一度回すことはあっても、漏れることはない。
    if ($dirty)
    {
        Write-Host "  未コミットの変更を含む状態でした。基点は HEAD のため、その分は次回も対象に入ります。" -ForegroundColor Yellow
    }
}

if ($Record)
{
    Save-RecordedCommit
    exit 0
}

# ------------------------------------------------------------------
# 基点
# ------------------------------------------------------------------
if (-not $Since)
{
    $Since = Get-RecordedCommit

    if (-not $Since)
    {
        Write-Host "  **検証の済んだコミットの記録がありません。**" -ForegroundColor Red
        Write-Host "  通し（0_RunAll.ps1）が通った後に -Record で記録するか、-Since で基点を指定してください。" -ForegroundColor Yellow
        Write-Host "    .\SelectTargets.ps1 -Record"
        Write-Host "    .\SelectTargets.ps1 -Since develop"
        exit 2
    }
}

$base = (& git -C $repoRoot rev-parse --verify --quiet "$Since^{commit}")
if (-not $base)
{
    Write-Host ("  **基点 '{0}' をコミットとして解決できません。**" -f $Since) -ForegroundColor Red
    exit 2
}

# ------------------------------------------------------------------
# 変更されたファイル
# ------------------------------------------------------------------
# **--no-renames で、移動元と移動先の両方を拾う。**
#   既定の改名検出では移動先しか出ず、移動元を参照していたものを見落とす。
# 未追跡のファイル（新規作成）も加える。
$changed = @(
    @(& git -C $repoRoot diff --name-only --no-renames $base)
    @(& git -C $repoRoot ls-files -o --exclude-standard)
) | Where-Object { $_ } | Sort-Object -Unique

Write-Host ("=== 基点 : {0}（{1}） ===" -f $base.Substring(0, 10), $Since) -ForegroundColor Cyan
Write-Host ("  変更されたファイル : {0} 件（未コミット・未追跡を含む）" -f $changed.Count)

# ------------------------------------------------------------------
# バッチ → フォルダ の対応
# ------------------------------------------------------------------
# バッチが建てる .sln / .csproj / .vbproj を読む。
#   ・パスはバッチのあるフォルダ（CS / VB）からの相対
#   ・set NUNIT_SLN=... のように引用符が無いものもあるので、トークンで拾う
#   ・"Samples\\2CS_sample\..." のように区切りが重なっているものがある
#
# **.sln は、束ねているプロジェクトのフォルダも対応付ける。**
#   .sln の置き場所の外にあるプロジェクトを参照していることがあるため。
function ConvertTo-RepoPath([string]$full)
{
    $full = [IO.Path]::GetFullPath($full)
    return ($full.Substring($repoRoot.Length).TrimStart('\', '/') -replace '\\', '/').ToLowerInvariant().TrimEnd('/') + '/'
}

function Get-BatDirs([string]$lang)
{
    $langDir = Join-Path $progRoot $lang
    $result  = @()

    foreach ($bat in (Get-ChildItem $langDir -Filter *.bat))
    {
        $text = [IO.File]::ReadAllText($bat.FullName)
        $dirs = @()

        foreach ($m in [regex]::Matches($text, '[A-Za-z0-9_.\\]+\.(sln|csproj|vbproj)\b'))
        {
            $full = Join-Path $langDir ($m.Value -replace '\\\\', '\')
            if (-not (Test-Path $full)) { continue }

            $dirs += ConvertTo-RepoPath (Split-Path $full)

            if ($full -like "*.sln")
            {
                foreach ($p in [regex]::Matches([IO.File]::ReadAllText($full),
                    'Project\("\{[^}]+\}"\)\s*=\s*"[^"]*",\s*"([^"]+\.(csproj|vbproj))"'))
                {
                    $proj = Join-Path (Split-Path $full) $p.Groups[1].Value
                    if (Test-Path $proj) { $dirs += ConvertTo-RepoPath (Split-Path $proj) }
                }
            }
        }

        foreach ($d in ($dirs | Sort-Object -Unique))
        {
            $result += [pscustomobject]@{ Dir = $lang; Bat = $bat.Name; Path = $d }
        }
    }

    return $result
}

$batDirs = @(Get-BatDirs "CS") + @(Get-BatDirs "VB")

# ------------------------------------------------------------------
# 検証スクリプト自身の変更
# ------------------------------------------------------------------
# Uses : どのスクリプトの実行で確かめられるか（1 / 2 / 3）
#        **1 つも無いものは手で実行する**（自動では回せない）。
$scriptUses = @{
    "1_buildall.ps1"    = @(1)
    "2_runalltests.ps1" = @(2)
    "compareresult.ps1" = @(2)
    "3_smoketest.ps1"   = @(3)
    "summarytable.ps1"  = @(1, 2, 3)
    "prerequisite.ps1"  = @(2, 3)
}

# 変えたスクリプトが、選ばれた対象では回らないときに使う軽い対象。
# **名前は各スクリプトの -List が一次情報。** 一覧に無ければ手作業に回す。
$probes = @{
    1 = "Framework_ToolCore"
    2 = "NUnit TestPublic (net10.0)"
    3 = "DaoGen_Tool /HELP (net10.0)"
}

# ------------------------------------------------------------------
# 分類
# ------------------------------------------------------------------
$full        = @{}            # 言語 → 通しの理由
$bats        = @{}            # "CS|xxx.bat" → 理由
$scriptRuns  = @{}            # 1/2/3 → 理由
$manual      = @()            # 手で確かめるもの
$rows        = @()
$linux       = $null          # -Linux を付ける理由（#632）

# **Linux でも回すもの**（#632）。Public・Public.Security と、それを Linux で回すテスト。
#   Public\Db の Dam*（Oracle / MySQL / PostgreSQL）は、Linux の回の対象（TestPublic）に入らないので外す。
$linuxPattern   = '^root/programs/cs/frameworks/(infrastructure/public/|tests/nunit/testpublic/|tests/golden/encanddecutilcui/)'
$linuxExclusion = '^root/programs/cs/frameworks/infrastructure/public/db/dam'

foreach ($f in $changed)
{
    $lf   = $f.ToLowerInvariant()
    $lang = $(if ($lf -match '^root/programs/vb/') { "VB" } else { "CS" })
    $kind = $null

    if (-not $linux -and $lf -match $linuxPattern -and $lf -notmatch $linuxExclusion -and $lf -notmatch '\.md$')
    {
        $linux = $f
    }

    if ($lf -match '\.md$')
    {
        $kind = "影響なし（文書）"
    }
    elseif ($lf -match '^\.github/' -or $lf -in @('.gitignore', '.gitattributes'))
    {
        # Actions は push 後に CI が回す。手元の 1/2/3 とは別物。
        $kind = "影響なし（リポジトリの設定）"
    }
    elseif ($lf -match '^root/programs/(cs|vb)/frameworks/infrastructure/')
    {
        $full[$lang] = "基盤に触れている"
        $kind = "通し（基盤）"
    }
    elseif ($lf -match '^root/programs/(cs|vb)/z_common2?\.bat$')
    {
        $full[$lang] = "全バッチの共通処理に触れている"
        $kind = "通し（共通バッチ）"
    }
    elseif ($lf -match '^root/programs/(cs|vb)/[^/]+\.sh$')
    {
        # WSL（Linux）で実行するもの。Windows の 1/2/3 では回らない。
        $manual += $f
        $kind = "手で確かめる（Linux で実行するもの）"
    }
    elseif ($f -match '^root/programs/(cs|vb)/([^/]+\.bat)$')
    {
        $bats["$lang|$($Matches[2])"] = $f
        $kind = "バッチ : $($Matches[2])"
    }
    elseif ($lf -match '^root/programs/([^/]+\.ps1)$')
    {
        # st_*.ps1 は 3_SmokeTest.ps1 がドット ソースで読む部品（#571）
        $name = $(if ($Matches[1] -like "st_*.ps1") { "3_smoketest.ps1" } else { $Matches[1] })
        if ($scriptUses.ContainsKey($name))
        {
            foreach ($u in $scriptUses[$name]) { $scriptRuns[$u] = $f }
            $kind = "スクリプト（{0} で確かめる）" -f (($scriptUses[$name] | ForEach-Object { "$_" }) -join "・")
        }
        else
        {
            $manual += $f
            $kind = "スクリプト（手で実行する）"
        }
    }
    else
    {
        $hits = @($batDirs | Where-Object { $_.Dir -eq $lang -and $lf.StartsWith($_.Path) })
        if ($hits.Count -gt 0)
        {
            foreach ($h in $hits) { $bats["$($h.Dir)|$($h.Bat)"] = $f }
            $kind = "バッチ : " + (($hits | ForEach-Object { $_.Bat } | Sort-Object -Unique) -join ", ")
        }
        elseif (-not (Test-Path (Join-Path $repoRoot $f)))
        {
            # **消えたファイルで、フォルダごと対応が無いもの。**
            #   フォルダを移動した場合の移動元がこれに当たる（--no-renames で拾っている）。
            #   移動先は別の行として現れ、そちらで対象が引ける。
            #   単独の削除なら、そのフォルダは残っていて上で引けている。
            $kind = "影響なし（移動元・削除済みのフォルダ）"
        }
        elseif ($lf -match '^root/programs/(cs|vb)/frameworks/tests/' -and
                ($proj = ((($lf -split '/') | Select-Object -First 7) -join '/') + "/") -and
                @($batDirs | Where-Object { $_.Dir -eq $lang -and $_.Path.StartsWith($proj) }).Count -gt 0)
        {
            # **テストのフォルダ直下で、.sln の外にあるもの。**
            #   例: Golden/TestBatch/Result*.txt（期待値。.sln は SimpleBatch/ の下）
            #   同じテストのフォルダを建てるバッチに対応付ける。
            $hits = @($batDirs | Where-Object { $_.Dir -eq $lang -and $_.Path.StartsWith($proj) })
            foreach ($h in $hits) { $bats["$($h.Dir)|$($h.Bat)"] = $f }
            $kind = "バッチ : " + (($hits | ForEach-Object { $_.Bat } | Sort-Object -Unique) -join ", ")
        }
        elseif ($lf -match '^root/programs/(cs|vb)/frameworks/tests/')
        {
            # **テストの末端で、どのバッチにも載っていないもの。**
            #   例: Else/TestLog（ビルドするバッチが無い）、Else/LaunchJson（テストではない）
            #   通しにしても回らないので、通しにはしない。手で確かめる。
            # フォルダ単位にまとめる（root/programs/CS/Frameworks/Tests/<区分>/<名前>）
            $manual += ((($f -split '/') | Select-Object -First 7) -join '/') + "/"
            $kind = "手で確かめる（どの検証にも載っていない）"
        }
        else
        {
            # **対応が引けないものは通し。** 迷ったら通し（AGENTS.md）。
            #   例: root/files/resource（実行時に読む定義ファイル）
            $full[$lang] = "対応が引けないファイルがある"
            $kind = "通し（対応が引けない）"
        }
    }

    $rows += [pscustomobject]@{ ファイル = $f; 扱い = $kind }
}

# **扱いごとにまとめて出す。** ファイルを全部並べると、移動を含む変更では数百行になる。
#   通しの理由になったファイルだけは、全部見せる（何が通しを招いたかが要るため）。
if ($rows.Count -gt 0)
{
    Write-Host ""
    $summary = @($rows | Group-Object 扱い | Sort-Object Name | ForEach-Object {
        [pscustomobject]@{ 扱い = $_.Name; 件数 = $_.Count; 例 = $_.Group[0].ファイル }
    })
    Write-SummaryTable $summary

    $causes = @($rows | Where-Object { $_.扱い -like "通し*" })
    if ($causes.Count -gt 0)
    {
        Write-Host ""
        Write-Host "  通しの理由になったファイル:" -ForegroundColor Yellow
        foreach ($c in ($causes | Select-Object -First 20)) { Write-Host ("    " + $c.ファイル) -ForegroundColor Yellow }
        if ($causes.Count -gt 20) { Write-Host ("    ... 他 {0} 件" -f ($causes.Count - 20)) -ForegroundColor Yellow }
    }
}

# ------------------------------------------------------------------
# 対象名を引く
# ------------------------------------------------------------------
# **片方の言語が通しなら、その言語は部分指定と混ぜずに通しにする。**
#   1_BuildAll.ps1 は「CS は通し、VB は一部」を 1 回では表せない。
#   VB だけが一部、という組み合わせなら、VB 側も通しにする（安全側）。
$listBuild = @(& (Join-Path $progRoot "1_BuildAll.ps1")    -Lang Both -List -PassThru 6>$null)
$listTest  = @(& (Join-Path $progRoot "2_RunAllTests.ps1") -List -PassThru 6>$null)
# **Linux の対象は分けておく**（#632）。-Linux を付けるときにだけ -Only に入れる。
#   混ぜると、-Linux を付けない回の -Only に入り、2_RunAllTests.ps1 が「一致する対象が無い」で止まる。
$listLinux = @($listTest | Where-Object { $_.Linux })
$listTest  = @($listTest | Where-Object { -not $_.Linux })
$listSmoke = @(& (Join-Path $progRoot "3_SmokeTest.ps1")   -Lang Both -List -PassThru 6>$null)

# **どの検証にも載っていないバッチは、手で実行する。**
#   例: y_Build_TestCode_SecGUI.bat（GUI の確認ツール。1/2/3 のどれも呼ばない）
#   黙って落とすと「回す必要が無い」と読めてしまう。
$known = @{}
foreach ($x in @($listBuild + $listTest + $listSmoke)) { $known["$($x.Dir)|$($x.Bat)"] = $true }
foreach ($key in @($bats.Keys))
{
    if (-not $known.ContainsKey($key))
    {
        $manual += ("{0}（バッチ {1} は 1/2/3 のどれにも載っていない）" -f $bats[$key], ($key -split '\|')[1])
    }
}

function Select-ByBat($list)
{
    return @($list | Where-Object { $bats.ContainsKey("$($_.Dir)|$($_.Bat)") -and -not $full.ContainsKey($_.Dir) } |
             ForEach-Object { $_.Name })
}

$plan = @{}

# 1: ビルド / 3: 疎通 は言語ごと。2: 単体テストは C# だけ。
foreach ($no in @(1, 3))
{
    $list  = $(if ($no -eq 1) { $listBuild } else { $listSmoke })
    $names = Select-ByBat $list
    $langs = @($full.Keys)

    # 部分指定の言語（通しでない言語の対象名）
    $partialLangs = @($list | Where-Object { $names -contains $_.Name } | ForEach-Object { $_.Dir } | Sort-Object -Unique)

    if ($langs.Count -gt 0 -and $partialLangs.Count -gt 0)
    {
        # 混在 → 部分指定の言語も通しにする（安全側）
        $langs += $partialLangs
        $names  = @()
    }

    $langs = @($langs | Sort-Object -Unique)

    if ($langs.Count -gt 0)
    {
        $plan[$no] = @{ Lang = $(if ($langs.Count -eq 2) { "Both" } else { $langs[0] }); Only = @() }
    }
    elseif ($names.Count -gt 0)
    {
        $plan[$no] = @{ Lang = $(if ($partialLangs.Count -eq 2) { "Both" } else { $partialLangs[0] }); Only = $names }
    }
}

if ($full.ContainsKey("CS"))
{
    $plan[2] = @{ Only = @() }
}
else
{
    $names = Select-ByBat $listTest
    if ($names.Count -gt 0) { $plan[2] = @{ Only = $names } }
}

# Linux（#632）。2 を回すなら -Linux を付ける。絞っているなら、Linux の対象も -Only に足す。
if ($linux -and $plan.ContainsKey(2))
{
    $plan[2].Linux = $true
    if ($plan[2].Only.Count -gt 0)
    {
        $plan[2].Only = @($plan[2].Only) + @($listLinux | ForEach-Object { $_.Name })
    }
}

# 検証スクリプトを変えたのに、その本が回らないなら、軽い対象を 1 件足す。
foreach ($no in @($scriptRuns.Keys | Sort-Object))
{
    if ($plan.ContainsKey($no)) { continue }

    $list = @($listBuild, $listTest, $listSmoke)[$no - 1]
    $probe = @($list | Where-Object { $_.Name -eq $probes[$no] })

    if ($probe.Count -eq 1)
    {
        $plan[$no] = @{ Lang = $probe[0].Dir; Only = @($probe[0].Name); Probe = $true }
    }
    else
    {
        $manual += ("{0}（軽い対象 '{1}' が一覧に無い）" -f $scriptRuns[$no], $probes[$no])
    }
}

# ------------------------------------------------------------------
# 段階 fixing : 重いものを後回しにする（#610）
# ------------------------------------------------------------------
# **後回しにしたものは $deferred に残し、必ず表示する。** 黙って落とすと「回した」と読める。
# 記録は進めない（後回しにした分を、次の fixed で拾うため）。
$deferred = @()

# 対象名の一覧から、DB を使うもの（NeedDb）を外す。外したものは後回しに積む。
function Remove-DbTargets([int]$no, $list)
{
    $p = $plan[$no]
    $cands = $(if ($p.Only.Count -eq 0) { @($list | Where-Object { $p.Lang -eq "Both" -or -not $p.Lang -or $_.Dir -eq $p.Lang } | ForEach-Object { $_.Name }) } else { @($p.Only) })
    $db    = @($list | Where-Object { $_.NeedDb -and $cands -contains $_.Name } | ForEach-Object { $_.Name })

    if ($db.Count -eq 0) { return }

    $script:deferred += ("{0} : DB を使う {1} 件（{2}）" -f $scriptNames[$no], $db.Count, ($db -join ", "))
    $keep = @($cands | Where-Object { $db -notcontains $_ })

    if ($keep.Count -eq 0) { $plan.Remove($no) } else { $p.Only = $keep }
}

$scriptNames = @{ 1 = "1_BuildAll.ps1"; 2 = "2_RunAllTests.ps1"; 3 = "3_SmokeTest.ps1" }

if ($Stage -eq "fixing")
{
    # --- 1 : 通しなら、基盤（C#）だけ建てる ---
    #   単体テストは基盤の DLL（Build_net48 / Build_netcore100）だけを見るので、それで回せる。
    #   ツール・サンプル・VB は疎通と一緒に fixed で建てる。
    if ($plan.ContainsKey(1) -and $plan[1].Only.Count -eq 0)
    {
        # **基盤のクリーンも含める。**（CHEATSHEET.md 1 節「-Only ＋ -SkipClean は万能ではない」、#574）
        #   net48 版と core 版のプロジェクトが同じフォルダにあり、前回の通しで最後に建てた
        #   core 版の restore 結果（obj）が残っていると、Business.RichClient (net48) が
        #   「does not reference .NETFramework,Version=v4.8」で落ちる（#610 で実際に踏んだ）。
        #   クリーンは CS 配下の bin / obj をすべて消すが、fixing では疎通を後回しにし、
        #   単体テストのバッチは自分でビルドし直すので困らない。
        $base = @($listBuild | Where-Object {
                $_.Dir -eq "CS" -and $_.Name -notlike "VB :*" -and (
                    $_.Bat -match '^(2_Build_NuGet|3_Build_Business|4_Build_CopyAssemblies)' -or
                    ($_.Bat -match '^1_Delete' -and $_.Name -like "*基盤*")) } |
            ForEach-Object { $_.Name })

        $deferred += ("1_BuildAll.ps1 : 基盤以外（ツール・サンプル{0}）" -f $(if ($plan[1].Lang -ne "CS") { "・VB" } else { "" }))
        $plan[1] = @{ Lang = "CS"; Only = $base; Base = $true }
    }

    # --- 2 : Linux は後回し（コンテナを使う。#632）---
    #   先に外す。Remove-DbTargets は -Only の名前を見るので、Linux の名前が残っていると混ざる。
    if ($plan.ContainsKey(2) -and $plan[2].Linux)
    {
        $deferred += "2_RunAllTests.ps1 : -Linux（Public・Public.Security に触れている）"
        $plan[2].Linux = $false
        $plan[2].Only  = @($plan[2].Only | Where-Object { @($listLinux | ForEach-Object { $_.Name }) -notcontains $_ })
    }

    # --- 2 : DB を使う対象を外す ---
    if ($plan.ContainsKey(2)) { Remove-DbTargets 2 $listTest }

    # --- 3 : 通しなら後回し。絞った範囲なら、DB を使う対象を外す ---
    if ($plan.ContainsKey(3))
    {
        if ($plan[3].Only.Count -eq 0)
        {
            $deferred += "3_SmokeTest.ps1 : 通し（基盤の変更の疎通は fixed で見る）"
            $plan.Remove(3)
        }
        else
        {
            Remove-DbTargets 3 $listSmoke
        }
    }
}

# ------------------------------------------------------------------
# 計画の表示
# ------------------------------------------------------------------

function Get-Splat([int]$no)
{
    $p = $plan[$no]
    $s = @{}

    if ($no -ne 2 -and $p.Lang -and $p.Lang -ne "CS") { $s.Lang = $p.Lang }
    if ($p.Only.Count -gt 0) { $s.Only = $p.Only }
    if ($no -eq 2 -and $p.Linux) { $s.Linux = $true }

    # **絞ったビルドは Clean を挟まない。**（CHEATSHEET.md 1 節）
    #   Clean は bin を再帰的に消すため、絞った範囲の外の成果物まで失う。
    if ($no -eq 1)
    {
        # 基盤だけのビルド（fixing）はクリーンを含めて建てる（上の理由）。
        if ($p.Only.Count -gt 0 -and -not $p.Base) { $s.SkipClean = $true }
        $s.IgnoreErrors = $IgnoreErrors
    }
    return $s
}

function Format-Command([int]$no)
{
    $s = Get-Splat $no
    $cmd = ".\" + $scriptNames[$no]
    if ($s.Lang)      { $cmd += " -Lang " + $s.Lang }
    if ($s.Only)      { $cmd += " -Only " + (($s.Only | ForEach-Object { "'$_'" }) -join ", ") }
    if ($s.SkipClean) { $cmd += " -SkipClean" }
    if ($s.Linux)     { $cmd += " -Linux" }
    if ($s.IgnoreErrors -and $no -eq 1) { $cmd += " -IgnoreErrors '...'" }
    return $cmd
}

Write-Host ""
Write-Host ("=== 回す範囲（段階 : {0}） ===" -f $Stage) -ForegroundColor Cyan

foreach ($k in ($full.Keys | Sort-Object))
{
    Write-Host ("  {0} は通し : {1}" -f $k, $full[$k]) -ForegroundColor Yellow
}
if ($plan.ContainsKey(2) -and $plan[2].Linux)
{
    Write-Host ("  Linux も回す : {0}" -f $linux) -ForegroundColor Yellow
}

if ($plan.Count -eq 0)
{
    Write-Host "  **回す必要のあるものはありません。**" -ForegroundColor Green
}

foreach ($no in @(1, 2, 3))
{
    if (-not $plan.ContainsKey($no))
    {
        Write-Host ("  {0,-18} 不要" -f $scriptNames[$no])
        continue
    }

    $p = $plan[$no]
    $what = $(if ($p.Only.Count -eq 0) { "通し" } elseif ($p.Probe) { "スクリプトの確認（1 件）" }
              elseif ($p.Base) { "基盤だけ（{0} ステップ）" -f $p.Only.Count } else { "{0} 件" -f $p.Only.Count })
    Write-Host ("  {0,-18} {1}" -f $scriptNames[$no], $what)
    Write-Host ("      " + (Format-Command $no)) -ForegroundColor DarkGray
}

if ($manual.Count -gt 0)
{
    Write-Host ""
    Write-Host "  **手で確かめるもの（自動では回せない）:**" -ForegroundColor Yellow
    $manual = @($manual | Sort-Object -Unique)
    foreach ($m in ($manual | Select-Object -First 15)) { Write-Host ("    " + $m) -ForegroundColor Yellow }
    if ($manual.Count -gt 15) { Write-Host ("    ... 他 {0} 件" -f ($manual.Count - 15)) -ForegroundColor Yellow }
}

if ($deferred.Count -gt 0)
{
    Write-Host ""
    Write-Host "  **後回しにしたもの（fixed で必ず回す）:**" -ForegroundColor Yellow
    foreach ($d in $deferred) { Write-Host ("    " + $d) -ForegroundColor Yellow }
}

if (-not $Run)
{
    Write-Host ""
    Write-Host "  実行するには -Run を付けます。順序は 1 → 2 → 3 で固定です。"
    exit 0
}

# ------------------------------------------------------------------
# 実行
# ------------------------------------------------------------------
# コード ページは先に切り替える（2_RunAllTests.ps1 と同じ理由）。
#   子のバッチの chcp 65001 で画面が再描画され、ここまでの表示が消えるため。
if ((cmd /c chcp) -notmatch '65001') { cmd /c chcp 65001 | Out-Null }
if ([Console]::OutputEncoding.CodePage -ne 65001)
{
    [Console]::OutputEncoding = New-Object Text.UTF8Encoding $false
}

$results = @()

foreach ($no in @(1, 2, 3))
{
    if (-not $plan.ContainsKey($no))
    {
        $results += [pscustomobject]@{ スクリプト = $scriptNames[$no]; 終了コード = "不要"; 秒 = "-" }
        continue
    }

    $splat = Get-Splat $no
    Write-Host ""
    Write-Host ("################ {0} ################" -f (Format-Command $no)) -ForegroundColor Cyan

    $sw = [Diagnostics.Stopwatch]::StartNew()
    & (Join-Path $progRoot $scriptNames[$no]) @splat
    $code = $LASTEXITCODE
    $sw.Stop()

    $results += [pscustomobject]@{
        スクリプト = $scriptNames[$no]
        終了コード = $code
        秒         = ("{0:N1}" -f $sw.Elapsed.TotalSeconds)
    }
}

Write-Host ""
Write-Host "================ 絞り込みのまとめ ================"
Write-Host ""
Write-SummaryTable $results
Write-Host ""

$ng = @($results | Where-Object { $_.終了コード -ne 0 -and $_.終了コード -ne "不要" })

# **fixing では記録しない。** 後回しにした分が回っていないため（#610）。
#   ここで記録すると、次の fixed で後回しの分が対象から外れてしまう。
if ($Stage -eq "fixing")
{
    if ($ng.Count -eq 0)
    {
        Write-Host "  fixing の範囲はすべて OK です（記録は進めません）。" -ForegroundColor Green
        if ($deferred.Count -gt 0)
        {
            Write-Host ("  後回しにしたもの {0} 件は、fixed で回してください。" -f $deferred.Count) -ForegroundColor Yellow
        }
        exit 0
    }
    Write-Host ("  {0} 本が 0 以外で終了しました。" -f $ng.Count) -ForegroundColor Yellow
    exit 1
}

# **記録するのは、全部 OK で、手作業が残っていないときだけ。**
#   手作業が残っているのに記録すると、次回その変更が対象から外れる。
if ($ng.Count -eq 0 -and $manual.Count -eq 0)
{
    Write-Host "  すべて OK です。" -ForegroundColor Green
    Save-RecordedCommit
    exit 0
}

if ($ng.Count -gt 0)
{
    Write-Host ("  {0} 本が 0 以外で終了しました。記録は更新しません。" -f $ng.Count) -ForegroundColor Yellow
}
else
{
    Write-Host "  手で確かめるものが残っているため、記録は更新しません。" -ForegroundColor Yellow
    Write-Host "  確かめたら -Record で記録してください。" -ForegroundColor Yellow
}
exit 1
