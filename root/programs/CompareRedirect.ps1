<#
.SYNOPSIS
    bindingRedirect の newVersion を、そのプロジェクトが配布するアセンブリの版と突き合わせる。

.DESCRIPTION
    `bindingRedirect` は「この範囲の要求が来たら、この版を読め」という宣言である。
    **宣言した版が配布されていなければ、その範囲の要求は解決に失敗する。**

    ＜今は動いているのが厄介＞

      実体の版が範囲の外にあるうちは、転送されず直接束縛されるため**表面化しない。**
      **古い版を要求する依存が 1 つ増えた瞬間に落ちる**、という形で顕在化する。
      `0_RunAll.ps1` は通る（実測）。**通ることは「正しい」の証拠にならない。**

    ＜net10.0 とは別の現象＞

      | | net10.0 | net48（本スクリプトの対象） |
      |---|---|---|
      | bindingRedirect | **仕組みが無い** | ある |
      | ずれると | **即 FileNotFoundException** | **黙って通る** |

      net10.0 側の話は Samples4NetCore/ANALYSIS.md 7 節。

    ＜判定の基準は「そのプロジェクトの配下」＞

      **リポジトリ全体から探してはいけない。**（#556 で踏んだ）
      bindingRedirect が決めるのは**そのアプリが読む先**であり、
      他のプロジェクトの bin に在るかどうかは関係がない。

      全体から探すと、**ビルドの状態で答えが変わる。**
      1_BuildAll.ps1 は bin を消すため、測るタイミングで結果が動いてしまう。

    ＜ビルド後に実行すること＞

      配布物を見るため、**ビルドしていないと「判定不能」ばかりになる。**
      0_RunAll.ps1 に組み込むなら 1_BuildAll.ps1 の後。

      **「判定不能」は「問題なし」ではない。**
      不一致には数えないが、件数は必ず出す。

      ＜「判定不能」の原因は 1 つではない＞

        **ビルドしていない、とは限らない。**（#566 で踏んだ）
        ビルド済みでも、**参照そのものが落ちていれば bin に配られない。**

        csproj の `<Reference Include="..., Version=X">` に強い名前を書くと、
        SpecificVersion は既定で true になる。宣言と実体の版がずれると、
        MSBuild は**警告だけ出して参照を落とす**（ビルドは成功する）。

        このとき本スクリプトは「判定不能」と答えるが、実態は
        **「宣言した版が配布されていない」そのもの**であり、不一致より悪い。

        **建てたはずのものが「判定不能」なら、まず bin を見ること。**
        DLL が無ければ、packages.config・csproj のパス表記・Reference の
        Version の 3 つが揃っているかを疑う。

    ＜対象外＞

      **CS/NuGet/proj 配下は見ない。** NuGet パッケージの検証用で、
      1_BuildAll.ps1 が建てないため、**必ず「判定不能」になる**（39 件）。
      常に出続ける「判定不能」は、見るべきものを埋もれさせる。
      個別に見るときは、そのプロジェクトを建ててから -Only を使う。

      **クラス ライブラリの app.config は、そもそも実行時に読まれない。**
      bindingRedirect が効くのはアプリケーションの構成ファイルだけで、
      Foo.dll.config は参照されない。基盤ライブラリの分（11 件）が
      「判定不能」で残るのは、その意味では正しい。

.PARAMETER Detail
    「判定不能」の内訳も表示する。

.PARAMETER Only
    対象を相対パスの部分一致で絞る（例: -Only "WS_sample"）。

.PARAMETER Check
    **合否を返す。** 不一致が 1 件でもあれば終了コード 1 を返す。
    「判定不能」では落とさない。

.EXAMPLE
    .\CompareRedirect.ps1

.EXAMPLE
    .\CompareRedirect.ps1 -Detail -Only "Tests"

.EXAMPLE
    .\CompareRedirect.ps1 -Check

.NOTES
    作成者          ：玄人 幸道
    更新履歴        ：
     日時        更新者            内容
     ----------  ----------------  -------------------------------------------------
     2026/08/17  玄人 幸道         新規作成（#556）

    -Check を付けない限り、終了コードは常に 0。

    **bindingRedirect は NuGet の生成物である。** 手で直しても、パッケージ操作で
    また書き換わる。だからこそ「直す」より「ずれを検知できる状態にしておく」方を採る。
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
# config から bindingRedirect を読む
# ------------------------------------------------------------------
$script:unreadable = @()

function Get-Redirects([string]$path)
{
    # **Get-Content でテキストとして読まない。**（#579）
    #   Windows PowerShell 5.1 の Get-Content は、BOM が無いと既定の文字コード（CP932）で
    #   読む。UTF-8 の日本語が壊れて XML として解析できなくなり、
    #   **その config は宣言ごと数えられずに消えていた。**
    #   実測で 5 ファイル・38 宣言が 5.1 でだけ測られていなかった。
    #
    #   XmlDocument.Load は BOM と XML 宣言を見て復号するため、5.1 と 7 で揃う。
    $x = New-Object System.Xml.XmlDocument

    try { $x.Load($path) }
    catch
    {
        # **黙って捨てない。** 捨てると「宣言が無い」と区別がつかない。
        $script:unreadable += [PSCustomObject]@{
            Config = $path; 理由 = $_.Exception.Message
        }
        return @()
    }

    $list = @()
    foreach ($d in $x.SelectNodes("//*[local-name()='dependentAssembly']"))
    {
        $id = $d.SelectSingleNode("*[local-name()='assemblyIdentity']")
        $br = $d.SelectSingleNode("*[local-name()='bindingRedirect']")
        if (-not $id -or -not $br) { continue }

        $name = $id.GetAttribute("name")
        $new  = $br.GetAttribute("newVersion")
        if (-not $name -or -not $new) { continue }

        $list += [PSCustomObject]@{ Name = $name; New = $new }
    }
    return $list
}

# ------------------------------------------------------------------
# 対象（追跡下の config のうち、bindingRedirect を持つもの）
# ------------------------------------------------------------------
$tracked = & git -C $root ls-files
if ($LASTEXITCODE -ne 0 -or -not $tracked)
{
    Write-Host "  git ls-files が使えないため、中止する。" -ForegroundColor Red
    exit 0
}

$rows      = @()
$mismatch  = @()
$unknown   = @()
$total     = 0
$okCount   = 0

foreach ($t in $tracked)
{
    if ($t -notmatch '\.config$') { continue }
    if ($t -match '/packages/')   { continue }

    # **通しビルドの対象外は見ない。**（#557）
    #   CS/NuGet/proj は NuGet パッケージの検証用で、1_BuildAll.ps1 が建てない。
    #   建てていないものは配下に実体が無く、**必ず「判定不能」になる。**
    #   常に出続ける「判定不能」は、見るべきものを埋もれさせる。
    #   検証するときは、そのプロジェクトを建ててから -Only で個別に見ればよい。
    if ($t -match '/NuGet/proj/') { continue }

    if ($Only -and ($t -notlike "*$Only*")) { continue }

    $full = Join-Path $root ($t -replace '/', '\')
    if (-not (Test-Path $full)) { continue }

    $redirects = @(Get-Redirects $full)
    if ($redirects.Count -eq 0) { continue }

    $rel = $t -replace '^root/programs/', ''
    $dir = Split-Path $full -Parent

    # **参照されている名前だけを読む。**
    #   配下の DLL を全部 GetAssemblyName にかけると桁違いに遅い（#556 で 7.3 分かかった）。
    $wanted = @{}
    foreach ($r in $redirects) { $wanted[$r.Name] = $true }

    $found = @{}

    # **配下の DLL の総数も数える。**（#579）
    #   0 なら「建てていない」と分かり、判定不能の理由を切り分けられる。
    $dlls = @(Get-ChildItem $dir -Recurse -File -Filter *.dll -EA SilentlyContinue)

    foreach ($f in $dlls)
    {
        if (-not $wanted.ContainsKey($f.BaseName)) { continue }
        try { $v = [System.Reflection.AssemblyName]::GetAssemblyName($f.FullName).Version.ToString() }
        catch { continue }

        if (-not $found.ContainsKey($f.BaseName))
        {
            $found[$f.BaseName] = New-Object System.Collections.Generic.HashSet[string]
        }
        $null = $found[$f.BaseName].Add($v)
    }

    $ok = 0; $ng = 0; $unk = 0

    foreach ($r in $redirects)
    {
        $total++
        $vers = $found[$r.Name]

        if ($null -eq $vers)
        {
            $unk++
            $unknown += [PSCustomObject]@{
                Config = $rel; Name = $r.Name; New = $r.New
                Dir    = $dir; DllCount = $dlls.Count
            }
        }
        elseif ($vers.Contains($r.New))
        {
            $ok++
        }
        else
        {
            $ng++
            $mismatch += [PSCustomObject]@{
                Config = $rel; Name = $r.Name; New = $r.New
                実体   = (($vers | Sort-Object) -join ", ")
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
# 判定不能の分類（#579）
# ------------------------------------------------------------------
#   **判定不能を一律に「怪しい」と出すと、見るべきものが埋もれる。**
#   実測すると、大半は仕組み上そうなるだけで実害が無い。
#   残る「要調査」だけが、参照の落ち（#566）を疑う対象である。

$KindNote = [ordered]@{
    "未ビルド"       = "配下に DLL が無い。建ててから測り直す"
    "ライブラリ"     = "**実行時に読まれない。**効くのはアプリの構成ファイルだけ"
    "連鎖ごと未配布" = "要求元も配られていない。読み込まれ得ない"
    "要調査"         = "**要求元は在るのに実体が無い。参照が落ちている疑い**"
}

function Get-ProjectKind
{
    <#
      .SYNOPSIS
        その構成ファイルが、実行時に読まれる側のものかを判定する。
      .DESCRIPTION
        **bindingRedirect が効くのはアプリケーションの構成ファイルだけ**である。
        クラス ライブラリの app.config は、そのままでは実行時に読まれない。

        Web アプリは OutputType が Library になるため、**先に Web.config で判定する。**
        OutputType だけで見ると、Web アプリをライブラリと誤って扱う。
    #>
    param([string]$Dir, [string]$Config)

    if ($Config -match '(?i)[\\/]web\.config$') { return "アプリ" }

    $proj = @(Get-ChildItem $Dir -File -EA SilentlyContinue |
              Where-Object { $_.Extension -eq ".csproj" -or $_.Extension -eq ".vbproj" })
    if ($proj.Count -eq 0) { return "アプリ" }

    $txt = Get-Content $proj[0].FullName -Raw -EA SilentlyContinue
    if ($txt -match '<OutputType>\s*([^<]+?)\s*</OutputType>')
    {
        if ($Matches[1] -match '(?i)^library$') { return "ライブラリ" }
        return "アプリ"
    }

    return "ライブラリ"
}

function Test-Requester
{
    <#
      .SYNOPSIS
        そのアセンブリを要求している側が、配下に在るかを調べる。
      .DESCRIPTION
        アセンブリ参照は metadata に名前がそのまま入るため、バイト列を見れば分かる。
        GetReferencedAssemblies は読み込みを伴い、**遅い上に失敗しやすい。**
    #>
    param([string]$Dir, [string[]]$Names)

    $hit = @{}
    foreach ($n in $Names) { $hit[$n] = $false }

    foreach ($f in (Get-ChildItem $Dir -Recurse -File -Filter *.dll -EA SilentlyContinue))
    {
        $rest = @($Names | Where-Object { -not $hit[$_] })
        if ($rest.Count -eq 0) { break }
        if ($Names -contains $f.BaseName) { continue }

        try { $txt = [Text.Encoding]::ASCII.GetString([IO.File]::ReadAllBytes($f.FullName)) }
        catch { continue }

        foreach ($n in $rest) { if ($txt.Contains($n)) { $hit[$n] = $true } }
    }

    return $hit
}

$kinds = @{}

foreach ($g in ($unknown | Group-Object Dir))
{
    $names = @($g.Group | ForEach-Object { $_.Name } | Sort-Object -Unique)
    $req   = $null

    foreach ($u in $g.Group)
    {
        $key = $u.Config + "|" + $u.Name

        if ($u.DllCount -eq 0)
        {
            $kinds[$key] = "未ビルド"
            continue
        }

        if ((Get-ProjectKind -Dir $u.Dir -Config $u.Config) -eq "ライブラリ")
        {
            $kinds[$key] = "ライブラリ"
            continue
        }

        # **要求元を見るのは、ここまで絞ってから。**（配下の DLL を全部読むため）
        if ($null -eq $req) { $req = Test-Requester -Dir $u.Dir -Names $names }

        if ($req[$u.Name]) { $kinds[$key] = "要調査" }
        else               { $kinds[$key] = "連鎖ごと未配布" }
    }
}

$review = @($unknown | Where-Object { $kinds[($_.Config + "|" + $_.Name)] -eq "要調査" })

# ------------------------------------------------------------------
# 出力
# ------------------------------------------------------------------
Write-Host ""
Write-Host "================ bindingRedirect ================"
Write-SummaryTable -Rows $rows -Columns @("対象", "結果", "内容")

Write-Host ""
Write-Host ("  宣言 {0} 件 : 一致 {1} / **不一致 {2}** / 判定不能 {3}" -f `
    $total, $okCount, $mismatch.Count, $unknown.Count)

if ($unknown.Count -gt 0)
{
    $byKind = @{}
    foreach ($k in $kinds.Values)
    {
        if ($byKind.ContainsKey($k)) { $byKind[$k] = $byKind[$k] + 1 } else { $byKind[$k] = 1 }
    }

    Write-Host ""
    Write-Host "  判定不能の内訳 :"

    foreach ($k in $KindNote.Keys)
    {
        if (-not $byKind.ContainsKey($k)) { continue }
        # **-f の桁指定は文字数で数える。** 日本語は全角なので崩れる（SummaryTable.ps1 1 節）。
        Write-Host ("    {0} {1} 件  {2}" -f `
            (Add-Padding $k 16), (Add-LeftPadding ([string]$byKind[$k]) 3), $KindNote[$k])
    }

    if ($review.Count -eq 0)
    {
        Write-Host "  **要調査は 0 件。** 残りは仕組み上そうなるだけで、実行時に読まれない。"
    }
}

if ($mismatch.Count -gt 0)
{
    Write-Host ""
    Write-Host "=== 不一致（宣言した版が、そのプロジェクトの配下に無い）==="
    foreach ($m in ($mismatch | Sort-Object Config, Name))
    {
        Write-Host ("  " + $m.Config)
        Write-Host ("      {0} : 宣言 {1} / 配下の実体 {2}" -f $m.Name, $m.New, $m.実体)
    }
}

# **要調査は -Detail が無くても出す。** ここだけが実害を疑う対象である。
if ($review.Count -gt 0)
{
    Write-Host ""
    Write-Host "=== 要調査（要求元は配られているのに、実体だけが無い）==="
    foreach ($u in ($review | Sort-Object Config, Name))
    {
        Write-Host ("  {0} : {1} → {2}" -f $u.Config, $u.Name, $u.New)
    }
    Write-Host "  **参照が落ちている可能性がある。**（#566）"
    Write-Host "  bin を見て、csproj の Reference / PackageReference が在るかを確かめること。"
}

if ($Detail -and $unknown.Count -gt 0)
{
    Write-Host ""
    Write-Host "=== 判定不能の一覧（分類つき）==="
    foreach ($u in ($unknown | Sort-Object Config, Name))
    {
        Write-Host ("  [{0,-14}] {1} : {2} → {3}" -f `
            $kinds[($u.Config + "|" + $u.Name)], $u.Config, $u.Name, $u.New)
    }
}

if ($script:unreadable.Count -gt 0)
{
    Write-Host ""
    Write-Host "=== 読めなかった config（解析に失敗）==="
    foreach ($u in $script:unreadable)
    {
        Write-Host ("  " + $u.Config)
        Write-Host ("    " + $u.理由)
    }
    Write-Host "  **宣言が無いのと区別がつかないため、件数に表れない。**"
}

if ($Check)
{
    Write-Host ""
    Write-Host "================ 判定 ================"

    if ($mismatch.Count -eq 0)
    {
        if ($review.Count -eq 0)
        {
            Write-Host ("  不一致なし。（判定不能 {0} 件 / **要調査 0**）" -f $unknown.Count) -ForegroundColor Green
        }
        else
        {
            Write-Host ("  不一致なし。ただし**要調査 {0} 件**（判定不能 {1} 件）" -f `
                $review.Count, $unknown.Count) -ForegroundColor Yellow
        }
    }
    else
    {
        Write-Host ("  **不一致 {0} 件**" -f $mismatch.Count) -ForegroundColor Red
        Write-Host ""
        Write-Host "  newVersion を実体に合わせること。**ただし NuGet の生成物なので、"
        Write-Host "  パッケージ操作でまた書き換わる。**（#556）"
    }
}

Write-Host ""

if ($Check -and $mismatch.Count -gt 0) { exit 1 }
exit 0
