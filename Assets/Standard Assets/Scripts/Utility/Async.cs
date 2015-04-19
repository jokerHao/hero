using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Async : MonoBehaviour {
	
	public class ActAbstract {
		public float delay;
		public bool isDone {get; protected set;}
		public virtual void Do(){}
	}
	public class Act : ActAbstract{
		public Action action;
		public Act (float delay, Action action) {
			isDone = false;
			this.delay = delay;
			this.action = action;
		}
		public override void Do () {
			action();
			isDone = true;
		}
	}
	public class ActLoop : ActAbstract{
		public Func<bool> action;
		public ActLoop (float delay, Func<bool> action) {
			isDone = false;
			this.delay = delay;
			this.action = action;
		}
		public override void Do () {
			isDone = action();
		}
	}

	
	static Async _singleton;
	static Async singleton {
		get {
			if (_singleton==null) {
				GameObject obj = new GameObject("Async") as GameObject;
				GameObject.DontDestroyOnLoad(obj);
				_singleton = obj.AddComponent<Async>();
			}
			return _singleton;
		}
	}
	List<ActAbstract> actListBuffer = new List<ActAbstract>();
	List<ActAbstract> actList = new List<ActAbstract>();
	
	void Update () {
		foreach (ActAbstract act in actListBuffer) {
			actList.Add(act);
		}
		actListBuffer.Clear();
		
		foreach (ActAbstract act in actList) {
			act.delay -= Time.deltaTime;
			if (act.delay<=0) {
				act.Do();
			}
		}
		actList.RemoveAll((ActAbstract act)=> {
			return act.isDone;
		});
	}
	
	static void Add (ActAbstract act) {
		singleton.actListBuffer.Add(act);
	}
	
	public static void DelayTime (float delay, Action action) {
		Add(new Act(delay, action));
	}
	
	public static void DelayFrame (int frame, Action action) {
		singleton.StartCoroutine(DelayFrame_C(frame, action));
	}
	static IEnumerator DelayFrame_C (int frame, Action action) {
		while (frame>0) {
			frame--;
			yield return 1;
		}
		Add(new Act(0, action));
	}	
	
	public static Action CallNextOverTime (float time, Action next) {
		float start = Time.realtimeSinceStartup;
		return ()=>{
			float over = (Time.realtimeSinceStartup-start)-time;
			if (over>0) {
				next();
			}
			else {
				DelayTime(over*-1, next);
			}
		};
	}
	
	public static void WaterFall (Action<string> end, params Action<Action<string>>[] actions) {
		WaterFall(0, end, actions);
	}
	static void WaterFall (int index, Action<string> end, params Action<Action<string>>[] actions) {
		actions[index]((string err)=>{
			index++;
			if (!string.IsNullOrEmpty(err) || actions.Length==index) {
				end(err);
			}
			else {
				WaterFall(index, end, actions);
			}
		});
	}	
	
	public static void WaterFall<T, T1> (T globalData, Action<string> end, params Action<T, Action<string, T1>>[] actions) {
		WaterFall<T, T1>(0, globalData, default(T1), end, actions);
	}
	static void WaterFall<T, T1> (int index, T globalData, T1 data, Action<string> end, params Action<T, Action<string, T1>>[] actions) {
		actions[index](globalData, (string err, T1 data1)=>{
			int next = index+1;
			if (!string.IsNullOrEmpty(err) || actions.Length==index) {
				end(err);
			}
			else {
				WaterFall(next, globalData, data1, end, actions);
			}
		});
	}		
	
	public static void WaterFall<T> (T globalData, Action<string> end, params Action<T, Action<string>>[] actions) {
		WaterFall(0, globalData, end, actions);
	}
	static void WaterFall<T> (int index, T globalData, Action<string> end, params Action<T, Action<string>>[] actions) {
		actions[index](globalData, (string err)=>{
			index++;
			if (!string.IsNullOrEmpty(err) || actions.Length==index) {
				end(err);
			}
			else {
				WaterFall(index, globalData, end, actions);
			}
		});
	}
	
	/// action return isDone
	public static void StartCoroutine (Func<bool> action) {
		StartCoroutine(0, action);
	}
	/// action return isDone
	public static void StartCoroutine (float delay,  Func<bool> action) {
		Add(new ActLoop(delay, action));
	}
}
