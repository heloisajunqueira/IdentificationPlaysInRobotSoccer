using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Path = Common.Path;
using System.IO;

namespace ClusteringSpace
{
    public class DBSCAN
    {
		public static int region=0;
		
		public static int numSelectedDimensions = -1;


		static FrechetDistance frechet;

        public static bool[] dimensionEnabled = new bool[] { true, true, false, false, false, false, false };
        //private static String[] dimensions = new String[] { "X", "Y", "Time", "Danger", "LOS", "Near Miss", "Health" };
        
		private static int distMetric = 0;
		
		public static int numPaths = -1;

		
		public static void reset()
		{
			distMetric = -1;
			numPaths = -1;
			numSelectedDimensions = -1;
			Clustering.reset();
		}


		public static void GetDistances(List<Path> paths, int distMetric_){

			setDistMetric(distMetric_);
			
			Clustering.initWithPaths(paths, (numSelectedDimensions > 1) ? true : false);

			for(int i=0; i<paths.Count(); i++){
				for(int j=i; j<paths.Count(); j++){
					double dist = FindDistance(paths[i], paths[j]);
				}
			}
		}
		public static void PrintDistances(List<Path> paths)
		{
			for(int i=0; i<paths.Count(); i++){
				for(int j=i; j<paths.Count(); j++){
					if (Clustering.distances[i][j] != -1) {
						Console.WriteLine("Distance [{0}][{1}]:{2}",i,j,Clustering.distances[i][j]);
					}
				
					else if (Clustering.distances[j][i] != -1) {
						Console.WriteLine("Distance [{0}][{1}]:{2}",j,i,Clustering.distances[j][i]);
					}
					else{
						Console.WriteLine("NÃO TEM DISTÂNCIA");
					}
				}
			}
		}
        public static void DoDBSCAN(List<Path> paths, float eps, int minPathsForCluster)
		{
			
			int k=0, w=0, total=0, count=0, epsInt=0; 
			double totalDist=0.0, totalCluster=0.0, totalCluster2=0.0;
			int clustAlg=0;
			if (paths.Count == 0)
			{ 
				Console.WriteLine("No paths to cluster!");
			}
			
			else
			{	
				List<PathCollection> clusters;
				int noise,classified;
				
				//Clustering the paths using the parameters defined
				cluster(paths, System.Convert.ToDouble(eps), minPathsForCluster,out clusters,out noise,out classified); //out clusters,out noise,out classified
				

				List<Path> clusterCentroids = new List<Path>();	
				List<double> clusterDist = new List<double>();
				clustAlg=1;
				foreach(PathCollection c in clusters)
				{
					c.UpdateCentroid(clustAlg);
					clusterCentroids.Add(c.Centroid);
					clusterDist.Add(c.getCenterDistPath(c.Centroid));
				}
				
				epsInt=System.Convert.ToInt16(eps*10);
				string directoryr = Directory.GetCurrentDirectory();
				string filenamer = "/data/";
				string filer = "statistics_"+minPathsForCluster.ToString()+"_"+epsInt.ToString()+".txt";
				string pathResultr = directoryr+filenamer+filer;
				using( StreamWriter writetextr = new StreamWriter(pathResultr))
				{

					writetextr.WriteLine("Total of paths: {0}", paths.Count());
					writetextr.WriteLine("Noise: {0}", noise.ToString());
					writetextr.WriteLine("Classified: {0}",classified.ToString());
					writetextr.WriteLine("Number of clusters: {0}", clusters.Count().ToString());
					writetextr.WriteLine();
					for (int i = 0; i < clusters.Count(); i++)
					{
						count = clusters[i].Count;
						total += count;
						writetextr.WriteLine("Cluster {0} consists of the following {1} point :\n", i + 1, count);
						foreach (Path path in clusters[i])
						{
							writetextr.WriteLine(" {0} ;", path, CultureInfo.InvariantCulture);
						}
						writetextr.WriteLine();
					}
					foreach (Path path in clusterCentroids)
					{
						writetextr.WriteLine(" CLUSTER CENTROID[{0}]: {1} ;",k, path.ToString());
						k++;
					}
					writetextr.WriteLine();
					foreach (double dist in clusterDist){
						writetextr.WriteLine("DIST CLUSTER[{0}]: {1} ;",w, dist.ToString());
						w++;
						totalDist = totalDist+dist;
					}
					writetextr.WriteLine();
					totalCluster = (totalDist/clusters.Count());
					writetextr.WriteLine("Total Dist Cluster: {0}/{1} = {2}",totalDist.ToString(),clusters.Count().ToString(),totalCluster.ToString());
					totalCluster2 = (totalDist/classified);
					writetextr.WriteLine("Total Dist Cluster: {0}/{1} = {2}",totalDist.ToString(),classified.ToString(),totalCluster2.ToString());
				}
				
				string directory = Directory.GetCurrentDirectory();
				string filename = "/data/";
				string file = "resultCluster_"+minPathsForCluster.ToString()+"_"+epsInt.ToString()+".txt";
				string pathResult = directory+filename+file;
				//Console.WriteLine("CURRENT DIRECTORY:{0}",directory+filename+file);
				using(StreamWriter writetext2 = new StreamWriter(pathResult))
				{
					foreach (Path path in clusterCentroids)
					{
						writetext2.WriteLine("{0}", path.ToString(), CultureInfo.InvariantCulture);
					}
				}
				
			}
        }
		
		public static void cluster(List<Path> points, double eps, int minPts,out List<PathCollection> clusters,out int noise,out int classified)
		{ // src : http://www.c-sharpcorner.com/uploadfile/b942f9/implementing-the-dbscan-algorithm-using-C-Sharp/
			//List<PathCollection> 
			clusters = new List<PathCollection> ();
			noise=-1;
			classified=-1;
			//Console.WriteLine("ENTROU CLUSTER");
			
			eps *= eps;
			int clusterID = 1;
			//Console.WriteLine("points.Count: {0}", points.Count);
			int k =0;
			for (int i = 0; i < points.Count; i++)
			{
				Path p = points[i];
				p.clusterID = Path.UNCLASSIFIED;
			}
			for (int i = 0; i < points.Count; i++)
			{
			   Path p = points[i];
			   if (p.clusterID == Path.UNCLASSIFIED)
			   {
				   k++;
			       if (ExpandCluster(points, p, clusterID, eps, minPts)) { 
				   		clusterID++;
				   }  
			   }
			}
			//Console.WriteLine("NÚMERO DE CLUSTERS: {0}", clusterID);
			//Console.WriteLine("K: {0}", k);
			// sort out points into their clusters, if any
			int maxclusterID = points.OrderBy(p => p.clusterID).Last().clusterID;
			for (int i = 0; i < maxclusterID; i++) clusters.Add(new PathCollection());
			
			foreach (Path p in points)
			{
			   if (p.clusterID > 0)
			   { 
                    clusters[p.clusterID - 1].Add(p);
               }
			}

			
			for (int i = 0; i < points.Count; i++)
			{
			   Path p = points[i];
			   if (p.clusterID == Path.NOISE)
			   {
				   //Console.WriteLine("Path.NOISE");
			       noise = noise + 1;
			   }
			   else
			   {
				   //Console.WriteLine("Path classified");
				   classified = classified + 1;
			   }
			}

			//Console.WriteLine("VOID2 TOTAL OF NOISE _ {0} _ {1} : {2}", minPts.ToString(), eps.ToString(), noise);
			//Console.WriteLine("VOID2 TOTAL OF CLASSIFIED:{0}", classified);
			
			//return(clusters,noise,classified);
		}

		static List<Path> GetRegion(List<Path> points, Path p, double eps)  
	    {
			//Console.WriteLine("ENTROU GetRegion");
	        List<Path> region = new List<Path>();
	        for (int i = 0; i < points.Count; i++)
	        {	
				double dist = FindDistance(p, points[i]);
	            if (dist <= eps) 
				{
					region.Add(points[i]);
				}
	        }
	        return region;
	    }
	    static bool ExpandCluster(List<Path> points, Path p, int clusterID, double eps, int minPts)  
	    {
			//Console.WriteLine("ENTROU ExpandCluster");
			List<Path> seeds = GetRegion(points, p, eps); //parei o print
	        
			if (seeds.Count < minPts) // no core point
	        {
				p.clusterID = Path.NOISE;
	            return false;
	        }
	        else // all points in seeds are density reachable from point 'p'
	        {
				
	            for (int i = 0; i < seeds.Count; i++) seeds[i].clusterID = clusterID;
	            seeds.Remove(p);
	            while (seeds.Count > 0)
	            {
	                Path currentP = seeds[0];
	                List<Path> result = GetRegion(points, currentP, eps);
	                if (result.Count >= minPts)
	                {
	                    for (int i = 0; i < result.Count; i++)
	                    {
	                        Path resultP = result[i];
	                        if (resultP.clusterID == Path.UNCLASSIFIED || resultP.clusterID == Path.NOISE)
	                        {
	                            if (resultP.clusterID == Path.UNCLASSIFIED) seeds.Add(resultP);
	                            resultP.clusterID = clusterID;
	                        }
	                    }
	                }
	                seeds.Remove(currentP);
	            }

				return true;
	        }
	    }
		
		private static void setDistMetric(int distMetric_) 
		{
			distMetric = distMetric_;
			
			if (distMetric == (int)Metrics.Frechet || distMetric == (int)Metrics.Hausdorff)
			{
				numSelectedDimensions = 0;
				for (int dim = 0; dim < dimensionEnabled.Count(); dim ++)
				{
					if (dimensionEnabled[dim])
					{
						numSelectedDimensions ++;
					}
				}
				
				if (distMetric == (int)Metrics.Frechet)
					frechet = new PolyhedralFrechetDistance(PolyhedralDistanceFunction.L1(numSelectedDimensions));
			}
		}

        public static double FindDistance(Path path1_, Path path2_) 
        {
			if (path1_.points == null) { Console.WriteLine(("P1NULL")); return -1; }  ////{ Debug.Log("P1NULL"); return -1; }
			else if (path2_.points == null) { Console.WriteLine(("P2NULL")); return -1; } ////{ Debug.Log("P2NULL"); return -1; }
			int p1num = Convert.ToInt32(path1_.name);
			int p2num = Convert.ToInt32(path2_.name);

			if (path1_.name == path2_.name) // same path
			{ 
				return 0.0;
			}

			Path path1, path2;
			
			bool saveDistances = false;

			if (p1num <= Clustering.distances.Count() && p2num <= Clustering.distances.Count())
			{
				if (Clustering.distances[p1num][p2num] != -1) {
					return Clustering.distances[p1num][p2num];
				}
				
				if (Clustering.distances[p2num][p1num] != -1) {
					return Clustering.distances[p2num][p1num];
				}
				
				
				path1 = Clustering.normalizedPaths[p1num];
				path2 = Clustering.normalizedPaths[p2num];
				
				saveDistances = true;
			}
			else
			{
				Console.WriteLine("Note - not storing this distance."); ////Debug.Log("Note - not storing this distance.");
				
				path1 = path1_;
				path2 = path2_;
			}

			double result = 0.0;
			
			if (distMetric == (int)Metrics.Frechet || distMetric == (int)Metrics.Hausdorff)
			{
				double[][] curveA = new double[path1.points.Count][];
				double[][] curveB = new double[path2.points.Count][];
				
				for (int i = 0; i < path1.points.Count; i ++)
				{
					double[] curve = new double[numSelectedDimensions];
					int curvePos = 0;
					for (int j = 0; j < dimensionEnabled.Count(); j ++)
					{
						if (dimensionEnabled[j])
						{
							if (j == (int)Dimensions.X) curve[curvePos] = path1.points[i].xD;
							 
							else if (j == (int)Dimensions.Y) curve[curvePos] = path1.points[i].yD; 
							else if (j == (int)Dimensions.Time) curve[curvePos] = path1.points[i].tD;
							curvePos ++;
						}
					}
					curveA[i] = curve;
				}
				
				for (int i = 0; i < path2.points.Count; i ++)
				{
					double[] curve = new double[numSelectedDimensions];
					int curvePos = 0;
					for (int j = 0; j < dimensionEnabled.Count(); j ++)
					{
						if (dimensionEnabled[j])
						{
							if (j == (int)Dimensions.X) curve[curvePos] = path2.points[i].xD;
							else if (j == (int)Dimensions.Y) curve[curvePos] = path2.points[i].yD;
							else if (j == (int)Dimensions.Time) curve[curvePos] = path2.points[i].tD;
							curvePos ++;
						}
					}
					curveB[i] = curve;
				}
				
				if (distMetric == (int)Metrics.Frechet)
					result = frechet.computeDistance(curveA, curveB);
			}
			else
			{
				Console.WriteLine("Invalid distance metric ("+distMetric+")!");  ////Debug.Log("Invalid distance metric ("+distMetric+")!");
				return -1;
			}
			
			if (saveDistances)
			{
				Clustering.distances[p1num][p2num] = result;
				Clustering.distances[p2num][p1num] = result;
			}
            
			return result;
        }
    }
}