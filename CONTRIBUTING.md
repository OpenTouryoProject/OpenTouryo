# How to contribute

Click [here](Contributing.ja.md) for Japanese version of this file.

## Programming Rules and Conventions

### Comment
 - The amount of comments aim about 1/3(33%) of the amount of code.  
   It has set a higher ratio comment in consideration of the maintenance.  
   Redundant comment is unnecessary. Amount of redundant comment at the present time was about 43%.

 - Change history will continue only history of the header.  
   Descriptions such as start and end point of the fixes are not required.

### Coding
 - Writing of duplicate code and information has been prohibited in this project.  
   This is "Once and Only Once" or "Don't repeat yourself".

 - Please keep in mind that it is limited to the statement  
   that can be used in run-time which was specified in the project file.  

   For example, Contained the support of VS 2010 and .NET 3.5 previously  
   (The current support is from VS 2015 and .NET 4.6 to latest version).  
   e.g. , such as async/await keyword that can not be used in VS 2010 and .NET 3.5 has not been used in this project.

## C#, VB
 Provision of VB version is not required.  
 If necessary, we will converte to VB by using the Sharpdeveloper.

## Flow

### GitHub Flow
 GitHubFlow is not adopted because there is no culture of the shipment  
 that test and push constantly and deploy the product every day.  

 - GitHub Flow - Scott Chacon（原文）  
   http://scottchacon.com/2011/08/31/github-flow.html  
 - 上記記事の日本語訳  
   https://gist.github.com/Gab-km/3705015

### git-flow
 This repository is based on the branch model of the the following URL.  
 - develop branch and master branch has been permanent.  
 - Create other branches as needed.

#### Reference
 - A successful Git branching model » nvie.com  
   http://nvie.com/posts/a-successful-git-branching-model/  
 - 見えないチカラ A successful Git branching model を翻訳しました  
   http://keijinsonyaban.blogspot.jp/2010/10/successful-git-branching-model.html  

### About the "pull request"

#### Size of the "pull request"
 - Reduce the size of "commit" and "pull request" for review of "pull request".

 - If multiple tasks of bug or enhance are present in the one "pull request", one delay affects the other.

 - For this reason, you must create a feature branch for each task of bug or enhance.  
   After that, ask the review to me by sending each time "pull request" from the feature branch.

 - Also, please do not commit unnecessary changes that indentation is changed by the IDE or editor.

#### Send a "pull request"
 - Fork to the repository by each user from OpenTouryo repository.  
   After having done fork, create a feature branch from the develop branch.  

 - Send "pull request" to the "develop branch of fork source" from your feature branch.

 - If "develop branch of fork source" was changed,  
   then you should do the "pull" or "fetch & merge" operation to the feature branch.

#### Notes for "pull request"
 - You keep the develop / feature branch up to date as follows.  

   - "pull" or "fetch & merge" OpenTouryo develop branch from your develop / feature branch.  
     and you recreate the feature branch from your develop branch and you reflect changes to the feature branch.  
     After that, test and push, and finally send "pull request".

   - There is also a way to fork again instead of pull or fetch & marge,  
     but comments and changes that are not merged original fork source are lost.

 - To prevent contaminating the commit log by rolling back program after the incorrect change, follow these steps.  
   - You recreate the feature branch from your develop branch.  
   - And you reflect changes again to the feature branch.  
   - After that, test and push, and finally send "pull request".
