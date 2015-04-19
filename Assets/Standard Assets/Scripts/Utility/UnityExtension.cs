using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class UnityExtension {

	public static T GetComponentInParent<T>(this Transform transform) where T: Component {
		GameObject parent = transform.parent.gameObject;
		while(parent != null) {
			T component = parent.GetComponent<T>();
			if(component != null) {
				return component;
			}
			if (parent.transform.parent==null)
				return default(T);
			parent = parent.transform.parent.gameObject;
		}
		return default(T);
	}
	public static T[] GetTypesChildren<T>(this MonoBehaviour gameObject, bool includeInactive) where T : class {
		List<T> temp = new List<T>();
		foreach (Component c in gameObject.gameObject.GetComponentsInChildren(typeof(T),includeInactive)) {
			temp.Add(c as T);
		}
		if (temp.Count > 0) {
			return temp.ToArray();
		} 
		else {
			return null;
		}
	}
	public static void SetLayerRecursively(this GameObject gameObject, LayerMask layer)	{
		foreach (Transform trans in gameObject.GetComponentsInChildren<Transform>(true)) {
			trans.gameObject.layer = layer;
		}
	}	
	public static void SetLayerRecursively(this GameObject gameObject, string layerName)	{
		foreach (Transform trans in gameObject.GetComponentsInChildren<Transform>(true)){
			trans.gameObject.layer = LayerMask.NameToLayer(layerName);
		}
	}
	public static GameObject FindChildByName(this GameObject gameObject, string name) {
		Component[] transforms = gameObject.GetComponentsInChildren<Transform>(true);	
		foreach (Transform transform in transforms){
			if (transform.gameObject.name == name)	{ 
				return transform.gameObject;
			}
		}
		Debug.Log("FindChildByName Null:"+gameObject+"  "+name);
		return null;
	}
	public static GameObject Clone(this GameObject gameObject) {
		GameObject clone = GameObject.Instantiate(gameObject) as GameObject;
		clone.transform.position = gameObject.transform.position;
		clone.gameObject.transform.parent = gameObject.gameObject.transform.parent;
		clone.transform.localScale = gameObject.transform.localScale;
		clone.transform.localRotation = gameObject.transform.localRotation;
		return clone;
	}
	public static void SetActiveChild(this GameObject gameObject, bool value) {
		foreach (Transform val in gameObject.transform) {
			val.gameObject.SetActive(value);
		}
	}
	public static string GetHierarchy (this Transform transform, int level) {
		string result = string.Empty;
		Transform t = transform;
		for (int i=0; i<level; i++) {
			result = t.gameObject.name+"/"+result;
			if (t.parent==null)
				return result;
			else
				t = t.parent;
		}
		return result;
	}
	public static bool TryGetIndex (this Transform transform, Transform value, out int index) {
		index = 0;
		foreach (Transform val in transform) {
			if (val==value) {
				return true;
			}
			index++;
		}
		return false;
	}
	public static Transform GetChildActive (this Transform transform, int index) {
		foreach (Transform val in transform) {
			if (val.gameObject.activeSelf) {
				if (index==0)
					return val;
				index--;
			}
		}
		return null;
	}	
	
	public static void EnsureContains<T>(this List<T> list, T value) {
		if (!list.Contains(value))
			list.Add(value);
	}
	public static void EnsureNoContains<T>(this List<T> list, T value) {
		if (list.Contains(value))
			list.Remove(value);
	}
	// Dictionary
	public static KeyValuePair<TKey,TValue> GetFromIndex<TKey,TValue>(this Dictionary<TKey,TValue> dictionary, int index) {
		foreach (KeyValuePair<TKey,TValue> pair in dictionary) {
			if (index==0) {
				return pair;
			}
			index --;
		}
		return default(KeyValuePair<TKey,TValue>);
	}
	public static TKey[] KeyToArray<TKey,TValue>(this Dictionary<TKey,TValue> dictionary) {
		TKey[] result = new TKey[dictionary.Count];
#if UNITY_IPHONE
		int i=0;
		foreach (TKey val in dictionary.Keys) {
			result[i] = val;
			i++;
		}
#else
		dictionary.Keys.CopyTo(result,0);
#endif
		return result;
	}
}
