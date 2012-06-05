# GraphSample

グラフをサンプリングするツール．

Random Vertex Sampling と Forest Fire Sampling が実装されている．

## コマンドライン引数

    -vertices              Required. 出力するグラフの頂点数
    --input                Required. 入力元のファイル名
    --output               Required. 出力先のファイル名
    --seed                 乱数のシード
    --quiet                トレースメッセージを出力しない
    --method               仕様するサンプリングの手法 (RandomVertex, ForestFire)
    --burning-probability  ForestFireにおけるBurning Probability

## Random Vertex Sampling

ランダムに指定された個数の頂点を選び，それらの頂点が誘導する部分グラフをサンプリング結果として返す．


## Forest Fire Sampling

以下の方法でサンプリングを行う．

1. 一様な確率で頂点をひとつ選ぶ．(vとする)

2. x を p/(1-p) を平均とする幾何分布に従う乱数とする．v から出る辺を x 個ランダムに選ぶ．

3. 選ばれた辺の v でない端点を w_1, w_2, ..., w_x とする．
   各 w_i に対して，まだ w_i を訪れていない場合，w_i に対して 2 以降の処理を行う．

4. 処理対象の辺が無くなったら 1 に戻ってやり直す．

5. 十分な数の頂点をサンプルできたら終了．

詳しくは次の論文を参照: Jure Leskovec, Christos Faloutsos: _Sampling from Large Graphs_, In KDD, 2006
