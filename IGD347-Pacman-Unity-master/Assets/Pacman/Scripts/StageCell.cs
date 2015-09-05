using UnityEngine;
using System.Collections;

namespace Pacman {

	public class StageCell {

		public StageCell North;
		public StageCell South;
		public StageCell West;
		public StageCell East;
		public Vector3 Position;

		public int TraversalPoint;

	}

}