using UnityEngine;
using System.Collections;
using System;

public class UI_GameServerTest : MonoBehaviour {

	public GameServer gameSer;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI () {
		if (GUILayout.Button("Connect", GUILayout.Width(Screen.width))) {
			gameSer.Connect("192.168.2.5",6012);
		}
		if (GUILayout.Button("Game_Entry", GUILayout.Width(Screen.width))) {
			gameSer.Game_Entry();
		}
		if (GUILayout.Button("Game_GetPlayers", GUILayout.Width(Screen.width))) {
			gameSer.Game_GetPlayers();
		}
		if (GUILayout.Button("Game_Ready", GUILayout.Width(Screen.width))) {
			gameSer.Game_Ready();
		}
	}
}
