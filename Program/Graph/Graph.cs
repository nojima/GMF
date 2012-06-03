using System;
using System.Collections.Generic;

namespace Graph {
    /// <summary>
    /// 有向辺
    /// </summary>
    public class Edge {
        public int Src;
        public int Dst;

        public Edge(int src, int dst) {
            Src = src;
            Dst = dst;
        }
    }

    /// <summary>
    /// 有向グラフ
    /// </summary>
    public class DirectedGraph {
        private int mEdgeCount;

        /// <summary>
        /// 各頂点に対して，その頂点に入る辺のリストを格納した配列
        /// </summary>
        public List<Edge>[] InEdges;

        /// <summary>
        /// 各頂点に対して，その頂点から出る辺のリストを格納した配列
        /// </summary>
        public List<Edge>[] OutEdges;

        /// <summary>
        /// コンストラクタ
        /// 頂点数は最初に確定される必要がある
        /// </summary>
        /// <param name="vertexCount">グラフの頂点数</param>
        /// <param name="adjListCapacity">隣接リストの初期容量</param>
        public DirectedGraph(int vertexCount, int adjListCapacity = 4) {
            InEdges = new List<Edge>[vertexCount];
            OutEdges = new List<Edge>[vertexCount];
            for (int i = 0; i < vertexCount; ++i) {
                InEdges[i] = new List<Edge>(adjListCapacity);
                OutEdges[i] = new List<Edge>(adjListCapacity);
            }
        }

        /// <summary>
        /// 辺を追加する
        /// </summary>
        /// <param name="e"></param>
        public void AddEdge(Edge e) {
            InEdges[e.Dst].Add(e);
            OutEdges[e.Src].Add(e);
            ++mEdgeCount;
        }

        /// <summary>
        /// 頂点数
        /// </summary>
        public int VertexCount { get { return InEdges.Length; } }

        /// <summary>
        /// 辺数
        /// </summary>
        public int EdgeCount { get { return mEdgeCount; } }

        /// <summary>
        /// vの入次数
        /// </summary>
        public int InDegree(int v) { return InEdges[v].Count; }

        /// <summary>
        /// vの出次数
        /// </summary>
        public int OutDegree(int v) { return OutEdges[v].Count; }
    }
}
