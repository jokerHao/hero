using UnityEngine;
using System.Collections;

public class DelaySendMessage : MonoBehaviour {

	public float time;
	public bool loop;
	public Msg message;

	void OnEnable () {
		StartCoroutine(Do());
	}
	IEnumerator Do () {
		yield return new WaitForSeconds(time);
		message.Send ();
		if (loop)
			StartCoroutine(Do());
	}
}
