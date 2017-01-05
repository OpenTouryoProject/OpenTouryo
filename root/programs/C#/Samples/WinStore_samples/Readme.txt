クロスドメインで実行する場合、
clientaccesspolicy.xmlを、

・IISの場合
　→ Webサービス側が配置してあるIISのルートか、
　
・開発用Webサーバの場合
　→ 開発用WebサーバのWebサービス プロジェクトのルート

に配置する必要がある。

・・・

しかし、開発用Webサーバの場合、仮想パスを設定してしまうと
ルートパス（FQDN名の直下）にclientaccesspolicy.xmlを配置できなくなる。

ASP.NET WebサービスプロジェクトであるASPNETWebServiceは、
開発用Webサーバでルート仮想パス「ASPNETWebService」に設定されているため
silverlightではASPNETWebServiceの汎用サービスインターフェイスを使用できない。
