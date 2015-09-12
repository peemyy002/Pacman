using UnityEngine;
using System.Collections;

namespace Pacman
{
	public class PacmanController : MonoBehaviour
	{

		public float MoveSpeed;
		public Stage CurrentStage;
		float mTimer;
		Vector3 mNextPosition;
		Vector3 mNextDirection;
		Vector3 mMoveDirection;

		// Use this for initialization
		void Start ()
		{
			Debug.Log ("Pacman - Start");
			mNextPosition = transform.position;
			mTimer = 0.001f;
		}
		
		// Update is called once per frame
		void Update ()
		{
			float h = Input.GetAxisRaw ("Horizontal");
			float v = Input.GetAxisRaw ("Vertical");
			if (h > 0)
				mNextDirection = new Vector3 (1, 0, 0);
			else if (h < 0)
				mNextDirection = new Vector3 (-1, 0, 0);
			else if (v > 0)
				mNextDirection = new Vector3 (0, 0, 1);
			else if (v < 0)
				mNextDirection = new Vector3 (0, 0, -1);

			if (mTimer > 0) {
				mTimer -= Time.deltaTime;
				transform.position += mMoveDirection * MoveSpeed * CurrentStage.CellSize * Time.deltaTime;
			} else {
				transform.position = mNextPosition;
				int layerMask = 1 << PacmanConstants.LAYER_WALL;
				bool isCollided = true;
				if (!Physics.Raycast (transform.position, mNextDirection, 1.5f, layerMask)) {
					mMoveDirection = mNextDirection;
					isCollided = false;
				} else {
					if (!Physics.Raycast (transform.position, mMoveDirection, 1.5f, layerMask)) {
						isCollided = false;
					}
				}

				if (!isCollided) { 
					mNextPosition = transform.position + (mMoveDirection * CurrentStage.CellSize);
					mTimer = 1 / MoveSpeed;
				}
			}
		}

		void OnTriggerEnter (Collider other)
		{
			if (other.gameObject.tag == PacmanConstants.TAG_PACDOT) {
				Destroy (other.gameObject);
				Game.Instance.IncreaseScore (1);
			} else if (other.gameObject.tag == PacmanConstants.TAG_POWERUP) {
				Destroy (other.gameObject);
				for (int i=0; i<Game.Instance.Ghosts.Length; i++) {
					GhostController ghost = Game.Instance.Ghosts [i];
					ghost.ChangeGhostMode (GhostController.GhostModeEnum.Frightened);
				}
			}
				if (other.gameObject.tag == PacmanConstants.TAG_GHOST) {
					GhostController ghost = other.gameObject.GetComponent<GhostController> ();
					if (ghost.GhostMode == GhostController.GhostModeEnum.Frightened) {
						Destroy (other.gameObject);
				
					} else {
						Destroy (this.gameObject);
					}

				}
			

		}
	}
}