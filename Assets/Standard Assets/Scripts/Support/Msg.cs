using UnityEngine;
using System.Collections;

[System.Serializable]
public class Msg {

	public enum ArgType {
		String,
		Int,
		Float,
		Bool,
	}
	public GameObject target;
	public string functionName;
	public string argument;
	public ArgType argType = ArgType.String;
	public bool includeChildren;
	public SendMessageOptions messageOptions = SendMessageOptions.RequireReceiver;
	
	public void Send () {
		if (string.IsNullOrEmpty(functionName))
			throw new System.NullReferenceException();
		if (target.gameObject.activeInHierarchy) {
			if (includeChildren) {
				if (argument==null)
					target.BroadcastMessage(functionName, messageOptions);
				else {
					switch (argType) {
					case ArgType.String:
						target.BroadcastMessage(functionName, (string)argument, messageOptions);
						break;
					case ArgType.Int:
						target.BroadcastMessage(functionName, int.Parse(argument), messageOptions);
						break;
					case ArgType.Float:
						target.BroadcastMessage(functionName, float.Parse(argument), messageOptions);
						break;
					case ArgType.Bool:
						target.BroadcastMessage(functionName, bool.Parse(argument), messageOptions);
						break;						
					}				
				}
			}
			else {
				if (argument==null)
					target.SendMessage(functionName, messageOptions);
				else {
					switch (argType) {
					case ArgType.String:
						target.SendMessage(functionName, (string)argument, messageOptions);
						break;
					case ArgType.Int:
						target.SendMessage(functionName, int.Parse(argument), messageOptions);
						break;
					case ArgType.Float:
						target.SendMessage(functionName, float.Parse(argument), messageOptions);
						break;
					case ArgType.Bool:
						target.SendMessage(functionName, bool.Parse(argument), messageOptions);
						break;						
					}
				}
			}
		}
	}
}
