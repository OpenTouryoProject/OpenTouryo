# 貢献の方法

このファイルの英語版は[こちら](CONTRIBUTING.md)から。

## プログラミングのルールと規則

### コメント
 - コメント量はコード量の1/3 (33%) 程度を目安にして下さい。  
   メンテナンスを考慮して、高めのコメント比率を設定しています。  
   冗長なコメントは不要です。現時点の冗長なコメントの量は43%程度でした。  

 - 変更履歴はヘッダの履歴だけ継続します。修正の開始・終了などの記載は不要です。  

### コーディング
 - このプロジェクトでは重複したコードや情報の書き込みが禁止されています。  
   これは "Once and Only Once" や "Don't repeat yourself" です。  

 - プロジェクトファイルで指定されたのランタイムで使用可能なステートメントに限られますので注意下さい。  
   例えば、以前はVS 2010, .NET 3.5のサポートが含まれていました（現在はVS 2015, .NET 4.6からのサポート）。  
   このためVS 2010, .NET 3.5で使用できないasync/awaitキーワードなどは本プロジェクトで使用されていません。  

## C#, VB
 VB版の提供は必須ではありません。必要であればこちらでSharpdeveloperを使用してVBに変換します。  

##手順

### GitHub Flow
 「製品を毎日pushしコンスタントにテストしデプロイする。」  
 という出荷の文化はこのプロジェクトに無い為、GitHubFlowは採用しません。  

 - GitHub Flow - Scott Chacon（原文）  
   http://scottchacon.com/2011/08/31/github-flow.html  
 - 上記記事の日本語訳  
   https://gist.github.com/Gab-km/3705015  

### git-flow
 このリポジトリは、次の URL の git-flow ブランチ・モデルに基づいています。
 
 - master と develop ブランチを常設しています。  
 - その他のブランチは必要に応じて作成します。  

####参考
 - A successful Git branching model » nvie.com  
   http://nvie.com/posts/a-successful-git-branching-model/  
 - 見えないチカラ A successful Git branching model を翻訳しました  
   http://keijinsonyaban.blogspot.jp/2010/10/successful-git-branching-model.html  

### "プルリクエスト"について

#### "プルリクエスト"のサイズ
 - "プルリクエスト"のレビューのために"コミット"と"プルリクエスト"のサイズを小さくします。  

 - １つの"プルリクエスト"の中に複数のバグやエンハンスのタスクが存在する場合、1つの遅延が他の遅延に影響します。

 - このため、あなたはバグやエンハンスのタスク毎に feature ブランチを作成する必要があります。  
   その後、feature ブランチから都度"プルリクエスト"を送って私にレビューを依頼して下さい。  

 - また、IDEやEditorによりインデントが変更されるような不要な修正もコミットしないで下さい。  

#### "プルリクエスト"を送る
 - Opentouryoリポジトリから各ユーザのリポジトリに fork します。  
   その後に develop ブランチから feature ブランチを作成し作業します。  

 - "プルリクエスト"は feature ブランチから fork 元の develop ブランチに対して送信します。  

 - もし fork 元の develop ブランチが変更されているようなら、feature ブランチに pull 若しくは fetch & merge をします。  

#### "プルリクエスト"についての注意事項
 - あなたは、次のように develop / feature ブランチを最新の状態に維持します。  

   - OpenTouryoの develop ブランチを、あなたの develop / feature ブランチから pull 若しくは fetch & merge してください。  
     そして、あなたはあなたの develop ブランチから feature ブランチを再作成し、あなたはfeatureブランチへの変更を反映します。  
     その後、テストおよびプッシュし、"プルリクエスト"を送ります。  

    - pull や fetch & margeの代わりに再度 fork する方法もありますが、コメントや元のfork 元にマージされていない変更は失われます。  

 - 間違った変更の後にプログラムをロールバックしてコミットログが汚染されることを防止するために、次の手順に従ってください。  
   - あなたは、あなたの develop ブランチから feature ブランチを再作成します。
   - そして、あなたは feature ブランチへの変更を反映します。
   - その後、テストおよびプッシュし、"プルリクエスト"を送ります。  
