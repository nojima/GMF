using System;
using System.Collections.Generic;
using System.Diagnostics;
using CommandLine;
using CommandLine.Text;
using Graph;

namespace GraphSample {
    class Options {
        [Option(null, "vertices", Required = true, HelpText = "出力するグラフの頂点数")]
        public int VertexCount { get; set; }

        [Option(null, "input", Required = true, HelpText = "入力元のファイル名")]
        public string Input { get; set; }

        [Option(null, "output", Required = true, HelpText = "出力先のファイル名")]
        public string Output { get; set; }

        [Option(null, "seed", Required = false, HelpText = "乱数のシード")]
        public int Seed { get; set; }

        [Option(null, "quiet", HelpText = "トレースメッセージを出力しない")]
        public bool Quiet { get; set; }

        [Option(null, "method", HelpText = "仕様するサンプリングの手法 (RandomVertex, ForestFire)")]
        public string Method { get; set; }

        [Option(null, "burning-probability", HelpText = "ForestFireにおけるBurning Probability")]
        public double BurningProbability { get; set; }

        [HelpOption]
        public string GetUsage() {
            var help = new HelpText {
                AddDashesToOption = true,
                Heading = "Usage: GraphSample [OPTIONS]",
            };
            help.AddPostOptionsLine("詳しくは ReadMe.md を参照．\n");
            help.AddOptions(this);
            return help;
        }
    }

    class Program {
        static int Main(string[] args) {
            var options = new Options();
            if (!CommandLineParser.Default.ParseArguments(args, options)) { Environment.Exit(1); }

            if (options.VertexCount < 0) {
                Console.Error.WriteLine("Error: 頂点数は非負整数である必要があります．");
                Environment.Exit(1);
            }

            if (!options.Quiet) {
                Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
                Trace.AutoFlush = true;
            }
            if (options.Method == "") {
                options.Method = "ForestFire";
            }
            if (options.BurningProbability == 0.0) {
                options.BurningProbability = 0.7;
            }

            Trace.WriteLine("Input: " + options.Input);
            Trace.WriteLine("Output: " + options.Output);
            Trace.WriteLine("Output Vertex Count: " + options.VertexCount.ToString());
            Trace.WriteLine("RNG Seed: " + options.Seed.ToString());
            Trace.WriteLine("Method: " + options.Method);
            if (options.Method == "ForestFire") {
                Trace.WriteLine("BurningProbability: " + options.BurningProbability);
            }

            var g = GraphIO.LoadCSV(options.Input);

            if (g.Vertices.Count < options.VertexCount) {
                Console.Error.WriteLine("Error: 指定された頂点数が入力グラフの頂点数よりも大きいです．");
                Environment.Exit(1);
            }

            var random = (options.Seed == 0) ? new Random() : new Random(options.Seed);
            DirectedGraph result = null;

            switch (options.Method) {
                case "RandomVertex":
                    result = new RandomVertexSampler().Sample(g, options.VertexCount, random);
                    break;
                case "ForestFire":
                    result = new ForestFireSampler().Sample(g, options.VertexCount, options.BurningProbability, random);
                    break;
                default:
                    Console.Error.WriteLine("Error: {0} という手法はサポートされていません．", options.Method);
                    Environment.Exit(1);
                    break;
            }

            GraphIO.SaveCSV(result, options.Output);

            Trace.WriteLine("Done");

            return 0;
        }
    }
}
