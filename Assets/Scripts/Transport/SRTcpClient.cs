/*///////////////////////////////////////////////
{                                              }
{     Модуль асинхронного сокетного клиента    }
{       Copyright (c) 2016 UAshota             }
{                                              }
{  Rev A  2016.12.17                           }
{  Rev B  2017.05.15                           }
{                                              }
/*///////////////////////////////////////////////
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;

public class SRTcpClient
{
    // Делегат каллбака о совершении коннекта
    public delegate void OnConnectedDelegate();
    // Делегат каллбака о приеме данных
    public delegate void OnReceiveDelegate(byte[] ABuffer, int ABufferSize);
    // Делегат каллбака об ошибке
    public delegate void OnErrorDelegate(string AMessage);
    // Каллбак соединения
    public OnConnectedDelegate OnConnected;
    // Каллбак приема данных
    public OnReceiveDelegate OnReceive;
    // Каллбак ошибки
    public OnErrorDelegate OnError;
    // Клиент будем одноразовым, сокет используем тут-же
    private Socket FSocket;

    // Структура используется для получения буфера с сокета
    private class StateObject
    {
        // Указатель на сокет
        public Socket Socket = null;
        // Максимальный буфер приема
        public const int RecvSize = 512000;
        // Буффер приема
        public byte[] RecvBuffer = new byte[RecvSize];
        // Признак ожидания команды
        public Boolean CommandWant = true;
        // Размер командного сообщения
        public int CommandSize = sizeof(int);
        // Ожидаемый размер пакета
        public int WantSize = sizeof(int);
        // Размер принятого пакета
        public int TotalSize = 0;
    }

    // Соединение с указанным сервером и запуск потока ожидания сообщений
    public void Connect(String AHostName, int APort)
    {
        try
        {
            // Координаты сервера
            IPAddress LIPAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint LRemoteEP = new IPEndPoint(LIPAddress, APort);
            // Новый сокет
            Socket LSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                NoDelay = true
            };
            // Попытка асинхронного соединения
            LSocket.BeginConnect(LRemoteEP,
                new AsyncCallback(ConnectCallback), LSocket);
        }
        catch (Exception e)
        {
            string LError = "SRTcpClient StartClient " + e.ToString();
            if (OnError != null)
                OnError(LError);
            Debug.LogError(LError);
        }
    }

    // Принудительное отсоединение
    public void Disconnect()
    {
        if (FSocket != null)
            FSocket.Disconnect(false);
    }

    // Каллбак успешного соединения с сервером
    private void ConnectCallback(IAsyncResult AAsyncResult)
    {
        try
        {
            // Получение сокета от каллбака
            FSocket = (Socket)AAsyncResult.AsyncState;
            // Завершить асинхронный вызов
            FSocket.EndConnect(AAsyncResult);
            // Вызов каллбака соединения
            if (OnConnected != null)
                OnConnected();
            // Запустить монитор входящих пакетов
            Receive();
        }
        catch (Exception e)
        {
            string LError = "SRTcpClient ConnectCallback " + e.Message;
            if (OnError != null)
                OnError(LError);
            Debug.LogError(LError);
        }
    }

    // Асинхронная отправка пакета данных
    public void Send(byte[] ABytes)
    {
        try
        {
            // Запуск асинхронной отправки данных
            FSocket.BeginSend(ABytes, 0, ABytes.Length, 0,
                new AsyncCallback(SendCallback), FSocket);
        }
        catch (Exception e)
        {
            string LError = "SRTcpClient Send " + e.ToString();
            if (OnError != null)
                OnError(LError);
            Debug.LogError(LError);
        }
    }

    // Каллбак успешной отправки данных
    private void SendCallback(IAsyncResult AAsyncResult)
    {
        try
        {
            // Получение сокета от каллбака
            Socket LSocket = (Socket)AAsyncResult.AsyncState;
            // Завершить асинхронный вызов
            LSocket.EndSend(AAsyncResult);
        }
        catch (Exception e)
        {
            string LError = "SRTcpClient SendCallback " + e.ToString();
            if (OnError != null)
                OnError(LError);
            Debug.LogError(LError);
        }
    }

    // Асинхронный прием пакета данных
    private void Receive()
    {
        try
        {
            // Создание объекта для движения в потоке
            StateObject LState = new StateObject
            {
                Socket = FSocket
            };
            // Запуск асинхронного приема данных
            FSocket.BeginReceive(LState.RecvBuffer, 0, LState.CommandSize, 0,
                new AsyncCallback(ReceiveCallback), LState);
        }
        catch (Exception e)
        {
            string LError = "SRTcpClient Receive " + e.ToString();
            if (OnError != null)
                OnError(LError);
            Debug.LogError(LError);
        }
    }

    // Каллбак асинхронного приема данных
    private void ReceiveCallback(IAsyncResult AAsyncResult)
    {
        try
        {
            // Получение объекта из потока для дальнейшего движения в потоке
            StateObject LState = (StateObject)AAsyncResult.AsyncState;
            Socket LSocket = LState.Socket;
            // Завершить асинхронное чтение
            int LBytesRead = LSocket.EndReceive(AAsyncResult);
            // Если принят полный набор байт, нужно определить - команда это или буфер действий
            if (LState.TotalSize + LBytesRead == LState.WantSize)
            {
                // Если ожидалась команда - загрузить байты размера буфера действий
                if (LState.CommandWant)
                    LState.WantSize = BitConverter.ToInt32(LState.RecvBuffer, 0);
                // Иначе загрузить байты команды
                else
                {
                    // Передача буффера клиента на анализ
                    if (OnReceive != null)
                        OnReceive(LState.RecvBuffer, LState.WantSize);
                    // И следуюший набор снова ожидает команду
                    LState.WantSize = LState.CommandSize;
                }
                LState.TotalSize = 0;
                // Передернем назад режим приема
                LState.CommandWant = !LState.CommandWant;
                // И снова запуск асинхронного приема на размер указанного буффера
                LSocket.BeginReceive(LState.RecvBuffer, 0, LState.WantSize, 0,
                    new AsyncCallback(ReceiveCallback), LState);
            }
            else
            {
                // Иначе если приняли неполный пакет, запрашиваем остатки в тот-же буффер
                if (LBytesRead > 0)
                {
                    LState.TotalSize += LBytesRead;
                    LSocket.BeginReceive(LState.RecvBuffer, LState.TotalSize,
                        LState.WantSize - LState.TotalSize, 0, new AsyncCallback(ReceiveCallback), LState);
                }
                else
                {
                    // Иначе если приняли 0 - значит сервер отключился
                    string LError = "SRTcpClient Server disconnected";
                    if (OnError != null)
                        OnError(LError);
                    Debug.LogError(LError);
                }
            }
        }
        catch (Exception e)
        {
            string LError = "ReceiveCallback Receive " + e.ToString();
            if (OnError != null)
                OnError(LError);
            Debug.LogError(LError);
        }
    }
}