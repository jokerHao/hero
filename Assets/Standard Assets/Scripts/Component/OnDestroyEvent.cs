using UnityEngine;
using System.Collections;

public class OnDestroyEvent : MonoBehaviour {

	System.Action<GameObject> act;
	public void When (System.Action<GameObject> action) {
		act = action;
	}
	void OnDestroy () {
		act(this.gameObject);
	}
	
}
