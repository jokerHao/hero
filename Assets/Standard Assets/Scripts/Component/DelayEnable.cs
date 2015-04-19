using UnityEngine;
using System.Collections;

public class DelayEnable : MonoBehaviour {

	public GameObject target;
	public float seconds;

	void OnEnable () {
		StartCoroutine(Enable());
	}
	IEnumerator Enable () {
		yield return new WaitForSeconds(seconds);
		target.gameObject.SetActive(true);
	}
}
