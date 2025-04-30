﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Security.Cryptography;// 암호화
using System.Collections;
using System.Diagnostics;
using System.Text.RegularExpressions;//정규표현식

//#####
// BASE64 
//

namespace ConsoleApplication_MyLibs
{

    public class MyClass_Security // SIMPLE
    {
        public MyClass_Security()
        {

        }
        ~MyClass_Security()
        {

        }
        public static string encodeBase64(string value) 
        {
            byte[] strByte = Encoding.UTF8.GetBytes(value); // Base64로 문자열 인코딩
            return Convert.ToBase64String(strByte); // 인코딩 결과 문자열 변환
        }
        public static string decodeBase64(string value)
        {
            byte[] byteDecode = Convert.FromBase64String(value); // Base64로 문자열 디코딩
            //return byteDecode.ToString();
            return Encoding.Default.GetString(byteDecode); // 문자열로 디코딩된 결과 변환
        }
        public static string encodeSHA256(string value) //one-way hash function
        {
            byte[] strByte = Encoding.UTF8.GetBytes(value); // string -> byte 변환
            SHA256 hSHA256 = SHA256Managed.Create();
            byte[] resHash = hSHA256.ComputeHash(strByte); // SHA256 변환 암호화
            return BitConverter.ToString(resHash).Replace("-", "").ToLower(); // Hex 문자열
        }
        public static string CaesarCipherEncrypt(string value, int shift)
        {
            bool isLowLetter = false;

            char[] buffer = value.ToCharArray();
            for (int i = 0; i < buffer.Length; i++)
            {
                // Letter.
                char letter = buffer[i];
                isLowLetter = char.IsLower(letter);
                if (!char.IsLetter(letter)) // encrypt only letter 
                {
                    buffer[i] = letter;
                    continue;
                }
                // Add shift to all.
                letter = (char)(letter + shift);
                // Subtract 26 on overflow.
                // Add 26 on underflow.
                if (isLowLetter)
                {
                    if (letter > 'z')
                    {
                        letter = (char)(letter - 26);
                    }
                    else if (letter < 'a')
                    {
                        letter = (char)(letter + 26);
                    }
                }
                else
                {
                    if (letter > 'Z')
                    {
                        letter = (char)(letter - 26);
                    }
                    else if (letter < 'A')
                    {
                        letter = (char)(letter + 26);
                    }
                }
                // Store.
                buffer[i] = letter;
            }
            return new string(buffer);
        }
        public static string Decrypt(string textToDecrypt, string key)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            rijndaelCipher.Mode = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.PKCS7;
            rijndaelCipher.KeySize = 128;
            rijndaelCipher.BlockSize = 128;
            byte[] encryptedData = Convert.FromBase64String(textToDecrypt);
            byte[] pwdBytes = Encoding.UTF8.GetBytes(key);
            byte[] keyBytes = new byte[16];
            int len = pwdBytes.Length;
            if (len > keyBytes.Length)
            {
                len = keyBytes.Length;
            }
            Array.Copy(pwdBytes, keyBytes, len);
            rijndaelCipher.Key = keyBytes;
            rijndaelCipher.IV = keyBytes;
            byte[] plainText = rijndaelCipher.CreateDecryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length);
            return Encoding.UTF8.GetString(plainText);
        }
        public static string Encrypt(string textToEncrypt, string key)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            rijndaelCipher.Mode = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.PKCS7;
            rijndaelCipher.KeySize = 128;
            rijndaelCipher.BlockSize = 128;
            byte[] pwdBytes = Encoding.UTF8.GetBytes(key);
            byte[] keyBytes = new byte[16];
            int len = pwdBytes.Length;
            if (len > keyBytes.Length)
            {
                len = keyBytes.Length;
            }
            Array.Copy(pwdBytes, keyBytes, len);
            rijndaelCipher.Key = keyBytes;
            rijndaelCipher.IV = keyBytes;
            ICryptoTransform transform = rijndaelCipher.CreateEncryptor();
            byte[] plainText = Encoding.UTF8.GetBytes(textToEncrypt);
            return Convert.ToBase64String(transform.TransformFinalBlock(plainText, 0, plainText.Length));
        }
    }

}
