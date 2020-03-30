# OpenTouryo
*Open Touryo* is an application framework for .NET Framework which was developed using the accumulated know-how with a longstanding application development results.

Click [here](Readme.ja.md) for Japanese version of this file.

## Develop/Run-time environment
The programs in this repository are developed in the following IDE/targetFramework:

- IDE (Integrated Development Environment)
  - Visual Studio 2015
  - Visual Studio 2017
  - Visual Studio 2019
- targetFramework (Run-time environment)
  - .NET Framework 4.5.2 (net452)
  - .NET Framework 4.6 (net46)
  - .NET Framework 4.7 (net47)
  - .NET Framework 4.8 (net48)
  - .NET Core 2.0 (netcoreapp2.0)
  - .NET Core 3.0 (netcoreapp3.0)
  - .NET Standard 2.0 (netstandard2.0)
  - .NET Standard 2.1 (netstandard2.1)

The default targetFramework of projects and solutions is net46(.NET Framework 4.6).
The name of projects and solutions for other targetFramework include the targetFramework.
For example, the projects and solutions for net47(.NET Framework 4.7) are named {identifier}_net47.{ext}.

The programs in this repository are for *open source developers*.
The users who use Open Touryo in a system development project need to use [OpenTouryoTemplates repository](https://github.com/OpenTouryoProject/OpenTouryoTemplates).

## Summary
Please refer to the following files.
 - [List of documents, Japanese Version](https://github.com/OpenTouryoProject/OpenTouryoDocuments/blob/master/documents/0_Introduction/ja-JP)
 - [Function List, Japanese Version (Excel)](https://github.com/OpenTouryoProject/OpenTouryoDocuments/blob/master/documents/0_Introduction/ja-JP/Functional_list.xlsx)

## Details
The documents are stored in [OpenTouryoDocuments](https://github.com/OpenTouryoProject/OpenTouryoDocuments) repository.
For more details, refer to the documents in [OpenTouryoDocuments](https://github.com/OpenTouryoProject/OpenTouryoDocuments) repository.

## Contents

### Directory

#### [/license/](https://github.com/OpenTouryoProject/OpenTouryo/tree/master/license)
License files are stored in this directory.

#### [/root/](https://github.com/OpenTouryoProject/OpenTouryo/tree/master/root)
Programs, configuration files, sql files, and more are stored in this directory.

## Templates base
*Open Touryo Template Base* is the *foundation* of the development infrastructure (project template) of the programs using Open Touryo.
The samples included in *Open Touryo Template Base* can be used to evaluate Open Touryo. 

When the mismatch is generated between *the features of Open Touryo* and *the requirements of the development project*, the customizing template base is useful for resolving the mismatch.  
Refer to the [tutorial document](https://github.com/OpenTouryoProject/OpenTouryo/wiki) about the customizing method of template base.

For more information, please refer to the Readme files in the following repository.
 - [OpenTouryoTemplates](https://github.com/OpenTouryoProject/OpenTouryoTemplates)
