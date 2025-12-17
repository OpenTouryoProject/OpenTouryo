# 以下の様にしてWSLから実行する。改行コードはLFに変更してある。
#$ cd /mnt/c/OpenTouryo/root/programs/CS
#$ ./y_Build_TestCode_SecCUI.sh

cd /mnt/c/OpenTouryo/root/programs/CS/Frameworks/Tests/EncAndDecUtilCUI/core100
dotnet publish -c Debug -r linux-x64 --self-contained
cd bin/Debug/net10.0/linux-x64
dotnet EncAndDecUtilCUICore.dll > ../../../../../ResultCore100OnLinux.txt