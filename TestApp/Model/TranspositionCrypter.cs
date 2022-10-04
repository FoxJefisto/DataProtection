using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestApp.Model
{
    public class TranspositionCrypter
    {
        private static TranspositionCrypter instance;

        public static TranspositionCrypter GetInstance()
        {
            if (instance == null)
                instance = new TranspositionCrypter();
            return instance;
        }

        public int[] GetIntKey(string userName)
        {
            var dict = new Dictionary<char, int>();
            int i = 1;
            foreach (var ch in userName.OrderBy(x => x))
            {
                dict[ch] = i;
                i++;
            }
            var result = new int[userName.Length];
            i = 0;
            foreach (var ch in userName)
            {
                result[i] = dict[ch];
                i++;
            }
            return result;
        }

        public string Encrypt(string text, string userName)
        {
            //Генерация числового ключа из строки имени пользователя
            var key = GetIntKey(userName);
            //Для достижения кратности ключу вместо усечения ключа я решил дополнять пароль пробелами.
            //При расшифровке будет достаточно просто обрезать пробелы в конце.
            for (int i = 0; i < text.Length % key.Length; i++)
            {
                text += " ";
            }
            string result = "";
            for (int i = 0; i < text.Length; i += key.Length)
            {
                var transposition = new char[key.Length];
                for (int j = 0; j < key.Length; j++)
                {
                    transposition[key[j] - 1] = text[i + j];
                }
                for (int j = 0; j < key.Length; j++)
                {
                    result += transposition[j];
                }
            }
            return result;
        }

        public string Decrypt(string text, string userName)
        {
            var key = GetIntKey(userName);
            string result = "";
            for (int i = 0; i < text.Length; i += key.Length)
            {
                var transposition = new char[key.Length];
                for (int j = 0; j < key.Length; j++)
                {
                    transposition[j] = text[i + key[j] - 1];
                }
                for (int j = 0; j < key.Length; j++)
                {
                    result += transposition[j];
                }
            }
            //Обрезаем символы пробелов в конце пароля
            return result.Trim();
        }

        protected TranspositionCrypter() { }
    }
}
