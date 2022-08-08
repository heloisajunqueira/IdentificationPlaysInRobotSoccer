using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Linq;
using Common;
using Path = Common.Path;
using ClusteringSpace;
using System.Diagnostics;

namespace GenCluster
{
    class Program
    {
        public static List<Path> paths = new List<Path> ();
        
        private static int distMetric = 0, minPathsForCluster = 0;

        public static Stopwatch sw = new Stopwatch();
        public static bool[] dimensionEnabled = new bool[] { true, true, false, false, false, false, false };
        private static float dbsScanEps = 0.0f;


        static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
            List<float> points = new List<float> ();
            string[] str_points;
            string directoryData = Directory.GetCurrentDirectory();
            string filenameData = "/data/HELIOS.txt"; //fill with the data file
            string textFile = directoryData + filenameData;
            if (File.Exists(textFile))
            { 
                // Read a text file line by line.  
                string[] lines = File.ReadAllLines(textFile);
                foreach(string line in lines)
                {
                    List<Node> nodes = new List<Node> ();
                    
                    str_points = line.Split(',').ToArray();
                    for(int i=0; i<str_points.Count(); i+=2)
                    {
                        points.Add(float.Parse(str_points[i], CultureInfo.InvariantCulture));
                        points.Add(float.Parse(str_points[i+1], CultureInfo.InvariantCulture));
                        
                        nodes.Add(new Node(points[i]*1000,points[i+1]*1000,0.0));
                    }
                    points.Clear();
                    paths.Add (new Path (nodes));
                }
                    
            }
            else Console.WriteLine("FALSE");

            int j=paths.Count()-1;
            Console.WriteLine("j: {0}", j);
            foreach(Path path in paths)
            {
                path.name = j.ToString();
                j--;
            }
            Console.WriteLine("j: {0}", j);
            Console.WriteLine("TOTAL PATHS: {0}", paths.Count());
            

            
            DBSCAN.GetDistances(paths, distMetric);
            Console.WriteLine("PRINT DISTANCES:");
            //DBSCAN.PrintDistances(paths);

            List<Path> pathsToCluster = new List<Path> ();
            minPathsForCluster = 8;
            dbsScanEps = 10.5f;
            DBSCAN.DoDBSCAN(paths, dbsScanEps, minPathsForCluster);

            minPathsForCluster = 8;
            dbsScanEps = 11.0f;
            DBSCAN.DoDBSCAN(paths, dbsScanEps, minPathsForCluster);


            minPathsForCluster = 8;
            dbsScanEps = 11.5f;
            DBSCAN.DoDBSCAN(paths, dbsScanEps, minPathsForCluster);
        
        }
    }
}
