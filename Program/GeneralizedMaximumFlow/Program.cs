﻿using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using Graph;
using CommandLine;
using CommandLine.Text;

namespace GeneralizedMaximumFlow {
    class Options {
        [Option(null, "input", Required = true, HelpText = "入力元のファイル名")]
        public string Input { get; set; }

        [Option(null, "output", Required = true, HelpText = "出力先のディレクトリ名")]
        public string Output { get; set; }

        [Option(null, "eps", Required = true, HelpText = "許容誤差")]
        public double Eps { get; set; }

        [Option(null, "source", Required = true, HelpText = "GMFの始点")]
        public int Source { get; set; }

        [Option(null, "sink", Required = true, HelpText = "GMFの終点")]
        public int Sink { get; set; }

        [Option(null, "quiet", HelpText = "トレースメッセージを出力しない")]
        public bool Quiet { get; set; }

        [HelpOption]
        public string GetUsage() {
            var help = new HelpText {
                AddDashesToOption = true,
                Heading = "Usage: GeneralizedMaximumFlow [OPTIONS]",
            };
            help.AddPostOptionsLine("詳しくは ReadMe.md を参照．\n");
            help.AddOptions(this);
            return help;
        }
    }

    class Program {
        static void OutputFlow(StreamWriter writer, DirectedGraph graph, double[] cap, double[] gain, double[] flow) {
            for (int e = 0; e < graph.Edges.Count; ++e) {
                writer.WriteLine("{0},{1},{2},{3},{4}",
                    graph.Vertices[graph.Edges[e].Src].OriginalId,
                    graph.Vertices[graph.Edges[e].Dst].OriginalId,
                    cap[e], gain[e], flow[e]);
            }
        }

        static void Main(string[] args) {
            var options = new Options();
            if (!CommandLineParser.Default.ParseArguments(args, options)) { Environment.Exit(1); }

            if (!options.Quiet) {
                Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
                Trace.AutoFlush = true;
            }
            Trace.WriteLine("Input: " + options.Input);
            Trace.WriteLine("Output: " + options.Output);

            Trace.WriteLine("Loading the graph from " + options.Input);
            DirectedGraph graph = GraphIO.LoadCSV(options.Input);

            int n = graph.Vertices.Count;
            int m = graph.Edges.Count;
            int s = -1, t = -1;
            for (int i = 0; i < n; ++i) {
                if (graph.Vertices[i].OriginalId == options.Source) { s = i; }
                if (graph.Vertices[i].OriginalId == options.Sink) { t = i; }
            }
            if (s == -1 || t == -1 || s == t) {
                Console.Error.WriteLine("Error: 始点または終点が不正です．");
                Environment.Exit(1);
            }

            Trace.WriteLine("VertexCount: " + n);
            Trace.WriteLine("EdgeCount: " + m);
            Trace.WriteLine("Source: " + options.Source);
            Trace.WriteLine("Sink: " + options.Sink);

            var cap = new double[m];
            var gain = new double[m];
            
            // とりあえず cap と gain は一つの値で埋める
            for (int e = 0; e < m; ++e) {
                cap[e] = 1.0;
                gain[e] = 0.5;
            }

            Trace.WriteLine("Caluculating the generalized maximum flow");
            var flow = new double[m];
            double value = FleischerWayne.GeneralizedMaximumFlow(graph, cap, gain, s, t, options.Eps, out flow);

            Trace.WriteLine("Writing the results");
            using (var writer1 = new StreamWriter(options.Output + "/Flow.csv")) {
                OutputFlow(writer1, graph, cap, gain, flow);
            }
            using (var writer2 = new StreamWriter(options.Output + "/Value.txt")) {
                writer2.WriteLine(value);
            }

            Trace.WriteLine("Done");
        }
    }
}