using UnityEngine;
using System.Collections;

namespace Pacman {

	public class Game : MonoBehaviour {

		public static Game Instance;

		public PacmanController Pacman;
		public Stage CurrentStage;
		public GhostController[] Ghosts;

		void Awake() {
			if (Instance != null) {
				Destroy(gameObject);
				return;
			}
			Instance = this;
		}

	}

}