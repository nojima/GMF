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
        /// <summary>
        /// graphからk個の頂点をランダムに選択し，選ばれた頂点と，それらが誘導する辺のみを持つグラフを返す
        /// </summary>
        static DirectedGraph Sample(DirectedGraph graph, int k, Random random) {
            var choice = new Dictionary<int, int>(k);
            for (int i = 0; i < k; ++i) {
                for (; ; ) {
                    int n = random.Next(graph.VertexCount);
                    if (!choice.ContainsKey(n)) {
                        Trace.WriteLine("Choose: " + n.ToString());
                        choice.Add(n, i);
                        break;
                    }
                }
            }

            var result = new DirectedGraph(k);
            foreach (var kv in choice) {
                int v = kv.Key, i = kv.Value;
                foreach (Edge e in graph.OutEdges[v]) {
                    if (choice.ContainsKey(e.Dst)) {
                        result.AddEdge(new Edge(i, choice[e.Dst]));
                    }
                }
            }

            return result;
        }

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

            Trace.WriteLine("Input: " + options.Input);
            Trace.WriteLine("Output: " + options.Output);
            Trace.WriteLine("Output Vertex Count: " + options.VertexCount.ToString());
            Trace.WriteLine("RNG Seed: " + options.Seed.ToString());

            var g = GraphIO.LoadCSV(options.Input);

            if (g.VertexCount < options.VertexCount) {
                Console.Error.WriteLine("Error: 指定された頂点数が入力グラフの頂点数よりも大きいです．");
                Environment.Exit(1);
            }

            var random = (options.Seed == 0) ? new Random() : new Random(options.Seed);
            var result = Sample(g, options.VertexCount, random);
            GraphIO.SaveCSV(result, options.Output);

            Trace.WriteLine("Done");

            return 0;
        }
    }
}
