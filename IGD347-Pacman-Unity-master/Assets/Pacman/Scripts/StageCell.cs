using UnityEngine;
using System.Collections;

namespace Pacman {

	public class StageCell {

		public StageCell North;
		public StageCell South;
		public StageCell West;
		public StageCell East;
		public Vector3 Position;
		public int R=0;
		public int C=0;


		public int TraversalPoint;

	}

}