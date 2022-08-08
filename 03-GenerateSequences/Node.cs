using System;
using System.Collections.Generic;
////using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
////using Objects;

namespace Common {
	[Serializable]
	// Structure that holds the information used in the AStar cells
		public class Node : Priority_Queue.PriorityQueueNode {
		public int x, y, t;
		public double xD, yD, tD;
		
        //public float playerhp;
		//public float danger, danger3, danger3Norm;
		//[XmlIgnore]
		//public Dictionary<Enemy, float> enemyhp;
		//[XmlIgnore]
		///public List<Enemy> fighting;
		//[XmlIgnore]
		//public Enemy died;
		//[XmlIgnore]
		//public Node parent;
		//[XmlIgnore]
		//public Cell cell;
		//[XmlIgnore]
		//public bool visited = false;
		//[XmlIgnore]
		//public double accSpeed = 0.0d;
		//[XmlIgnore]
		//public List<HealthPack> picked;
		
		public Node() { }
		
		public Node(Node n)
		{
			x = n.x;
			y = n.y;
			t = n.t;
			xD = n.xD;
			yD = n.yD;
			tD = n.tD;
			//parent = n.parent;
			//cell = n.cell;
			//danger = n.danger;
			//danger3 = n.danger3;
			//danger3Norm = n.danger3Norm;
		}
		
		//public Node(int x_, int y_, int t_)
		//{
			//xD = x = x_;
			//yD = y = y_;
			//tD = t = t_;
		//}
		
		public Node(double x_, double y_, double t_)
		{
			xD = x_;
			x = (int)xD;
			yD = y_;
			y = (int)yD;
			tD = t_;
			t = (int)tD;
		}

		/*public float DistanceFrom (Node n) {
			Vector2 v1, v2;
			v1 = new Vector2 (this.x, this.y);
			v2 = new Vector2 (n.x, n.y);

			return (v1 - v2).magnitude + Math.Abs (this.t - n.t);
		}

		public Vector2 GetVector2 () {
			return new Vector2 (x, y);	
		}

		public Vector3 GetVector3 () {
			return new Vector3 (x, t, y);	
		}*/

		public double[] GetArray () {
			return new double[] {x, y};
		}

		public Boolean equalTo (Node b) {
			if (this.x == b.x & this.y == b.y & this.t == b.t)
				return true;
			return false; 
		}

		public override string ToString () {
			return  x + ",";
		}


		public int Axis (int axis) {
			switch (axis) {
			case 0:
				return x;
			case 1:
				return t;
			case 2:
				return y;
			default:
				throw new ArgumentException ();
			}
		}
	}
}