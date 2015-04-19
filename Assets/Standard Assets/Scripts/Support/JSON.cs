using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

public class JSON {

	public class Value {
		
		public Value this[int idx] {get {return (mix as JSON)[idx];} set {(mix as JSON)[idx] = value;}}
		public Value this[string name] {get {return (mix as JSON)[name];} set {(mix as JSON)[name] = value;}}
		
		public int count {get {return mix is JSON ? (mix as JSON).count : 0;}}
		public bool empty {get {return mix is JSON ? (mix as JSON).empty : mix==null;}}
		public bool exist {get {return mix != null;}}
		public bool isAry {get {return mix is JSON && (mix as JSON).isAry;}}
		public bool isObj {get {return mix is JSON && (mix as JSON).isObj;}}
		
		public JSON json {get {return mix as JSON;} set {Set(value);}}
		public string str {get {return mix is JSON ? string.Empty : ToString().Trim('"');} set {Set(value);}}
		public bool flag {get {return mix is JSON ? false : (ToString().Equals("true") ? true : false);} set {Set(value);}}
		public int num {get {return (int)d;} set {Set(value);}}

		public float f {
			get {
				if(mix is JSON) {
					return 0;
				} else {
					try {
						return float.Parse(str);
					} catch {
						return 0;
					}
				}
			}
			set {
				Set(value);
			}
		}

		public double d {
			get {
				if(mix is JSON) {
					return 0;
				} else {
					try {
						return double.Parse(str);
					} catch {
						return 0;
					}
				}
			}
			set {
				Set(value);
			}
		}
		
		public bool visible = true;
		
		private object mix = null;
		
		public Value() {}
		
		public Value(string obj) {
			Set(obj);
		}
		
		public Value(bool obj) {
			Set(obj);
		}
		
		public Value(object obj) {
			Set(obj);
		}
		
		public void Set(string obj) {
			mix = obj != null ? string.Concat('"', obj, '"') : null;
		}
		
		public void Set(bool obj) {
			mix = obj ? "true" : "false";
		}
		
		public void Set(object obj) {
			mix = obj != null ? obj : null;
		}
		
		public bool TryGetValue(int idx, out Value val) {
			if(mix is JSON) {
				return (mix as JSON).TryGetValue(idx, out val);
			}
			val = null;
			return false;
		}
		
		public bool TryGetValue(string key, out Value val) {
			if(mix is JSON) {
				return (mix as JSON).TryGetValue(key, out val);
			}
			val = null;
			return false;
		}

		public IEnumerator<Pair> GetEnumerator() {
			if(mix is JSON) {
				return (mix as JSON).GetEnumerator();
			}
			return null;
		}
		
		public override string ToString() {
			return mix != null ? mix.ToString() : "null";
		}
	}
	
	public class Pair {
		
		public string name;
		public Value value;
		
		public Pair() {
		
		}
		
		public Pair(string name) {
			this.name = name;
			this.value = new Value();
		}
		
		public Pair(string name, string value) {
			this.name = name;
			this.value = new Value(value);
		}
		
		public Pair(string name, bool value) {
			this.name = name;
			this.value = new Value(value);
		}
		
		public Pair(string name, object value) {
			this.name = name;
			this.value = new Value(value);
		}	
	}

	public Value this[int idx] {
		get {
			if (pairs.Count>idx)
				return pairs[idx].value;
			else
				return null;
		} 
		set {
			pairs[idx].value = value;
		}
	}
	public Value this[string name] {
		get {
			foreach(Pair pair in pairs) {
				if(name.Equals(pair.name)) {
					return pair.value;
				}
			}
			return null;
		}
		set {
			int count = pairs.Count;
			for(int i = 0; i < count; ++i) {
				if(name.Equals(pairs[i].name)) {
					pairs[i].value = value;
				}
			}
		}
	}
	
	public int count {get {return pairs!=null ? pairs.Count : 0;}}
	public bool empty {get {return pairs==null || pairs.Count==0;}}
	public bool isAry {get {return isArray;}}
	public bool isObj {get {return ! isArray;}}
	
	protected bool isArray = true;
	protected List<Pair> pairs = new List<Pair>();

	public static JSON Parse(string text) {
		return Parse(text, 0);
	}

	public static JSON Parse(string text, int idx) {
		string ru = Regex.Unescape(text);
		return Parse(ru, ref idx) as JSON;
	}

	public static object Parse(string text, ref int idx) {
		if(idx < text.Length) {
			int end;
			char str = text[idx];
			switch(str) {
			case '[':	
				++idx;			
				Value[] vals = null;
				if(TryGetAry(text, ref idx, 0, out vals)) {
					return new JSON(vals);
				}
				break;
			case '{':
				Pair[] pairs = null;
				++idx;
				if(TryGetObj(text, ref idx, 0, out pairs)) {
					return new JSON(pairs);
				}
				break;
			case '"':
				end = (text.IndexOfAny(new char[] {'"'}, idx+1))+1;
				if(end != -1) {
					string obj = text.Substring(idx, end - idx);
					idx = end;
					return obj.Equals("null") ? null : obj;
				}
				idx = text.Length;
				break;
			case ' ':
				++idx;
				return Parse(text, ref idx);
				break;
			default:
				end = text.IndexOfAny(new char[] {',', ']', '}'}, idx);
				if(end != -1) {
					string obj = text.Substring(idx, end - idx);
					idx = end;
					return obj.Equals("null") ? null : obj;
				}
				idx = text.Length;
				break;
			}
		}
		return null;
	}

	private static bool TryGetAry(string text, ref int idx, int count, out Value[] objs) {
		if(idx < text.Length) {
			if(text[idx].Equals(']')) {
				++idx;
				objs = new Value[count];
				return true;
			}
			Value val = new Value();
			do {
				switch(text[idx]) {
				case ',':
					++idx;
					if(TryGetAry(text, ref idx, count + 1, out objs)) {
						objs[count] = val;
						return true;
					}
					return false;
				case ']':
					++idx;
                    
                    if (val.ToString()=="null") {
                        objs = new Value[count];
                        return true;
                    }
					objs = new Value[count + 1];
					objs[count] = val;
                        return true;
				case ' ':
					++idx;
					break;
				default:
					val.Set(Parse(text, ref idx));
					break;
				}
			} while(idx < text.Length);
		}
		objs = null;
		return false;
	}

	private static bool TryGetObj(string text, ref int idx, int count, out Pair[] pairs) {
		Pair pair = new Pair();
		while(idx < text.Length) {
			switch(text[idx]) {
			case '"':
				++idx;
				int end = text.IndexOf('"', idx);
				if(end != -1) {
					pair.name = text.Substring(idx, end - idx);
					idx = end + 1;
				}
				break;
			case ':':
				++idx;
				pair.value = new Value(Parse(text, ref idx));
				break;
			case ',':
				++idx;
				if(TryGetObj(text, ref idx, count + 1, out pairs)) {
					pairs[count] = pair;
					return true;
				}
				return false;			
			case '}':
				++idx;
				pairs = new Pair[count + 1];
				pairs[count] = pair;
				return true;
			case ' ':
				++idx;
				break;			
			default:
				idx = text.Length;
				break;
			}
		}
		pairs = null;
		return false;
	}
	
	public JSON() {
		isArray = false;
	}

	public JSON(int capacity) {
		pairs = new List<Pair>();
		pairs.AddRange(new Pair[capacity]);
		int count = pairs.Count;
		for(int i = 0; i < count; ++i) {
			pairs[i] = new Pair();
		}
	}
	
	public JSON(params Value[] values) {
		isArray = true;
		pairs = new List<Pair>();
		pairs.AddRange(new Pair[values != null ? values.Length : 0]);
		int count = pairs.Count;
		for(int i = 0; i < count; ++i) {
			pairs[i] = new Pair();
			pairs[i].value = values[i];
		}
	}
	
	public JSON(params string[] names) {
		isArray = false;
		pairs = new List<Pair>();
		pairs.AddRange(new Pair[names != null ? names.Length : 0]);
		int count = pairs.Count;
		for(int i = 0; i < count; ++i) {
			pairs[i] = new Pair();
			pairs[i] = new Pair(string.IsNullOrEmpty(names[i]) ? i.ToString() : names[i]);
		}
	}
	
	public JSON(params Pair[] pairs) {
		if(pairs != null) {
			isArray = false;
			this.pairs = new List<Pair>(pairs);
		}
	}

	public bool TryGetValue<T> (string key, out T val) {
		JSON.Value jsonValue;
		if (TryGetValue(key,out jsonValue)) {
			if (jsonValue.empty) {
				val = default(T);
				return false;
			}
			string typeName = typeof(T).Name;
			switch (typeName) {
			case "Boolean":
				val = (T)System.Convert.ChangeType(jsonValue.flag,typeof(T));
				return true;
			case "Int32":
				val = (T)System.Convert.ChangeType(jsonValue.num,typeof(T));
				return true;
			case "String":
				val = (T)System.Convert.ChangeType(jsonValue.str,typeof(T));
				return true;
			case "Single":
				val = (T)System.Convert.ChangeType(jsonValue.f,typeof(T));
				return true;
			default:
				val = default(T);
				return false;
			}
		}
		else {
			val = default(T);
			return false;
		}
	}

	public bool TryGetValue(int idx, out Value val) {
		if(idx < count) {
			val = pairs[idx].value;
			return true;
		}
		val = null;
		return false;
	}
	
	public bool TryGetValue(string key, out Value val) {
		foreach(Pair pair in pairs) {
			if(key.Equals(pair.name)) {
				val = pair.value;
				return true;
			}
		}
		val = null;
		return false;
	}

	public IEnumerator<Pair> GetEnumerator() {
		foreach(Pair pair in pairs) {
			yield return pair;
		}
	}
	
	public override string ToString() {
		if(pairs!=null && pairs.Count>0) {
			Pair[] json = new Pair[pairs.Count];
			int count = 0;
			foreach(Pair pair in pairs) {
				if(pair.value != null && pair.value.visible) {
					json[count++] = pair;
				}
			}
			if(count != 0) {
				StringBuilder builder = new StringBuilder();
				if(isArray) {
					builder.Append('[');
					builder.Append(json[0].value);
					if(count != 1) {
						for(int i = 1; i < count; ++i) {
							builder.Append(',');
							builder.Append(json[i].value);
						}
					}
					builder.Append(']');
				} else {
					builder.Append('{');
					int idx = 0;
					builder.AppendFormat("\"{0}\":{1}", json[0].name != null ? json[0].name : idx++.ToString(), json[0].value);
					if(count != 1) {
						for(int i = 1; i < count; ++i) {
							builder.AppendFormat(",\"{0}\":{1}", json[i].name != null ? json[i].name : idx++.ToString(), json[i].value);
						}
					}
					builder.Append('}');
				}
				return builder.ToString();
			}
		}
		if (isAry)
			return "[]";
		else
			return "{}";
	}
	
	
	public void Add (string name, object value) {
		pairs.Add(new Pair(name, value));
	}
	public void Add (string name, string value) {
		pairs.Add(new Pair(name, value));
	}
	public void Add (string name, bool value) {
		pairs.Add(new Pair(name, value));
	}
	public void Add (Pair pair) {
		pairs.Add(pair);
	}	
	public void Add (object value) {
		pairs.Add(new Pair(string.Empty, value));
	}
	
	public void Remove (string name) {
		if (isAry) {
			throw new System.InvalidOperationException("is array");
		}
		else {
			int idx;
			while (TryGetIndex(name, out idx)) {
				pairs.RemoveAt(idx);
			}
		}
	}
	public void RemoveAt (int index) {
		pairs.RemoveAt(index);
	}	
	
	public bool TryGetIndex (string name, out int index) {
		if (isAry) {
			index = -1;
			return false;
		}
		int count = pairs.Count;
		for(int i = 0; i < count; ++i) {
			if (pairs[i].name == name) {
				index = i;
				return true;
			}
		}
		index = -1;
		return false;
	}
	public object[] ToArray () {
		if (isAry) {
			int count = pairs.Count;
			JSON.Value[] values = new Value[count];
			int i=0;
			foreach (JSON.Pair pair in pairs) {
				values[i] = new Value(pair.value);
				i++;
			}
			return values;
		}
		else
			return pairs.ToArray();
	}
	public JSON.Value GetByRoute (string route) {
		return GetByRoute(this, route);
	}		
	JSON.Value GetByRoute (JSON json, string route) {
		string[] paths = route.Split('.');
		string key = paths[0];
		#if UNITY_EDITOR			
		if (string.IsNullOrEmpty(key)) {
			throw new System.Exception();
		}
		#endif
		int idx;
		JSON.Value result;
		try {
			if (int.TryParse(key, out idx))
				result = json[idx];
			else
				result = json[key];
		}
		catch {
			throw new System.IndexOutOfRangeException(route);
		}
		
		if (result==null) {
			return null;
		}
		if (paths.Length==1) {
			return result;
		}
		else {
			return GetByRoute(result.json, route.Remove(0,key.Length+1));
		}
	}
	public void Merge (JSON other) {
		int selfCount = this.count;
		int otherCount = other.count;
		int count = selfCount + otherCount;
		JSON.Pair[] pairs = new JSON.Pair[count];
		int i=0;
		foreach (JSON.Pair val in this) {
			pairs[i] = new JSON.Pair(val.name, val.value);
			i++;
		}
		foreach (JSON.Pair val in other) {
			pairs[i] = new JSON.Pair(val.name, val.value);
			i++;
		}
	}	
	public static JSON Parse<T>(T[] array) {
		int count = array.Length;
		JSON.Value[] values = new JSON.Value[count];
		for (int i=0; i<count; i++) {
			values[i] = new JSON.Value(array[i]);
		}
		return new JSON(values);
	}
	
	public static JSON Parse<T>(Dictionary<string,T> dictionary) {
		JSON.Pair[] pairs = new JSON.Pair[dictionary.Count];
		int i=0;
		foreach (KeyValuePair<string, T> pair in dictionary) {
			pairs[i] = new JSON.Pair(pair.Key, pair.Value);
			i++;
		}
		return new JSON(pairs);
	}		
	public static JSON Parse<T>(Dictionary<string,T[]> dictionary) {
		JSON.Pair[] pairs = new JSON.Pair[dictionary.Count];
		int i=0;
		foreach (KeyValuePair<string, T[]> pair in dictionary) {
			pairs[i] = new JSON.Pair(pair.Key, JSON.Parse<T>(pair.Value));
			i++;
		}
		return new JSON(pairs);
	}	
}