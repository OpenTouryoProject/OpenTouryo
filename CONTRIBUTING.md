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
  - このプロジェクトでは重複したコードや情報の書き込みが禁止されています。  
    これは "Once and Only Once" や "Don't repeat yourself" です。  
    
    Writing of duplicate code and information has been prohibited in this project.
    This is "Once and Only Once" or "Don't repeat yourself".
    
  - プロジェクトファイルで指定されたのランタイムで  
    使用可能なステートメントに限られますので注意下さい。  
      
    (Please keep in mind that it is limited to the statement  
    that can be used in run-time which was specified in the project file.)  
    
    例えば、  
    以前は.NET Fx 2.0のサポートが含まれていました（現在は.NET Fx 3.5からのサポート）。  
    このため.NET Fx 2.0で使用できないLinq構文などは本プロジェクトで使用されていません。  
    
    (For example,  
    Contained the support of .NET Fx 2.0 previously(The support from. NET Fx 3.5 now).  
    Such as Linq syntax that can not be used in .NET Fx 2.0 has not been used in this project.)  

    - 本体(Body)  
    .NET Fx 3.5  

    - ASP.NET MVC  
    .NET Fx v4.5  

    - Silverlight  
    v5.0  

    - Windows Store App  
    Windows 8  

##C#, VB
  VB版の提供は必須ではありません。  
  必要であればこちらでSharpdeveloperを使用してVBに変換します。  
  
  (Provision of VB version is not required.  
  It is converted to VB by using the Sharpdeveloper here, if necessary.)

##手順(Flow)

  - Gitブランチを使いこなすgit-flow／GitHub Flow入門  
    http://www.atmarkit.co.jp/ait/kw/gitflow_nyuumon.html  

###GitHub Flow

  製品を毎日pushしコンスタントにテストしデプロイする  
  という出荷の文化はこのプロジェクトにないためGitHubFlowは採用しません。  

  (GitHubFlow is not adopted because there is no culture of the shipping  
  that test and push constantly and deploy the product every day.)  

  - GitHub Flow - Scott Chacon（原文）  
    http://scottchacon.com/2011/08/31/github-flow.html  
  - 上記記事の日本語訳  
    https://gist.github.com/Gab-km/3705015  
    
###git-flow

  このリポジトリは、次のURLのgit-flowブランチ・モデルに基づいています。  
  (This repository is based on the branch model of the the following URL.)  
  - masterとdevelopブランチを常設しています。  
    (develop branch and master branch has been permanent.)  
  - その他のブランチは必要に応じて作成します。  
    (Create as needed to other branches.)  

####参考(Reference)  
  - A successful Git branching model » nvie.com  
    http://nvie.com/posts/a-successful-git-branching-model/  
  - 見えないチカラ A successful Git branching model を翻訳しました  
    http://keijinsonyaban.blogspot.jp/2010/10/successful-git-branching-model.html  

###コミットのサイズ(Size of the commit)
  - Pull Requestのレビューのためにコミットのサイズを小さくします。  
    (Reduce the size of the commit for review of the Pull Request.)  
  
  - １つの"プルリクエスト"の中に複数のコミットが存在する場合、 
    １つの”プルリクエスト”に対してレビューを行うためレビューがボトルネックになります。  
    If multiple commits are present in the one "pull request",  
    The review becomes a bottleneck because carrying out a review to one "pull request".  
  
  - このため、タスク毎にFeature Branchを作成して欲しいです。  
    その後、Feature Branchから都度”プルリクエスト”を送って私にレビューを依頼して下さい。  
    (For this reason, I want you to create a Feature Branch for each task.  
    Then, ask the review to me by sending each time "pull request" from the Feature Branch.)  
  
###プルリクエスト(Pull Request)
  - Opentouryoリポジトリから各ユーザのリポジトリにForkします。  
    その後にdevelopブランチからfeatureブランチを作成し作業します。  
    (Fork to the repository for each user from OpenTouryo repository.  
    After having done Fork, You will work to create a feature branch from the develop branch.)  

  - Pull RequestはfeatureブランチからFork元のdevelopブランチに対して送信します。  
    (Send Pull Request to the develop branch of fork source from the feature branch.)  
  
  - もしFork元のdevelopブランチが変更されているようなら、pull 若しくはfetch & mergeをします。  
    (If "develop branch of fork source" was changed,  
    then you should do pull operation or fetch & merge operation.)  
  
  - 古いコードを含むプルリクエストをマージした後、コードがロールバックされていることがありました。  
    従って、プルリクエストを送る前に、OpenTouryoのDevelop BranchをPull若しくはFetch＆Margeしてください。  
    (There is a case that the develop branch's code of OpenTouryo has been rollbacked by merging the pullrequest Including old code. Therefor, Before sending a pull request, please fetch & marge or Pull the develop branch of OpenTouryo.)  
  
  - Fetch＆Margeの代わりに再度Forkする方法もありますが、コメントや元のリポジトリにマージされていない変更は失われます。  
    (There is also a way to re-fork instead of Fetch & Marge, but comment and changes that have not been merge to original repository will be lost.)
　
####参考(Reference)  
  - git fetchの理解からgit mergeとpullの役割 - Qiita  
    http://qiita.com/osamu1203/items/cb94ef9da02e1ec3e921

###テストコード(Test code)
  テストコードを実装可能な場合、以下のパスに実装して下さい。  
  (If you can implement the test code, should be implemented in the following path.)  

  \root\programs\C#\Tests  

###プッシュ(push)
  developブランチでのビルドとテストが完了してからmasterブランチにpushします。  
  (After build and test in the develop brunch are completed, it pushes to a master brunch.)
