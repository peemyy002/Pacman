using UnityEngine;
using System.Collections;

namespace Pacman {

	public class StageGridDrawer : MonoBehaviour {

		public int GridSize = 1;
		public Color GridColor = new Color(1, 1, 1, 0.45f);

		void OnDrawGizmos() {
			Gizmos.color = GridColor;

			for (int z=-100; z<=100; z+=GridSize) {
				Vector3 p1 = new Vector3(-100, 0, z);
				Vector3 p2 = new Vector3(100, 0, z);
				Gizmos.DrawLine(p1, p2);
			}

			for (int x=-100; x<=100; x+=GridSize) {
				Vector3 p1 = new Vector3(x, 0, -100);
				Vector3 p2 = new Vector3(x, 0, 100);
				Gizmos.DrawLine(p1, p2);
			}
		}

	}

}