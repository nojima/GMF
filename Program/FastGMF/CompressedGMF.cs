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

            long compressionTime = 0;
            long gmfTime = 0;
            double minValue = 1e10;
            double[] minFlow = null;
            DirectedGraph minGraph = null;

            for (int i = 0; i < 4; ++i) {
                Trace.WriteLine("Compressing the given graph...");
                stopwatch.Restart();
                Compressor compressor = new Compressor(graph, prob, new Random());
                compressionTime += stopwatch.ElapsedMilliseconds;
                var compressedGraph = compressor.CompressedGraph;
                var mapping = compressor.Mapping;

                Trace.WriteLine("Compressed Vertex Count: " + compressedGraph.Vertices.Count);
                Trace.WriteLine("Compressed Edge Count: " + compressedGraph.Edges.Count);

                Trace.WriteLine("Calculating the generalized maximum flow...");
                stopwatch.Restart();
                double[] flow;
                double value = FleischerWayne.GeneralizedMaximumFlow(compressor.CompressedGraph, cap, gain, mapping[s], mapping[t], eps, out flow);
                gmfTime += stopwatch.ElapsedMilliseconds;

                if (value < minValue) {
                    minValue = value;
                    minFlow = flow;
                    minGraph = compressor.CompressedGraph;
                }
            }

            Trace.WriteLine("Writing the results");
            Utility.CreateDirectoryIfNotExists(output);
            using (var writer1 = new StreamWriter(output + "/Flow.csv")) {
                OutputFlow(writer1, minGraph, cap, gain, minFlow);
            }
            using (var writer2 = new StreamWriter(output + "/Value.txt")) {
                writer2.WriteLine(minValue);
            }
            using (var writer3 = new StreamWriter(output + "/Sunnary.txt")) {
                writer3.WriteLine("VertexCount: " + n);
                writer3.WriteLine("EdgeCount: " + m);
                writer3.WriteLine("Source: " + graph.Vertices[s].OriginalId);
                writer3.WriteLine("Sink: " + graph.Vertices[t].OriginalId);
                writer3.WriteLine("Prob: " + prob);
                writer3.WriteLine("Compressed Vertex Count: " + minGraph.Vertices.Count);
                writer3.WriteLine("Compressed Edge Count: " + minGraph.Edges.Count);
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
