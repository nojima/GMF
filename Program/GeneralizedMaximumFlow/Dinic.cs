using System;
using System.Collections.Generic;
using System.Diagnostics;
using Graph;

namespace GeneralizedMaximumFlow {
    public static class Dinic {
        private const double SMALL = 1e-6;

        public static double MaximumFlow(
                DirectedGraph graph, double[] cap, int s, int t, out double[] flow) {
            if (s == t) {
                throw new ArgumentException("始点と終点が同じ頂点です.");
            }

            int n = graph.Vertices.Count;
            int m = graph.Edges.Count;
            flow = new double[m];

            double total = 0;
            for (bool cont = true; cont; ) {
                cont = false;

                var level = new int[n];
                for (int i = 0; i < n; ++i)
                    level[i] = -1;
                level[s] = 0;

                var Q = new Queue<int>();
                Q.Enqueue(s);

                for (int d = n; Q.Count > 0 && level[Q.Peek()] < d; ) {
                    int u = Q.Dequeue();
                    if (u == t) d = level[u];
                    foreach (var e in graph.OutEdges[u]) {
                        if (cap[e.Index] - flow[e.Index] >= SMALL && level[e.Dst] == -1) {
                            Q.Enqueue(e.Dst);
                            level[e.Dst] = level[u] + 1;
                        }
                    }
                    foreach (var e in graph.InEdges[u]) {
                        if (flow[e.Index] >= SMALL && level[e.Dst] == -1) {
                            Q.Enqueue(e.Dst);
                            level[e.Dst] = level[u] + 1;
                        }
                    }
                }

                var finished = new bool[n];
                for (double f = 1; f > 0; ) {
                    f = Augment(graph, cap, flow, level, finished, s, t, double.PositiveInfinity);
                    if (f < SMALL) break;
                    total += f;
                    cont = true;
                }
            }
            return total;
        }

        private static double Augment(DirectedGraph graph, double[] cap, double[] flow, int[] level, bool[] finished, int u, int t, double cur) {
            if (u == t || cur < SMALL) return cur;
            if (finished[u]) return 0;
            finished[u] = true;
            foreach (var e in graph.OutEdges[u]) {
                if (level[e.Dst] > level[u]) {
                    double f = Augment(graph, cap, flow, level, finished, e.Dst, t, Math.Min(cur, cap[e.Index] - flow[e.Index]));
                    if (f >= SMALL) {
                        flow[e.Index] += f;
                        finished[u] = false;
                        return f;
                    }
                }
            }
            foreach (var e in graph.InEdges[u]) {
                if (level[e.Dst] > level[u]) {
                    double f = Augment(graph, cap, flow, level, finished, e.Dst, t, Math.Min(cur, flow[e.Index]));
                    if (f >= SMALL) {
                        flow[e.Index] -= f;
                        finished[u] = false;
                        return f;
                    }
                }
            }
            return 0;
        }
    }
}
