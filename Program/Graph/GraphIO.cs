using System;
using System.Collections.Generic;
using System.IO;

namespace Graph {
    public static class GraphIO {
        // グラフをCSVから読み込む
        public static DirectedGraph LoadCSV(string fileName) {
            using (var reader = new StreamReader(fileName)) {
                string line;
                var vertexIds = new Dictionary<int, int>();
                var srcs = new List<int>();
                var dsts = new List<int>();

                for (int lineNo = 1; (line = reader.ReadLine()) != null; ++lineNo) {
                    if (line.Length == 0) { continue; }
                    string[] fields = line.Split(',');
                    if (fields.Length < 2) { throw new Exception(string.Format("{0}:{1} Fields Too Few", fileName, lineNo)); }
                    int src = int.Parse(fields[0]);
                    int dst = int.Parse(fields[1]);
                    if (!vertexIds.ContainsKey(src)) {
                        int id = vertexIds.Count;
                        vertexIds.Add(src, id);
                    }
                    if (!vertexIds.ContainsKey(dst)) {
                        int id = vertexIds.Count;
                        vertexIds.Add(dst, id);
                    }
                    srcs.Add(src);
                    dsts.Add(dst);
                }

                var g = new DirectedGraph(vertexIds.Count);
                for (int i = 0; i < srcs.Count; ++i) {
                    g.AddEdge(new Edge(vertexIds[srcs[i]], vertexIds[dsts[i]]));
                }
                return g;
            }
        }

        // グラフをCSVに書きこむ
        public static void SaveCSV(DirectedGraph g, string fileName) {
            using (var writer = new StreamWriter(fileName)) {
                for (int i = 0; i < g.VertexCount; ++i) {
                    foreach (Edge e in g.OutEdges[i]) {
                        writer.WriteLine("{0},{1}", e.Src, e.Dst);
                    }
                }
            }
        }
    }
}
