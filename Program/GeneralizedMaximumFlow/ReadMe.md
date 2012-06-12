# Generalized Maximum Flow

source から sink までの Generalized Maximum Flow の近似値を求めるプログラム．


## 引数

    --input     Required. 入力元のファイル名
    --output    Required. 出力先のディレクトリ名
    --eps       Required. 許容誤差
    --source    Required. GMFの始点
    --sink      Required. GMFの終点
    --quiet     トレースメッセージを出力しない
    --help      Display this help screen.


## アルゴリズム

現在の実装は Fleischer-Wayne のアルゴリズムに基づいている．

詳しくは次の論文を参照:
Lisa K. Fleischer, Kevin D. Wayne: _Fast and Simple Approximation Schemes for Generalized Flow_, Mathematical Programming, Vol. 91, No. 2, pp. 215–238, 2002.