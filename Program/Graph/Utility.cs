using System;
using System.Collections.Generic;
using System.IO;

namespace Graph {
    /// <summary>
    /// 便利関数たち
    /// Graphとは関係ないのもあるけど，とりあえずここに置いておく
    /// </summary>
    public static class Utility {
        public static void CreateDirectoryIfNotExists(string path) {
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }
        }

        public static void FindSourceAndSink(int source, int sink, DirectedGraph graph, out int s, out int t) {
            s = -1;
            t = -1;

            for (int i = 0; i < graph.Vertices.Count; ++i) {
                if (graph.Vertices[i].OriginalId == source) { s = i; }
                if (graph.Vertices[i].OriginalId == sink) { t = i; }
            }
            if (s == -1 || t == -1 || s == t) {
                Console.Error.WriteLine("Error: 始点または終点が不正です．");
                Environment.Exit(1);
            }
        }
    }
}
