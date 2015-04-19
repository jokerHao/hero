using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {

	public Vector3 axis;
	public float anglePreSecond;
	
	void Update () {
		this.transform.Rotate(axis, anglePreSecond*Time.deltaTime);
	}
}
