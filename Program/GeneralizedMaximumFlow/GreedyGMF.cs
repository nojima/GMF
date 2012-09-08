using System;
using System.Collections.Generic;
using System.Diagnostics;
using Graph;

namespace GeneralizedMaximumFlow {
    public static class GreedyGMF {
        private const double Infty = 1e12;

        public static double GeneralizedMaximumFlow(
                DirectedGraph graph, double[] cap, double[] gain, int s, int t, ref double[] flow) {
            if (s == t) {
                throw new ArgumentException("始点と終点が同じ頂点です.");
            }

            int n = graph.Vertices.Count;
            int m = graph.Edges.Count;

            if (flow == null)
                flow = new double[m];
            var income = new double[n];
            var prev = new Edge[n];
            while (true) {
                var queue = new PriorityQueue<State>();

                for (int i = 0; i < n; ++i) {
                    income[i] = 0;
                    prev[i] = null;
                }

                income[s] = Infty;
                queue.Enqueue(new State(Infty, s, new Edge(s, s)));

                while (queue.Count > 0) {
                    State state = queue.Dequeue();
                    int v = state.Vertex;
                    if (prev[v] != null) { continue; }
                    prev[v] = state.Edge;
                    if (v == t) { break; }

                    foreach (Edge e in graph.OutEdges[v]) {
                        if (cap[e.Index] - flow[e.Index] <= 1e-10) { continue; }
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

                double maxVaiolateRatio = 0.0;
                double f = 1.0;
                int u = t;
                while (u != s) {
                    Edge e = prev[u];
                    f /= gain[e.Index];
                    double vaiolateRatio = f / (cap[e.Index] - flow[e.Index]);
                    if (vaiolateRatio > maxVaiolateRatio) { maxVaiolateRatio = vaiolateRatio; }
                    u = e.Src;
                }
                f = 1.0 / maxVaiolateRatio;
                u = t;
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

        public static double ConstructInitialFlow(DirectedGraph graph, double[] cap, double[] gain, int s, int t, out double[] flow) {
            int n = graph.Vertices.Count;
            int m = graph.Edges.Count;

            var prev = new Edge[n];
            var income = new double[n];
            var queue = new PriorityQueue<State>();

            income[s] = 1.0;
            queue.Enqueue(new State(1.0, s, new Edge(s, s)));

            while (queue.Count > 0) {
                State state = queue.Dequeue();
                int v = state.Vertex;
                if (prev[v] != null) { continue; }
                prev[v] = state.Edge;

                foreach (Edge e in graph.OutEdges[v]) {
                    int w = e.Dst;
                    double newIncome = income[v] * gain[e.Index];
                    if (newIncome > income[w]) {
                        income[w] = newIncome;
                        queue.Enqueue(new State(newIncome, w, e));
                    }
                }
            }

            var cap2 = new double[m];
            foreach (var e in graph.Edges) {
                cap2[e.Index] = cap[e.Index] / income[e.Src];
            }

            var flow2 = new double[m];
            Dinic.MaximumFlow(graph, cap2, s, t, out flow2);

            flow = new double[m];
            var visit = new bool[n];
            Translate(graph, gain, flow2, flow, visit, t);

            foreach (Edge e in graph.Edges) {
                if (cap[e.Index] < flow[e.Index]) {
                    if (flow[e.Index] - cap[e.Index] < 1e-8)
                        flow[e.Index] = cap[e.Index];           // 計算誤差対策
                    else
                        Debug.Assert(false);
                }
            }

            double value = 0;
            foreach (Edge e in graph.InEdges[t]) {
                value += gain[e.Index] * flow[e.Index];
            }
            return value;
        }

        private static void Translate(DirectedGraph graph, double[] gain, double[] flow, double[] gflow, bool[] visit, int v) {
            visit[v] = true;
            double x = 0, y = 0;
            foreach (var e in graph.InEdges[v]) {
                if (flow[e.Index] > 1e-10) {
                    if (!visit[e.Src])
                        Translate(graph, gain, flow, gflow, visit, e.Src);
                    x += flow[e.Index];
                    y += gflow[e.Index] * gain[e.Index];
                }
            }
            foreach (var e in graph.OutEdges[v]) {
                if (flow[e.Index] > 1e-10)
                    gflow[e.Index] = (x < 1e-10) ? flow[e.Index] : y * flow[e.Index] / x;
            }
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
