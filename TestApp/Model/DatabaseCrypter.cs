using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace TestApp.Model
{
    internal class DatabaseCrypter
    {
        private static DatabaseCrypter instance;
        private SymmetricAlgorithm rmCrypto;
        private byte[] salt;
        private byte[] key;
        private const int SaltCount = 64;
        public string PathToSalt { get; init; } = "salt.json";
        private string PathToTempFile { get; init; }
        private string PathToFile { get; init; }
        public static DatabaseCrypter GetInstance(string pathToFile, string pathToTempFile)
        {
            if (instance == null)
            {
                instance = new DatabaseCrypter(pathToFile, pathToTempFile);
            }
            return instance;
        }

        public void Encrypt()
        {
            var content = File.ReadAllText(PathToTempFile);
            var file = File.Open(PathToFile, FileMode.Create);

            rmCrypto.Key = key;
            

            var encryptor = rmCrypto.CreateEncryptor();
            var crypt = new CryptoStream(file, encryptor, CryptoStreamMode.Write);

            using (var sw = new StreamWriter(crypt, Encoding.Unicode))
            {
                sw.Write(content);
                sw.Flush();
            }
            file.Close();
        }

        public void Decrypt()
        {
            var file = File.Open(PathToFile, FileMode.Open);

            rmCrypto.Key = key;

            var decryptor = rmCrypto.CreateDecryptor();
            var crypt = new CryptoStream(file, decryptor, CryptoStreamMode.Read);

            using (var sr = new StreamReader(crypt, Encoding.Unicode))
            {
                var plaintext = sr.ReadToEnd();
                File.WriteAllText(PathToTempFile, plaintext);
            }
            file.Close();
        }



        public void GenerateKey(string passphrase)
        {
            salt = new byte[SaltCount];
            new RNGCryptoServiceProvider().GetBytes(salt);
            SaveSalt();

            var passphraseBytes = Encoding.Unicode.GetBytes(passphrase);
            var bytes = passphraseBytes.Concat(salt).ToArray();
            HashAlgorithm hash = MD5.Create();
            key = hash.ComputeHash(bytes);
        }

        public void CalculateKey(string passphrase)
        {
            var passphraseBytes = Encoding.Unicode.GetBytes(passphrase);
            salt = ReadSalt();
            var bytes = passphraseBytes.Concat(salt).ToArray();
            HashAlgorithm hash = MD5.Create();
            key = hash.ComputeHash(bytes);
        }

        private DatabaseCrypter(string pathToFile, string pathToTempFile)
        {
            rmCrypto = new AesManaged();
            rmCrypto.Mode = CipherMode.CBC;
            rmCrypto.BlockSize = 128;
            rmCrypto.IV = new byte[]{ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
            this.PathToFile = pathToFile;
            this.PathToTempFile = pathToTempFile;
        }

        private void SaveSalt()
        {
            var jsonString = JsonSerializer.Serialize(salt);
            File.WriteAllText(PathToSalt, jsonString);
        }

        private byte[] ReadSalt()
        {
            if (File.Exists(PathToSalt))
            {
                var jsonString = File.ReadAllText(PathToSalt);
                return JsonSerializer.Deserialize<byte[]>(jsonString);
            }
            else
            {
                throw new System.IO.IOException("Не удалось найти файл с \"солью\".\nБез него доступ к БД будет навсегда утерян.");
            }
        }
    }
}
