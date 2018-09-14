＜使い方＞
事前にNorthwindデータベースに対し「CREATE ORDERS2.sql」を実行しORDERS2テーブルを作成して下さい。
実行の都度、Northwindデータベースに対し「DELETE FROM ORDERS2」と実行しORDERS2テーブルのデータをクリアして下さい。

＜特徴＞
通常のオンライン処理と同様のデータアクセスをバッチ的に行います。
通常は、静的SQLを使います。必要に応じて動的SQLへ切替えて評価可能です。
（LayerBクラス内のコードを、dao.S1_Insert → D1_Insertに修正する）。
