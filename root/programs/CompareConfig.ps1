<#
.SYNOPSIS
    CS 版と VB 版の設定ファイルを、XML の要素・属性で突き合わせる。

.DESCRIPTION
    「CS と VB の設定ファイルは同じはずだ」と考えて丸ごとコピーすると、
    **VB 固有の記述が消える。** 実際に消した（#549 の作業中）。

    素の diff ではコメント・空白・改行・BOM の差に埋もれて、**値の差が見えない。**
    本スクリプトは XML の要素と属性だけを取り出して比較するため、
    **値が変わったかどうかだけ**が分かる。

    ＜CI では代わりにならない＞

      VB の疎通テストは 6 対象しか動かさないため、**差分のあるファイルの半分以上は
      一度も実行されない。** 実行される分でも、`compilation/assemblies` の欠落は
      `0_RunAll.ps1 -Lang Both` を通過した（実測）。
      **CI は「壊れていないこと」の証拠にはなるが、「意図どおり」の証拠にはならない。**

    ＜違ってよい差分がある＞

      自動生成（`startup` / `bindingRedirect` / `packages.config`）は、
      そもそも同期の対象ではない。
      **同じにすると壊れる差分**もある（埋め込みリソース名。CS はフォルダ名を含み、
      VB は含まない）。判断の材料は [`CONFIGURATION.md`](CONFIGURATION.md) 11 節。

.PARAMETER Detail
    差分の内訳（どの要素・属性が増減したか）を表示する。

.PARAMETER Only
    対象を相対パスの部分一致で絞る（例: -Only "WebApp_sample"）。

.PARAMETER Max
    -Detail のときに、1 組あたり表示する件数の上限。既定は 20。

.PARAMETER Check
    **合否を返す。** 差分を「違ってよい種類」（下記）と突き合わせ、
    そこに当てはまらない差分が 1 件でもあれば終了コード 1 を返す。

    ＜違ってよい種類＞

      /configuration/startup 配下              VS が VB プロジェクトに自動で入れる
      /configuration/runtime 配下              NuGet が生成する bindingRedirect
      system.web/compilation/assemblies 配下   VB は明示参照が要ることがある
      appSettings の SqlTextFilePath           **同じにすると壊れる**（言語で
                                               　埋め込みリソース名の規則が違う）
      appSettings の ClientSettingsProvider.*  VS のテンプレート由来

    **この一覧は「今そうなっている」ではなく「そうであってよい」を表す。**
    増やすときは、なぜ違ってよいのかを CONFIGURATION.md 11 節にも書くこと。

.EXAMPLE
    .\CompareConfig.ps1

.EXAMPLE
    .\CompareConfig.ps1 -Detail -Only "WebApp_sample"

.EXAMPLE
    .\CompareConfig.ps1 -Check

.NOTES
    作成者          ：玄人 幸道
    更新履歴        ：
     日時        更新者            内容
     ----------  ----------------  -------------------------------------------------
     2026/08/16  玄人 幸道         新規作成（#553）
     2026/08/16  玄人 幸道         -Check を追加（#553）

    -Check を付けない限り、終了コードは常に 0。
    **差分があること自体は異常ではない**ため、既定は一覧として使う。
#>
[CmdletBinding()]
param(
    [switch]$Detail,
    [string]$Only,
    [int]$Max = 20,
    [switch]$Check
)

# ------------------------------------------------------------------
# 違ってよい差分（-Check のときだけ使う）
# ------------------------------------------------------------------
#   要素のパスと属性に対する正規表現。**当てはまるものは「想定内」**とみなす。
$allowed = @(
    '^/configuration/startup'
    '^/configuration/runtime'
    '^/configuration/system\.web/compilation/assemblies'
    '^/configuration/appSettings/add \[key=SqlTextFilePath'
    '^/configuration/appSettings/add \[key=ClientSettingsProvider\.'
)

$ErrorActionPreference = "Continue"

# ------------------------------------------------------------------
# カレント ディレクトリからの実行を許可する
# ------------------------------------------------------------------
$root = $PSScriptRoot
. (Join-Path $root "SummaryTable.ps1")

# Windows PowerShell 5.1 は既定で ANSI 出力になるため、UTF-8 に揃える。
if ([Console]::OutputEncoding.CodePage -ne 65001)
{
    [Console]::OutputEncoding = New-Object Text.UTF8Encoding $false
}

# ------------------------------------------------------------------
# XML から「要素のパスと属性」だけを取り出す
# ------------------------------------------------------------------
#   コメント・空白・改行・BOM は落ちる。**値の差だけが残る。**
function Get-Elements([string]$path)
{
    $xml = New-Object System.Xml.XmlDocument
    $xml.PreserveWhitespace = $false
    try { $xml.Load($path) } catch { return $null }

    $list = New-Object System.Collections.Generic.List[string]

    function Walk($node, [string]$parentPath)
    {
        if ($node.NodeType -ne [System.Xml.XmlNodeType]::Element) { return }

        $here = $parentPath + "/" + $node.Name

        $attrs = ""
        if ($node.Attributes -and $node.Attributes.Count -gt 0)
        {
            $pairs = @()
            foreach ($a in ($node.Attributes | Sort-Object Name))
            {
                $pairs += ($a.Name + "=" + $a.Value)
            }
            $attrs = " [" + ($pairs -join "; ") + "]"
        }

        $list.Add($here + $attrs)
        foreach ($c in $node.ChildNodes) { Walk $c $here }
    }

    Walk $xml.DocumentElement ""
    return $list
}

# ------------------------------------------------------------------
# 対象の組を作る
# ------------------------------------------------------------------
$csRoot = Join-Path $root "CS"
$vbRoot = Join-Path $root "VB"

# **Git の追跡下にあるものだけを対象にする。**
#   ビルド出力（Build\*.dll.config など）や NuGet の展開物（packages\）を拾うと、
#   「生成物どうしを比べる」ことになり、判断できない差分ばかりが並ぶ。
$tracked = & git -C $root ls-files
if ($LASTEXITCODE -ne 0 -or -not $tracked)
{
    Write-Host "  git ls-files が使えないため、中止する。" -ForegroundColor Red
    exit 0
}

$targets = @()
foreach ($t in $tracked)
{
    if ($t -notmatch '\.config$' -and $t -notmatch 'appsettings.*\.json$') { continue }
    if ($t -match '/packages/') { continue }
    if ($t -notlike "CS/*") { continue }
    $targets += Join-Path $root ($t -replace '/', '\')
}

$pairs = @()
foreach ($full in $targets)
{
    $f   = Get-Item $full -EA SilentlyContinue
    if (-not $f) { continue }
    $rel = $f.FullName.Substring($csRoot.Length + 1)
    $vb  = Join-Path $vbRoot $rel

    # VB 側はファイル名の大文字小文字が違うことがある（web.config / Web.config）。
    if (-not (Test-Path $vb))
    {
        $dir = Split-Path $vb -Parent
        if (Test-Path $dir)
        {
            $hit = Get-ChildItem $dir -File -EA SilentlyContinue |
                   Where-Object { $_.Name -eq (Split-Path $vb -Leaf) } | Select-Object -First 1
            if ($hit) { $vb = $hit.FullName } else { continue }
        }
        else { continue }
    }

    if ($Only -and ($rel -notlike "*$Only*")) { continue }
    $pairs += [PSCustomObject]@{ Rel = $rel; CS = $f.FullName; VB = $vb }
}

# ------------------------------------------------------------------
# 突き合わせ
# ------------------------------------------------------------------
$rows    = @()
$same    = 0
$diff    = 0
$skipped = 0
$details = @()

foreach ($p in $pairs)
{
    $a = Get-Elements $p.CS
    $b = Get-Elements $p.VB

    if ($null -eq $a -or $null -eq $b)
    {
        $skipped++
        $rows += [PSCustomObject]@{ 対象 = $p.Rel; 結果 = "SKIP"; 内容 = "XML として読めない" }
        continue
    }

    # 並びまで同じか
    $seqSame = (($a -join "`n") -eq ($b -join "`n"))
    $onlyCS  = @(Compare-Object $a $b | Where-Object { $_.SideIndicator -eq "<=" } | ForEach-Object { $_.InputObject })
    $onlyVB  = @(Compare-Object $a $b | Where-Object { $_.SideIndicator -eq "=>" } | ForEach-Object { $_.InputObject })

    if ($seqSame)
    {
        $same++
        $rows += [PSCustomObject]@{ 対象 = $p.Rel; 結果 = "同一"; 内容 = "" }
    }
    elseif ($onlyCS.Count -eq 0 -and $onlyVB.Count -eq 0)
    {
        # 集合は同じで順序だけが違う。実害は無い。
        $same++
        $rows += [PSCustomObject]@{ 対象 = $p.Rel; 結果 = "同一"; 内容 = "順序のみ相違" }
    }
    else
    {
        $diff++
        $rows += [PSCustomObject]@{
            対象 = $p.Rel; 結果 = "差分"
            内容 = ("CS のみ {0} / VB のみ {1}" -f $onlyCS.Count, $onlyVB.Count)
        }
        $details += [PSCustomObject]@{ Rel = $p.Rel; OnlyCS = $onlyCS; OnlyVB = $onlyVB }
    }
}

# ------------------------------------------------------------------
# 出力
# ------------------------------------------------------------------
Write-Host ""
Write-Host "================ 突き合わせ ================"
Write-SummaryTable -Rows $rows -Columns @("対象", "結果", "内容")

Write-Host ""
Write-Host ("  対象 {0} 組 : 同一 {1} / 差分 {2} / 読めない {3}" -f $pairs.Count, $same, $diff, $skipped)
Write-Host "  違ってよい差分の判断は CONFIGURATION.md 11 節。"

# ------------------------------------------------------------------
# 合否（-Check）
# ------------------------------------------------------------------
$unexpected = @()

if ($Check)
{
    foreach ($d in $details)
    {
        foreach ($x in (@($d.OnlyCS) + @($d.OnlyVB)))
        {
            $ok = $false
            foreach ($pat in $allowed)
            {
                if ($x -match $pat) { $ok = $true; break }
            }
            if (-not $ok)
            {
                $unexpected += [PSCustomObject]@{ Rel = $d.Rel; Item = $x }
            }
        }
    }

    Write-Host ""
    Write-Host "================ 判定 ================"

    if ($unexpected.Count -eq 0)
    {
        Write-Host "  想定外の差分なし。" -ForegroundColor Green
    }
    else
    {
        Write-Host ("  **想定外の差分 {0} 件**" -f $unexpected.Count) -ForegroundColor Red
        Write-Host ""
        foreach ($u in $unexpected)
        {
            Write-Host ("  " + $u.Rel)
            Write-Host ("    " + $u.Item)
        }
        Write-Host ""
        Write-Host "  意図した差分なら CompareConfig.ps1 の `$allowed に足し、"
        Write-Host "  **なぜ違ってよいのかを CONFIGURATION.md 11 節にも書くこと。**"
    }
}

if ($Detail -and $details.Count -gt 0)
{
    foreach ($d in $details)
    {
        Write-Host ""
        Write-Host ("### " + $d.Rel)
        foreach ($x in ($d.OnlyCS | Select-Object -First $Max)) { Write-Host ("  CS: " + $x) }
        if ($d.OnlyCS.Count -gt $Max) { Write-Host ("  CS: ... 他 " + ($d.OnlyCS.Count - $Max) + " 件") }
        foreach ($x in ($d.OnlyVB | Select-Object -First $Max)) { Write-Host ("  VB: " + $x) }
        if ($d.OnlyVB.Count -gt $Max) { Write-Host ("  VB: ... 他 " + ($d.OnlyVB.Count - $Max) + " 件") }
    }
}

Write-Host ""

if ($Check -and $unexpected.Count -gt 0) { exit 1 }
exit 0
