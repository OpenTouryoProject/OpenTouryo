# Security Policy

Click [here](Security.ja.md) for the Japanese version of this file.

## Supported Versions

Security fixes are applied to the latest release line only.

| Version | Supported |
|---|---|
| 3.0.x | :white_check_mark: |
| 2.x | :x: |

Prerelease packages (`-preview*` / `-alpha*`) are for evaluation and are not supported.

## Reporting a Vulnerability

**Please do not open a public issue for a security problem.**

Use **[Private vulnerability reporting](https://github.com/OpenTouryoProject/OpenTouryo/security/advisories/new)**.
The report stays private until a fix is published, and the discussion happens in the same place.

Please include:

- Which assembly and version (for example, `OpenTouryo.Public.Security` 3.0.0)
- Which target framework (`net48` or `net10.0`) — **the two are separate implementations
  in several places**, so a problem may exist in only one of them
- Steps to reproduce, or the code path you believe is affected
- What an attacker gains

We are a small team. We will acknowledge the report and tell you what we intend to do,
but we cannot promise a fixed turnaround time.

## Scope

This repository contains both the framework and the samples that show how to use it.

| Path | Scope |
|---|---|
| `root/programs/CS/Frameworks/Infrastructure/` | **In scope.** This is what ships as NuGet packages |
| `root/programs/CS/Frameworks/Tools/` | **In scope** |
| `root/programs/CS/Samples/`, `Samples4NetCore/`, `root/programs/VB/` | Samples. **Reports are welcome**, but they are teaching material and are not shipped |
| `root/files/resource/X509/` | **Out of scope.** Self-signed certificates and private keys **for tests only** |

## Already known and accepted

**Static analysis (CodeQL) runs on this repository, and its findings have been triaged.**
Before reporting a scanner result, please check
**[#536](https://github.com/OpenTouryoProject/OpenTouryo/issues/536)** — it records what was
fixed, what was dismissed, and why.

The following are known and deliberate:

- **`CipherMode_ECB`** is marked `[Obsolete]`. It is one of five cipher modes the caller can
  choose, and it is **not the default** — when no mode is given, .NET's default (CBC) is used.
  It is kept for backward compatibility
- **`BinarySerialize`** (`BinaryFormatter`) exists **for `net48` only**. It is excluded from
  the `net10.0` build (`<Compile Remove>` in `Public_netcore100.csproj`)
- **Sample `Web.config` files use `requireSSL="false"`**, because the samples are meant to be
  run over HTTP. The production setting is provided next to it, commented out, with a note to
  enable it

Reports that show a **concrete exploit** for any of the above are still welcome.

## Security Practices in This Repository

| | |
|---|---|
| Secret scanning ＋ Push protection | Enabled |
| Code scanning (CodeQL) | Enabled. `csharp`, `javascript-typescript`, `actions` |
| Dependabot alerts / security updates | Enabled |
| Private vulnerability reporting | Enabled |
| Branch protection (`master`) | Review required ＋ CI must pass |

The settings themselves live on GitHub and are not visible from the files in this repository,
so they are written down in [`GitHubUsage.md`](GitHubUsage.md).
