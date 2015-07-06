(1)
 Silverlight クラス ライブラリ プロジェクトの
Portable クラス ライブラリ プロジェクト化に伴い、
以下のツールのインストールが必要になりました。

Portable Library Tools 2 拡張機能
https://visualstudiogallery.msdn.microsoft.com/b0e0b5e9-e138-410b-ad10-00cb3caf4981/

(2)
クロスドメインで実行する場合、
clientaccesspolicy.xmlを、

・IISの場合
　→ Webサービス側が配置してあるIISのルートか、
　
・開発用Webサーバの場合
　→ 開発用WebサーバのWebサービス プロジェクトのルート

に配置する必要があります。

・・・

しかし、開発用Webサーバの場合、仮想パスを設定してしまうと
ルートパス（FQDN名の直下）にclientaccesspolicy.xmlを配置できなくなります。

ASP.NET WebサービスプロジェクトであるASPNETWebServiceは、
開発用Webサーバでルート仮想パス「ASPNETWebService」に設定されているため
silverlightではASPNETWebServiceの汎用サービスインターフェイスを使用できません。
