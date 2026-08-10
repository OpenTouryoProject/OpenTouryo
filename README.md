# OpenTouryo
*Open Touryo* is an application framework for .NET which was developed using the accumulated know-how with a longstanding application development results.

Click [here](Readme.ja.md) for Japanese version of this file.

## Develop/Run-time environment
The programs in this repository are developed in the following IDE/targetFramework:

- IDE (Integrated Development Environment)  
  Visual Studio 2026
- targetFramework (Run-time environment)
  - .NET Framework 4.8 (net48)
  - .NET 10.0 (net10.0)

The name of projects and solutions include an identifier of the targetFramework.
For example, the projects and solutions for .NET Framework 4.8 are named {identifier}_net48.{ext},
and the ones for .NET 10.0 are named {identifier}_netcore100.{ext}.

## Documents
The documents of *Open Touryo* are stored in the [OpenTouryoDocuments](https://github.com/OpenTouryoProject/OpenTouryoDocuments) repository.

 - [List of documents, Japanese Version](https://github.com/OpenTouryoProject/OpenTouryoDocuments/blob/master/documents/0_Introduction/ja-JP)
 - [Function List, Japanese Version (Excel)](https://github.com/OpenTouryoProject/OpenTouryoDocuments/blob/master/documents/0_Introduction/ja-JP/Functional_list.xlsx)

Some of the documents are [available as a Wiki in this repository](https://github.com/OpenTouryoProject/OpenTouryo/wiki).
## Contents

### [/license/](https://github.com/OpenTouryoProject/OpenTouryo/tree/master/license)
License files are stored in this directory.

### [/root/](https://github.com/OpenTouryoProject/OpenTouryo/tree/master/root)
Programs, configuration files, sql files, and more are stored in this directory.

For the setup and build steps, refer to [/root/README.md](root/README.md).

### [AGENTS.md](AGENTS.md)
This is the **entry point** when working on this repository with a coding agent.
It summarizes the policies to follow and the links to the documents to refer to.

### [CONTRIBUTING.md](CONTRIBUTING.md)
Coding rules and conventions, the branching model, and the granularity of pull requests.
**The same rules apply to people and to coding agents.**

### [SECURITY.md](SECURITY.md)
How to report a vulnerability, which versions are supported, and what is out of scope.
**Do not open a public issue for a security problem** — use
[private vulnerability reporting](https://github.com/OpenTouryoProject/OpenTouryo/security/advisories/new).
