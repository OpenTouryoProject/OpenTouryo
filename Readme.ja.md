# Open棟梁
”Open棟梁”は、長年の.NETアプリケーション開発実績にて蓄積したノウハウに基づき開発した.NET用アプリケーション フレームワークです。

このファイルの英語版は[こちら](README.md)から。

## 開発 / 動作環境
このリポジトリのプログラムは、以下のIDE/targetFramework を前提に開発しています。

- IDE（統合開発環境）  
  Visual Studio 2026
- targetFramework（実行環境）
  - .NET Framework 4.8 (net48)
  - .NET 10.0 (net10.0)

プロジェクトやソリューションの名前には、targetFramework の識別子が含まれます。
たとえば、.NET Framework 4.8 を対象としたプロジェクト・ソリューションは "{identifier}_net48.{ext}"、
.NET 10.0 を対象としたものは "{identifier}_netcore100.{ext}" と命名しています。

## 資料
Open 棟梁のドキュメントは、[OpenTouryoDocuments リポジトリ](https://github.com/OpenTouryoProject/OpenTouryoDocuments)にあります。

 - [資料一覧](https://github.com/OpenTouryoProject/OpenTouryoDocuments/blob/master/documents/0_Introduction/ja-JP)
 - [機能一覧 (Excel)](https://github.com/OpenTouryoProject/OpenTouryoDocuments/blob/master/documents/0_Introduction/ja-JP/Functional_list.xlsx)

一部の資料は[当該リポジトリ上でWiki化](https://github.com/OpenTouryoProject/OpenTouryo/wiki/Home.ja)されています。
## 内容物

### [/license/](https://github.com/OpenTouryoProject/OpenTouryo/tree/master/license)
このディレクトリには、ライセンスファイルが格納されています。

### [/root/](https://github.com/OpenTouryoProject/OpenTouryo/tree/master/root)
このディレクトリには、プログラム、設定ファイル、SQL ファイルなどが格納されています。

セットアップとビルドの手順は [/root/Readme.ja.md](root/Readme.ja.md) を参照してください。

### [AGENTS.md](AGENTS.md)
コーディング エージェントで本リポジトリを扱う場合の**入口**です。
守るべきポリシーと、参照すべき文書への導線をまとめています。
