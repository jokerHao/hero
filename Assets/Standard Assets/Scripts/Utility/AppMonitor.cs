using UnityEngine;
using System.Collections;

public class AppMonitor : MonoBehaviour {
	
	public static event System.Action onBackPrevious, onOpenMenu, onPause, onResume, onQuit;
	public static event System.Action<bool> onNetworkReachChange;
	public static bool networkReach {get; private set;}
	
	void Start () {
		
	}
	void Update () {
//		if(Input.GetKeyDown(KeyCode.Escape)){
//			OnTriggerBackPrevious();
//		}
//		if(Input.GetKeyDown(KeyCode.Menu)){
//			OnTriggerOpenMenu();
//		}
		// network
		bool nowStatus = Application.internetReachability != NetworkReachability.NotReachable;
		// when status change
		if (nowStatus != networkReach) {
			networkReach = nowStatus;
			if (onNetworkReachChange!=null)
				onNetworkReachChange(nowStatus);
			#if DEBUGLOG
			Debug.Log(string.Format("onNetworkReach({0})",nowStatus));
			#endif
		}
	}
	void OnApplicationPause (bool pauseStatus) {	
		if (pauseStatus) {
			#if DEBUGLOG
			Debug.Log("App Pause");
			#endif
			if (onPause!=null) {
				onPause();
			}
		}
		else {
			#if DEBUGLOG
			Debug.Log("App Resume");
			#endif		
			if (onResume!=null) {
				onResume();
			}
		}
	}
	void OnApplicationQuit () {
		#if DEBUGLOG
		Debug.Log("App Quit");
		#endif		
		if (onQuit!=null) {
			onQuit();
		}
	}
	
	public void OnTriggerBackPrevious () {
		#if DEBUGLOG
		Debug.Log("App BackPrevious");
		#endif
		if (onBackPrevious!=null)
			onBackPrevious();
	}
	
	public void OnTriggerOpenMenu () {
		#if DEBUGLOG
		Debug.Log("App OpenMenu");
		#endif
		if (onOpenMenu!=null)
			onOpenMenu();
	}
}
