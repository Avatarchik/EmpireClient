/*///////////////////////////////////////////////
{                                              }
{     Обертка над сокетным компонентом         }
{       Copyright (c) 2016 UAshota             }
{                                              }
{  Rev A  2016.12.15                           }
{  Rev B  2017.05.15                           }
{                                              }
/*///////////////////////////////////////////////
using System;
using System.IO;
using System.Collections;

// Пакет очереди сокетных сообщений
public class SSHConnectionCommandBuffer
{
    public MemoryStream Buffer;
    public int Command;
}

// Простой клиент доступа
public static class SSHConnection
{
    // Сокет для взаимодействия
    public static SRTcpClient Socket;
    // Признак тестового сервера
    public static bool TestServer;

    // Порт сервера
    private const int FSocketPortDefault = 25599;
    // Порт тестового сервера
    private const int FSocketPortTest = 25600;
    // Адрес сервера
    private const string FSocketServer = "galaxyhopes.ru";
    // Очередь полученных сообщений 
    private static Queue FQueue;
    // Объект для реализации блокировки очереди сообщений
    private static object FLocker;

    // Сообщения принимаются асинхронно в очередь
    static SSHConnection()
    {
        FLocker = new object();
        FQueue = new Queue(100, 10);
        Socket = new SRTcpClient
        {
            OnReceive = OnReceive
        };
    }

    // Прием сообщения с сервера
    private static void OnReceive(byte[] ABuffer, int ABufferSize)
    {
        // Прочитаем код команды
        int Command = BitConverter.ToInt32(ABuffer, 0);
        // Создадим сообщение
        SSHConnectionCommandBuffer LData = new SSHConnectionCommandBuffer
        {
            Command = Command,
            Buffer = new MemoryStream()
        };
        LData.Buffer.Write(ABuffer, 0, ABufferSize);
        LData.Buffer.Position = sizeof(int);
        // Добавим сообщение в очередь
        lock (FLocker)
        {
            FQueue.Enqueue(LData);
        }
    }

    // Выдача сообщения клиенту
    public static bool Dequeue(out SSHConnectionCommandBuffer LData)
    {
        lock (FLocker)
        {
            if (FQueue.Count > 0)
            {
                LData = (SSHConnectionCommandBuffer)FQueue.Dequeue();
                return true;
            }
            else
            {
                LData = null;
                return false;
            }
        }
    }

    // Попытка соединиться с сервером
    public static void ServerConnect()
    {
        if (!TestServer)
            Socket.Connect(FSocketServer, FSocketPortDefault);
        else
            Socket.Connect(FSocketServer, FSocketPortTest);
    }
}