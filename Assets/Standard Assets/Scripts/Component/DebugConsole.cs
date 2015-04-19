using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DebugConsole : MonoBehaviour {


    public class Log {
        public string value;
        public LogType type;
        public Log (string log, LogType type) {
            this.value = log;
            this.type = type;
        }
    }
    public int width = 100;
    public int height = 50;
    public List<Log> logs = new List<Log>();
    public event Action<bool> onOpen;
    public bool open {
        get {
            return _open;
        }
        set {
            if (onOpen!=null)
                onOpen(value);
            _open = value;
        }
    }
    public bool _open;
    public int max_view = 1000;
    public int depth=-1000;
    Vector2 sv_pos = new Vector2(0,0);
    Rect windowRect;
    int selGridInt;
    string[] selStrings = new string[4]{"All", "Log", "Warning", "Error"};
	GUIStyle currentStyle = null;
		
	// Use this for initialization
	void Start () {
        Application.RegisterLogCallback(HandleLog);
        windowRect = new Rect(0, 0, width, height);
	}
	Texture2D MakeTex( int width, int height, Color col )
	{
		Color[] pix = new Color[width * height];
		for( int i = 0; i < pix.Length; ++i )
		{
			pix[ i ] = col;
		}
		Texture2D result = new Texture2D( width, height );
		result.SetPixels( pix );
		result.Apply();
		return result;
	}
	
	void InitStyles()
	{
		if( currentStyle == null )
		{
			currentStyle = new GUIStyle( GUI.skin.box );
			currentStyle.normal.background = MakeTex( 2, 2, new Color( 0f, 0f, 0f, 0.8f ) );
		}
	}
	
    void OnGUI () {
		InitStyles();
        if (open)
        {
			GUI.depth = depth;
			
			GUI.Box(new Rect(0,0,Screen.width, Screen.height),"",currentStyle);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("clear", GUILayout.Height(height))) {
                Clear();
            }
            if (GUILayout.Button("save", GUILayout.Height(height))) {
                Save();
            }
            if (GUILayout.Button("x", GUILayout.Width(width), GUILayout.Height(height))) {
                open = false;
            }
            GUILayout.EndHorizontal();
			
            selGridInt = GUILayout.SelectionGrid(selGridInt, selStrings, 4);

            sv_pos = GUILayout.BeginScrollView(sv_pos, GUILayout.Width(Screen.width));
            for (int i=(logs.Count>max_view) ? (logs.Count-max_view) : 0; i<logs.Count; i++) {
                Log log = logs[i];
                switch(selGridInt) {
                    case 0:
                        GUILayout.TextField(log.value);
                        break;
                    case 1:
                        if (log.type == LogType.Log) {
                            GUILayout.TextField(log.value);
                        }
                        break;
                    case 2:
                        if (log.type == LogType.Warning) {
                            GUILayout.TextField(log.value);
                        }
                        break;
                    case 3:
                        if (log.type == LogType.Error || log.type == LogType.Exception) {
                            GUILayout.TextField(log.value);
                        }
                        break;
                }
            }
            GUILayout.EndScrollView();
        } 
        else
        {
            windowRect = GUI.Window (0, windowRect, DoMyWindow, "console");
        }
    }

    void DoMyWindow (int windowID) {
        if (GUILayout.Button("open")) {
            open = true;
        }
        GUI.DragWindow();
    }

    public void Clear () {
        logs.Clear();
    }

    public void Save () {
        string dirname = System.DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss");
        string dirpath = Application.dataPath + "/" + dirname;

        List<string> log_list = new List<string>();
        List<string> warning_list = new List<string>();
        List<string> error_list = new List<string>();

        foreach (Log val in logs) {
            switch (val.type) {
                case LogType.Log:
                    log_list.Add(val.value);
                    break;
                case LogType.Warning:
                    warning_list.Add(val.value);
                    break;
                case LogType.Error:
                case LogType.Exception:
                case LogType.Assert:
                    error_list.Add(val.value);
                    break;
            }
        }
        System.IO.Directory.CreateDirectory(dirpath);
        #if !UNITY_WEBPLAYER
        System.IO.File.WriteAllLines(dirpath+"/log.txt", log_list.ToArray());
        System.IO.File.WriteAllLines(dirpath+"/warning.txt", warning_list.ToArray());
        System.IO.File.WriteAllLines(dirpath+"/error.txt", error_list.ToArray());
		#endif
    }

    void HandleLog (string log, string stackTrace, LogType type) {
        string time = System.DateTime.Now.ToString("MM-dd hh:mm:ss");
        logs.Add(new Log(time+" # "+log, type));
        if (logs.Count>10000) {
            logs.RemoveAt(0);
        }
    }
}
