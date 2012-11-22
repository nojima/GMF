using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using Graph;
using CommandLine;
using CommandLine.Text;
using GeneralizedMaximumFlow;
using FastGMF;

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
            var random = new Random(12345);

            int capAndGainType = 2;
            if (capAndGainType == 0) {
                // とりあえず cap と gain はランダムに決める
                for (int e = 0; e < m; ++e) {
                    cap[e] = random.NextDouble() * 2;
                    gain[e] = random.NextDouble();
                }
            } else if (capAndGainType == 1) {
                // cap と gain を固定
                for (int e = 0; e < m; ++e) {
                    cap[e] = 1.0;
                    gain[e] = 0.5;
                }
            } else if (capAndGainType == 2) {
                // 数種類の gain をつくる
                double[] gains = new double[] { 0.32768, 0.512, 0.8 };
                for (int e = 0; e < m; ++e) {
                    cap[e] = 1.0;
                    gain[e] = gains[random.Next(gains.Length)];
                }
            } else {
                Debug.Assert(false);
            }

            /*
            // TODO: オプションで動作を変えれるように (コメントアウトではなく)
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
            */

            // TODO: このリストを消す
            var sourceSinkList = new int[][] {
                new [] {0,1984791,2372945},
                new [] {1,264514,1455230},
                new [] {2,1565106,537207},
                new [] {3,2057261,724254},
                new [] {4,169984,33753},
                new [] {5,1784,51266},
                new [] {6,401788,8507},
                new [] {7,66374,912920},
                new [] {8,401521,55723},
                new [] {9,59089,98501},
                new [] {10,60273,25023},
                new [] {11,2045019,454549},
                new [] {12,630536,708776},
                new [] {13,211916,815026},
                new [] {14,1465142,760241},
                new [] {15,368092,103455},
                new [] {16,526631,1733815},
                new [] {17,267114,25345},
                new [] {18,75047,41969},
                new [] {19,2311072,558297},
                new [] {20,1969830,2491104},
                new [] {21,1467338,1784952},
                new [] {22,695016,1394381},
                new [] {23,1209419,849893},
                new [] {24,814203,1650550},
                new [] {25,172935,229647},
                new [] {26,34172,1191961},
                new [] {27,1110728,28546},
                new [] {28,645989,542652},
                new [] {29,306856,84886},
                new [] {30,319649,6592},
                new [] {31,2087594,2498803},
                new [] {32,1118471,346577},
                new [] {33,7181,1654205},
                new [] {34,24573,348841},
                new [] {35,88659,190061},
                new [] {36,1332695,1698868},
                new [] {37,1929820,1741628},
                new [] {38,240024,1642087},
                new [] {39,589145,51259},
                new [] {40,994136,30044},
                new [] {41,361633,1994189},
                new [] {42,227124,99439},
                new [] {43,848550,1598414},
                new [] {44,1488051,409696},
                new [] {45,441214,380092},
                new [] {46,439509,641713},
                new [] {47,19886,264814},
                new [] {48,1586703,778143},
                new [] {49,25304,872317},
            };

            for (int i = 0; i < sourceSinkList.Length; ++i) {
                var output = string.Format("{0}/{1}", options.Output, i);
                int s, t;
                Utility.FindSourceAndSink(sourceSinkList[i][1], sourceSinkList[i][2], graph, out s, out t);
                /*
                try {
                    CompressedGMF.Run(graph, graphLoadTime, s, t, cap, gain, 0.5, output, options.Eps);
                } catch (ArgumentException) {
                    // 圧縮後にsとtが同じ頂点に縮約された場合

                    // ディレクトリだけ作って何もしない
                    Utility.CreateDirectoryIfNotExists(output);
                }
                */
                GMF.Run(options.Input, output, graphLoadTime, graph, cap, gain, s, t, options.Eps, "FleischerWayne");
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
