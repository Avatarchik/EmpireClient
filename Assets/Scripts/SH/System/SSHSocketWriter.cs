/*///////////////////////////////////////////////
{                                              }
{  Базовый модуль отправки запросов на сервер  }
{       Copyright (c) 2016 UAshota             }
{                                              }
{                                              }
/*///////////////////////////////////////////////
using System.IO;
using UnityEngine;

public class SSHSocketWriter
{
    // Определение доступности плантарной системы
    protected const int CmdPlanetarAvailable = 0x1F01;
    // Подписка на планетарную систему
    protected const int CmdPlanetarSubscribe = 0x1F02;

    // Выбор галактической системы для загрузки
    private const int C_SELECT_GALAXY = 0x2F01;
    // Чат временно здесь
    private const int C_CHAT_MESSAGE = 0x3000;

    // Открыть буффер для указанной комманды
    protected BinaryWriter DoOpen(int ACommand)
    {
        MemoryStream LStream = new MemoryStream();
        BinaryWriter LWriter = new BinaryWriter(LStream);
        // Сразу запишем размер пакета
        LWriter.Write(ACommand);        
        // Сразу запишем команду
        LWriter.Write(ACommand);
        // И вернем врайтер
        return LWriter;
    }

    // Закрыть буффер и отправить комманду
    protected void DoClose(BinaryWriter AWriter)
    {
        // Запишем размер пакета
        AWriter.Seek(0, SeekOrigin.Begin);
        AWriter.Write((int)AWriter.BaseStream.Length - sizeof(int));
        // Асинхронно отправим пакет
        SSHConnection.Socket.Send(((MemoryStream)AWriter.BaseStream).ToArray());        
    }

    // Отправка строки
    protected void DoWriteString(string AValue, BinaryWriter AWriter)
    {
        byte[] LBytes = System.Text.Encoding.UTF8.GetBytes(AValue);
        AWriter.Write(LBytes.Length);
        AWriter.Write(LBytes);
    }

    public void PlanetarAvailable(int APlanetarID)
    {
        BinaryWriter LWriter = DoOpen(CmdPlanetarAvailable);
        LWriter.Write(APlanetarID);
        DoClose(LWriter);
    }

    public void SendSelectGalaxy()
    {
        BinaryWriter LWriter = DoOpen(C_SELECT_GALAXY);
        DoClose(LWriter);
    }

    public void ChatMessage(string AMessage)
    {
        BinaryWriter LWriter = DoOpen(C_CHAT_MESSAGE);
        DoWriteString(AMessage, LWriter);
        DoClose(LWriter);
    }
}