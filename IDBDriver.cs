using System;
using Microsoft.Data.Sqlite;



public interface class IDBDriver
{
    public bool IsCreate { get; }
    void Create();

    bool SignIn(string nick, string pasw);
    bool SignOut();
}
