using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graph;
using GeneralizedMaximumFlow;

namespace Test {
    class Program {
        static void Main(string[] args) {
            DirectedGraph graph = new DirectedGraph();

            for (int i = 0; i < 6; ++i)
                graph.AddVertex(new Vertex());

            double[] cap = new double[8];
            int m = 0;

            graph.AddEdge(new Edge(0, 1));
            cap[m++] = 1;

            graph.AddEdge(new Edge(0, 2));
            cap[m++] = 1;

            graph.AddEdge(new Edge(1, 2));
            cap[m++] = 1;

            graph.AddEdge(new Edge(1, 3));
            cap[m++] = 1;

            graph.AddEdge(new Edge(2, 4));
            cap[m++] = 0.8;

            graph.AddEdge(new Edge(3, 5));
            cap[m++] = 1;

            graph.AddEdge(new Edge(4, 3));
            cap[m++] = 1;

            graph.AddEdge(new Edge(4, 5));
            cap[m++] = 0.5;

            double[] flow;
            double value = Dinic.MaximumFlow(graph, cap, 0, 5, out flow);

            Console.WriteLine("|f| = {0}", value);
            foreach (var e in graph.Edges) {
                Console.WriteLine("f({0}, {1}) = {2}", e.Src, e.Dst, flow[e.Index]);
            }
        }
    }
}
