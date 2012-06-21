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
        public static void Run(string input, string output, long graphLoadTime, DirectedGraph graph, double[] cap, double[] gain, int s, int t, double eps) {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var flow = new double[graph.Edges.Count];
            double value = FleischerWayne.GeneralizedMaximumFlow(graph, cap, gain, s, t, eps, out flow);
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
                writer3.WriteLine("Eps: " + eps);
                writer3.WriteLine("Graph Load Time [ms]: " + graphLoadTime);
                writer3.WriteLine("GMF Time [ms]: " + gmfTime);
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
