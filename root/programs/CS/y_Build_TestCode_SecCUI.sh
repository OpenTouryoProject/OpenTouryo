# 以下の様にしてWSLから実行する。改行コードはLFに変更してある。
#$ cd /mnt/c/OpenTouryo/root/programs/CS
#$ ./y_Build_TestCode_SecCUI.sh

cd /mnt/c/OpenTouryo/root/programs/CS/Frameworks/Tests/EncAndDecUtilCUI/core80
dotnet publish -c Debug -r ubuntu.16.04-x64 --self-contained
cd bin/Release/net8.0/ubuntu.16.04-x64
dotnet EncAndDecUtilCUICore.dll > ../../../../../ResultCore80OnLinux.txt