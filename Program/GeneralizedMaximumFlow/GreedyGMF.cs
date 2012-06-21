using System;
using System.Collections.Generic;
using System.Diagnostics;
using Graph;

namespace GeneralizedMaximumFlow {
    public static class GreedyGMF {
        private const double Infty = 1e12;

        public static double GeneralizedMaximumFlow(
                DirectedGraph graph, double[] cap, double[] gain, int s, int t, out double[] flow) {
            if (s == t) {
                throw new ArgumentException("始点と終点が同じ頂点です.");
            }

            int n = graph.Vertices.Count;
            int m = graph.Edges.Count;

            flow = new double[m];
            var income = new double[n];
            var prev = new Edge[n];
            while (true) {
                var queue = new PriorityQueue<State>();

                for (int i = 0; i < n; ++i) { income[i] = 0; prev[i] = null; }

                income[s] = Infty;
                queue.Enqueue(new State(Infty, s, new Edge(s, s)));

                while (queue.Count > 0) {
                    State state = queue.Dequeue();
                    int v = state.Vertex;
                    if (prev[v] != null) { continue; }
                    prev[v] = state.Edge;
                    if (v == t) { break; }

                    foreach (Edge e in graph.OutEdges[v]) {
                        int w = e.Dst;
                        double newIncome = Math.Min(income[v], cap[e.Index] - flow[e.Index]) * gain[e.Index];
                        if (newIncome > income[w]) {
                            income[w] = newIncome;
                            queue.Enqueue(new State(newIncome, w, e));
                        }
                    }
                }

                if (prev[t] == null) {
                    // 流せなかったら終了
                    break;
                }

                int u = t;
                double f = income[t];
                while (u != s) {
                    Edge e = prev[u];
                    f /= gain[e.Index];
                    flow[e.Index] += f;
                    u = e.Src;
                }
            }

            // 値を計算して返す
            double value = 0.0;
            foreach (Edge e in graph.InEdges[t]) {
                value += flow[e.Index] * gain[e.Index];
            }
            return value;
        }

        struct State : IComparable<State> {
            public double Income;
            public int Vertex;
            public Edge Edge;

            public State(double income, int vertex, Edge edge) {
                Income = income;
                Vertex = vertex;
                Edge = edge;
            }

            public int CompareTo(State other) {
                return other.Income.CompareTo(Income);
            }
        }
    }
}
