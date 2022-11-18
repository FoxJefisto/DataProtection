using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace TestApp.Model
{
    public class Database
    {
        public ObservableCollection<User> Users;
        private DatabaseCrypter dbCrypter;
        private string pathToFile;
        private string pathToTempFile;

        public Database(string path)
        {
            pathToFile = path;
            pathToTempFile = "temp.json";
            Users = new ObservableCollection<User>();
            dbCrypter = DatabaseCrypter.GetInstance(pathToFile, pathToTempFile);
        }

        public void CalculateKey(string passphrase)
        {
            dbCrypter.CalculateKey(passphrase);
        }

        public bool IsDBExists()
        {
            return File.Exists(pathToFile);
        }

        public void SaveDB()
        {
            var jsonString = JsonSerializer.Serialize(Users);
            File.WriteAllText(pathToTempFile, jsonString);
            Debug.WriteLine("Файл базы данных обновлен");
        }

        public bool LoadFromDB()
        {
            try
            {
                Users.CollectionChanged -= UserCollectionChanged;
                dbCrypter.Decrypt();
                var jsonString = File.ReadAllText(pathToTempFile);
                Users = JsonSerializer.Deserialize<ObservableCollection<User>>(jsonString, new JsonSerializerOptions());
                Users.CollectionChanged += UserCollectionChanged;
                if (Users.Any(x => x.Login == "admin"))
                    return true;
                else
                    return false;
            }
            catch
            {
                Users.CollectionChanged += UserCollectionChanged;
                return false;
            }
        }

        public void Close()
        {
            if (File.Exists(pathToTempFile))
            {
                dbCrypter.Encrypt();
                File.Delete(pathToTempFile);
            }
        }

        public void Delete()
        {
            File.Delete(pathToFile);
        }

        private void UserCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add: // если добавление
                    if (e.NewItems?[0] is User newUser)
                        Debug.WriteLine($"В БД добавлен новый пользователь: {newUser.Login}");
                    break;
            }
            SaveDB();
        }
    }
}
