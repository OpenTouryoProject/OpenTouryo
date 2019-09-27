# Open棟梁
”Open棟梁”は、長年の.NETアプリケーション開発実績にて蓄積したノウハウに基づき開発した.NET用アプリケーション フレームワークです。

このファイルの英語版は[こちら](README.md)から。

## 開発 / 動作環境
このリポジトリのプログラムは、以下のIDE/targetFramework を前提に開発しています。

- IDE（統合開発環境）
  - Visual Studio 2015
  - Visual Studio 2017
  - Visual Studio 2019
- targetFramework（実行環境）
  - .NET Framework 4.5.2 (net452)
  - .NET Framework 4.6 (net46)
  - .NET Framework 4.7 (net47)
  - .NET Framework 4.8 (net48)
  - .NET Core 2.0 (netcoreapp2.0)
  - .NET Core 3.0 (netcoreapp3.0)
  - .NET Standard 2.0 (netstandard2.0)
  - .NET Standard 2.1 (netstandard2.1)
  

プロジェクト・ソリューションの既定の targetFramework は net46(.NET Framework 4.6) です。
その他の targetFramework 向けのプロジェクト・ソリューションには、それぞれ targetFramework がプロジェクト・ソリューション名に含まれます。
たとえば、net47(.NET Framework 4.7) を対象としたプロジェクト・ソリューションは、"{identifier}_net47.{ext}" と命名しています。

このリポジトリのプログラムは、OSS 開発者向けのものです。
システム開発プロジェクトで Open 棟梁を使用される方は、[OpenTouryoTemplates リポジトリ](https://github.com/OpenTouryoProject/OpenTouryoTemplates)を使用してください。

## 概要
以下のファイルを参照してください。
 - [概要資料集](https://github.com/OpenTouryoProject/OpenTouryoDocuments/blob/master/documents/0_Introduction/ja-JP/Introduction.md)
 - [機能一覧 (Excel)](https://github.com/OpenTouryoProject/OpenTouryoDocuments/blob/master/documents/0_Introduction/ja-JP/Functional_list.xlsx)

## 詳細
Open 棟梁のドキュメントは、[OpenTouryoDocuments リポジトリ](https://github.com/OpenTouryoProject/OpenTouryoDocuments)にあります。
詳細は、[OpenTouryoDocuments リポジトリ](https://github.com/OpenTouryoProject/OpenTouryoDocuments)のドキュメントを参照してください。

## 内容物

### ディレクトリ

#### [/license/](https://github.com/OpenTouryoProject/OpenTouryo/tree/master/license)
このディレクトリには、ライセンスファイルが格納されています。

#### [/root/](https://github.com/OpenTouryoProject/OpenTouryo/tree/master/root)
このディレクトリには、プログラム、設定ファイル、SQL ファイルなどが格納されています。

## テンプレート・ベース
「Open 棟梁テンプレート・ベース」とは、Open 棟梁を使用したシステムの開発基盤 (プロジェクトテンプレート) を作るための元になるものです。
Open 棟梁テンプレート・ベースに含まれるサンプルは、Open 棟梁の評価に利用できます。

もし Open 棟梁の機能の中で、システム開発プロジェクトの要件に合わない部分がありましたら、このテンプレート・ベースをカスタマイズすることでご対応いただけます。
テンプレートベースのカスタマイズ方法につきましては、[チュートリアル](https://github.com/OpenTouryoProject/OpenTouryoDocuments/blob/master/documents/2_Tutorial/ja-JP/Tutorial_Template_development.doc)をご覧ください。

詳細は、以下リポジトリの Readme ファイルを参照してください。
 - [OpenTouryoTemplates](https://github.com/OpenTouryoProject/OpenTouryoTemplates)
