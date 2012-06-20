using System;
using System.Collections.Generic;
using System.Linq;

namespace Graph {
    public static class Algorithms {
        /// <summary>
        /// グラフを連結成分分解する
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="ids"></param>
        /// <param name="edgeUsed">その辺を使うか否かを表す配列．nullの場合は全部使う．</param>
        /// <returns>
        /// グラフの連結成分数を返す
        /// また，ids[v]にvが属する連結成分のIDを格納する
        /// </returns>
        public static int WeaklyConnectedComponents(DirectedGraph graph, out int[] ids, bool[] edgeUsed = null) {
            int n = graph.Vertices.Count;
            ids = new int[n];
            for (int i = 0; i < n; ++i) { ids[i] = -1; }
            int currId = 0;
            var stack = new Stack<int>();
            for (int i = 0; i < n; ++i) {
                if (ids[i] != -1) { continue; }
                stack.Push(i);
                ids[i] = currId;
                while (stack.Count > 0) {
                    int v = stack.Pop();
                    foreach (Edge e in graph.OutEdges[v]) {
                        if (edgeUsed != null && !edgeUsed[e.Index]) { continue; }
                        int w = e.Dst;
                        if (ids[w] == -1) {
                            ids[w] = currId;
                            stack.Push(w);
                        }
                    }
                    foreach (Edge e in graph.InEdges[v]) {
                        if (edgeUsed != null && !edgeUsed[e.Index]) { continue; }
                        int w = e.Src;
                        if (ids[w] == -1) {
                            ids[w] = currId;
                            stack.Push(w);
                        }
                    }
                }
                currId += 1;
            }
            return currId;
        }

        /// <summary>
        /// グラフを mapping に従って縮約したものを返す．
        /// </summary>
        /// <param name="graph">縮約するグラフ</param>
        /// <param name="mapping">mapping[i] は 縮約元のグラフのi番目の頂点の縮約先の頂点</param>
        /// <returns>縮約されたグラフ</returns>
        public static DirectedGraph Contract(DirectedGraph graph, int[] mapping) {
            var result = new DirectedGraph();
            int nVertices = mapping.Max() + 1;
            for (int i = 0; i < nVertices; ++i) {
                result.AddVertex(new Vertex());
            }
            for (int i = 0; i < mapping.Length; ++i) {
                result.Vertices[mapping[i]] = graph.Vertices[i];    // copy vertex
            }
            for (int u = 0; u < graph.Vertices.Count; ++u) {
                int src = mapping[u];
                foreach (Edge e in graph.OutEdges[u]) {
                    int dst = mapping[e.Dst];
                    if (src != dst) {
                        result.AddEdge(new Edge(src, dst));
                    }
                }
            }
            return result;
        }
    }
}
