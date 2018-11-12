## 実行方法のメモ
OpenTouryoリポジトリを「C:\Git」にクローンした場合。

### WSL(Ubuntu)
```CMD
cd /mnt/c/Git1/OpenTouryo/root/programs/CS/Frameworks/Tools/Encryption/EncAndDecUtilCUI
dotnet publish -c Release -r ubuntu.16.04-x64 --self-contained
cd bin/Release/netcoreapp2.0/ubuntu.16.04-x64/
dotnet EncAndDecUtilCUI.dll
```