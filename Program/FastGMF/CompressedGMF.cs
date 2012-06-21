using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using Graph;
using GeneralizedMaximumFlow;

namespace FastGMF {
    public static class CompressedGMF {
        public static void Run(DirectedGraph graph, long graphLoadTime, int s, int t, double[] cap, double[] gain, double prob, string output, double eps) {
            int n = graph.Vertices.Count, m = graph.Edges.Count;
            Stopwatch stopwatch = new Stopwatch();

            Trace.WriteLine("Compressing the given graph...");
            stopwatch.Start();
            Compressor compressor = new Compressor(graph, prob, new Random());
            var compressionTime = stopwatch.ElapsedMilliseconds;
            var compressedGraph = compressor.CompressedGraph;
            var mapping = compressor.Mapping;

            Trace.WriteLine("Compressed Vertex Count: " + compressedGraph.Vertices.Count);
            Trace.WriteLine("Compressed Edge Count: " + compressedGraph.Edges.Count);

            Trace.WriteLine("Calculating the generalized maximum flow...");
            stopwatch.Restart();
            double[] flow;
            double value = FleischerWayne.GeneralizedMaximumFlow(compressor.CompressedGraph, cap, gain, mapping[s], mapping[t], eps, out flow);
            var gmfTime = stopwatch.ElapsedMilliseconds;

            Trace.WriteLine("Writing the results");
            Utility.CreateDirectoryIfNotExists(output);
            using (var writer1 = new StreamWriter(output + "/Flow.csv")) {
                OutputFlow(writer1, compressedGraph, cap, gain, flow);
            }
            using (var writer2 = new StreamWriter(output + "/Value.txt")) {
                writer2.WriteLine(value);
            }
            using (var writer3 = new StreamWriter(output + "/Sunnary.txt")) {
                writer3.WriteLine("VertexCount: " + n);
                writer3.WriteLine("EdgeCount: " + m);
                writer3.WriteLine("Source: " + graph.Vertices[s].OriginalId);
                writer3.WriteLine("Sink: " + graph.Vertices[t].OriginalId);
                writer3.WriteLine("Prob: " + prob);
                writer3.WriteLine("Compressed Vertex Count: " + compressedGraph.Vertices.Count);
                writer3.WriteLine("Compressed Edge Count: " + compressedGraph.Edges.Count);
                writer3.WriteLine("Graph Load Time [ms]: " + graphLoadTime);
                writer3.WriteLine("Compression Time [ms]: " + compressionTime);
                writer3.WriteLine("GMF Time [ms]: " + gmfTime);
            }
        }

        private static void OutputFlow(StreamWriter writer, DirectedGraph graph, double[] cap, double[] gain, double[] flow) {
            for (int e = 0; e < graph.Edges.Count; ++e) {
                writer.WriteLine("{0},{1},{2},{3},{4}",
                    graph.Vertices[graph.Edges[e].Src].OriginalId,
                    graph.Vertices[graph.Edges[e].Dst].OriginalId,
                    cap[e], gain[e], flow[e]);
            }
        }
    }
}
