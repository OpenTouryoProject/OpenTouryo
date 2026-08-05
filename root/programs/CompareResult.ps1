<#
.SYNOPSIS
    単体テストの実行結果を、同梱の期待結果ファイルと比較して合否を判定する。

.DESCRIPTION
    従来「diff で目視」していた判定を機械化するためのスクリプト。

    期待結果ファイルと実行結果には、実行のたびに変わる値（日時・処理時間・乱数・
    署名値・パス等）が含まれるため、素の diff では必ず差分が出る。
    本スクリプトはそれらを正規化したうえで比較し、残った差分だけを報告する。

    正規化しても残る差分は「実質的な差分」であり、次のいずれかを意味する。
      ・退行（コードの不具合）
      ・期待結果ファイルの陳腐化
      ・テスト データの汚染（DB のレコードが増減している等）

.PARAMETER Expected
    期待結果ファイルのパス。

.PARAMETER Actual
    実行結果ファイルのパス。

.PARAMETER SkipLog4netTrace
    log4net の内部トレース（"log4net: " で始まる行）を比較対象から除外する。
    設定内容の確認が目的でなければ、除外した方が判定が読みやすい。

.PARAMETER ShowAll
    差分の全件を表示する（既定は先頭 20 件）。

.EXAMPLE
    .\CompareResult.ps1 -Expected TestCode\Result48.txt -Actual out\TestCode48.txt

.EXAMPLE
    .\CompareResult.ps1 -Expected TestBatch\ResultSimpleBatch48.txt `
                        -Actual out\TestBatch48.txt -SkipLog4netTrace

.NOTES
    作成者          ：玄人 幸道
    更新履歴        ：
     日時        更新者            内容
     ----------  ----------------  -------------------------------------------------
     2026/08/01  玄人 幸道         新規作成（リリース ワークのエージェント化）
     2026/08/05  玄人 幸道         OSメッセージの正規化を追加（CI の英語環境への対応）
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)][string]$Expected,
    [Parameter(Mandatory = $true)][string]$Actual,
    [switch]$SkipLog4netTrace,
    [switch]$ShowAll,
    # 判定結果をオブジェクトとして出力する（呼び出し元での集計用）。
    # 画面表示は Write-Host で行っておりパイプラインに乗らないため、
    # 件数を機械的に受け取りたい場合はこれを指定する。
    [switch]$PassThru
)

# ------------------------------------------------------------------
# 正規化ルール
# ------------------------------------------------------------------
# 実行のたびに変わる値を、比較前にプレースホルダへ置き換える。
# ここに無いものが差分として残る＝実質的な差分、という設計。
$normalizers = @(
    # 日時（log4net の ConversionPattern : yyyy/MM/dd HH:mm:ss,fff）
    @{ Name = '日時';           Pattern = '\d{4}/\d{2}/\d{2} \d{2}:\d{2}:\d{2},\d{3}'; Replace = '<DATETIME>' }
    # 日時（Dao 生成物のヘッダ等 : yyyy/M/d）
    @{ Name = '日付';           Pattern = '\d{4}/\d{1,2}/\d{1,2}';                     Replace = '<DATE>' }
    # 処理時間（フレームワークのアクセス ログ）
    # ※ net48 は 2 項目、.NET (Core) は 2 項目目が空になることがあるため、
    #    「数値,数値」「数値,」の両方を吸収する。
    @{ Name = '処理時間';       Pattern = '(?<=\],)\d+,\d*(?=,\[commandText\])';       Replace = '<ELAPSED>' }
    @{ Name = '処理時間(末尾)'; Pattern = '(?<=%-?,)\d+,\d*\s*$';                      Replace = '<ELAPSED>' }
    # 絶対パス（実行環境に依存する）
    @{ Name = 'パス';           Pattern = '[A-Za-z]:\\\\[^\s,]+';                      Replace = '<PATH>' }
    @{ Name = 'パス(単一)';     Pattern = '[A-Za-z]:\\[^\s,]+';                        Replace = '<PATH>' }
    # OS が返すエラー文字列（Windows の表示言語で変わる）
    # ※ .NET の CurrentUICulture ではなく OS の表示言語に従うため、設定では固定できない。
    # 　 開発環境は日本語、GitHub ホステッドの runner は英語になる。
    # ※ 例外が起きたこと自体は「行の存在」で分かるため、文言だけを潰す。
    # 　 行ごと落とすと、例外が起きなくなったときに検知できない。
    @{ Name = 'OSメッセージ'
       Pattern = 'キーがありません。|Key does not exist\.|プロバイダーの公開キーは無効です。|Provider''s public key is invalid\.'
       Replace = '<OSMSG>' }
    # GUID
    @{ Name = 'GUID';           Pattern = '[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}'; Replace = '<GUID>' }
    # XML 署名の値（要素で囲まれているため、先に要素単位で潰す）
    # ※ 値そのものは Base64 だが、実行のたびに変わるうえ改行を含まないため
    #    汎用の Base64 パターンより先に処理する。
    @{ Name = 'XML署名値';      Pattern = '<(SignatureValue|DigestValue|Modulus|Exponent)>[^<]*</\1>'; Replace = '<$1><XMLSIG></$1>' }
    # Base64URL の長い値（JWT・署名・鍵・IV・認証タグ等）
    @{ Name = 'Base64URL';      Pattern = '[A-Za-z0-9_\-]{16,}';                       Replace = '<B64URL>' }
    # Base64 の長い値（鍵・ハッシュ等）
    @{ Name = 'Base64';         Pattern = '[A-Za-z0-9+/]{16,}={0,2}';                  Replace = '<B64>' }
    # スレッド ID（実行ごとに変わり得る）
    @{ Name = 'スレッドID';     Pattern = '(?<=\],)\[\d+\](?=,)';                      Replace = '[<TID>]' }
)

function Normalize([string[]]$lines)
{
    $result = New-Object System.Collections.Generic.List[string]

    foreach ($line in $lines)
    {
        $s = $line

        if ($SkipLog4netTrace -and $s -match '^log4net: ')
        {
            continue
        }

        if ($s.Trim() -eq '')
        {
            continue
        }

        # 非対話実行の副産物を除外する。
        # サンプルは末尾に Console.ReadKey() を持つものがあり、
        # 出力をリダイレクトして実行すると必ず例外で終わる。
        # テスト内容とは無関係なため、比較対象から外す。
        if ($s -match 'Cannot read keys when either application does not have a console'  `
            -or $s -match 'ConsolePal\.ReadKey'                                           `
            -or $s -match 'Exception\.ToString\(\) が失敗したため'                        `
            -or $s -match '^Unhandled exception\.'                                        `
            -or $s -match '^\s+at .+\.Program\.Main\(')
        {
            continue
        }

        foreach ($n in $normalizers)
        {
            $s = [regex]::Replace($s, $n.Pattern, $n.Replace)
        }

        $result.Add($s.TrimEnd())
    }

    return , $result.ToArray()
}

# ------------------------------------------------------------------
# 比較
# ------------------------------------------------------------------
if (-not (Test-Path $Expected)) { Write-Error "期待結果ファイルが見つかりません : $Expected"; exit 2 }
if (-not (Test-Path $Actual))   { Write-Error "実行結果ファイルが見つかりません : $Actual";   exit 2 }

# 読み込みは UTF-8 を明示する。
# 比較対象の *.txt は BOM 無しの UTF-8 だが、Get-Content の既定エンコードは
# PowerShell のエディションで異なる。
#   Windows PowerShell 5.1 … ANSI（日本語環境では Shift_JIS）
#   PowerShell 7           … UTF-8
# 指定しないと 5.1 だけ文字化けし、同じファイルなのに差分が出る。
$expLines = Normalize (Get-Content $Expected -Encoding UTF8)
$actLines = Normalize (Get-Content $Actual   -Encoding UTF8)

$diff = @(Compare-Object $expLines $actLines -SyncWindow 20)

Write-Host ""
Write-Host "=== 比較結果 ==="
Write-Host ("  期待 : {0}  （{1} 行）" -f $Expected, $expLines.Count)
Write-Host ("  実測 : {0}  （{1} 行）" -f $Actual,   $actLines.Count)
Write-Host ""

if ($diff.Count -eq 0)
{
    Write-Host "  OK : 正規化後の差分はありません。" -ForegroundColor Green
    if ($PassThru) { [pscustomobject]@{ Result = "OK"; DiffCount = 0 } }
    exit 0
}

Write-Host ("  NG : 正規化後も {0} 件の差分があります。" -f $diff.Count) -ForegroundColor Yellow
Write-Host "       退行／期待結果の陳腐化／テスト データの汚染のいずれかを確認してください。"
Write-Host ""

$show = if ($ShowAll) { $diff } else { $diff | Select-Object -First 20 }
foreach ($d in $show)
{
    $mark = if ($d.SideIndicator -eq '<=') { '期待のみ' } else { '実測のみ' }
    $text = $d.InputObject
    if ($text.Length -gt 160) { $text = $text.Substring(0, 160) + ' ...' }
    Write-Host ("  [{0}] {1}" -f $mark, $text)
}

if (-not $ShowAll -and $diff.Count -gt 20)
{
    Write-Host ("  ... 他 {0} 件（全件表示は -ShowAll）" -f ($diff.Count - 20))
}

if ($PassThru) { [pscustomobject]@{ Result = "NG"; DiffCount = $diff.Count } }
exit 1
