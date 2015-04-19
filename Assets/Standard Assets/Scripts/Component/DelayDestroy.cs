using UnityEngine;
using System.Collections;

public class DelayDestroy : MonoBehaviour {

	public float time;
	void Start () {
		Destroy(this.gameObject, time);
	}
}
