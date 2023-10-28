using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System;

[System.Serializable]
public class RedsCryptographicEngine : MonoBehaviour
{
    private void UpdateHash(CryptoData cryptoData, string data)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            cryptoData.hashedData = GetByteArrayToString(sha256.ComputeHash(StringToByteArray(data)));
        }
    }

    public static byte[] StringToByteArray(string str)
    {
        return Encoding.ASCII.GetBytes(str);
    }

    public static string GetByteArrayToString(byte[] byteArray)
    {
        StringBuilder stringBuilder = new StringBuilder();

        for (int i = 0; i < byteArray.Length; i++)
        {
            stringBuilder.Append(byteArray[i].ToString("x2"));
        }
        
        return stringBuilder.ToString();
    }

    [System.Serializable]
    public class CryptoData
    {
        public string hashedData;

        public void CalculateHash(string inputData)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                hashedData = GetByteArrayToString(sha256.ComputeHash(StringToByteArray(inputData)));
            }
        }

        public void StoreHashValue(string newHash)
        {
            hashedData = newHash;
        }

        public bool CompareHash(string hashData)
        {
            return hashedData == hashData;
        }
    }

    public static string GenerateCryptoToken(string email, CryptoData password, TimeStamp timestamp)
    {
        timestamp.CheckExpired();

        if (timestamp.isExpired)
        {
            return "Timestamp expired";
        }

        using (SHA256 sha256 = SHA256.Create())
        {
            return GenerateCryptoToken(GetByteArrayToString(sha256.ComputeHash(StringToByteArray(email + password.hashedData))), timestamp);
        }
    }

    public static string GenerateCryptoToken(string hash, TimeStamp timestamp)
    {
        timestamp.CheckExpired(); 

        if (timestamp.isExpired)
        {
            return "Timestamp expired";
        }

        using (SHA256 sha256 = SHA256.Create())
        {
            return GetByteArrayToString(sha256.ComputeHash(StringToByteArray(hash + timestamp.timestamp)));
        }
    }

    public static TimeStamp GenerateTimestamp()
    {
        TimeStamp timestamp = new TimeStamp();
        //timestamp.createdDate = DatabaseManager.Now();

        timestamp.timestamp = timestamp.createdDate.ToString("yyyy MM dd HH mm ss");
        timestamp.expiryDate = timestamp.createdDate.AddSeconds(10);

        return timestamp;
    }

    [SerializeField]
    public class TimeStamp
    {
        public string timestamp;
        public System.DateTime createdDate;
        public System.DateTime expiryDate;
        public bool isExpired;

        public void CheckExpired()
        {
            //isExpired = expiryDate < DatabaseManager.Now();
        }
    }

    public static string Encrypt(string plainText, int shift = 3)
    {
        char[] characters = plainText.ToCharArray();

        for (int i = 0; i < characters.Length; i++)
        {
            if (char.IsLetter(characters[i]))
            {
                char baseChar = char.IsUpper(characters[i]) ? 'A' : 'a';
                characters[i] = (char)(((characters[i] - baseChar + shift) % 26) + baseChar);
            }
        }

        return new string(characters);
    }

    public static string Decrypt(string cipherText)
    {
        return Encrypt(cipherText, 26 - 3);
    }
}
