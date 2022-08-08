using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Collections;
////using UnityEngine;
////using Exploration;
////using Objects;
////using Extra;

namespace Common {
	[Serializable]
	public class Path : IEquatable<Path> {
		public String name;
		//public Color color;
		public List<Node> points;
		// Metrics
		
        public float time; //length2d, length3d, danger, los, danger3, los3, danger3Norm, los3Norm, crazy, velocity;
		
		public const int NOISE = -1;
		public const int UNCLASSIFIED = 0;
		public int clusterID;
		
		/// <summary>
		/// Serialization only.
		/// </summary>
		public Path () {
			points = new List<Node>();
		}
		
		public Path (List<Node> points) {
//			Debug.Log("Path without name");
			this.points = points;
			if (points == null)
				throw new ArgumentNullException ("Points can't be null");
		}
		
		public Path (Path p) {
			this.points = new List<Node>();
			foreach (Node n in p.points)
			{
				this.points.Add(new Node(n));
			}
			this.name = p.name;
			this.time = p.time;
			//this.length2d = p.length2d;
			//this.length3d = p.length3d;
			//this.danger = p.danger;
			//this.los = p.los;
			//this.danger3 = p.danger3;
			//this.los3 = p.los3;
			//this.danger3Norm = p.danger3Norm;
			///this.los3Norm = p.los3Norm;
			//this.crazy = p.crazy;
			//this.velocity = p.velocity;
			this.clusterID = p.clusterID;
			
			if (points == null)
				throw new ArgumentNullException ("Points can't be null");
		}

		
		
		public void ZeroValues () {
			time = 0f;//length2d = length3d = danger = los = danger3 = los3 = danger3Norm = los3Norm = crazy = velocity = 
		}

		public override string ToString () {
			string str="";
			foreach(Node node in points)
			{
				str += node.ToString();
			}
			if(str.Length==0){
				return("()");
			}
			return str.Remove(str.Length-1);
		}
		
		public bool Equals(Path other)
		{
			if (points.Count != other.points.Count) return false;
			
			for (int i = 0; i < points.Count; i ++)
			{
				if (!points[i].equalTo(other.points[i])) return false;
			}
			
			return true;
		}

		//This is for drawing
		/*public Vector3[] getPoints3D()
		{
			List<Vector3> vertex = new List<Vector3>(); 

			//foreach(Node n in points)
			for(int i =0;i<points.Count-1;i++)
			{
				Node n = points	[i]; 
				Node n1 = points[i+1]; 
				vertex.Add(new Vector3(n.x,n.t,n.y)); 
				vertex.Add(new Vector3(n1.x,n1.t,n1.y)); 
//				vertex.Add(new Vector3(n.x,n.t/10,n.y)); 
//				vertex.Add(new Vector3(n1.x,n1.t/10,n1.y)); 
			}
			return vertex.ToArray(); 
		}
		public Vector3[] getPoints3DFlat()
		{
			List<Vector3> vertex = new List<Vector3>(); 
			
			//foreach(Node n in points)
			for(int i =0;i<points.Count-1;i++)
			{
				Node n = points	[i]; 
				Node n1 = points[i+1]; 
				vertex.Add(new Vector3(n.x,2,n.y)); 
				vertex.Add(new Vector3(n1.x,2,n1.y)); 
				//				vertex.Add(new Vector3(n.x,n.t/10,n.y)); 
				//				vertex.Add(new Vector3(n1.x,n1.t/10,n1.y)); 
			}
			return vertex.ToArray(); 
		}*/


	}
	
	// Export / Import paths area

	[XmlRoot("bulk"), XmlType("bulk")]
	public class PathBulk {
		public List<Path> paths;
		
		public PathBulk () {
			paths = new List<Path> ();
		}
		
		public static void SavePathsToFile (string file, List<Path> paths) {
			XmlSerializer ser = new XmlSerializer (typeof(PathBulk));
			
			PathBulk bulk = new PathBulk ();
			bulk.paths.AddRange (paths);
			
			using (FileStream stream = new FileStream (file, FileMode.Create)) {
				ser.Serialize (stream, bulk);
				stream.Flush ();
				stream.Close ();
			}
		}
		
		public static List<Path> LoadPathsFromFile (string file) {
			XmlSerializer ser = new XmlSerializer (typeof(PathBulk));
			
			PathBulk loaded = null;
			using (FileStream stream = new FileStream (file, FileMode.Open)) {
				loaded = (PathBulk)ser.Deserialize (stream);
				stream.Close ();
			}
			
			// Setup parenting
			/*foreach (Path p in loaded.paths) {
				for (int i = p.points.Count - 1; i > 0; i--) {
					p.points [i].parent = p.points [i - 1];
				}
			}*/
			
			return loaded.paths;
		}
	}
	
	/*public class PathML : NodeProvider {
		/*
		//public List<TimeStamp> times = new List<TimeStamp> ();
		private SpaceState state;

		/// <summary>
		/// Serialization only.
		/// </summary>
		public PathML () {
		}

		public PathML (SpaceState state) {
			this.state = state;
		}
		
		/*public void SavePathsToFile (string file, List<Vector3> points) {

			for (int i = 0; i < points.Count; i++) {
				
				TimeStamp ts = new TimeStamp ();
				ts.t = i;
				ts.playerPos = points [i];
				
				for (int k = 0; k < SpaceState.Running.enemies.Length; k++) {

					EnemyStamp es = new EnemyStamp ();
					es.id = k;
					es.position = SpaceState.Running.enemies [k].positions [i];
					
					//int mapPX = (int)((ts.playerPos.x - SpaceState.Running.floorMin.x) / SpaceState.Running.tileSize.x);
					//int mapPY = (int)((ts.playerPos.z - SpaceState.Running.floorMin.z) / SpaceState.Running.tileSize.y);
					
					//int mapEX = (int)((es.position.x - SpaceState.Running.floorMin.x) / SpaceState.Running.tileSize.x);
					//int mapEY = (int)((es.position.z - SpaceState.Running.floorMin.z) / SpaceState.Running.tileSize.y);
					
					Node n1 = new Node ();
					n1.x = mapPX;
					n1.t = ts.t;
					//n1.y = mapPY;
					n1.cell = SpaceState.Running.fullMap [n1.t] [n1.x] [n1.y];
					
					Node n2 = new Node ();
					//n2.x = mapEX;
					n2.t = ts.t;
					//n2.y = mapEY;
					n2.cell = SpaceState.Running.fullMap [n2.t] [n2.x] [n2.y];
					
					//es.angle = Vector3.Angle (SpaceState.Running.enemies [k].forwards [i], (ts.playerPos - es.position).normalized);
					
					//es.los = ! CheckCollision (n1, n2, 0);
					//es.los = ! Extra.Collision.CheckCollision (n1, n2, this, state);
					
					ts.enemies.Add (es);
				}
				
				times.Add (ts);
			}
			
			XmlSerializer ser = new XmlSerializer (typeof(PathML));
			
			using (FileStream stream = new FileStream (file, FileMode.Create)) {
				ser.Serialize (stream, this);
				stream.Flush ();
				stream.Close ();
			}
		}

		public Node GetNode (int t, int x, int y) {
			Node n3 = new Node ();
			n3.cell = state.fullMap [t] [x] [y];
			n3.x = x;
			n3.y = y;
			n3.t = t;
			return n3;
		}
		
	}*/
	
	/*public class TimeStamp {
		
		[XmlAttribute]
		public int
			t;
		public Vector3 playerPos;
		public List<EnemyStamp> enemies = new List<EnemyStamp> ();
		
	}
	
	public class EnemyStamp {
		
		[XmlAttribute]
		public int
			id;
		public Vector3 position;
		public bool los;
		public float angle;
		
	}*/
}