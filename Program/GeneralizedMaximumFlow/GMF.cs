using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using Graph;

namespace GeneralizedMaximumFlow {
    public static class GMF {
        /// <summary>
        /// GMFを計算して結果を出力する
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <param name="graphLoadTime"></param>
        /// <param name="graph"></param>
        /// <param name="cap"></param>
        /// <param name="gain"></param>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <param name="eps"></param>
        public static void Run(string input, string output, long graphLoadTime, DirectedGraph graph, double[] cap, double[] gain,
                int s, int t, double eps, string method = "FleischerWayne") {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            double[] flow = null;
            double value;
            if (method == "FleischerWayne") {
                value = FleischerWayne.GeneralizedMaximumFlow(graph, cap, gain, s, t, eps, out flow);
            } else if (method == "Greedy") {
                value = GreedyGMF.GeneralizedMaximumFlow(graph, cap, gain, s, t, ref flow);
            } else if (method == "GreedyImproved") {
                GreedyGMF.ConstructInitialFlow(graph, cap, gain, s, t, out flow);
                value = GreedyGMF.GeneralizedMaximumFlow(graph, cap, gain, s, t, ref flow);
            } else {
                throw new ArgumentException("method " + method + " は定義されていません．");
            }
            var gmfTime = stopwatch.ElapsedMilliseconds;
            stopwatch.Stop();

            Trace.WriteLine("Writing the results");
            Utility.CreateDirectoryIfNotExists(output);
            using (var writer1 = new StreamWriter(output + "/Flow.csv")) {
                OutputFlow(writer1, graph, cap, gain, flow);
            }
            using (var writer2 = new StreamWriter(output + "/Value.txt")) {
                writer2.WriteLine(value);
            }
            using (var writer3 = new StreamWriter(output + "/Summary.txt")) {
                writer3.WriteLine("Input: " + input);
                writer3.WriteLine("Vertex Count: " + graph.Vertices.Count);
                writer3.WriteLine("Edge Count: " + graph.Edges.Count);
                writer3.WriteLine("Source: " + graph.Vertices[s].OriginalId);
                writer3.WriteLine("Sink: " + graph.Vertices[t].OriginalId);
                writer3.WriteLine("Method: " + method);
                writer3.WriteLine("Eps: " + eps);
                writer3.WriteLine("Graph Load Time [ms]: " + graphLoadTime);
                writer3.WriteLine("GMF Time [ms]: " + gmfTime);
                writer3.WriteLine("Value: " + value);
            }
        }

        static void OutputFlow(StreamWriter writer, DirectedGraph graph, double[] cap, double[] gain, double[] flow) {
            for (int e = 0; e < graph.Edges.Count; ++e) {
                writer.WriteLine("{0},{1},{2},{3},{4}",
                    graph.Vertices[graph.Edges[e].Src].OriginalId,
                    graph.Vertices[graph.Edges[e].Dst].OriginalId,
                    cap[e], gain[e], flow[e]);
            }
        }
    }
}
