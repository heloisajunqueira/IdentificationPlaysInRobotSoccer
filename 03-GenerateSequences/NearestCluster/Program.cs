using System.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Linq;
using Common;
using Path = Common.Path;
using ClusteringSpace;
using System.Diagnostics;



namespace NearestCluster
{
    class Program
    {
        public static List<Path> paths = new List<Path> ();

        public static List<Path> clusters = new List<Path> ();

        public static List<Path> seqState = new List<Path> ();
        
        private static int distMetric = 0;
        static int Main(string[] args)
        {
            List<float> points = new List<float> ();
            string[] str_points;
                
            //enter the paths  
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
                    // points[0] = float.Parse(str_points[0], CultureInfo.InvariantCulture);
                    for(int l=0; l<str_points.Count(); l+=2)
                    {
                        points.Add(float.Parse(str_points[l], CultureInfo.InvariantCulture));
                        points.Add(float.Parse(str_points[l+1], CultureInfo.InvariantCulture));

                        nodes.Add(new Node(points[l]*1000,points[l+1]*1000,0.0));
                    }
                    points.Clear();
                    paths.Add (new Path (nodes));
                }       
            }
            else Console.WriteLine("FALSE");

            int j=0;
            foreach(Path path in paths){
                path.name = j.ToString();
                j++;
                //Console.WriteLine("PATH: {0}", path.ToString());
            }    
            Console.WriteLine("TOTAL PATHS: {0}", paths.Count());
            
            int totalLines = paths.Count();
            
            
            List<float> pointsClusters = new List<float> ();
            string[] str_pointsClusters;

            //enter the center cluster  
            string directoryData2 = Directory.GetCurrentDirectory();
            string filenameData2 = "/data/resultCluster_8_110.txt"; //fill with the data file
            string textFile2 = directoryData2 + filenameData2;
            
            if (File.Exists(textFile2))
            { 
                // Read a text file line by line.  
                string[] linesClusters = File.ReadAllLines(textFile2);
                foreach(string line2 in linesClusters)
                {
                    List<Node> nodesClusters = new List<Node> ();
                    
                    str_pointsClusters = line2.Split(',').ToArray();
                    // points[0] = float.Parse(str_points[0], CultureInfo.InvariantCulture);
                    for(int i=0; i<str_pointsClusters.Count(); i+=2)
                    {
                        pointsClusters.Add(float.Parse(str_pointsClusters[i], CultureInfo.InvariantCulture));
                        pointsClusters.Add(float.Parse(str_pointsClusters[i+1], CultureInfo.InvariantCulture));

                        nodesClusters.Add(new Node(pointsClusters[i],pointsClusters[i+1],0.0));

                    }
                    pointsClusters.Clear();
                    clusters.Add (new Path (nodesClusters));
                }     
            }


            int k=0;
            foreach(Path cluster in clusters){
                cluster.name = k.ToString();
                k++;
            }    
            Console.WriteLine("TOTAL CLUSTERS: {0}", clusters.Count());

            int totalCluster = clusters.Count();

            DBSCAN.setDistMetric(distMetric);

            List<Node> nodespathDist = new List<Node> ();
            double minimumDistance = 0.0;
            int nearestClusterIndex = 0;

            List<Path> pathDist = new List<Path> ();
            
            foreach(Path path in paths){
                
                List<Node> nearestCluster = new List<Node> (); 

                for(int i=1; i<path.points.Count(); i++)
                {
                    //Seleciona o seg de reta entre 2 pontos
                    nodespathDist.Add(new Node (path.points[i-1].Axis(0),path.points[i-1].Axis(2),0.0));
                    pathDist.Add(new Path (nodespathDist));
                    nodespathDist.Add(new Node (path.points[i].Axis(0),path.points[i].Axis(2),0.0));
                    pathDist.Add(new Path (nodespathDist));
                    //calcula a distancia entre o seg para cada cluster e depois acha o cluster mais proximo
                    for (int z = 0; z < clusters.Count(); z++) //find nearest cluster
                    {
                        double distance = DBSCAN.FindDistance(clusters[z], pathDist[0]);
                        if (z == 0)
                        {
                            minimumDistance = distance;
                            nearestClusterIndex = 0;
                        }
                        else if (minimumDistance > distance)
                        {
                            minimumDistance = distance;
                            nearestClusterIndex = z;
                        }
                    }
                    nodespathDist.Clear();
                    pathDist.Clear();
                    nearestCluster.Add(new Node(nearestClusterIndex, 0.0, 0.0));
                }
                seqState.Add(new Path(nearestCluster));
            }


            string directoryData3 = Directory.GetCurrentDirectory();
            string filenameData3 = "/data/RawStates"; //fill with the data file
            string textFile3 = directoryData3 + filenameData3;
            //imprime a sequencia de clusters
            using(StreamWriter writetext = new StreamWriter(textFile3))
            {
                foreach(Path states in seqState){
                    writetext.WriteLine("{0}", states.ToString(), CultureInfo.InvariantCulture);
                }
            }


            int totalPath = seqState.Count();

            if (totalPath%12 != 0){
                Console.WriteLine("ERRO: este dataset não contém a quantiidade de linhas necessárias para a conversão em máquina de estados necessária");
                return 0;
            }
            
            //Contar a quantidade de linhas
            /*int lineArr=0; //numero total de linhas
            int auxline=0; //numero de linhas
            float p=0;
            int t=0;  // auxiliar na contagem 
            
            int auxCount=0;
            int auxMaxLine=0;
            foreach(Path state in seqState)
            {
                p=(t%11);
                if(p==0 && t!=0){
                    auxline=state.points.Count();
                    if(auxMaxLine<auxline){
                        auxMaxLine = auxline;
                    }
                    t=0;
                    lineArr= auxline+lineArr;
                }
                else
                {
                    t++;
                }
                auxCount++;
            } */
            
            bool canPrint = false;
            int auxCount = 0, t=0;
            int[,] AuxSeq = new int[12,151];
			foreach(Path seqstate in seqState){
                
				for(int i=0; i<seqstate.points.Count(); i++)
                {
					AuxSeq[t,i]=seqstate.points[i].Axis(0);
				}
                t++;
                auxCount++;
                if(t == 12){
                    int auxCol = seqstate.points.Count();
                    if(auxCount == totalPath){
                      canPrint = true;
                    }
                    DBSCAN.FindState(AuxSeq,auxCol,canPrint, totalCluster, totalPath, totalLines); //FindState(stateArray, posição inicial do array, posição final do array) algoritmo para calcular as sequencias 
                    AuxSeq = new int[12,151];
                   
                    t=0;
                }
            }
           
            return 0;
        }
    }
}
