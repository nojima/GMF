using System;
using System.Collections.Generic;
using System.Diagnostics;
using Graph;

namespace GraphSample {
    /// <summary>
    /// Forest Fire Sampling という手法を使ってサンプリングを行う
    /// </summary>
    /// <remarks>
    /// Forest Fire Sampling に関しては以下の論文を参照
    /// Jure Leskovec, Christos Faloutsos: Sampling from Large Graphs, In KDD, 2006
    /// </remarks>
    class ForestFireSampler {
        public DirectedGraph Sample(DirectedGraph graph, int sampledVertexCount, double burningProbability, Random random) {
            var result = new DirectedGraph();
            var index2index = new Dictionary<int, int>();
            var visit = new bool[graph.Vertices.Count];
            
            while (result.Vertices.Count < sampledVertexCount) {
                // 開始点を決める
                int start;
                do { start = random.Next(graph.Vertices.Count); } while (visit[start]);
                
                var stack = new Stack<int>();
                stack.Push(start);
                AddVertexToResultGraph(start, graph, result, index2index);
                visit[start] = true;

                while (stack.Count > 0 && result.Vertices.Count < sampledVertexCount) {
                    int v = stack.Pop();
                    var outEdges = graph.OutEdges[v];
                    for (int i = 0; i < outEdges.Count; ++i) {
                        if (random.NextDouble() >= burningProbability) { break; }

                        // i番目以降の辺を一つ選んでi番目と入れ替える
                        int j = random.Next(i, outEdges.Count);
                        Edge t = outEdges[i];
                        outEdges[i] = outEdges[j];
                        outEdges[j] = t;

                        // 結果に追加
                        int dst = outEdges[i].Dst;
                        if (!visit[dst]) { AddVertexToResultGraph(dst, graph, result, index2index); }
                        result.AddEdge(new Edge(index2index[v], index2index[dst]));

                        // 再帰的に探索
                        if (!visit[dst]) {
                            visit[dst] = true;
                            stack.Push(dst);
                        }
                    }
                }
            }

            return result;
        }

        private void AddVertexToResultGraph(int v, DirectedGraph graph, DirectedGraph result, Dictionary<int, int> index2index) {
            index2index.Add(v, result.Vertices.Count);
            result.AddVertex(graph.Vertices[v]);
        }
    }
}
