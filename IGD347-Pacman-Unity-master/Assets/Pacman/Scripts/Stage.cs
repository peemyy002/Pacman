using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pacman {

	public class Stage : MonoBehaviour {

		public int CellSize;
		public Vector3 BottomLeftCellPosition;
		public Vector3 TopRightCellPosition;

		StageCell[,] mCells;
		int mNumberOfColumns;
		int mNumberOfRows;

		void Awake() {
			//Get XZ of BTlC
			int bottomLeftCell_x = Mathf.FloorToInt(BottomLeftCellPosition.x);
			int bottomLeftCell_z = Mathf.FloorToInt(BottomLeftCellPosition.z);
			//Get XZ of TRC
			int topRightCell_x = Mathf.FloorToInt(TopRightCellPosition.x);
			int topRightCell_z = Mathf.FloorToInt(TopRightCellPosition.z);
			//Get NUM of C
			mNumberOfColumns = ((topRightCell_x - bottomLeftCell_x) / CellSize) + 1;
			//Get NUM of R
			mNumberOfRows = ((topRightCell_z - bottomLeftCell_z) / CellSize) + 1;
			Debug.Log (mNumberOfColumns);
			Debug.Log (mNumberOfRows);
			// Create all cells
			mCells = new StageCell[mNumberOfColumns, mNumberOfRows];
			for (int c=0; c<mNumberOfColumns; c++) {
				for (int r=0;r<mNumberOfRows; r++) {
					StageCell cell = new StageCell();
					cell.Position = new Vector3(bottomLeftCell_x + (c * CellSize), 0, bottomLeftCell_z + (r * CellSize));
					cell.North = null;
					cell.South = null;
					cell.West = null;
					cell.East = null;
					cell.C=c;
					cell.R=r;
					mCells[c, r] = cell;
				}
			}

			// Link cells
			for (int c=0; c<mNumberOfColumns; c++) {
				for (int r=0; r<mNumberOfRows; r++) {
					StageCell cell = mCells[c, r];

					// link north cell
					if (r + 1 < mNumberOfRows) {
						StageCell northCell = mCells[c, r + 1];
						bool canConnect = CheckCellConnection(cell, northCell);
						if (canConnect) {
							cell.North = northCell;
						}
					}

					// link south cell
					if (r - 1 >= 0) {
						StageCell southCell = mCells[c, r - 1];
						bool canConnect = CheckCellConnection(cell, southCell);
						if (canConnect) {
							cell.South = southCell;
						}
					}

					// link west cell
					if (c - 1 >= 0) {
						StageCell westCell = mCells[c - 1, r];
						bool canConnect = CheckCellConnection(cell, westCell);
						if (canConnect) {
							cell.West = westCell;
						}
					}

					// link east cell
					if (c + 1 < mNumberOfColumns) {
						StageCell eastCell = mCells[c + 1, r];
						bool canConnect = CheckCellConnection(cell, eastCell);
						if (canConnect) {
							cell.East = eastCell;
						}
					}
				}
			}
		}

		bool CheckCellConnection(StageCell fromCell, StageCell toCell) {
			Vector3 origin = fromCell.Position;
			Vector3 destination = toCell.Position;

			Vector3 diff = destination - origin;
			Vector3 direction = diff.normalized;
			float distance = diff.magnitude;

			RaycastHit hit;
			if (Physics.Raycast(origin, direction, out hit, distance, 1 << PacmanConstants.LAYER_WALL)) {
				return false;
			}
			else {
				return true;
			}
		}

		public int GetNumberOfColumns() {
			return mNumberOfColumns;
		}

		public int GetNumberOfRows() {
			return mNumberOfRows;
		}

		public StageCell GetStageCellAtPosition(Vector3 position) {
			Vector3 pos = position - BottomLeftCellPosition;
			int col = Mathf.FloorToInt(pos.x / Game.Instance.CurrentStage.CellSize);
			int row = Mathf.FloorToInt(pos.z / Game.Instance.CurrentStage.CellSize);
			if (col < 0 || col >= mNumberOfColumns) Debug.LogError("Column out of range: " + col);
			if (row < 0 || row >= mNumberOfRows) Debug.LogError("Row out of range: " + row);
			return mCells[col, row];
		}

		public List<StageCell> FindShortestPath(StageCell fromCell, StageCell toCell) {

			// Reset all cell's traversal point
			for (int c=0; c<mNumberOfColumns; c++) {
				for (int r=0; r<mNumberOfRows; r++) {
					mCells[c, r].TraversalPoint = int.MaxValue;
				}
			}

			Stack<StageCell> cellStack = new Stack<StageCell>();
			fromCell.TraversalPoint = 0;
			cellStack.Push(fromCell);

			// 1. Fill Traversal point until toCell found!
			while (cellStack.Count > 0) {
				StageCell cell = cellStack.Pop();

				int v = cell.TraversalPoint + 1;

				if (cell.North != null) {
					if (cell.North.TraversalPoint > v) {
						cell.North.TraversalPoint = v;
						if (cell.North != toCell) {
							cellStack.Push(cell.North);
						}
					}
				}

				if (cell.East != null) {
					if (cell.East.TraversalPoint > v) {
						cell.East.TraversalPoint = v;
						if (cell.East != toCell) {
							cellStack.Push(cell.East);
						}
					}
				}
				
				if (cell.South != null) {
					if (cell.South.TraversalPoint > v) {
						cell.South.TraversalPoint = v;
						if (cell.South != toCell) {
							cellStack.Push(cell.South);
						}
					}
				}
				
				if (cell.West != null) {
					if (cell.West.TraversalPoint > v) {
						cell.West.TraversalPoint = v;
						if (cell.West != toCell) {
							cellStack.Push(cell.West);
						}
					}
				}
			}

			// 2. We won't use cell stack anymore
			cellStack.Clear();
			cellStack = null;

			// 3. Traverse back
			List<StageCell> path = new List<StageCell>();
			StageCell traverseBackCell = toCell;
			path.Add(traverseBackCell);

			while (traverseBackCell != fromCell) {
				StageCell[] neightbourCells = new StageCell[] {
					traverseBackCell.North,
					traverseBackCell.East,
					traverseBackCell.South,
					traverseBackCell.West
				};

				StageCell minTraversePointCell = null;
				int minTraversePoint = int.MaxValue;
				for (int i=0; i<neightbourCells.Length; i++) {
					if (neightbourCells[i] == null) {
						continue;
					}
					if (neightbourCells[i].TraversalPoint < minTraversePoint) {
						minTraversePoint = neightbourCells[i].TraversalPoint;
						minTraversePointCell = neightbourCells[i];
					}
				}

				traverseBackCell = minTraversePointCell;
				path.Add(traverseBackCell);
			}

			path.Reverse();
			return path;
		}

		void OnDrawGizmos() {
			if (mCells == null) {
				return;
			}
			
			Gizmos.color = Color.green;
			for (int c=0; c<mNumberOfColumns; c++) {
				for (int r=0; r<mNumberOfRows; r++) {
					StageCell cell = mCells[c, r];
					if (cell.North != null) Gizmos.DrawLine(cell.Position, cell.North.Position);
					if (cell.South != null) Gizmos.DrawLine(cell.Position, cell.South.Position);
					if (cell.West != null) Gizmos.DrawLine(cell.Position, cell.West.Position);
					if (cell.East != null) Gizmos.DrawLine(cell.Position, cell.East.Position);
				}
			}
		}
		public StageCell GetStageCellAt(int col, int row) {
			if (col < 0 || col >= mNumberOfColumns) Debug.LogError("Column out of range: " + col);
			if (row < 0 || row >= mNumberOfRows) Debug.LogError("Row out of range: " + row);
			return mCells[col, row];
		}
		

	}

}