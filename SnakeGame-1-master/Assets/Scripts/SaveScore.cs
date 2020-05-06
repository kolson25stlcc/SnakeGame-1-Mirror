using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveScore : MonoBehaviour
{

    private string dataPath;
    private string pathToHighScores;

    private readonly byte[] key = new byte[32] { 248, 55, 23, 89, 161, 152, 35, 2, 126, 213, 16, 115, 86, 217, 99, 108, 56, 218, 5, 78, 28, 73, 113, 10, 61, 56, 10, 87, 187, 162, 233, 38 };
    private readonly byte[] iv = new byte[16] { 75, 241, 14, 16, 109, 18, 14, 228, 4, 54, 6, 5, 60, 27, 16, 191 };
    private readonly string salt = ";~Qll(hf&nW8_=js$e?d";


    void Awake()
    {
        dataPath = Application.persistentDataPath;
        pathToHighScores = dataPath + "/highscores.txt";

        if (!File.Exists(pathToHighScores))
        {
            ResetEncryptedScores();
        }
    }

    private byte[] StringToByteArray(string str, char delimeter)
    {
        string[] strArr = str.Split(delimeter);
        byte[] byArr = new byte[strArr.Length];
        for (int i = 0; i < byArr.Length; i++)
        {
            byArr[i] = Convert.ToByte(strArr[i]);
        }

        return byArr;
    }

    private string GetHighScores()
    {
        StreamReader textIn =
            new StreamReader(
            new FileStream(pathToHighScores, FileMode.Open, FileAccess.Read));

        byte[] cipherText = StringToByteArray(textIn.ReadToEnd(), ',');
        textIn.Close();

        string clearText = DecryptStringFromBytes_Aes(cipherText, key, iv);
        string[] contents = clearText.Split(',');

        string highScores = contents[1] + "," + contents[2] + "," + contents[3];

        return highScores;
    }

    public bool[] SaveIfHighScore(int newScore, GameSettings.Difficulty difficulty)
    {
        bool[] highScoreAndWorked = { false, false };
        try
        {
            string[] highScores = GetHighScores().Split(',');

            if (newScore <= Convert.ToInt16(highScores[(int)difficulty]))
            {
                highScoreAndWorked = new bool[] { false, true };
                return highScoreAndWorked;
            }
                

            else
            {
                string text = salt + ",";
                for (int i = 0; i < 3; i++)
                {
                    if (i != (int)difficulty)
                        text += highScores[i] + ",";
                    else
                        text += newScore + ",";
                }
                text += salt;

                byte[] cipherText = EncryptStringToBytes_Aes(text, key, iv);

                StreamWriter textOut =
                    new StreamWriter(
                    new FileStream(pathToHighScores, FileMode.Create, FileAccess.Write));

                for (int i = 0; i < cipherText.Length - 1; i++)
                {
                    textOut.Write(cipherText[i] + ",");
                }
                textOut.Write(cipherText[cipherText.Length - 1]);

                textOut.Close();

                highScoreAndWorked = new bool[] { true, true };
                return highScoreAndWorked;
            }
        }
        catch (Exception)
        {
            highScoreAndWorked = new bool[] { false, false };
            return highScoreAndWorked;
        }

    }

    private void ResetEncryptedScores()
    {
        string[] highScores = { "0", "0", "0" };

        string text = salt + ",";
        for (int i = 0; i < 3; i++)
        {
            text += highScores[i] + ",";
        }
        text += salt;

        byte[] cipherText = EncryptStringToBytes_Aes(text, key, iv);

        StreamWriter textOut =
            new StreamWriter(
            new FileStream(pathToHighScores, FileMode.Create, FileAccess.Write));

        for (int i = 0; i < cipherText.Length - 1; i++)
        {
            textOut.Write(cipherText[i] + ",");
        }
        textOut.Write(cipherText[cipherText.Length - 1]);

        textOut.Close();
    }

    // copied from https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.aes?view=netframework-4.7.2
    public static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
    {
        // Check arguments.
        if (plainText == null || plainText.Length <= 0)
            throw new ArgumentNullException("plainText");
        if (Key == null || Key.Length <= 0)
            throw new ArgumentNullException("Key");
        if (IV == null || IV.Length <= 0)
            throw new ArgumentNullException("IV");
        byte[] encrypted;

        // Create an Aes object
        // with the specified key and IV.
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Key;
            aesAlg.IV = IV;

            // Create an encryptor to perform the stream transform.
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for encryption.
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        //Write all data to the stream.
                        swEncrypt.Write(plainText);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }
        }

        // Return the encrypted bytes from the memory stream.
        return encrypted;

    }

    // copied from https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.aes?view=netframework-4.7.2
    public static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
    {
        // Check arguments.
        if (cipherText == null || cipherText.Length <= 0)
            throw new ArgumentNullException("cipherText");
        if (Key == null || Key.Length <= 0)
            throw new ArgumentNullException("Key");
        if (IV == null || IV.Length <= 0)
            throw new ArgumentNullException("IV");

        // Declare the string used to hold
        // the decrypted text.
        string plaintext = null;

        // Create an Aes object
        // with the specified key and IV.
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Key;
            aesAlg.IV = IV;
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Padding = PaddingMode.PKCS7;

            // Create a decryptor to perform the stream transform.
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for decryption.
            using (MemoryStream msDecrypt = new MemoryStream(cipherText))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {

                        // Read the decrypted bytes from the decrypting stream
                        // and place them in a string.
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }

        }

        return plaintext;

    }

}
