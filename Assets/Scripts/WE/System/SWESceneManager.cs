/*///////////////////////////////////////////////
{                                              }
{  Заготовка менеджера для сцены авторизации   }
{       Copyright (c) 2016 UAshota             }
{                                              }
{  Rev A  2016.12.28                           }
{  Rev B  2017.05.15                           }
{                                              }
/*///////////////////////////////////////////////
using UnityEngine;

public class SWESceneManager : Shared.SSHSceneManager
{
    // UI панель описания игрока (ангар, валюта)
    public MWEUIPanelAuth PanelAuth;

    void Start()
    {
        SWEShared.UIPanelAuth = PanelAuth;
        Application.runInBackground = true;
        SWEShared.IsMainSceneStarted = true;
    }

    void Update()
    {
        DoReadQueue(SWEShared.SocketReader);
    }
}