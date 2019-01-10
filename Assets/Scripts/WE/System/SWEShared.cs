/*///////////////////////////////////////////////
{                                              }
{ Модуль глобальных переменных для авторизации }
{       Copyright (c) 2016 UAshota             }
{                                              }
{                                              }
/*///////////////////////////////////////////////
public class SWEShared : SSHShared
{
    // Сокет чтения
    public static SWESocketReader SocketReader;
    // Сокет записи
    public static SWESocketWriter SocketWriter;
    // UI панель авторизации
    public static MWEUIPanelAuth UIPanelAuth;

    static SWEShared()
    {
        SocketReader = new SWESocketReader();
        SocketWriter = new SWESocketWriter();
    }
}