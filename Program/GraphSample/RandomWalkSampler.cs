using System;
using System.Collections.Generic;
using System.Diagnostics;
using Graph;

namespace GraphSample {
    class RandomWalkSampler {
        /// <summary>
        /// Random Walk Sampling という手法を用いてサンプリングを行う
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="sampledVertexCount"></param>
        /// <param name="random"></param>
        /// <returns></returns>
        public DirectedGraph Sample(DirectedGraph graph, int sampledVertexCount, Random random) {
            int n = graph.Vertices.Count;
            int m = graph.Edges.Count;
            var result = new DirectedGraph();
            var visit = new bool[n];
            var used = new bool[m];
            var index2index = new Dictionary<int, int>();

            while (result.Vertices.Count < sampledVertexCount) {
                int start = random.Next(graph.Vertices.Count);
                if (!visit[start]) {
                    AddVertexToResultGraph(start, graph, result, index2index);
                    visit[start] = true;
                }

                int v = start;
                for (int step = 0; step < n && result.Vertices.Count < sampledVertexCount; ++step) {
                    if (graph.OutDegree(v) == 0 || random.NextDouble() < 0.15) {
                        v = start;
                    } else {
                        int choice = random.Next(graph.OutDegree(v));
                        Edge e = graph.OutEdges[v][choice];
                        v = e.Dst;
                        if (!visit[v]) {
                            AddVertexToResultGraph(v, graph, result, index2index);
                            visit[v] = true;
                        }
                        if (!used[e.Index]) {
                            result.AddEdge(new Edge(index2index[e.Src], index2index[e.Dst]));
                            used[e.Index] = true;
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
