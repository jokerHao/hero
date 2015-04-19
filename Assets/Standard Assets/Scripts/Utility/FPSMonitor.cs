using UnityEngine;
using System.Collections;

public class FPSMonitor : MonoBehaviour {
	
	public Rect windowRect = new Rect(20, 20, 100, 120);
	public float updateInterval = 1;
	public float fps;
	float timer;
	
	void Update () {
		timer += Time.smoothDeltaTime;
		if (timer>updateInterval) {
			timer = 0;
			fps = 1f/Time.smoothDeltaTime;
		}		
	}
	void OnGUI() {
		windowRect = GUI.Window(100, windowRect, DoMyWindow, "FPS");
	}
	void DoMyWindow(int windowID) {
		GUILayout.Label(string.Format("FPS : {0:0}", fps));
		GUILayout.Label(string.Format("target : {0}",Application.targetFrameRate));
		if (GUILayout.Button("+")) {
			Application.targetFrameRate += 5;
		}
		if (GUILayout.Button("-")) {
			Application.targetFrameRate -= 5;
		}
		GUI.DragWindow(new Rect(0, 0, 10000, 200));
	}
}
