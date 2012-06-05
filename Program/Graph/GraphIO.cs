using System;
using System.Collections.Generic;
using System.IO;

namespace Graph {
    public static class GraphIO {
        // グラフをCSVから読み込む
        public static DirectedGraph LoadCSV(string fileName) {
            using (var reader = new StreamReader(fileName)) {
                string line;
                var id2index = new Dictionary<int, int>();
                var g = new DirectedGraph();

                for (int lineNo = 1; (line = reader.ReadLine()) != null; ++lineNo) {
                    if (line.Length == 0) { continue; }
                    string[] fields = line.Split(',');
                    if (fields.Length < 2) { throw new Exception(string.Format("{0}:{1} Fields Too Few", fileName, lineNo)); }
                    int src = int.Parse(fields[0]);
                    int dst = int.Parse(fields[1]);
                    if (!id2index.ContainsKey(src)) {
                        id2index.Add(src, g.Vertices.Count);
                        g.AddVertex(new Vertex(src));
                    }
                    if (!id2index.ContainsKey(dst)) {
                        id2index.Add(dst, g.Vertices.Count);
                        g.AddVertex(new Vertex(dst));
                    }
                    g.AddEdge(new Edge(id2index[src], id2index[dst]));
                }

                return g;
            }
        }

        // グラフをCSVに書きこむ
        public static void SaveCSV(DirectedGraph g, string fileName) {
            using (var writer = new StreamWriter(fileName)) {
                foreach (Edge e in g.Edges) {
                    writer.WriteLine("{0},{1}", g.Vertices[e.Src].OriginalId, g.Vertices[e.Dst].OriginalId);
                }
            }
        }
    }
}
