# ホストビルド型の Dockerfile（#582）
#
#   ホスト側で `dotnet publish` した成果物を、ランタイム イメージへ置くだけ。
#   **コンテナ内ではビルドしない。**
#
#   理由：ASPNETWebService.csproj はフレームワークを
#     <Reference><HintPath>..\Frameworks\Infrastructure\Build_netcore100\net10.0\*.dll
#   というビルド済み DLL への相対参照で持っている。
#   Frameworks を先にビルドしないと restore すら通らないため、
#   コンテナ内でビルドするならビルド コンテキストを root/programs/CS 全体へ
#   広げることになる（Samples4NetCore/ANALYSIS.md 2-1）。
#
#   publish は 1_PublishAndUp.bat が行う。
#
#   **MVC_Sample/Dockerfile と diff が取れる形にしてある。**
#   違うのは ENTRYPOINT の DLL 名だけである。

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

# publish 成果物
COPY ./publish/ ./

# コンテナ用の設定（appsettings.json を上書きする。ASPNETCORE_ENVIRONMENT=Container で効く）
COPY ./appsettings.Container.json ./
# コンテナ用の log4net 定義（出力先を %env{OT_LOG_ROOT}）
COPY ./conf/ ./conf/

# ログと鍵の置き場所を、非 root ユーザで書けるようにしておく。
#
#   **名前付きボリュームは、初回にイメージ側の所有者と権限を引き継ぐ。**
#   ここで chown しておかないと、マウント後に root 所有のままとなり、
#   USER で切り替えた非 root ユーザが書けない。
#
#   **keys は、この Resource Server では使っていない。**（#582）
#   Cookie 認証やセッションを足すときに要る（Startup.cs のコメント参照）ため、
#   置き場所だけ MVC_Sample と揃えてある。
RUN mkdir -p /app/logs /app/keys && chown -R $APP_UID /app/logs /app/keys

# 非 root で動かす（ベース イメージが用意している app ユーザ）
USER $APP_UID

EXPOSE 8080
EXPOSE 8081

ENTRYPOINT ["dotnet", "ASPNETWebService.dll"]
