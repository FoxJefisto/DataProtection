using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace TestApp.Model
{
    public class CaesarCrypter
    {
        private static CaesarCrypter instance;

        public static CaesarCrypter GetInstance()
        {
            if (instance == null)
                instance = new CaesarCrypter();
            return instance;
        }

        private char CipherRussian(char ch, int key)
        {
            char d = char.IsUpper(ch) ? 'А' : 'а';
            return (char)((((ch + key) - d) % 33) + d);
        }

        private char CipherEnglish(char ch, int key)
        {
            char d = char.IsUpper(ch) ? 'A' : 'a';
            return (char)((((ch + key) - d) % 26) + d);
        }

        public string Encrypt(string text, int key)
        {
            string output = string.Empty;

            foreach (char ch in text)
            {
                var matchRussian = Regex.Match(ch.ToString(), "[А-Яа-я]");
                var matchEnglish = Regex.Match(ch.ToString(), "[A-Za-z]");
                if (matchRussian.Success)
                {
                    output += CipherRussian(ch, key);
                }
                else if (matchEnglish.Success)
                {
                    output += CipherEnglish(ch, key);
                }
                else
                {
                    output += ch;
                }

            }
            return output;
        }

        public string Decrypt(string text, int key)
        {
            string output = string.Empty;
            foreach (char ch in text)
            {
                var matchRussian = Regex.Match(ch.ToString(), "[А-Яа-я]");
                var matchEnglish = Regex.Match(ch.ToString(), "[A-Za-z]");
                if (matchRussian.Success)
                {
                    output += CipherRussian(ch, 33 - key);
                }
                else if (matchEnglish.Success)
                {
                    output += CipherEnglish(ch, 26 - key);
                }
                else
                {
                    output += ch;
                }
            }
            return output;
        }

        protected CaesarCrypter() { }
    }
}
