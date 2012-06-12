using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using CommandLine;
using CommandLine.Text;
using Graph;


namespace GraphMetrics {
    class Options {
        [Option(null, "input", Required = true, HelpText = "入力元のファイル名")]
        public string Input { get; set; }

        [Option(null, "output", Required = true, HelpText = "出力先のディレクトリ名")]
        public string Output { get; set; }

        [Option(null, "quiet", HelpText = "トレースメッセージを出力しない")]
        public bool Quiet { get; set; }

        [HelpOption]
        public string GetUsage() {
            var help = new HelpText {
                AddDashesToOption = true,
                Heading = "Usage: GraphMetrics [OPTIONS]",
            };
            help.AddPostOptionsLine("詳しくは ReadMe.md を参照．\n");
            help.AddOptions(this);
            return help;
        }
    }

    class Program {
        static void DegreeDistribution(DirectedGraph graph, out List<int> inDegree, out List<int> outDegree) {
            inDegree = new List<int>();
            outDegree = new List<int>();
            for (int i = 0; i < graph.Vertices.Count; ++i) {
                while (inDegree.Count <= graph.InDegree(i)) { inDegree.Add(0); }
                while (outDegree.Count <= graph.OutDegree(i)) { outDegree.Add(0); }
                inDegree[graph.InDegree(i)] += 1;
                outDegree[graph.OutDegree(i)] += 1;
                if (graph.OutDegree(i) == 15324) {
                    Trace.WriteLine("originalId = " + graph.Vertices[i].OriginalId);
                }
            }
        }

        static void OutputDegree(StreamWriter writer, List<int> degreeDistribution) {
            for (int i = 0; i < degreeDistribution.Count; ++i) {
                writer.WriteLine("{0},{1}", i, degreeDistribution[i]);
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

            Trace.WriteLine("Caluculating the degree distribution");
            List<int> inDegree, outDegree;
            DegreeDistribution(graph, out inDegree, out outDegree);

            Trace.WriteLine("Writing the result");
            using (var writer1 = new StreamWriter(options.Output + "/InDegree.csv")) {
                OutputDegree(writer1, inDegree);
            }
            using (var writer2 = new StreamWriter(options.Output + "/OutDegree.csv")) {
                OutputDegree(writer2, outDegree);
            }

            Trace.WriteLine("Done");
        }
    }
}
