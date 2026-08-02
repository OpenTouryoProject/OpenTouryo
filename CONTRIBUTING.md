# How to contribute

Click [here](Contributing.ja.md) for the Japanese version of this file.

## Programming Rules and Conventions

### Comment
 - Aim at about 1/3 (33%) of the amount of code for the amount of comments.  
   - A higher comment ratio is set in consideration of the maintenance.  
   - Redundant comments are unnecessary. The amount of (redundant) comments at the present time was about 43%.  

 - Change history will continue only as the history of the header, even after the introduction of Git. Descriptions such as the start and end point of the fixes are not required.  

### Coding
 - Writing duplicate code and information is prohibited in this project.  
   This is "Once and Only Once" or "Don't repeat yourself".  

 - This project is cross-compiled for C#, VB, .NET Framework (net48) and .NET Core (net10.0).  
   However, the VB version is provided only for .NET Framework (net48).

 - Backward compatibility is highly maintained, and breaking changes are kept to a minimum.
   - When supporting both a new and an old runtime, the implementation is aligned with the old one (e.g. async/await was not used while the support of VS 2010 and .NET 3.5 was included).
   - Because .NET Framework (net48) and .NET Core (net10.0) are different lines, they are cross-compiled by using conditional compilation (preprocessor).
   - Breaking changes are basically for the cases of discontinuation on the runtime side, and a sufficient "obsolete" period is provided before the change.

### Coding agent
The policy for making use of it is described in [AGENTS.md](AGENTS.md).

## C#, VB
 Provision of the VB version is not required. If necessary, we will convert it to VB by using tools (Sharpdeveloper, generative AI).  

## Flow

### GitHub Flow
 "Push the product every day, test it constantly and deploy it."  
 GitHub Flow is not adopted because there is no such culture of shipment in this project.  

 - GitHub Flow - Scott Chacon (original)  
   https://scottchacon.com/2011/08/31/github-flow/  
 - Japanese translation of the above article  
   https://gist.github.com/Gab-km/3705015  

### git-flow
 This repository is based on the git-flow branching model in "Reference" below.
 
 - The master and develop branches are permanent.  
 - Other branches are created as needed.  

#### Reference
 - A successful Git branching model » nvie.com  
   https://nvie.com/posts/a-successful-git-branching-model/  
 - 見えないチカラ A successful Git branching model を翻訳しました  
   http://keijinsonyaban.blogspot.com/2010/10/a-successful-git-branching-model.html  

 Note that a note by the author (Note of reflection, 2020/03/05) has been added to the original article above,  
 which states that git-flow is not recommended for web applications with continuous delivery and so on.  
 As described above, this project is not operated to ship every day, so git-flow is adopted.  

### About the "pull request"

#### Size of the "pull request"
 - Reduce the size of the "commit" and the "pull request" for the review of the "pull request".  

 - If multiple tasks of bugs or enhancements are present in one "pull request", one delay affects the others.

 - For this reason, you need to create a feature branch for each task of a bug or an enhancement.  
   After that, ask me for a review by sending a "pull request" from the feature branch each time.  

 - Also, please do not commit unnecessary changes such as indentation changed by the IDE or the editor.  

#### Send a "pull request"
 - Fork from the OpenTouryo repository to the repository of each user.  
   After that, create a feature branch from the develop branch and work on it.  

 - Send the "pull request" from your feature branch to the develop branch of the fork source.  

 - If the develop branch of the fork source has been changed, do "pull" or "fetch & merge" to the feature branch.  

#### Notes for the "pull request"
 - Keep the develop / feature branches up to date with the following steps.  

   1. "pull" or "fetch & merge" the develop branch of OpenTouryo into your develop / feature branch.  
   2. Recreate the feature branch from your develop branch.  
   3. Reflect the changes to the feature branch.  
   4. Test and push, and then send the "pull request".  

 - These steps serve the following two purposes.  

   - Keeping up with the changes of the develop branch of the fork source.  
   - Preventing the commit log from being contaminated by rolling back the program after an incorrect change.  

 - There is also a way to fork again instead of "pull" or "fetch & merge", but the comments and the changes that are not merged into the fork source are lost.  
