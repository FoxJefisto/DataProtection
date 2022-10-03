using System;
using System.Collections.Generic;
using System.Text;

namespace TestApp.Model
{
    public class Crypter
    {
        private static Crypter instance;
        private TranspositionCrypter transpositionCrypter;
        private CaesarCrypter caesarCrypter;

        public static Crypter GetInstance()
        {
            if (instance == null)
                instance = new Crypter();
            return instance;
        }

        public string Encrypt(string text, string userName)
        {
            var intermediate = transpositionCrypter.Encrypt(text, userName);
            var result = caesarCrypter.Encrypt(intermediate, 3);
            return result;
        }

        public string Decrypt(string text, string userName)
        {
            var intermediate = caesarCrypter.Decrypt(text, 3);
            var result = transpositionCrypter.Decrypt(intermediate, userName);
            return result;
        }

        protected Crypter()
        {
            transpositionCrypter = TranspositionCrypter.GetInstance();
            caesarCrypter = CaesarCrypter.GetInstance();
        }
    }
}
