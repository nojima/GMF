using System;
using System.Collections.Generic;
using System.Diagnostics;
using Graph;

namespace GeneralizedMaximumFlow {
    static public class FleischerWayne {
        /// <summary>
        /// sからtへε-最適な減衰流を流す
        /// </summary>
        /// <remarks>
        /// Fleischer and Wayne 2002
        /// O(ε^-2 m^2)
        /// </remarks>
        /// <param name="graph"></param>
        /// <param name="cap"></param>
        /// <param name="gain"></param>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <param name="eps"></param>
        /// <param name="flow"></param>
        /// <returns></returns>
        public static double GeneralizedMaximumFlow(
                DirectedGraph graph, double[] cap, double[] gain, int s, int t, double eps, out double[] flow) {
            int n = graph.Vertices.Count;
            int m = graph.Edges.Count;
            // 初期化
            flow = new double[m];
            double logDelta = -Math.Log(m) / eps;
            var logLength = new double[m];
            for (int e = 0; e < m; ++e) {
                flow[e] = 0.0;
                logLength[e] = logDelta - Math.Log(cap[e]);
            }

            // メモリ確保
            var prevs = new Edge[n];
            var costs = new double[n];
            var dummyEdge = new Edge(-1, s);
            var queue = new PriorityQueue<CostVertexEdgeTuple>();

            for (int iter = 0; ; ++iter) {
                if (iter % 100 == 0) { Trace.WriteLine("Iter #" + iter); }
                bool pathExists = GeneralizedShortestPath(graph, cap, gain, logLength, s, t, n, prevs, costs, dummyEdge, queue);
                if (!pathExists) { return 0.0; }
                UpdateFlowAndLength(graph, cap, gain, flow, logLength, s, t, eps, prevs);
                double dual = 0.0;
                for (int e = 0; e < m; ++e) {
                    dual += cap[e] * Math.Exp(logLength[e]);
                }
                if (dual >= 1.0) { break; }
            }

            DoScaling(graph, cap, flow);

            // 値を計算して返す
            double value = 0.0;
            for (int e = 0; e < m; ++e) {
                value += flow[e] * gain[e];
            }
            return value;
        }

        private static bool GeneralizedShortestPath(
                DirectedGraph graph, double[] cap, double[] gain, double[] logLength,
                int s, int t, int n, Edge[] prevs, double[] costs,
                Edge dummyEdge, PriorityQueue<CostVertexEdgeTuple> queue) {
            for (int i = 0; i < n; ++i) {
                prevs[i] = null;
                costs[i] = double.PositiveInfinity;
            }
            queue.Enqueue(new CostVertexEdgeTuple(0.0, s, dummyEdge));
            costs[s] = 0.0;
            while (queue.Count > 0) {
                CostVertexEdgeTuple tuple = queue.Dequeue();
                int v = tuple.Vertex;
                if (prevs[v] != null) { continue; }
                prevs[v] = tuple.Edge;
                if (v == t) { break; }
                foreach (Edge e in graph.OutEdges[v]) {
                    int w = e.Dst;
                    double newCost = (costs[v] + Math.Exp(logLength[e.Index])) / gain[e.Index];
                    if (newCost < costs[w]) {
                        costs[w] = newCost;
                        queue.Enqueue(new CostVertexEdgeTuple(newCost, w, e));
                    }
                }
            }
            if (prevs[t] == null) { return false; }
            queue.Clear();
            return true;
        }

        private struct CostVertexEdgeTuple : IComparable<CostVertexEdgeTuple> {
            public double Cost;
            public int Vertex;
            public Edge Edge;

            public CostVertexEdgeTuple(double cost, int vertex, Edge edge) {
                Cost = cost;
                Vertex = vertex;
                Edge = edge;
            }

            public int CompareTo(CostVertexEdgeTuple other) {
                return Cost.CompareTo(other.Cost);
            }
        }

        private static void UpdateFlowAndLength(
                DirectedGraph graph, double[] cap, double[] gain, double[] flow, double[] logLength,
                int s, int t, double eps, Edge[] prevs) {
            double maxVaiolateRatio = 0.0;
            double value = 1.0;
            int u = t;
            while (u != s) {
                Edge e = prevs[u];
                value /= gain[e.Index];
                double vaiolateRatio = value / cap[e.Index];
                if (vaiolateRatio > maxVaiolateRatio) { maxVaiolateRatio = vaiolateRatio; }
                u = e.Src;
            }
            value = 1.0 / maxVaiolateRatio;
            u = t;
            while (u != s) {
                Edge e = prevs[u];
                value /= gain[e.Index];
                flow[e.Index] += value;
                logLength[e.Index] += Math.Log(1.0 + (eps * value) / cap[e.Index]);
                u = e.Src;
            }
        }

        private static void DoScaling(DirectedGraph graph, double[] cap, double[] flow) {
            double maxVaiolateRatio = 1.0;
            int m = graph.Edges.Count;
            for (int e = 0; e < m; ++e) {
                double ratio = flow[e] / cap[e];
                if (ratio > maxVaiolateRatio) { maxVaiolateRatio = ratio; }
            }
            for (int e = 0; e < m; ++e) {
                flow[e] /= maxVaiolateRatio;
            }
        }    
    }
}
