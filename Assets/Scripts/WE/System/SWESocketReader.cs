/*///////////////////////////////////////////////
{                                              }
{   Модуль обработки сообщений сервера         }
{       Copyright (c) 2016 UAshota             }
{                                              }
{                                              }
/*///////////////////////////////////////////////
using UnityEngine;
using System.IO;

public class SWESocketReader : SSHSocketReader
{
    // Авторизация провалена
    private const int CmdLoginFailed = 0x0003;
    // Авторизация успешна
    private const int CmdLoginAccept = 0x0004;

    // Чтение буфера комманд
    protected override void DoRead(int ACommand, MemoryStream AReader)
    {
        FReader = new BinaryReader(AReader);

        // Пока такое решение, перебор
        if (ACommand == CmdLoginAccept)
            DoReadLoginAccept();
        else if (ACommand == CmdLoginFailed)
            DoReadLoginFailed();
        else if (ACommand == CmdPlanetarAvailable)
            DoReadPlanetarAvailable();
        else
            base.DoRead(ACommand, AReader);
    }

    // Авторизация успешна
    private void DoReadLoginAccept()
    {
        // Обработка команды успешного логина
        string LPassword = DoReadString();
        SSHShared.Player.UID = FReader.ReadInt32();
        SSHShared.Player.Race = (SSHRace)FReader.ReadInt32();
        SWEShared.UIPanelAuth.SavePassword(LPassword);
        // Подключение к своей планетарке
        SWEShared.SocketWriter.PlanetarAvailable(SWEShared.Player.UID);

    }

    // Авторизация провалена
    private void DoReadLoginFailed()
    {
        int LErrorCode = FReader.ReadInt32();
        SWEShared.UIPanelAuth.ShowError(LErrorCode);
    }

    // Оповещение о доступности созвездия
    private void DoReadPlanetarAvailable()
    {
        int LPlanetarID = FReader.ReadInt32();
        int LErrorCode = FReader.ReadInt32();
        // Запустим созвездие если доступно
        if (LErrorCode == 0)
            SSHShared.ShowPlanetar(LPlanetarID);
        else
            SWEShared.UIPanelAuth.ShowError(LErrorCode);
    }
}