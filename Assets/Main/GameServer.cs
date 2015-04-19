using UnityEngine;
using System.Collections;
using System;
using SimpleJson;

public class GameServer : MonoBehaviour {

	public string host = "0.0.0.0";
	public int port = 0;
	public Pomelo.DotNetClient.PomeloClient socket;

	void Awake () {
	}
	
	public void Connect (string host, int port) {
		this.host = host;
		this.port = port;
		socket = new Pomelo.DotNetClient.PomeloClient (this.host, this.port);
		socket.connect ((data_connect)=>{
			Log("socket connect");
			Request("connector.entryHandler.entry", (data_enter)=>{
				On ("START", (data)=>{
				});
			});
		});
	}

	public void Game_Entry () {
		Request("game.gameHandler.entry", new JsonObject(), (data)=>{
		});
	}

	public void Game_GetPlayers () {
		Request("game.gameHandler.getPlayers", (data)=>{
		});
	}

	public void Game_Ready () {
		Request("game.gameHandler.ready", (data)=>{
		});
	}

	void Request (string route, Action<JsonObject> next) {
		Request (route, new JsonObject(), next);
	}
	void Request (string route, JsonObject msg, Action<JsonObject> next) {
		Debug.Log (string.Format ("[>>] {0} : {1}", route, msg));
		socket.request (route, msg, (data)=>{
			Debug.Log (string.Format ("[<<] {0} : {1}", route, data));
			next(data);
		});
	}
	void On (string route, Action<JsonObject> next) {
		Debug.Log (string.Format ("[+] {0}", route));
		socket.on(route, (data)=>{
			Debug.Log (string.Format ("[<<] push {0} : {1}", route, data));

		});
	}
	void Log (object msg) {
		Debug.Log(string.Format("[{0}:{1}] {2}",host, port ,msg));
	}
}
