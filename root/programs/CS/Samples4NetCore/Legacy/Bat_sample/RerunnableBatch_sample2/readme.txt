＜使い方＞
事前にNorthwindデータベースに対し「CREATE ORDERS2.sql」を実行しORDERS2テーブルを作成して下さい。
実行の都度、Northwindデータベースに対し「DELETE FROM ORDERS2」と実行しORDERS2テーブルのデータをクリアして下さい。

＜特徴＞
ラウンドトリップ軽減のため、SQLUtilityと共に、ExecGenerateSQLメソッドを使用します。
通常は、静的SQLを使います。必要に応じて動的SQLへ切替えて評価可能です。
（LayerBクラス内のコードを、DaoOrders2_S1_Insert.sql → DaoOrders2_D1_Insert.xmlに修正する）。