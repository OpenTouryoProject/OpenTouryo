＜使い方＞
事前にNorthwindデータベースに対し「CREATE ORDERS2.sql」を実行しORDERS2テーブルを作成して下さい。
実行の都度、Northwindデータベースに対し「DELETE FROM ORDERS2」と実行しORDERS2テーブルのデータをクリアして下さい。

＜特徴＞
ラウンドトリップ軽減のため、SQLUtilityを使用してDataTableからInsert文を生成します。
この測定モデルでは、動的SQLは使用しません。静的SQLのみを使用します。
