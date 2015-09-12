using UnityEngine;
using System.Collections;

namespace Pacman {

	public class Game : MonoBehaviour {

		public static Game Instance;

		public PacmanController Pacman;
		public Stage CurrentStage;
		public GhostController[] Ghosts;

		public int Score;
		public int HighScore;


		void Awake() {
			if (Instance != null) {
				Destroy(gameObject);
				return;
			}
			Instance = this;
		}
		
		void Start() {
			ResetGame();
		}
		
		public void IncreaseScore(int score) {
			Score += score;
			if (Score > HighScore) {
				HighScore = Score;
			}
		}
		
		public void ResetGame() {
			Score = 0;
		}
		
		public void Save() {
			PlayerPrefs.SetInt("high_score", HighScore);
		}
		
		public void Load() {
			if (PlayerPrefs.HasKey("high_score")) {
				HighScore = PlayerPrefs.GetInt("high_score");
			}
		}
	}


	}

