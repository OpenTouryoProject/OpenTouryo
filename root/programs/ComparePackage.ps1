<#
.SYNOPSIS
    packages.config の版と、csproj / vbproj に書かれた版が揃っているかを突き合わせる。

.DESCRIPTION
    **パッケージの版は 4 か所に散らばっている。**（#566 / #568 で全部踏んだ）

      | | 場所                                          | 検査              |
      |---|-----------------------------------------------|-------------------|
      | ① | packages.config の version=                    | 本スクリプト（基準）|
      | ② | csproj の packages\X.Y\ というパス表記         | **本スクリプト**  |
      | ③ | csproj の <Reference Include="…, Version=">    | **本スクリプト**  |
      | ④ | *.config の bindingRedirect                    | CompareRedirect.ps1（#556）|

      ①を基準に、②③がそこから外れていないかを見る。

    ＜②がずれると＞

      **復元済みでも「パッケージが無い」と言われる。**

        error : このプロジェクトは、このコンピューター上にない NuGet パッケージを参照しています。
                見つからないファイルは ..\packages\System.ValueTuple.4.6.2\build\net471\
                System.ValueTuple.targets です。

      **HintPath だけを見ても足りない。**
      Import Project と Error Condition にも版が入るため、
      `packages\<フォルダ>\` を**全部拾って**突き合わせる。

    ＜③がずれると＞

      **参照が落ちて、bin に配られない。**

      <Reference Include> に強い名前（, Version=…）を書くと、
      SpecificVersion は既定で true になる。宣言と実体がずれると、
      MSBuild は**警告だけ出して参照を落とす。ビルドは成功する。**

      #566 では Microsoft.Data.SqlClient.dll が bin に配られず、
      実行時に FileNotFoundException になった。**0_RunAll.ps1 は通っていた。**

    ＜③は「パッケージの版」ではない＞

      **アセンブリの版である。計算で導いてはいけない。**

        パッケージ 10.0.5  → アセンブリ 10.0.0.5
        パッケージ 8.17.0  → アセンブリ 8.17.0.0
        パッケージ 13.0.4  → アセンブリ 13.0.0.0   ← 版が変わっても、ここは動かない

      HintPath が指す DLL を読んで測る。

    ＜そのプロジェクトの HintPath だけを見る＞

      **リポジトリ全体から名前で引いてはいけない。**（#556 と同じ理由）
      Microsoft.Owin が 4.2.2.0 と 4.2.3.0 に割れているため、
      全体から引くと、どちらが正かを決められない。

    ＜復元してから実行すること＞

      ②は復元前でも判定できる（テキストどうしの突き合わせ）。
      **③は DLL を読むので、復元していないと「判定不能」になる。**

      0_RunAll.ps1 に組み込むなら 1_BuildAll.ps1 の後。

      **「判定不能」は「問題なし」ではない。** 材料が無いだけである。
      不一致には数えないが、件数は必ず出す。

    ＜対象外＞

      **CS/NuGet/proj 配下は見ない。** NuGet パッケージの検証用で、
      1_BuildAll.ps1 が建てないため、必ず「判定不能」になる。
      CompareRedirect.ps1 と同じ扱い（#557）。

      **商用パッケージは復元されない。** DamDB2 の IBM.Data.DB2 系は
      nuget.org に無いため、③は常に「判定不能」になる。これは正しい。

.PARAMETER Detail
    「判定不能」の内訳も表示する。

.PARAMETER Only
    対象を相対パスの部分一致で絞る（例: -Only "ASPNETWebService"）。

.PARAMETER Check
    **合否を返す。** 不一致が 1 件でもあれば終了コード 1 を返す。
    「判定不能」では落とさない。

.EXAMPLE
    .\ComparePackage.ps1

.EXAMPLE
    .\ComparePackage.ps1 -Detail -Only "VB"

.EXAMPLE
    .\ComparePackage.ps1 -Check
#>
[CmdletBinding()]
param(
    [switch]$Detail,
    [string]$Only,
    [switch]$Check
)

$ErrorActionPreference = "Continue"

$root = $PSScriptRoot
. (Join-Path $root "SummaryTable.ps1")

if ([Console]::OutputEncoding.CodePage -ne 65001)
{
    [Console]::OutputEncoding = New-Object Text.UTF8Encoding $false
}

# ------------------------------------------------------------------
# packages.config から、宣言された版を読む
# ------------------------------------------------------------------
function Get-DeclaredPackages([string]$path)
{
    $map = @{}
    try { [xml]$x = Get-Content $path -Raw -EA Stop } catch { return $map }

    foreach ($p in $x.SelectNodes("//*[local-name()='package']"))
    {
        $id  = $p.GetAttribute("id")
        $ver = $p.GetAttribute("version")
        if ($id -and $ver) { $map[$id] = $ver }
    }
    return $map
}

# ------------------------------------------------------------------
# packages\<フォルダ>\ から、パッケージ ID を割り出す
# ------------------------------------------------------------------
# フォルダ名は「<id>.<version>」で、どちらもドットを含むため、
# **右から削りながら、宣言に在る ID を探す。**
function Resolve-PackageId([string]$folder, [hashtable]$declared)
{
    $id = $folder
    while ($id -and -not $declared.ContainsKey($id) -and $id.Contains("."))
    {
        $id = $id.Substring(0, $id.LastIndexOf("."))
    }
    if ($declared.ContainsKey($id)) { return $id }
    return $null
}

# ------------------------------------------------------------------
# 対象（追跡下の packages.config と、同じフォルダの csproj / vbproj）
# ------------------------------------------------------------------
$tracked = & git -C $root ls-files
if ($LASTEXITCODE -ne 0 -or -not $tracked)
{
    Write-Host "  git ls-files が使えないため、中止する。" -ForegroundColor Red
    exit 0
}

$rows     = @()
$mismatch = @()
$unknown  = @()
$total    = 0
$okCount  = 0

foreach ($t in $tracked)
{
    if ($t -notmatch '/packages\.config$') { continue }
    if ($t -match '/packages/')            { continue }

    # 通しビルドの対象外は見ない（#557。CompareRedirect.ps1 と同じ）
    if ($t -match '/NuGet/proj/') { continue }

    if ($Only -and ($t -notlike "*$Only*")) { continue }

    $full = Join-Path $root ($t -replace '/', '\')
    if (-not (Test-Path $full)) { continue }

    $declared = Get-DeclaredPackages $full
    if ($declared.Count -eq 0) { continue }

    $rel = $t -replace '^root/programs/', ''
    $dir = Split-Path $full -Parent

    $projs = @(Get-ChildItem $dir -File -EA SilentlyContinue |
               Where-Object { $_.Extension -in @(".csproj", ".vbproj") })
    if ($projs.Count -eq 0) { continue }

    $ok = 0; $ng = 0; $unk = 0

    foreach ($proj in $projs)
    {
        $text = Get-Content $proj.FullName -Raw -EA SilentlyContinue
        if (-not $text) { continue }

        # ---------- ② packages\<フォルダ>\ ----------
        # HintPath / Import Project / Error Condition の別を問わず、全部拾う。
        $folders = [regex]::Matches($text, 'packages\\([^\\]+)\\') |
                   ForEach-Object { $_.Groups[1].Value } |
                   Sort-Object -Unique

        foreach ($folder in $folders)
        {
            $id = Resolve-PackageId $folder $declared
            if (-not $id) { continue }

            $total++
            $want = "$id.$($declared[$id])"

            if ($folder -eq $want)
            {
                $ok++
            }
            else
            {
                $ng++
                $mismatch += [PSCustomObject]@{
                    対象   = $rel
                    種類   = "パス表記"
                    名前   = $id
                    宣言   = $want
                    記述   = $folder
                }
            }
        }

        # ---------- ③ <Reference Include="…, Version="> ----------
        $refs = [regex]::Matches($text,
            '<Reference\s+Include="([^",]+)([^"]*)"\s*>(.*?)</Reference>',
            [Text.RegularExpressions.RegexOptions]::Singleline)

        foreach ($m in $refs)
        {
            $name = $m.Groups[1].Value
            $rest = $m.Groups[2].Value
            $body = $m.Groups[3].Value

            $vm = [regex]::Match($rest, 'Version=([\d\.]+)')
            if (-not $vm.Success) { continue }

            $hm = [regex]::Match($body, '<HintPath>(.*?)</HintPath>',
                                 [Text.RegularExpressions.RegexOptions]::Singleline)
            if (-not $hm.Success) { continue }

            $total++

            # **そのプロジェクトの HintPath が指す DLL だけを測る。**
            $dll = Join-Path $proj.DirectoryName ($hm.Groups[1].Value.Trim())
            try { $dll = [IO.Path]::GetFullPath($dll) } catch { }

            if (-not (Test-Path $dll))
            {
                $unk++
                $unknown += [PSCustomObject]@{
                    対象 = $rel; 名前 = $name; 宣言 = $vm.Groups[1].Value
                }
                continue
            }

            try { $actual = [Reflection.AssemblyName]::GetAssemblyName($dll).Version.ToString() }
            catch
            {
                $unk++
                $unknown += [PSCustomObject]@{
                    対象 = $rel; 名前 = $name; 宣言 = $vm.Groups[1].Value
                }
                continue
            }

            if ($vm.Groups[1].Value -eq $actual)
            {
                $ok++
            }
            else
            {
                $ng++
                $mismatch += [PSCustomObject]@{
                    対象 = $rel
                    種類 = "Reference"
                    名前 = $name
                    宣言 = $vm.Groups[1].Value
                    記述 = "実体 $actual"
                }
            }
        }
    }

    $okCount += $ok

    $note = "一致 $ok"
    if ($ng  -gt 0) { $note += " / **不一致 $ng**" }
    if ($unk -gt 0) { $note += " / 判定不能 $unk" }

    $rows += [PSCustomObject]@{
        対象 = $rel
        結果 = $(if ($ng -gt 0) { "不一致" } elseif ($ok -eq 0) { "不明" } else { "OK" })
        内容 = $note
    }
}

# ------------------------------------------------------------------
# 出力
# ------------------------------------------------------------------
Write-Host ""
Write-Host "================ パッケージの版 ================"
Write-SummaryTable -Rows $rows -Columns @("対象", "結果", "内容")

Write-Host ""
Write-Host ("  照合 {0} 件 : 一致 {1} / **不一致 {2}** / 判定不能 {3}" -f `
    $total, $okCount, $mismatch.Count, $unknown.Count)

if ($unknown.Count -gt 0)
{
    Write-Host "  **判定不能は「問題なし」ではない。** HintPath の先に DLL が無い、という意味である。"
    Write-Host "  復元していないなら、復元してから測り直す（-Detail で内訳）。"
    Write-Host "  **nuget.org に無い商用パッケージ（IBM.Data.DB2 系）は、常にここに出る。**"
}

if ($mismatch.Count -gt 0)
{
    Write-Host ""
    Write-Host "=== 不一致 ==="
    foreach ($m in ($mismatch | Sort-Object 対象, 種類, 名前))
    {
        Write-Host ("  " + $m.対象)
        Write-Host ("      [{0}] {1} : 宣言 {2} / 記述 {3}" -f $m.種類, $m.名前, $m.宣言, $m.記述)
    }
}

if ($Detail -and $unknown.Count -gt 0)
{
    Write-Host ""
    Write-Host "=== 判定不能（HintPath の先に DLL が無い。未復元、または商用パッケージ）==="
    foreach ($u in ($unknown | Sort-Object 対象, 名前))
    {
        Write-Host ("  {0} : {1} → {2}" -f $u.対象, $u.名前, $u.宣言)
    }
}

if ($Check)
{
    Write-Host ""
    Write-Host "================ 判定 ================"

    if ($mismatch.Count -eq 0)
    {
        Write-Host ("  不一致なし。（判定不能 {0} 件）" -f $unknown.Count) -ForegroundColor Green
    }
    else
    {
        Write-Host ("  **不一致 {0} 件**" -f $mismatch.Count) -ForegroundColor Red
        Write-Host ""
        Write-Host "  packages.config の版に合わせること。**パス表記は HintPath だけではない。**"
        Write-Host "  Import Project と Error Condition にも版が入る（#566）。"
    }
}

Write-Host ""

if ($Check -and $mismatch.Count -gt 0) { exit 1 }
exit 0
