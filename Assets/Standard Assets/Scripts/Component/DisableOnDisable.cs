using UnityEngine;
using System.Collections;

public class DisableOnDisable : MonoBehaviour {

	void OnDisable () {
		this.gameObject.SetActive(false);
	}
}
