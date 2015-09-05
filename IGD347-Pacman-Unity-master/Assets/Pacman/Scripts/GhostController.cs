using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Pacman {

	public class GhostController : MonoBehaviour {

		public enum GhostModeEnum {
			Scatter,
			Frightened,
			Chase
		}

		public float MoveSpeed;
		public Stage CurrentStage;

		public GhostModeEnum GhostMode;
		public float ScatterTime;
		public float ChasingTime;

		List<StageCell> mPath;
		int mPathIndex;

		Vector3 mNextPosition;
		Vector3 mMoveDirection;
		float mTimer;

		void Start() {
			mPath = new List<StageCell>();
			mNextPosition = transform.position;
			mTimer = 0.01f;
			mMoveDirection = Vector3.zero;
			mPathIndex = 0;

			GhostMode = GhostModeEnum.Scatter;

			UpdatePathToPacman();
		}

		void Update() {
			if (mTimer > 0) {
				mTimer -= Time.deltaTime;
				transform.position += mMoveDirection * MoveSpeed * CurrentStage.CellSize * Time.deltaTime;
			}
			else {
				transform.position = mNextPosition;
				UpdatePathToPacman();
				if (mPathIndex < mPath.Count - 1) {
					mPathIndex += 1;
					StageCell nextCell = mPath[mPathIndex];
					mNextPosition = nextCell.Position;
					Vector3 diff = mNextPosition - transform.position;
					mMoveDirection = diff.normalized;
					mTimer = 1 / MoveSpeed;
				}
				else {
					UpdatePathToPacman();
				}
			}
		}

		void UpdatePathToPacman() {
			// Find cell where pacman is in
			Vector3 pacmanPos = Game.Instance.Pacman.transform.position;
			Vector3 ghostPos = transform.position;

			StageCell pacmanCell = Game.Instance.CurrentStage.GetStageCellAtPosition(pacmanPos);
			StageCell ghostCell = Game.Instance.CurrentStage.GetStageCellAtPosition(ghostPos);

			List<StageCell> newPath = Game.Instance.CurrentStage.FindShortestPath(ghostCell, pacmanCell);
			mPath.Clear();
			mPathIndex = 0;
			for (int i=0; i<newPath.Count; i++) {
				mPath.Add(newPath[i]);
			}
		}

		void OnDrawGizmos() {
			if (mPath == null) { 
				return;
			}

			Gizmos.color = Color.blue;
			for (int i=0; i<mPath.Count - 1; i++) {
				StageCell cell01 = mPath[i];
				StageCell cell02 = mPath[i + 1];
				Gizmos.DrawLine(cell01.Position, cell02.Position);
			}
		}
	}

}