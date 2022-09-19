using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;

namespace TestApp.Model
{
    public class Database
    {
        public ObservableCollection<User> Users;
        private string path;

        public Database(string path)
        {
            Users = new ObservableCollection<User>();
            this.path = path;
            Users.CollectionChanged += UserCollectionChanged;
        }
        public void SaveDB()
        {
            var jsonString = JsonSerializer.Serialize(Users);
            File.WriteAllText(path, jsonString);
            Debug.WriteLine("Файл базы данных обновлен");
        }

        public bool LoadFromDB()
        {
            try
            {
                Users.CollectionChanged -= UserCollectionChanged;
                var jsonString = File.ReadAllText(path);
                Users = JsonSerializer.Deserialize<ObservableCollection<User>>(jsonString);
                Users.CollectionChanged += UserCollectionChanged;
                if (Users.Count != 0)
                    return true;
                else
                    return false;
            }
            catch(Exception ex)
            {
                Users.CollectionChanged += UserCollectionChanged;
                return false;
            }
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
