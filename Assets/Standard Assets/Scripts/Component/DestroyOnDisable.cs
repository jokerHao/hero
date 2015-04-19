using UnityEngine;
using System.Collections;

public class DestroyOnDisable : MonoBehaviour {

	private void OnDisable () {
		Destroy(this.gameObject);
	}
}
