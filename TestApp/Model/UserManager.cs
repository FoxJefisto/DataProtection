using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Shapes;

namespace TestApp.Model
{
    public class UserManager
    {
        private Database DB { get; set; }
        public User CurrentUser { get; set; } = null;

        private Crypter crypter;

        public string pathDatabase = "users.json";

        public bool isFirstExecute;

        public UserManager()
        {
            DB = new Database(pathDatabase);
            crypter = Crypter.GetInstance();
            isFirstExecute = !DB.IsDBExists();
        }

        /// <summary>
        /// Выполнение входа в аккаунт
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public (bool state, string message) LogIn(string login, string password)
        {
            bool state = true;
            string message = "Операция выполнена успешно.";
            login = login.ToLower();
            CurrentUser = DB.Users.FirstOrDefault(x => string.Equals(x.Login, login, StringComparison.OrdinalIgnoreCase));
            if (CurrentUser != null)
            {
                if (!CheckPassword(password))
                {
                    state = false;
                    CurrentUser = null;
                    message = "Был введен неверный пароль.";
                }
                else if (CurrentUser.IsBanned)
                {
                    state = false;
                    CurrentUser = null;
                    message = "Эта учетная запись заблокирована администратором";
                }
            }
            else
            {
                state = false;
                message = "Пользователь с данным логином не найден.";
            }
            return (state, message);
        }

        /// <summary>
        /// Выход из аккаунта
        /// </summary>
        public void LogOut()
        {
            CurrentUser = null;
        }

        /// <summary>
        /// Регистрация нового пользователя
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <param name="rootAccess"></param>
        /// <param name="ignorePassword"></param>
        /// <returns></returns>
        public (bool state, string message) SignUp(string login, string password, bool rootAccess = false, bool ignorePassword = false)
        {
            bool state = true;
            string message = "Операция выполнена успешно.";
            login = login.ToLower();
            if (login == "")
            {
                return (false, "Логин не может быть пустым.");
            }
            var matchPassword = Regex.Match(password, @"[0-9]+[\.,:\-""?!;\(\)]+[0-9]+([\.,:\-""?!;\(\)]+[0-9]+)*");
            if (matchPassword.Success || ignorePassword)
            {
                if (!DB.Users.Any(x => string.Equals(x.Login, login, StringComparison.OrdinalIgnoreCase)))
                {
                    if (rootAccess)
                    {
                        isFirstExecute = false;
                    }
                    //Шифрование пароля
                    var cryptedPassword = crypter.Encrypt(password, login);
                    var user = new User(login, cryptedPassword, rootAccess);
                    DB.Users.Add(user);
                }
                else
                {
                    state = false;
                    message = "Пользователь с таким логином уже существует.";
                }
            }
            else
            {
                state = false;
                message = "Пароль не соответствует требованиям.";
            }
            return (state, message);
        }

        public (bool state, string message) LoadDatabase(string passphrase)
        {
            try
            {
                if (isFirstExecute)
                {
                    DB.GenerateKey(passphrase);
                }
                else
                {
                    DB.CalculateKey(passphrase);
                }
                return DB.LoadFromDB() switch
                {
                    true => (true, "Подключение к БД произошло успешно"),
                    false => (false, "Не удалось подключиться к БД")
                };
            }
            catch(Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Удаление пользователя из файла
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public (bool state, string message) RemoveUser(string login)
        {
            bool state = true;
            string message = "Операция выполнена успешно.";
            User user = DB.Users.Where(x => string.Equals(x.Login, login, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (user != null)
            {
                if (user.RootAccess != true)
                {
                    DB.Users.Remove(user);
                }
                else
                {
                    state = false;
                    message = "Невозможно удалить администратора.";
                }
            }
            else
            {
                state = false;
                message = "Пользователь с таким именем не зарегистрирован.";
            }
            return (state, message);
        }

        /// <summary>
        /// Изменение пароля для текущего пользователя
        /// </summary>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <param name="verifyPassword"></param>
        /// <returns></returns>
        public (bool state, string message) ChangeCurrentUserPassword(string oldPassword, string newPassword, string verifyPassword)
        {
            bool state = true;
            string message = "Операция выполнена успешно.";
            if (CheckPassword(oldPassword))
            {
                var matchPassword = Regex.Match(newPassword, @"[0-9]+[\.,:\-""?!;\(\)]+[0-9]+([\.,:\-""?!;\(\)]+[0-9]+)*");
                if (!CurrentUser.HasConstraint || matchPassword.Success)
                {
                    if (newPassword == verifyPassword)
                    {
                        //Шифрование нового пароля
                        CurrentUser.Password = crypter.Encrypt(newPassword, CurrentUser.Login);
                        DB.SaveDB();
                    }
                    else
                    {
                        state = false;
                        message = "Подтверждающий пароль введен неверно.";
                    }
                }
                else
                {
                    state = false;
                    message = "Новый пароль не соответствует требованиям.";
                }
            }
            else
            {
                state = false;
                message = "Старый пароль введен неверно.";
            }
            return (state, message);
        }

        /// <summary>
        /// Получить всех пользователей из файла
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public ObservableCollection<User> GetAllUsers()
        {
            if (!CurrentUser.RootAccess)
                throw new Exception("У данного пользователя нет таких прав.");
            return DB.Users;
        }

        /// <summary>
        /// Сравнение пароля с расщифрованным паролем из файла
        /// </summary>
        /// <param name="userPassword"></param>
        /// <returns></returns>
        public bool CheckPassword(string userPassword)
        {
            var password = crypter.Decrypt(CurrentUser.Password, CurrentUser.Login);
            return userPassword == password;
        }

        /// <summary>
        /// Сохранение изменений в файл
        /// </summary>
        public void SaveChanges()
        {
            DB.SaveDB();
        }

        public void CloseDB()
        {
            DB.Close();
        }

        public void DeleteDB()
        {
            DB.Delete();
        }
    }
}
