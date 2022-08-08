using System;
/*using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using Objects;*/

namespace Common {
	[Serializable]
	// Structure that holds the information used in the AStar cells
		public class Vector2 : Priority_Queue.PriorityQueueNode {
		public int x, y;
		public double xD, yD, tD;

        public Vector2 playerPos;

        public Vector2 position;

        public Vector2() { }


        public Vector2(Vector2 v)
		{
			x = v.x;
			y = v.y;
			xD = v.xD;
			yD = v.yD;
            playerPos = v.playerPos;
            position = v.position;
		}
        public Vector2(int x_, int y_)
		{
			xD = x = x_;
			yD = y = y_;
		}
    }
}