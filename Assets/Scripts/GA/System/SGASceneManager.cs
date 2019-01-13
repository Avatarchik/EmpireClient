/*///////////////////////////////////////////////
{                                              }
{ Galaxy                                       }
{ Модуль менеджера сцены                       }
{ Copyright (c) 2016 UAshota                   }
{                                              }
{ Rev B  2017.06.06                            }
{                                              }
/*///////////////////////////////////////////////

namespace Galaxy
{
    public class SGASceneManager : Shared.SSHSceneManager
    {
        // Панель карты
        public MSHCommonMapControl MapControl;

        void Start()
        {
            // Для отладки, всегда загружается сперва сцена приветствия
            if (!SGAShared.IsMainSceneStarted)
                SGAShared.ShowWelcome(false);
            // Панель карты
            SGAShared.MapControl = MapControl;
        }

        void Update()
        {
            // Считка команд с сервера
            DoReadQueue(SGAShared.SocketReader);
        }

        /* Временная заглушка */
        public void GoHome()
        {
            SGAShared.SocketWriter.PlanetarAvailable(SGAShared.Player.UID);
        }
    }
}