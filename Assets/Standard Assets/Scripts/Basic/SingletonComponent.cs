using UnityEngine;
using System.Collections;

public class SingletonComponent<T> : MonoBehaviour where T: SingletonComponent<T> {

	private static T _instance;
	public static T instance {
		get {
			if (_instance==null) {
				GameObject obj  = new GameObject(typeof(T).Name);
				obj.AddComponent<T>();
				// wait awake
				while (_instance==null) {
				}
				return _instance;				
			}
			else
				return _instance;
		}
	}
	protected virtual void Awake () {
		if (_instance!=null)
			throw new System.PlatformNotSupportedException();
		GameObject.DontDestroyOnLoad(this.gameObject);
		_instance = this.GetComponent<T>();
	}	
}
