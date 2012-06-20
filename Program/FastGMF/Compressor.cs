using System;
using System.Collections.Generic;
using Graph;

namespace FastGMF {
    class Compressor {
        public Compressor(DirectedGraph graph, double prob, Random random) {
            int[] mapping;
            CompressedGraph = Compress(graph, prob, random, out mapping);
            Mapping = mapping;
        }

        public DirectedGraph CompressedGraph { get; private set; }
        public int[] Mapping { get; private set; }

        private DirectedGraph Compress(DirectedGraph graph, double prob, Random random, out int[] mapping) {
            bool[] selected = RandomSample(graph, prob, random);
            Graph.Algorithms.WeaklyConnectedComponents(graph, out mapping, selected);
            return Graph.Algorithms.Contract(graph, mapping);
        }

        private bool[] RandomSample(DirectedGraph graph, double prob, Random random) {
            int m = graph.Edges.Count;
            var result = new bool[m];
            for (int i = 0; i < m; ++i) {
                if (random.NextDouble() < prob) {
                    result[i] = true;
                }
            }
            return result;
        }
    }
}
