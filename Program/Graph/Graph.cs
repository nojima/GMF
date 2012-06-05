using System;
using System.Collections.Generic;

namespace Graph {
    /// <summary>
    /// 頂点
    /// </summary>
    public struct Vertex {
        public int OriginalId;

        public Vertex(int originalId) {
            OriginalId = originalId;
        }
    }

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
        /// <summary>
        /// 頂点集合
        /// </summary>
        public List<Vertex> Vertices;

        /// <summary>
        /// 辺集合
        /// </summary>
        public List<Edge> Edges;

        /// <summary>
        /// 各頂点に対して，その頂点に入る辺のリストを格納した配列
        /// </summary>
        public List<List<Edge>> InEdges;

        /// <summary>
        /// 各頂点に対して，その頂点から出る辺のリストを格納した配列
        /// </summary>
        public List<List<Edge>> OutEdges;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DirectedGraph() {
            Vertices = new List<Vertex>();
            Edges = new List<Edge>();
            InEdges = new List<List<Edge>>();
            OutEdges = new List<List<Edge>>();
        }

        /// <summary>
        /// 頂点を追加する
        /// </summary>
        /// <param name="v"></param>
        public void AddVertex(Vertex v) {
            Vertices.Add(v);
            InEdges.Add(new List<Edge>());
            OutEdges.Add(new List<Edge>());
        }

        /// <summary>
        /// 辺を追加する
        /// </summary>
        /// <param name="e"></param>
        public void AddEdge(Edge e) {
            Edges.Add(e);
            InEdges[e.Dst].Add(e);
            OutEdges[e.Src].Add(e);
        }

        /// <summary>
        /// vの入次数
        /// </summary>
        public int InDegree(int v) { return InEdges[v].Count; }

        /// <summary>
        /// vの出次数
        /// </summary>
        public int OutDegree(int v) { return OutEdges[v].Count; }

        /// <summary>
        /// 指定された頂点集合から誘導される部分グラフを返す
        /// </summary>
        /// <param name="vertices"></param>
        /// <returns></returns>
        public DirectedGraph Induce(IEnumerable<int> vertices) {
            var g = new DirectedGraph();
            var index2index = new Dictionary<int, int>();
            foreach (int v in vertices) {
                index2index.Add(v, g.Vertices.Count);
                g.AddVertex(Vertices[v]);
            }
            foreach (Edge e in Edges) {
                if (index2index.ContainsKey(e.Src) && index2index.ContainsKey(e.Dst)) {
                    g.AddEdge(new Edge(index2index[e.Src], index2index[e.Dst]));
                }
            }
            return g;
        }
    }
}
