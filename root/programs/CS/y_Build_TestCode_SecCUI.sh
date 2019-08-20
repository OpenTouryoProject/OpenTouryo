# 以下の様にしてWSLから実行する。改行コードはLFに変更してある。
#$ cd /mnt/c/Git1/OpenTouryo/root/programs/CS
#$ ./y_Build_TestCode_SecCUI.sh


cd /mnt/c/Git1/OpenTouryo/root/programs/CS/Frameworks/Tests/EncAndDecUtilCUI/core20
dotnet publish -c Debug -r ubuntu.16.04-x64 --self-contained
cd bin/Release/netcoreapp2.0/ubuntu.16.04-x64
dotnet EncAndDecUtilCUICore.dll > ../../../../../ResultCore20OnLinux.txt

cd /mnt/c/Git1/OpenTouryo/root/programs/CS/Frameworks/Tests/EncAndDecUtilCUI/core30
dotnet publish -c Debug -r ubuntu.16.04-x64 --self-contained
cd bin/Release/netcoreapp3.0/ubuntu.16.04-x64
dotnet EncAndDecUtilCUICore.dll > ../../../../../ResultCore30OnLinux.txt