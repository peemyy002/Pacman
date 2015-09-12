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
			Vector3 pacmanPos = Game.Instance.Pacman.transform.position;
			StageCell pacmanCell = Game.Instance.CurrentStage.GetStageCellAtPosition (pacmanPos);
			int C = pacmanCell.C;
			int R = pacmanCell.R;
			int c1 = 0;
			int c2 = 0;
			int r1 = 0;
			int r2 = 0;
			StageCell ghostCell = Game.Instance.CurrentStage.GetStageCellAtPosition(ghostPos);

			int section = 0;
			int GhostSection= 0;
			int numberOfColumns = Game.Instance.CurrentStage.GetNumberOfColumns();
			int numberOfRows = Game.Instance.CurrentStage.GetNumberOfRows();
		
			//Find Sections
			//S1
			if ((C >= 0 && C <= numberOfColumns / 2) && (R >= 0 && R <= numberOfRows / 2)) 
			{
				section = 1;
			} else if ((C >= 0 && C <= numberOfColumns / 2) && (R >= (numberOfRows / 2) + 1 && R <= numberOfRows)) 
			{
				section = 2;
			} else if ((C >= (numberOfColumns / 2) + 1 && C <= numberOfColumns) && (R >= (numberOfRows / 2) + 1 && R <= numberOfRows)) 
			{
				section = 3;
			} else if ((C >= (numberOfColumns / 2) + 1 && C <= numberOfColumns) && (R >= 0 && R <= numberOfRows / 2)) 
			{
				section = 4;
			}

			//
			if (section == 1) 
			{
				GhostSection =3;
			}
			if (section == 3) 
			{
				GhostSection = 1;
			}
			if (section ==2)
			{
				GhostSection = 4;
			}
			if (section== 4)
			{
				GhostSection=2;
			}
			//
			if (GhostSection==1)
			{
				c1= 0;
				c2= numberOfColumns/2;
				r1=0;
				r2=numberOfRows/2;
			}
			if (GhostSection==2)
			{
				c1= 0;
				c2= numberOfColumns/2;
				r1=(numberOfRows/2)+1;
				r2=numberOfRows;
			}
			if (GhostSection==3)
			{
				c1= (numberOfColumns/2)+1;
				c2= numberOfColumns;
				r1=(numberOfRows/2)+1;
				r2=numberOfRows;
			}
			if (GhostSection==4)
			{
				c1= (numberOfColumns/2)+1;
				c2= numberOfColumns;
				r1=0;
				r2=numberOfRows/2;
			}

			int col = Random.Range(c1,c2);
			int row = Random.Range(r1, r2);
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