# GraphMetrics

グラフに対していくつかのメトリクスを計算する．


## 引数

    --input     Required. 入力元のファイル名
    --output    Required. 出力先のディレクトリ名
    --quiet     トレースメッセージを出力しない
    --help      Display this help screen.


## 出力

現在のところ，`$OUTPUT/InDegree.csv` と `$OUTPUT/OutDegree` にそれぞれ入次数と出次数の分布を出力し，`$OUTPUT/Summary.txt` に頂点数，辺数，連結成分数を出力する．
