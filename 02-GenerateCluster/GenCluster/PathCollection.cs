//PathCollection.cs
//modified from source at http://codeding.com/articles/k-means-algorithm

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Path = Common.Path;
using Node = Common.Node;
////using EditorArea;

////using UnityEngine;
namespace ClusteringSpace
{
    public class PathCollection : List<Path>
    {
        #region Properties

        public Path Centroid { get; set; }
		
		public bool changed = false;

        #endregion

        #region Constructors

// Implementation for the GetEnumerator method.
        public PathCollection()
            : base()
        {
            Centroid = new Path();
        }
		
        public PathCollection(List<Path> paths)
            : base()
        {
            Centroid = new Path();

			int clustAlg=1;
			
			foreach (Path p in paths)
			{
				this.Add(p);
			}
            UpdateCentroid(clustAlg);
        }
		
		public PathCollection(Path centroid_) : base()
		{
			Centroid = centroid_;
		}

        #endregion

        #region Methods

        public void AddPath(Path p)
        {
            this.Add(p);
			changed = true;
        }

        public Path RemovePath(Path p)
        {
            Path removedPath = new Path(p);
            this.Remove(p);
			changed = true;

            return (removedPath);
        }

        #endregion

        #region Internal-Methods

		public Path getAveragedCentroid()
		{
			// first, make a copy of all the paths in the cluster...
			List<Path> interpolatedPaths = new List<Path>();
			foreach (Path p in this)
			{
				interpolatedPaths.Add(new Path(p)); // make a copy
			}

			double maxTime = Double.NegativeInfinity;
			foreach (Path p in this)
			{ // find the highest time value over all paths in this cluster
				foreach (Node n in p.points)
				{
					if (n.tD > maxTime)
					{
						maxTime = n.tD;
					}
				}
			}
			
			/*for (int i = 0; i < interpolatedPaths.Count; i ++)
			{ // make each path have same # of points
				Vector3[] set1 = MapperWindowEditor.GetSetPointsWithN(interpolatedPaths[i].getPoints3D(), (int)(Math.Sqrt(maxTime)), false);
	//			Debug.Log("Paths now have " + Math.Sqrt(maxTime) + " points.");
				interpolatedPaths[i].points = new List<Node>();
				foreach(Vector3 v in set1)
				{
					if (v.x == 0 && v.y == 0 && v.z == 0) continue;
					interpolatedPaths[i].points.Add(new Node(v.x, v.z, v.y));
				}
			}*/
			
//			return interpolatedPaths[0];
			
			Node[] averagedNodes = new Node[interpolatedPaths[0].points.Count()];
			for (int count = 0; count < interpolatedPaths[0].points.Count(); count ++)
			{
				averagedNodes[count] = new Node(0, 0, 0);
				//if (count > 0) averagedNodes[count].parent = averagedNodes[count-1];
			}
            float avgDanger = 0f, avgLOS = 0f, avgNM = 0f;
			foreach (Path p in interpolatedPaths)
			{
                //avgDanger += p.danger3;
                //avgLOS += p.los3;
                //avgNM += p.crazy;
				for (int count = 0; count < p.points.Count; count ++)
				{
					averagedNodes[count].x += Math.Abs(p.points[count].x);
					averagedNodes[count].y += Math.Abs(p.points[count].y);
					averagedNodes[count].t += Math.Abs(p.points[count].t);
					averagedNodes[count].xD += Math.Abs(p.points[count].xD);
					averagedNodes[count].yD += Math.Abs(p.points[count].yD);
					averagedNodes[count].tD += Math.Abs(p.points[count].tD);
				}
			}
//			for (int count = 0; count < interpolatedPaths[0].points.Count(); count ++)
//			{
//				Debug.Log("cnx:"+averagedNodes[count].x+",y:"+averagedNodes[count].y+",t:"+averagedNodes[count].t);
//			}

            avgDanger /= interpolatedPaths.Count;
            avgLOS /= interpolatedPaths.Count;
            avgNM /= interpolatedPaths.Count;
			foreach(Node n in averagedNodes)
			{
				n.x /= interpolatedPaths.Count;
				n.y /= interpolatedPaths.Count;
				n.t /= interpolatedPaths.Count;
				n.xD /= interpolatedPaths.Count;
				n.yD /= interpolatedPaths.Count;
				n.tD /= interpolatedPaths.Count;
				//n.danger3 = avgDanger;
			}
		//	Debug.Log("end centr");
        
            Path averagedPath = new Path(new List<Node>(averagedNodes));
            //averagedPath.danger3 = avgDanger;
            //averagedPath.los3 = avgLOS;
            //averagedPath.crazy = avgNM;
		
			return averagedPath;
		}

        public void UpdateCentroid(int ClustAlg)
        {	
			int clustAlg = ClustAlg;
/*			if (MapperWindowEditor.altCentroidComp)
			{
				Node[] nodes = new Node[this[0].points.Count()];
				for (int count = 0; count < this[0].points.Count(); count ++)
				{
					nodes[count] = new Node(0, 0, 0);
				}
				foreach (Path p in this)
				{
					for (int count = 0; count < p.points.Count; count ++)
					{
						nodes[count].x += p.points[count].x;
						nodes[count].y += p.points[count].y;
						nodes[count].t += p.points[count].t;
					}
				}
				foreach(Node n in nodes)
				{
					n.x /= this[0].points.Count;
					n.y /= this[0].points.Count;
					n.t /= this[0].points.Count;
			//		Debug.Log("cnx:"+n.x+",y:"+n.y+",t:"+n.t);
				}
			//	Debug.Log("end centr");
			
				Centroid = new Path(new List<Node>(nodes));
			}*/
	//		else
	//		{
				
				double pathTotalMinDist = double.PositiveInfinity;
				int pIndex = -1;
				for (int i = 0; i < this.Count; i ++)
				{
					//Console.WriteLine("i:{0}", i);	
					if (clustAlg == 0 && KMeans.weights.Count() < Convert.ToInt32(this[i].name))
					{
						Console.WriteLine("KMeans.weights size: " + KMeans.weights.Count() + " but need index " + Convert.ToInt32(this[i].name));
					}
					//double weightOfI = (clustAlg == 0 ? KMeans.weights[Convert.ToInt32(this[i].name)] : 1.0);
					double currentPathTotalMinDist = 0;
					for (int j = 0; j < this.Count; j ++)
					{
						//Console.WriteLine("j:{0}", j);
						if (i == j) continue;

						currentPathTotalMinDist += (DBSCAN.FindDistance(this[i], this[j]));
						//currentPathTotalMinDist += (KMeans.FindDistance(this[i], this[j]));
						//Console.WriteLine("currentPathTotalMinDist:{0}", currentPathTotalMinDist);
					}
					if (currentPathTotalMinDist < pathTotalMinDist)
					{
						pathTotalMinDist = currentPathTotalMinDist;
						pIndex = i;
						//Console.WriteLine("pathTotalMinDist:{0}", pathTotalMinDist);
					}
				}
				//Console.WriteLine("pathTotalMinDistTOT:{0}", pathTotalMinDist);
				if (pIndex == -1)
				{
					Console.WriteLine("-1");
					Centroid = null;
					return;
				}

			Centroid = new Path(this[pIndex]);
		}
        
		
		public double getCenterDistPath(Path cluster)
		{
			double pathTotalMinDist = double.PositiveInfinity;
			int pIndex = -1;
			double currentPathTotalMinDist = 0.0;
			for (int i = 0; i < this.Count; i ++)
			{
				//Console.WriteLine("i:{0}", i);
				currentPathTotalMinDist += DBSCAN.FindDistance(this[i], cluster);
				//Console.WriteLine("distance: {0}", DBSCAN.FindDistance(this[i], cluster));
				if (currentPathTotalMinDist < pathTotalMinDist)
				{
					pathTotalMinDist = currentPathTotalMinDist;
					pIndex = i;
				}
			}
			//Console.WriteLine("distPathTotal:{0}", distPathTotal);
			if (pIndex == -1)
			{
				Console.WriteLine("-1");
				return 0.0;
			}

			return currentPathTotalMinDist;
		}
	}
        #endregion
}
