using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TestApp.Model
{
    internal class DatabaseCrypter
    {
        private static DatabaseCrypter instance;
        private SymmetricAlgorithm rmCrypto;
        private byte[] key;
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

        public void CalculateKey(string passphrase)
        {
            Md4 hash = Md4.Create();
            key = hash.ComputeHash(passphrase);
        }

        private DatabaseCrypter(string pathToFile, string pathToTempFile)
        {
            rmCrypto = new AesManaged();
            rmCrypto.Mode = CipherMode.CFB;
            rmCrypto.BlockSize = 128;
            rmCrypto.IV = new byte[]{ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
            this.PathToFile = pathToFile;
            this.PathToTempFile = pathToTempFile;
        }
    }
}
