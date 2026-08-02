# 貢献の方法

このファイルの英語版は[こちら](CONTRIBUTING.md)から。

## プログラミングのルールと規則

### コメント
 - コメント量はコード量の1/3 (33%) 程度を目安にして下さい。  
   - メンテナンスを考慮して、高めのコメント比率を設定しています。  
   - 冗長なコメントは不要です。現時点の（冗長な）コメントの量は43%程度でした。  

 - 変更履歴はGit導入後もヘッダの履歴だけ継続します。修正の開始・終了などの記載は不要です。  

### コーディング
 - このプロジェクトでは重複したコードや情報の書き込みが禁止されています。  
   これは "Once and Only Once" や "Don't repeat yourself" です。  

 - C#、VB、.NET Framework 系（net48）、.NET Core 系（net10.0）のクロスコンパイルです。  
   ただし、VB版は .NET Framework 系（net48）版のみの提供になります。

 - 下位互換は高く維持し、破壊的な変更は最小限にします。
   - 新・旧ランタイムをサポートする場合は、旧に合わせた実装を行います（例：VS2010, .NET3.5サポートが含まれていた時は async/await を使用しなかった）。
   - .NET Framework系（net48）、.NET Core系（net10.0）は別系列なので条件付きコンパイル（プリプロセッサ）を使用してクロスコンパイルします。
   - 破壊的な変更は、基本的にランタイム側のdisconのケースで、変更の前に十分に「obsolete」の期間を設けます。

### コーディングエージェント
活用方針は [AGENTS.md](AGENTS.md) に記載してあります。

## C#, VB
 VB版の提供は必須ではありません。必要であればこちらでツール（Sharpdeveloper、生成AI）を使用してVBに変換します。  

## 手順

### GitHub Flow
 「製品を毎日pushしコンスタントにテストしデプロイする。」  
 という出荷の文化はこのプロジェクトに無い為、GitHubFlowは採用しません。  

 - GitHub Flow - Scott Chacon（原文）  
   https://scottchacon.com/2011/08/31/github-flow/  
 - 上記記事の日本語訳  
   https://gist.github.com/Gab-km/3705015  

### git-flow
 このリポジトリは、下記「参考」の git-flow ブランチ・モデルに基づいています。
 
 - master と develop ブランチを常設しています。  
 - その他のブランチは必要に応じて作成します。  

#### 参考
 - A successful Git branching model » nvie.com  
   https://nvie.com/posts/a-successful-git-branching-model/  
 - 見えないチカラ A successful Git branching model を翻訳しました  
   http://keijinsonyaban.blogspot.com/2010/10/a-successful-git-branching-model.html  

 なお上記の原典には、著者による注記（Note of reflection、2020/03/05）が追記されており、  
 継続的デリバリを行う Web アプリなどには git-flow を勧めない旨が述べられています。  
 このプロジェクトは前述のとおり毎日出荷する運用ではないため、git-flow を採用しています。  

### "プルリクエスト"について

#### "プルリクエスト"のサイズ
 - "プルリクエスト"のレビューのために"コミット"と"プルリクエスト"のサイズを小さくします。  

 - １つの"プルリクエスト"の中に複数のバグやエンハンスのタスクが存在する場合、1つの遅延が他の遅延に影響します。

 - このため、あなたはバグやエンハンスのタスク毎に feature ブランチを作成する必要があります。  
   その後、feature ブランチから都度"プルリクエスト"を送って私にレビューを依頼して下さい。  

 - また、IDEやEditorによりインデントが変更されるような不要な修正もコミットしないで下さい。  

#### "プルリクエスト"を送る
 - OpenTouryoリポジトリから各ユーザのリポジトリに fork します。  
   その後に develop ブランチから feature ブランチを作成し作業します。  

 - "プルリクエスト"は feature ブランチから fork 元の develop ブランチに対して送信します。  

 - もし fork 元の develop ブランチが変更されているようなら、feature ブランチに pull 若しくは fetch & merge をします。  

#### "プルリクエスト"についての注意事項
 - develop / feature ブランチは、次の手順で最新の状態に維持します。  

   1. OpenTouryo の develop ブランチを、あなたの develop / feature ブランチへ pull 若しくは fetch & merge します。  
   2. あなたの develop ブランチから feature ブランチを再作成します。  
   3. feature ブランチへ変更を反映します。  
   4. テストおよびプッシュし、"プルリクエスト"を送ります。  

 - この手順は、次の 2 つを兼ねています。  

   - fork 元の develop ブランチの変更に追随すること。  
   - 間違った変更の後にプログラムをロールバックして、コミットログが汚染されるのを防ぐこと。  

 - pull や fetch & merge の代わりに再度 fork する方法もありますが、コメントや、fork 元にマージされていない変更は失われます。  
