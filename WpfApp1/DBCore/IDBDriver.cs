using System;
using Microsoft.Data.Sqlite;


namespace WpfApp1.DBcore
{
    public interface IDBDriver
    {
        public bool IsCreate { get; }
        void Create();

        bool SignIn(string nick, string pasw);
        bool LogIn(string nick, string pasw);
        void SignOut();
    }

}