#貢献の方法(How to contribute)

##プログラミングのルールと規則(Programming Rules and Conventions)

###コメント(Comment)

  - コメント量はコード量の1/3(33%)程度を目安にして下さい。  
    メンテナンスを考慮して、高めのコメント比率を設定しています。  
    冗長なコメントは不要です。現時点の冗長なコメントの量は43%程度でした。  
    
    (The amount of comments aim about 1/3(33%) of the amount of code.  
    It has set a higher ratio comment in consideration of the maintenance.  
    Redundant comment is unnecessary. Amount of redundant comment at the present time was about 43%.)  
    
  - 変更履歴はヘッダの履歴だけ継続します。  
    修正の開始・終了などの記載は不要です。  
    
    (Change history will continue only history of the header.  
    Descriptions such as start and end of the correction is not required.)  
    
###コーディング（Coding）
  プロジェクトファイルで指定されたのランタイムで  
  使用可能なステートメントに限られますので注意下さい。  
  
  (Please keep in mind that it is limited to the statement  
  that can be used in run-time which was specified in the project file.)  

  - 本体(Body)  
    .NET Fx 3.5  

  - ASP.NET MVC  
    .NET Fx v4.5  

  - Silverlight  
    v5.0  

  - Windows Store App  
    Windows 8  

  例えば、  
  以前は.NET Fx 2.0のサポートが含まれていました（現在は.NET Fx 3.5からのサポート）。  
  このため.NET Fx 2.0で使用できないLinq構文などは本プロジェクトで使用されていません。  

  (For example,  
  Contained the support of .NET Fx 2.0 previously(The support from. NET Fx 3.5 now).  
  Such as Linq syntax that can not be used in .NET Fx 2.0 has not been used in this project.)  

##手順(Flow)

  - Gitブランチを使いこなすgit-flow／GitHub Flow入門  
    http://www.atmarkit.co.jp/ait/kw/gitflow_nyuumon.html  

###GitHub Flow

  製品を毎日プッシュしコンスタントにテストしデプロイする  
  という出荷の文化はこのプロジェクトにないためGitHubFlowは採用しません。  

  (GitHubFlow is not adopted because there is no culture of the shipping  
  that test and push constantly and deploy the product every day.)  

  - GitHub Flow - Scott Chacon（原文）  
    http://scottchacon.com/2011/08/31/github-flow.html  
  - 上記記事の日本語訳  
    https://gist.github.com/Gab-km/3705015  
    
###git-flow

  このリポジトリは、次のURLのgit-flowブランチ・モデルに基づいています。  
  - masterとdevelopブランチを常設しています。  
  - その他のブランチは必要に応じて作成します。  

  (This repository is based on the branch model of the the following URL. 
  - develop branch and master branch has been permanent.  
  - Create as needed to other branches.)  

  - A successful Git branching model » nvie.com  
    http://nvie.com/posts/a-successful-git-branching-model/  
  - 見えないチカラ A successful Git branching model を翻訳しました  
    http://keijinsonyaban.blogspot.jp/2010/10/successful-git-branching-model.html  

###コミットのサイズ(Size of the commit)
  プルリクエストのレビューのためにコミットのサイズを小さくします。  
  (Reduce the size of the commit for review of the pull request.)  

###プルリクエスト(Pull Request)
  各ユーザのリポジトリにdevelopブランチからForkします。  
  Forkした後にfeatureブランチを作成し作業します。  
  (Fork from the develop branch in the repository for each user.  
  After having done Fork, Create feature branch, and do work.)  

  プルリクエストはfeatureブランチからdevelopブランチ対して送信します。  
  (Send for the develop branch from the feature branch pull request.)  

###テストコード(Test code)
  テストコードを実装可能な場合、以下のパスに実装して下さい。  
  (If you can implement the test code, should be implemented in the following path.)  

  \root\programs\C#\Tests  

###プッシュ(push)
  developブランチでのビルドとテストが完了してからmasterブランチにプッシュします。  
  (After build and test in the develop brunch are completed, it pushes to a master brunch.)
