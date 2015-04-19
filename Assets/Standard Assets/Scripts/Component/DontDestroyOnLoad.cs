using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour {

	private void Awake() {
		DontDestroyOnLoad(this);
	}
	void Start () {
	}
}