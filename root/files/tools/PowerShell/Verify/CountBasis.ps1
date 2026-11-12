<#
.SYNOPSIS
    NUnit のテストの期待値の根拠（[Basis]・[Observation]）を、フォルダごとに数える（QUALITY.md 6 節・7 節。#671）。

.DESCRIPTION
    root/programs/CS/Frameworks/Tests/NUnit の下の .cs を読み、テスト メソッド（[Test]・[TestCase]・[TestCaseSource] が付いたもの）ごとに分ける。

      検証   … [Basis] が 1 つ以上ある（クラスに付いた [Basis] も、そのクラスの全テストに掛かる）
      観測   … [Basis] が無く、[Observation] がある
      未分類 … どちらも無い

    検証のテストは、根拠の種類でも分ける（1 つのテストに複数の [Basis] があれば、上の種類を採る）。

      仕様   … RFC・OIDC・FAPI・JWS など、リポジトリの外の仕様を指すもの（「節あり」は、その中で節番号まで書いたもの。目安）
      別実装 … 「別実装：」で始まるもの
      設計   … 「設計：」で始まるもの
      Issue  … 上のどれでもなく、Issue の番号（#NNN）を指すもの
      その他 … 上のどれでもないもの

    [TestCase] が並ぶメソッドは、1 件と数える（ケースの数ではなく、メソッドの数）。
    リスクが「高」の名前空間（QUALITY.md 1 節）のテストのフォルダには、★ を付ける。

    **判定はしない。** 根拠の内容が正しいかは見ない（原典で確かめるのは人とレビューの役目）。
    ソースを行で読む簡易な数え方で、コメント アウトした属性（// で始まる行）は数えない。

.PARAMETER Root
    NUnit のテストのフォルダ。省略時は本スクリプトの隣の CS\Frameworks\Tests\NUnit。

.EXAMPLE
    .\CountBasis.ps1

.NOTES
    作成者          ：玄人 幸道
    更新履歴        ：
     日時        更新者            内容
     ----------  ----------------  -------------------------------------------------
     2026/11/09  玄人 幸道         新規作成（#671。汎用認証サイトの CountBasis.ps1 を、NUnit の属性を数える形にした）
     2026/11/12  玄人 幸道         root/files/tools/PowerShell/Verify に移した（#687）
#>
[CmdletBinding()]
param(
    [string]$Root
)

$ErrorActionPreference = "Stop"

# パスの既定値は param() に書かない（5.1 の -File で $PSScriptRoot が空になる。CODING.md 5 節）
if (-not $Root)
{
    # 本スクリプトは root\files\tools\PowerShell\Verify に置く（#687）。NUnit のテストは root\programs の下にある。
    $Root = Join-Path (Split-Path -Parent $PSCommandPath) "..\..\..\..\programs\CS\Frameworks\Tests\NUnit"
}
$Root = (Resolve-Path -LiteralPath $Root).Path

# 外部の仕様の名前。**大文字と小文字を区別する**
$specPattern = 'RFC ?\d|OIDC|OpenID|FAPI|SAML|OAuth|CIBA|JARM|DPoP|PKCE|JWS|JWE|JWK|JWA|JWT|NIST|FIPS|W3C|XML ?(Signature|Encryption|Signing)|ISO|ECMA|SP 800'
# リスクが「高」の名前空間のテストのフォルダ（QUALITY.md 1 節）
$highPattern = '^TestPublic/Security(/|$)|^TestFramework/Authentication(/|$)'

$attrStringPattern = '\[\s*{0}\s*\(\s*"((?:[^"\\]|\\.)*)"'
$basisPattern = $attrStringPattern -f 'Basis'
$obsPattern   = $attrStringPattern -f 'Observation'
$testPattern  = '[\[,]\s*(Test|TestCase|TestCaseSource)\s*[\],(]'

function Get-Kind([string[]]$Bases)
{
    $kinds = foreach ($b in $Bases)
    {
        if ($b -cmatch $specPattern) { 0 }
        elseif ($b -match '^\s*別実装') { 1 }
        elseif ($b -match '^\s*設計') { 2 }
        elseif ($b -match '#\d+') { 3 }
        else { 4 }
    }
    return @('仕様', '別実装', '設計', 'Issue', 'その他')[($kinds | Measure-Object -Minimum).Minimum]
}

function Test-HasSection([string[]]$Bases)
{
    # RFC の番号と、版（1.0 など）を除いて、まだ数字（節）が残れば「節あり」とする（目安）
    foreach ($b in $Bases)
    {
        if ($b -cnotmatch $specPattern) { continue }
        $rest = ($b -replace 'RFC ?\d+', '') -replace '\b\d+\.0\b', '' -replace 'SP 800-\d+\w*', ''
        if ($rest -match '§|\b\d+(\.\d+)*\b') { return $true }
    }
    return $false
}

$groups = [ordered]@{}
$files = Get-ChildItem -LiteralPath $Root -Recurse -Filter *.cs |
    Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' } | Sort-Object FullName

foreach ($f in $files)
{
    $rel = $f.DirectoryName.Substring($Root.Length).TrimStart('\') -replace '\\', '/'
    if (-not $rel) { $rel = '.' }

    $pendingAttr = New-Object System.Collections.Generic.List[string]
    $classBases  = @()
    $classObs    = @()

    foreach ($line in (Get-Content -LiteralPath $f.FullName -Encoding UTF8))
    {
        $t = $line.Trim()
        if ($t -eq '' -or $t.StartsWith('//')) { continue }

        if ($t.StartsWith('['))
        {
            $pendingAttr.Add($t)
            continue
        }

        $attrs = ($pendingAttr -join ' ')
        $bases = @([regex]::Matches($attrs, $basisPattern) | ForEach-Object { $_.Groups[1].Value })
        $obs   = @([regex]::Matches($attrs, $obsPattern) | ForEach-Object { $_.Groups[1].Value })

        if ($t -match '\bclass\s+\w+')
        {
            # クラスに付いた属性は、そのクラスの全テストに掛かる（入れ子のクラスは考えない）
            $classBases = $bases
            $classObs   = $obs
        }
        elseif ($attrs -match $testPattern -and $t -match '\(')
        {
            if (-not $groups.Contains($rel))
            {
                $groups[$rel] = [pscustomobject]@{ フォルダ = $rel; 計 = 0; 検証 = 0; 観測 = 0; 未分類 = 0; 仕様 = 0; 節あり = 0; 別実装 = 0; 設計 = 0; Issue = 0; その他 = 0 }
            }
            $g = $groups[$rel]
            $g.計++
            $allBases = @($bases + $classBases)
            if ($allBases.Count -gt 0)
            {
                $g.検証++
                $kind = Get-Kind $allBases
                $g.$kind++
                if ($kind -eq '仕様' -and (Test-HasSection $allBases)) { $g.節あり++ }
            }
            elseif (@($obs + $classObs).Count -gt 0) { $g.観測++ }
            else { $g.未分類++ }
        }

        $pendingAttr.Clear()
    }
}

function Format-Rate([int]$Value, [int]$All)
{
    if ($All -eq 0) { return "0" }
    return ("{0}（{1:N0}%）" -f $Value, (100.0 * $Value / $All))
}

function Get-Total([object[]]$Rows, [string]$Name)
{
    $t = [pscustomobject]@{ フォルダ = $Name; 計 = 0; 検証 = 0; 観測 = 0; 未分類 = 0; 仕様 = 0; 節あり = 0; 別実装 = 0; 設計 = 0; Issue = 0; その他 = 0 }
    foreach ($p in @('計', '検証', '観測', '未分類', '仕様', '節あり', '別実装', '設計', 'Issue', 'その他'))
    {
        $t.$p = [int](($Rows | Measure-Object $p -Sum).Sum)
    }
    return $t
}

$rows = @($groups.Values)
$high = @($rows | Where-Object { $_.フォルダ -match $highPattern })

"| フォルダ | テスト | 検証 | 観測 | 未分類 | 仕様（うち節あり） | 別実装 | 設計 | Issue | その他 |"
"|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|"
foreach ($r in ($rows + (Get-Total $high '計（リスク「高」★）') + (Get-Total $rows '計（全体）')))
{
    $name = $(if ($r.フォルダ -match $highPattern) { '★ ' + $r.フォルダ } else { $r.フォルダ })
    "| {0} | {1} | {2} | {3} | {4} | {5}（{6}） | {7} | {8} | {9} | {10} |" -f $name, $r.計,
        (Format-Rate $r.検証 $r.計), (Format-Rate $r.観測 $r.計), (Format-Rate $r.未分類 $r.計),
        $r.仕様, $r.節あり, $r.別実装, $r.設計, $r.Issue, $r.その他
}
