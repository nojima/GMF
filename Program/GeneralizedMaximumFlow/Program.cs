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
        static void Main(string[] args) {
            var options = InitializeOptions(args);
            Trace.WriteLine("Input: " + options.Input);
            Trace.WriteLine("Output: " + options.Output);
            
            Trace.WriteLine("Loading the graph from " + options.Input);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            DirectedGraph graph = GraphIO.LoadCSV(options.Input);
            var graphLoadTime = stopwatch.ElapsedMilliseconds;
            stopwatch.Stop();

            int n = graph.Vertices.Count;
            int m = graph.Edges.Count;
            int s, t;
            Utility.FindSourceAndSink(options.Source, options.Sink, graph, out s, out t);

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
            GMF.Run(options.Input, options.Output, graphLoadTime, graph, cap, gain, s, t, options.Eps);

            Trace.WriteLine("Done");
        }

        private static Options InitializeOptions(string[] args) {
            var options = new Options();
            if (!CommandLineParser.Default.ParseArguments(args, options)) { Environment.Exit(1); }

            if (!options.Quiet) {
                Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
                Trace.AutoFlush = true;
            }
            return options;
        }
    }
}
