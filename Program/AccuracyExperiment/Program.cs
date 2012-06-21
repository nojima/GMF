using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using Graph;
using CommandLine;
using CommandLine.Text;
using GeneralizedMaximumFlow;

namespace AccuracyExperiment {
    class Options {
        [Option(null, "input", Required = true, HelpText = "入力元のファイル名")]
        public string Input { get; set; }

        [Option(null, "output", Required = true, HelpText = "出力先のディレクトリ名")]
        public string Output { get; set; }

        [Option(null, "eps", Required = true, HelpText = "許容誤差")]
        public double Eps { get; set; }

        [Option(null, "testcase", Required = true, HelpText = "テストを何回行うか")]
        public int TestCase { get; set; }

        [Option(null, "quiet", HelpText = "トレースメッセージを出力しない")]
        public bool Quiet { get; set; }

        [HelpOption]
        public string GetUsage() {
            var help = new HelpText {
                AddDashesToOption = true,
                Heading = "Usage: AccuracyExperiment [OPTIONS]",
            };
            help.AddPostOptionsLine("詳しくは ReadMe.md を参照．\n");
            help.AddOptions(this);
            return help;
        }
    }

    class Program {
        static void Main(string[] args) {
            var options = InitializeOptions(args);

            Trace.WriteLine("Loading graph from " + options.Input);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var graph = GraphIO.LoadCSV(options.Input);
            var graphLoadTime = stopwatch.ElapsedMilliseconds;
            stopwatch.Stop();

            int n = graph.Vertices.Count;
            int m = graph.Edges.Count;
            var cap = new double[m];
            var gain = new double[m];

            // とりあえず cap と gain は一つの値で埋める
            for (int e = 0; e < m; ++e) {
                cap[e] = 1.0;
                gain[e] = 0.5;
            }

            var random = new Random();
            for (int i = 0; i < options.TestCase; ++i) {
                int s, t;
                do {
                    s = random.Next(n);
                    t = random.Next(n);
                } while (s == t);
                var output = string.Format("{0}/{1}", options.Output, i);
                GMF.Run(options.Input, output, graphLoadTime, graph, cap, gain, s, t, options.Eps);
            }
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
