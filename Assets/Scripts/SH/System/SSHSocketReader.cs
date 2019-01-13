/*///////////////////////////////////////////////
{                                              }
{  Базовый модуль приема запросов с сервера    }
{       Copyright (c) 2016 UAshota             }
{                                              }
{                                              }
/*///////////////////////////////////////////////
using System.IO;
using UnityEngine;

public class SSHSocketReader
{
    // Определение доступности плантарной системы
    protected const int CmdPlanetarAvailable = 0x1F01;


    // Читалка с потока
    protected BinaryReader FReader;

    // Разрешена загрузка созвездия
    private const int C_PLANETAR_STARTED = 0x1000;
    // Разрешена загрузка галактики
    private const int C_GALAXY_ACCEPT = 0x2F00;
    // Чат временно здесь
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
        if (ACommand == C_PLANETAR_STARTED)
            DoReadPlanetarStarted();
        else if (ACommand == C_GALAXY_ACCEPT)
            DoReadGalaxyAccept();
        else if (ACommand == C_CHAT_MESSAGE)
            DoReadChatMessage();
        else
            Debug.Log("Invalid cmd " + ACommand);
    }

    // Чтение строки, сперва ее длина, затем буфер строки, Unicode
    protected string DoReadString()
    {
        int LStrLen = FReader.ReadInt32();
        return System.Text.Encoding.UTF8.GetString(FReader.ReadBytes(LStrLen));
    }

    private void DoReadPlanetarStarted()
    {
        int tmpPlanetarID = FReader.ReadInt32();
        SSHShared.ShowPlanetar(tmpPlanetarID);
    }

    private void DoReadGalaxyAccept()
    {
        SSHShared.ShowGalaxy();
    }

    private void DoReadChatMessage()
    {
        string LMessage = DoReadString();
        SSHShared.UIChat.ShowMessage(LMessage);
    }
}