using System;
using System.Collections.Generic;
using System.IO;

namespace Graph {
    /// <summary>
    /// 便利関数たち
    /// Graphとは関係ないけど，とりあえずここに置いておく
    /// </summary>
    public static class Utility {
        public static void CreateDirectoryIfNotExists(string path) {
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }
        }
    }
}
