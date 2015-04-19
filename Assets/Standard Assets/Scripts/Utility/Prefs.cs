using UnityEngine;
using System.Collections;
using System.Text;

public class Prefs {
	const char SEPARATOR = '#';
	string prefix;
	StringBuilder buffer;
	char[] separator;
	
	public Prefs (string prefix) {
		this.prefix = prefix;
		buffer = new StringBuilder();
		separator = new char[1]{SEPARATOR};
	}
	string KeyPlus (string key) {
		return string.Concat(prefix, "@", key);
	}
	public bool Haskey (string key) {
		return PlayerPrefs.HasKey(KeyPlus(key));
	}
	public void DeleteKey (string key) {
		PlayerPrefs.DeleteKey(KeyPlus(key));
	}
	public void SetString (string key, string value) {
		PlayerPrefs.SetString(KeyPlus(key), value);
	}
	public string GetString (string key) {
		return GetString(key, string.Empty);
	}
	/// default is empty
	public string GetString (string key, string defaultValue) {
		return PlayerPrefs.GetString(KeyPlus(key), defaultValue);
	}
	public void SetInt (string key, int value) {
		PlayerPrefs.SetInt(KeyPlus(key), value);
	}
	/// default is 0
	public int GetInt (string key) {
		return GetInt(key, 0);
	}
	public int GetInt (string key, int defaultValue) {
		return PlayerPrefs.GetInt(KeyPlus(key), defaultValue);
	}
	/// note the separator = '#'
	public void SetStrAry (string key, string[] value) {
		buffer.Remove(0, buffer.Length);
		foreach (string val in value) {
			buffer.Append(val+SEPARATOR);
		}
		SetString(key, buffer.ToString());
	}
	public string[] GetStrArray (string key) {
		string str = GetString(key, string.Empty);
		if (string.IsNullOrEmpty(str)) {
			return null;
		}
		else {
			return str.Split(SEPARATOR);
		}
	}
	/// note the separator = '#'
	public void SetIntArray (string key, int[] value) {
		buffer.Remove(0, buffer.Length);
		foreach (int val in value) {
			buffer.Append(val.ToString()+SEPARATOR);
		}
		SetString(key, buffer.ToString());
	}
	public int[] GetIntArray (string key) {
		string str = GetString(key, string.Empty);
		if (string.IsNullOrEmpty(str)) {
			return null;
		}
		else {
			string[] values = str.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
			int length = values.Length;
			int[] ary = new int[length];
			for (int i=0; i<length; i++) {
				ary[i] = int.Parse(values[i]);
			}
			return ary;
		}
	}
	public void Save () {
		PlayerPrefs.Save();
	}
}
