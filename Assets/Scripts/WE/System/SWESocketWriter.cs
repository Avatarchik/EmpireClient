/*///////////////////////////////////////////////
{                                              }
{Модуль отправки запросов авторизации на сервер}
{       Copyright (c) 2016 UAshota             }
{                                              }
{                                              }
/*///////////////////////////////////////////////
using System.IO;

public class SWESocketWriter : SSHSocketWriter
{
    // Вход в игру по нику
    private const int C_LOGIN_AUTH = 0x0002;

    // Запрос авторизации
    public void Login(string ALogin, string APassword)
    {
        BinaryWriter LWriter = DoOpen(C_LOGIN_AUTH);
        DoWriteString(ALogin, LWriter);
        DoWriteString(APassword, LWriter);
        DoClose(LWriter);
    }
}