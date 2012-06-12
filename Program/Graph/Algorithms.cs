using System;
using System.Collections.Generic;

namespace Graph {
    public static class Algorithms {
        /// <summary>
        /// グラフを連結成分分解する
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="ids"></param>
        /// <returns>
        /// グラフの連結成分数を返す
        /// また，ids[v]にvが属する連結成分のIDを格納する
        /// </returns>
        public static int WeaklyConnectedComponents(DirectedGraph graph, out int[] ids) {
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
                        int w = e.Dst;
                        if (ids[w] == -1) {
                            ids[w] = currId;
                            stack.Push(w);
                        }
                    }
                    foreach (Edge e in graph.InEdges[v]) {
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
    }
}
