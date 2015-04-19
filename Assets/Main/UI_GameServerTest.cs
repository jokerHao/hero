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
			gameSer.Connect("127.0.0.1",6012);
		}
		if (GUILayout.Button("Game_EntryLobby", GUILayout.Width(Screen.width))) {
			gameSer.Game_EntryLobby();
		}
		if (GUILayout.Button("Game_GetLobbyPlayers", GUILayout.Width(Screen.width))) {
			gameSer.Game_GetLobbyPlayers();
		}
	}
}
