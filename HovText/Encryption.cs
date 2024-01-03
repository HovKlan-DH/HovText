/*
##################################################################################################
ENCRYPTION (CLASS)
------------------

This class provides encryption and decryption methods 
for the HovText application.

##################################################################################################
*/

using System;
using System.IO;
using System.Security.Cryptography;

namespace HovText
{
    public class Encryption
    {

        // ###########################################################################################

        public static void GenerateKeyAndIV(out byte[] key, out byte[] iv)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.GenerateKey();
                aesAlg.GenerateIV();
                key = aesAlg.Key;
                iv = aesAlg.IV;
            }
        }


        // ###########################################################################################

        public static byte[] EncryptStringToBytes_Aes(byte[] plainData, byte[] Key, byte[] IV)
        {
            if (plainData == null || plainData.Length <= 0)
                throw new ArgumentNullException("plainData");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(plainData, 0, plainData.Length);
                        csEncrypt.FlushFinalBlock();
                        return msEncrypt.ToArray();
                    }
                }
            }
        }


        // ###########################################################################################

        public static byte[] DecryptStringFromBytes_Aes(byte[] cipherData, byte[] Key, byte[] IV)
        {
            try
            {
                if (cipherData == null || cipherData.Length <= 0)
                    throw new ArgumentNullException("cipherData");
                if (Key == null || Key.Length <= 0)
                    throw new ArgumentNullException("Key");
                if (IV == null || IV.Length <= 0)
                    throw new ArgumentNullException("IV");

                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Key;
                    aesAlg.IV = IV;

                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    using (MemoryStream msDecrypt = new MemoryStream(cipherData))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (MemoryStream originalMemoryStream = new MemoryStream())
                            {
                                int bytesRead;
                                byte[] buffer = new byte[1024];
                                while ((bytesRead = csDecrypt.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    originalMemoryStream.Write(buffer, 0, bytesRead);
                                }
                                return originalMemoryStream.ToArray();
                            }
                        }
                    }
                }
            }
            catch (CryptographicException ex)
            {
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        // ###########################################################################################
    }
}
