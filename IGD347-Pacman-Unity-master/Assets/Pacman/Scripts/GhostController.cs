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
		public GameObject GhostSphere;
		public Material NormalMaterial;
			public Material FrightenedMaterial;
		public GhostModeEnum GhostMode;
		public float ScatterTime;
		public float ChasingTime;
		public float FrightenedTime;
		List<StageCell> mPath;
		int mPathIndex;

		Vector3 mNextPosition;
		Vector3 mMoveDirection;
		float mTimer;
		float mFrightenedTimer;
		float mScatterTime;

		void Start() {
			mPath = new List<StageCell>();
			mNextPosition = transform.position;
			mTimer = 0.01f;
			mMoveDirection = Vector3.zero;
			mPathIndex = 0;

			UpdatePathToPacman();
		}

		void Update() {
			if (GhostMode == GhostModeEnum.Chase) {
				if (mTimer > 0) {
					mTimer -= Time.deltaTime;
					transform.position += mMoveDirection * MoveSpeed * CurrentStage.CellSize * Time.deltaTime;
				}
				else {
					transform.position = mNextPosition;

					if (mPathIndex < mPath.Count - 1) {
						mPathIndex += 1;
						StageCell nextCell = mPath[mPathIndex];
						mNextPosition = nextCell.Position;
						Vector3 diff = mNextPosition - transform.position;
						mMoveDirection = diff.normalized;
						mTimer = CurrentStage.CellSize / (MoveSpeed * CurrentStage.CellSize);
					}
					else {
						UpdatePathToPacman();
					}
				}
			}
			else if (GhostMode == GhostModeEnum.Frightened) {
				mFrightenedTimer += Time.deltaTime;
				if (mFrightenedTimer >= FrightenedTime) {
					ChangeGhostMode(GhostModeEnum.Chase);
					return;
				}
				
				if (mTimer > 0) {
					mTimer -= Time.deltaTime;
					transform.position += mMoveDirection * MoveSpeed * CurrentStage.CellSize * Time.deltaTime;
				}
				else {
					transform.position = mNextPosition;
					if (mPathIndex < mPath.Count - 1) {
						mPathIndex += 1;
						StageCell nextCell = mPath[mPathIndex];
						mNextPosition = nextCell.Position;
						Vector3 diff = mNextPosition - transform.position;
						mMoveDirection = diff.normalized;
						mTimer = CurrentStage.CellSize / (MoveSpeed * CurrentStage.CellSize);
					}
					else {
						UpdatePathToGetAway();
					}
				}
			}
			else if (GhostMode == GhostModeEnum.Scatter) {
				mScatterTime += Time.deltaTime;
				if (mScatterTime >= FrightenedTime) {
					ChangeGhostMode(GhostModeEnum.Chase);
					return;
				}
				
				if (mTimer > 0) {
					mTimer -= Time.deltaTime;
					transform.position += mMoveDirection * MoveSpeed * CurrentStage.CellSize * Time.deltaTime;
				}
				else {
					transform.position = mNextPosition;
					if (mPathIndex < mPath.Count - 1) {
						mPathIndex += 1;
						StageCell nextCell = mPath[mPathIndex];
						mNextPosition = nextCell.Position;
						Vector3 diff = mNextPosition - transform.position;
						mMoveDirection = diff.normalized;
						mTimer = CurrentStage.CellSize / (MoveSpeed * CurrentStage.CellSize);
					}
					else {
						UpdatePathToGetAway();
					}
				}
			}
	}

		public void ChangeGhostMode(GhostModeEnum newMode) {
			if (GhostMode != newMode) {
				
				if (newMode == GhostModeEnum.Chase) {
					GhostSphere.GetComponent<Renderer>().sharedMaterial = NormalMaterial;
				}
				else if (newMode == GhostModeEnum.Frightened) {
					GhostSphere.GetComponent<Renderer>().sharedMaterial = FrightenedMaterial;
					mFrightenedTimer = 0;
				}
				else if (newMode == GhostModeEnum.Scatter) {
					
				}
				
				GhostMode = newMode;
			}
		}
		
		void UpdatePathToGetAway() {
			Vector3 ghostPos = transform.position;
			StageCell ghostCell = Game.Instance.CurrentStage.GetStageCellAtPosition(ghostPos);
			
			int numberOfColumns = Game.Instance.CurrentStage.GetNumberOfColumns();
			int numberOfRows = Game.Instance.CurrentStage.GetNumberOfRows();
			int col = Random.Range(0, numberOfColumns - 1);
			int row = Random.Range(0, numberOfRows - 1);
			StageCell targetCell = Game.Instance.CurrentStage.GetStageCellAt(col, row);
			
			List<StageCell> newPath = Game.Instance.CurrentStage.FindShortestPath(ghostCell, targetCell);
			mPath.Clear();
			mPathIndex = 0;
			for (int i=0; i<newPath.Count; i++) {
				mPath.Add(newPath[i]);
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