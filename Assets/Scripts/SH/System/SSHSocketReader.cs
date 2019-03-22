/*///////////////////////////////////////////////
{                                              }
{  Базовый модуль приема запросов с сервера    }
{       Copyright (c) 2016 UAshota             }
{                                              }
{                                              }
/*///////////////////////////////////////////////
using System;
using System.IO;
using UnityEngine;

public class SSHSocketReader
{
    // Определение доступности плантарной системы
    protected const int PlanetarAvailable = 0x1F01;

    // Читалка с потока
    protected BinaryReader FReader;


    private const int C_CHAT_MESSAGE = 0x0001;

    public void Read(int ACommand, MemoryStream AReader)
    {
        FReader = new BinaryReader(AReader);
        DoRead(ACommand, AReader);
    }

    // Чтение буфера комманд
    protected virtual void DoRead(int ACommand, MemoryStream AReader)
    {
        // Пока такое решение, перебор
        if (ACommand == C_CHAT_MESSAGE)
            DoReadChatMessage();
        else
            Debug.LogWarning(string.Format("Invalid cmd  0x{0:X}", ACommand));
    }

    // Чтение строки, сперва ее длина, затем буфер строки, Unicode
    protected string DoReadString()
    {
        int LStrLen = FReader.ReadInt32();
        return System.Text.Encoding.UTF8.GetString(FReader.ReadBytes(LStrLen));
    }

    private void DoReadChatMessage()
    {
        string LMessage = DoReadString();
        SSHShared.UIChat.ShowMessage(LMessage);
    }
}