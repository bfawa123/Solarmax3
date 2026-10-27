using System;
using System.Security.Cryptography;
using System.Text;

namespace Solarmax
{
	public class EncryptTools
	{
		public static string Encrypt(string str, string key)
		{
			key = EncryptTools.FmtPassword(key);
			byte[] bytes = Encoding.UTF8.GetBytes(key);
			byte[] bytes2 = Encoding.UTF8.GetBytes(str);
			ICryptoTransform cryptoTransform = new RijndaelManaged
			{
				Key = bytes,
				Mode = CipherMode.ECB,
				Padding = PaddingMode.PKCS7
			}.CreateEncryptor();
			byte[] array = cryptoTransform.TransformFinalBlock(bytes2, 0, bytes2.Length);
			return Convert.ToBase64String(array, 0, array.Length);
		}

		public static byte[] Encrypt(byte[] array, string key)
		{
			key = EncryptTools.FmtPassword(key);
			byte[] bytes = Encoding.UTF8.GetBytes(key);
			ICryptoTransform cryptoTransform = new RijndaelManaged
			{
				Key = bytes,
				Mode = CipherMode.ECB,
				Padding = PaddingMode.PKCS7
			}.CreateEncryptor();
			return cryptoTransform.TransformFinalBlock(array, 0, array.Length);
		}

		public static string Decrypt(string str, string key)
		{
			key = EncryptTools.FmtPassword(key);
			byte[] bytes = Encoding.UTF8.GetBytes(key);
			byte[] array = Convert.FromBase64String(str);
			ICryptoTransform cryptoTransform = new RijndaelManaged
			{
				Key = bytes,
				Mode = CipherMode.ECB,
				Padding = PaddingMode.PKCS7
			}.CreateDecryptor();
			byte[] bytes2 = cryptoTransform.TransformFinalBlock(array, 0, array.Length);
			return Encoding.UTF8.GetString(bytes2);
		}

		public static byte[] Decrypt(byte[] array, string key)
		{
			key = EncryptTools.FmtPassword(key);
			byte[] bytes = Encoding.UTF8.GetBytes(key);
			ICryptoTransform cryptoTransform = new RijndaelManaged
			{
				Key = bytes,
				Mode = CipherMode.ECB,
				Padding = PaddingMode.PKCS7
			}.CreateDecryptor();
			return cryptoTransform.TransformFinalBlock(array, 0, array.Length);
		}

		public static string FmtPassword(string s)
		{
			string text = s ?? string.Empty;
			if (text.Length < EncryptTools.AESKeyLength)
			{
				text += new string(EncryptTools.AESFillChar, EncryptTools.AESKeyLength - text.Length);
			}
			else if (text.Length > EncryptTools.AESKeyLength)
			{
				text = text.Substring(0, EncryptTools.AESKeyLength);
			}
			return text;
		}

		private static int AESKeyLength = 32;

		private static char AESFillChar = 'Y';

		public static string DefaultPassword = "SolarMax3";
	}
}
