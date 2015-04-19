using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// T : Component
public class GameObjectPool<T> where T:MonoBehaviour {
	
	public int size {
		get {
			return list.Count;
		}
	}
	GameObject prefab;
	bool autoResize;
	/// pair : <entity, idle>
	public List<T> list {get; private set;}
	public delegate void EntityHandler (ref T entity);
	public int usingCount {
		get {
			int result=0;
			foreach (bool val in statelist) {
				if (val==false)
					result++;
			}
			return result;
		}
	}
	
	List<bool> statelist;
	EntityHandler initHandler;
	
	public GameObjectPool (GameObject prefab, int size, bool autoResize, EntityHandler initHandler) {
		this.prefab = prefab;
		this.autoResize = autoResize;
		this.initHandler = initHandler;
		list = new List<T>();
		statelist = new List<bool>();
		for (int i=0; i<size; i++) {
			AddEntity();
		}
	}
	
	public T Pull () {
		int i=0;
		foreach (bool val in statelist) {
			if (val==true) {
				T entity = list[i];
				Dequeue(entity);
				return entity;
			}
			i++;
		}
		if (autoResize) {
			T entity = AddEntity();
			Dequeue(entity);
			return entity;
		}
		else {
			return null;
		}
	}
	
	public void Push (T target) {
		Enqueue(target);
	}
	
	public void PushAll (EntityHandler handler) {
		int i=0;
		foreach (bool val in statelist) {
			if (val==false) {
				T entity = list[i];
				handler(ref entity);
				Push(entity);
			}
			i++;
		}	
	}
	
	public void Dispose () {
		while(list.Count>0) {
			T entity = list[0];
			list.Remove(entity);
			GameObject.Destroy(entity.gameObject);
		}
		list = null;
		statelist = null;
		prefab = null;
		initHandler = null;
	}
	
	public bool IsUsing (T entity) {
		int idx = list.IndexOf(entity);
		return !statelist[idx];
	}
	
	T AddEntity () {
		GameObject obj = GameObject.Instantiate(prefab) as GameObject;
		T entity = obj.gameObject.GetComponent<T>();
		if (entity==null)
			entity = obj.gameObject.AddComponent<T>();
		list.Add(entity);
		statelist.Add(true);
		initHandler(ref entity);
		return entity;
	}
	
	void RemoveEntity (T entity) {
		int idx = list.IndexOf(entity);
		list.RemoveAt(idx);
		statelist.RemoveAt(idx);
	}
	
	void Enqueue (T entity) {
		int idx = list.IndexOf(entity);
		statelist[idx] = true;
	}
	
	void Dequeue (T entity) {
		int idx = list.IndexOf(entity);
		statelist[idx] = false;
	}
}
