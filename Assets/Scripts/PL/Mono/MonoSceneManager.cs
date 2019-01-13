/*///////////////////////////////////////////////
{                                              }
{ Planetar                                     }
{ Модуль управления сценой планетарных боев    }
{ Copyright (c) 2016 UAshota                   }
{                                              }
{ Rev A  2016.12.18                            }
{ Rev B  2017.06.06                            }
{                                              }
/*///////////////////////////////////////////////
using UnityEngine;

namespace Planetar
{
    public class MonoSceneManager : Shared.SSHSceneManager
    {
        // Панель карты
        public MSHCommonMapControl MapControl;
        // Панель деталей планеты
        public MonoUIPanelPlanet PanelPlanetDetails;
        // Панель технологий корабликов
        public MonoUIPanelTechShips PanelTechShips;
        // Панель игрока
        public MonoUIPanelPlayer PanelPlayer;
        // Панель чата
        public MSHUIPanelChat PanelChat;
        // Панель поля боя
        public Transform PanelBattlefield;
        // Панель мини-карты
        public Transform PanelMiniMap;
        // Панель координатной сетки
        public Transform PanelGrid;
        // Панель UI
        public Transform PanelUI;
        // Панель тумана войны
        public Transform FogPanel;
        // Камера тумана войны
        public Camera FogCamera;
        // Меню выбора товара на планете
        public Transform PopupStorageSelection;

        // Код последней принятой ошибки
        private string FLastError = "";
        // Для отладки признак загрузки системы
        private bool FLoaded = false;

        private void OnGUI()
        {
            if (Debug.isDebugBuild)
            {
                string LMonoSource;
                string LMonoTarget;
                string LMonoFocus;
                if (SSHShared.MonoSource != null)
                    LMonoSource = "Source " + SSHShared.MonoSource.ToString() + " : " + SSHShared.MonoSource.UID;
                else
                    LMonoSource = "Source empty";

                if (SSHShared.MonoTarget != null)
                    LMonoTarget = "Target " + SSHShared.MonoTarget.ToString() + " : " + SSHShared.MonoTarget.UID;
                else
                    LMonoTarget = "Target empty";

                if (SSHShared.MonoFocus != null)
                    LMonoFocus = "Focus " + SSHShared.MonoFocus.ToString() + " : " + SSHShared.MonoFocus.UID;
                else
                    LMonoFocus = "Focus empty";

                GUI.Label(new Rect(100, 100, 250, 50), LMonoSource);
                GUI.Label(new Rect(100, 130, 250, 50), LMonoTarget);
                GUI.Label(new Rect(100, 160, 250, 50), LMonoFocus);
                GUI.Label(new Rect(100, 190, 250, 50), "Group: " + Engine.ShipGroup.Ships.Count.ToString());
            }
        }

        void Start()
        {
            // Для отладки, всегда загружается сперва сцена приветствия
            if (!Engine.IsMainSceneStarted)
            {
                Engine.ShowWelcome(false);
                return;
            }
            SSHControls.ShowLoading("Построение объектов");
            // Установка панели деталей планеты
            Engine.UIPlanetDetails = PanelPlanetDetails;
            // Установка панели технологий корабликов
            Engine.UITechShips = PanelTechShips;
            // Установка панели поля боя
            Engine.UIBattlefield = PanelBattlefield;
            // Установка панели чата
            Engine.UIChat = PanelChat;
            // Установка панели игрока
            Engine.UIPlayerDetails = PanelPlayer;
            // Установка панели мини-карты
            Engine.UIMiniMap = PanelMiniMap;
            // Установка панели игрока
            Engine.UI = PanelUI;
            // Панель координатной сетки
            Engine.UIPanelGrid = PanelGrid;
            // Панель карты
            Engine.MapControl = MapControl;
            /**/
            Engine.Load();
            Engine.SceneManager = this;
            // Показ заставки о старте загрузки
            // Подпись на сообщение об ошибке
            SSHConnection.Socket.OnError = OnError;
            // Сообщение серверу о готовности принять данные созвездия
            Engine.SocketWriter.PlanetarSubscribe();
        }

        void Update()
        {
            // Для отладки, всегда загружается сперва сцена приветствия
            if (!Engine.IsMainSceneStarted)
                return;

            // За каждый фрейм обрабатываем все доступные сообщения, вероятно колхозный метод
            DoReadQueue(Engine.SocketReader);

            // Скроем экран загрузки
            if ((!FLoaded) && (Engine.IsSystemLoaded))
            {
                FLoaded = true;
                SSHControls.HideLoading();
            }
            // Если есть ошибка - то покажем ее
            if (FLastError.Length > 0)
                SSHControls.ShowLoading(FLastError);
        }

        void OnError(string AMessage)
        {
            FLastError = AMessage;
        }

        public void Load()
        {
            /* магическая константа */
            Engine.UIPlayerDetails.Hangar.Create();
            FogPanel.localScale = new Vector3(Engine.MapSize.x, 0, Engine.MapSize.y);
            FogPanel.localPosition = new Vector3(Engine.MapSize.x * 3.3f, FogPanel.localPosition.y, -Engine.MapSize.y * 3.3f);
            FogCamera.transform.localPosition = new Vector3(Engine.MapSize.x * 3.3f, FogCamera.transform.localPosition.y, -Engine.MapSize.y * 3.3f);
            FogCamera.orthographicSize = Engine.MapSize.x * 5;
        }
    }
}