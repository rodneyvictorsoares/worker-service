using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WorkerCambioRestricao.APPLICATION.Services
{
    public static class CriptoServiceHelper
    {
        private static readonly string chaveCriptografada = "kux_f`o|,<k(zjbf#~((d_/~c23~e?<j";

        public static string Criptografar(string descriptografada)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(chaveCriptografada);
                aes.IV = new byte[16];

                ICryptoTransform encript = aes.CreateEncryptor(aes.Key,aes.IV);

                using (MemoryStream msEncript = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncript, encript, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(descriptografada);
                        }
                    }
                    return Convert.ToBase64String(msEncript.ToArray());
                }
            }
        }

        public static string Descriptografar(string criptografada)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(chaveCriptografada);
                aes.IV = new byte[16];

                ICryptoTransform descrypt = aes.CreateDecryptor(aes.Key,aes.IV);

                using (MemoryStream mDescrypt = new MemoryStream(Convert.FromBase64String(criptografada)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(mDescrypt, descrypt, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
