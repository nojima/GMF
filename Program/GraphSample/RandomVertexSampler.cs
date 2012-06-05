using System;
using System.Collections.Generic;
using System.Diagnostics;
using Graph;

namespace GraphSample {
    class RandomVertexSampler {
        /// <summary>
        /// graphからk個の頂点をランダムに選択し，選ばれた頂点と，それらが誘導する辺のみを持つグラフを返す
        /// </summary>
        public DirectedGraph Sample(DirectedGraph graph, int sampledVertexCount, Random random) {
            var choice = new HashSet<int>();
            for (int i = 0; i < sampledVertexCount; ++i) {
                for (; ; ) {
                    int n = random.Next(graph.Vertices.Count);
                    if (!choice.Contains(n)) {
                        Trace.WriteLine("Choose: " + graph.Vertices[n].OriginalId.ToString());
                        choice.Add(n);
                        break;
                    }
                }
            }

            return graph.Induce(choice);
        }
    }
}
