using UnityEngine;
using System;
using System.Collections;
using System.Text;
using System.Security.Cryptography;

public class AES {

	/// key length 256||128, iv length 256||128, 
	public static string Encrypt (string source, string key, string iv) {
		byte[] sourceBytes = Encoding.UTF8.GetBytes(source);
		var aes = new RijndaelManaged();
		aes.Key = Encoding.UTF8.GetBytes(key);
		aes.IV = Encoding.UTF8.GetBytes(iv);
		aes.Mode = CipherMode.CBC;
		aes.Padding = PaddingMode.PKCS7;
		
		ICryptoTransform transform = aes.CreateEncryptor();
		return Convert.ToBase64String(transform.TransformFinalBlock(sourceBytes, 0, sourceBytes.Length));
	}
	
	public static string Decrypt (string encryptData, string key, string iv) {
		try {
			var encryptBytes = Convert.FromBase64String(encryptData);
			var aes = new RijndaelManaged();
			aes.Key = Encoding.UTF8.GetBytes(key);
			aes.IV = Encoding.UTF8.GetBytes(iv);
			aes.Mode = CipherMode.CBC;
			aes.Padding = PaddingMode.PKCS7;
			ICryptoTransform transform = aes.CreateDecryptor();
			return Encoding.UTF8.GetString(transform.TransformFinalBlock(encryptBytes, 0, encryptBytes.Length));
		}
		catch {
			return string.Empty;
		}
	}
}
