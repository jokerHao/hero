using UnityEngine;
using System.Collections;

public abstract class TouchBehaviour : MonoBehaviour {

#if UNITY_EDITOR
	private Vector3 prePosition;
#endif

	protected virtual void Update () {

		#if UNITY_EDITOR
		Vector3 position = Input.mousePosition;
		if (Input.GetMouseButtonDown(0)) {
			prePosition = position;
			Down(position);
		}
		else if (Input.GetMouseButtonUp(0)) {
			Up(position);
		}
		else if (Input.GetMouseButton(0)) {
			Vector3 delta = position-prePosition;
			Move(position, delta);
		}
		#else
		if (Input.touchCount > 0) {
			Touch curTouch = Input.GetTouch(0);
			Vector3 position = curTouch.position;
			switch (curTouch.phase) {
			case TouchPhase.Began:
				Down(position);
				break;
			case TouchPhase.Ended:
				Up(position);
				break;
			case TouchPhase.Moved:
				Move(position, curTouch.deltaPosition);
				break;
			}
		}
		#endif
	}
	protected abstract void Down (Vector3 position);
	protected abstract void Up (Vector3 position);
	protected abstract void Move (Vector3 position, Vector3 deltaPosition);

}
