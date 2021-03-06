﻿/*///////////////////////////////////////////////
{                                              }
{ Planetar                                     }
{ Интерактиыный класс планетарного планетоида  }
{ Copyright (c) 2016 UAshota                   }
{                                              }
{ Rev A  2016.11.15                            }
{ Rev B  2017.06.06                            }
{ Rev C  2017.09.26                            }
{                                              }
/*///////////////////////////////////////////////
using System.Collections.Generic;
using UnityEngine;

namespace Planetar
{
    // Класс поселения
    public enum PlanetClass
    {
        Empty,         // Нет
        Colony,        // Колония (без производства)
        Manufacture    // Колония (с производством)
    }

    /// <summary>
    /// Таймеры планеты
    /// </summary>
    internal enum PlanetTimer
    {
        /// <summary>
        /// Боевой тик
        /// </summary>
        Battle,
        /// <summary>
        /// Захват лояльности
        /// </summary>
        Capture,
        /// <summary>
        /// Тик смены состояния
        /// </summary>
        Activity,
        /// <summary>
        /// Активен гравитационный потенциал после аннигиляции
        /// </summary>
        LowGravity
    }

    /// <summary>
    /// Состояние планетоида
    /// </summary>
    public enum PlanetState
    {
        /// <summary>
        /// Активен
        /// </summary>
        Active,
        /// <summary>
        /// Активируется
        /// </summary>
        Activation,
        /// <summary>
        /// Неактивен
        /// </summary>
        Inactive,
        /// <summary>
        /// Заблокирован для изменения
        /// </summary>
        Blocked
    }

    /// <summary>
    /// Типы планет
    /// </summary>
    public enum PlanetType
    {
        /// <summary>
        /// Маленькая
        /// </summary>
        Earth,
        /// <summary>
        /// Звезда
        /// </summary>
        Sun,
        /// <summary>
        /// Гидросостав
        /// </summary>
        Hydro,
        /// <summary>
        /// Карлик
        /// </summary>
        Rock,
        /// <summary>
        /// Черная дыра
        /// </summary>
        Hole,
        /// <summary>
        /// Пульсар
        /// </summary>
        Pulsar
    }

    /// <summary>
    /// Режим планеты
    /// </summary>
    public enum PlanetMode
    {
        /// <summary>
        /// Обычная
        /// </summary>
        Normal,
        /// <summary>
        /// Большая
        /// </summary>
        Big
    }

    // Тип подписки на действия на планете
    public enum PlanetSubscription
    {
        Disabled,
        Queryed,
        Enabled
    }


    // Тип внешнего класса описания планетоида
    public class Planet : Shared.Interactive
    {
        // Владелец планетоида
        public Player Owner;
        // Тип планетоида
        public PlanetType PlanetType;
        public PlanetMode PlanetMode;
        // Имя планетоида
        public string Name;
        // Класс планеты
        public PlanetClass Class;
        // Координаты по X
        public int PosX;
        // Координаты по Y
        public int PosY;
        // Вес планеты при поиске кратчайшего пути
        public float Weight;
        // Наличие роли игрока на соседней планете
        public bool VisibleSoft;
        // Наличие роли игрока на планете
        public bool VisibleHard;
        // Количество своего контроля
        public bool IsCoverageSelf;
        // Количество контроля союзника
        public bool IsCoverageFriends;
        // Количество контроля врага
        public bool IsCoverageEnemy { get; set; }
        // Признак подписки на действия
        public PlanetSubscription Subscription { get; set; }
        // Идентификатор планеты, к которой подведен торговый путь 
        public Planet TradePlanet;
        // Признак возможности постройки нестационарных корабликов
        public int ShipyardsCount;
        // Количество модулей на планете
        public int ModulesCount;
        // Количество свободной энергии
        public int EnergyCount;
        // Количество складов
        public int StorageCount;
        // Лояльность захвата планеты
        public float CaptureValue;
        // Состояние активности планеты
        public PlanetState State;
        // Время смены активности планеты
        public int TimerStateTime;
        // Время последнего обновления состояния
        public float TimerUpdateTime;
        // Список кораблей на орбите
        public List<Ship> Ships;
        // Массив доступных производств
        public BuildingType[] Buildings;
        // Ссылки на ближние планеты
        public List<Planet> Links;
        // Портал
        public Portal Portal;
        // Признак наличия боя на планете
        public bool InBattle;
        // Признак окраины БЧТ
        public bool IsBigEdge;
        // Признак тайминговой планеты
        public bool IsTiming;
        // Общий компонент хинта
        private static MonoUIHintPlanet FHint;

        // Привязка к игровому скрипту
        private MonoPlanet FScript;
        // Сфера планеты выключается отдельно
        private MonoPlanetCustom FSubScript;

        protected override Shared.MonoUIHintCustom DoGetHint()
        {
            if (!FHint)
                FHint = PrefabManager.CreatePlanetHint();
            return FHint;
        }

        // Конструктор сразу определяет тип данных
        public Planet(int AUID, Transform AParent, int APosX, int APosY, PlanetType AType, PlanetMode aMode)
        {
            UID = AUID;
            Name = string.Empty;
            PlanetType = AType;
            PlanetMode = aMode;
            /* выенсти в функции */
            IsTiming = (PlanetType == PlanetType.Hole) || (PlanetType == PlanetType.Pulsar);
            Ships = new List<Ship>();
            Links = new List<Planet>();
            Buildings = new BuildingType[10];
            /* магические переменные */
            Vector3 LPosition = new Vector3(APosX / 100f * 5.65f - 3.39f, -APosY / 100f * 5.65f + 3.39f, 1);
            Transform = PrefabManager.CreatePlanet(LPosition).transform;
            Transform.SetParent(AParent, false);
            FScript = Transform.GetComponent<MonoPlanet>();
            FScript._Sphere = PrefabManager.CreatePlanetSphere(AType, aMode);
            FScript._Sphere.transform.SetParent(Transform, false);
            FSubScript = FScript._Sphere.GetComponent<MonoPlanetCustom>();
        }

        public void Allocate()
        {
            Engine.MapPlanets.Add(this);
            FScript.Init(this);
        }

        // Признак видимой и активной планеты
        public bool IsVisible()
        {
            return (PlanetType == PlanetType.Hole && PlanetMode == PlanetMode.Big) || IsBigEdge || VisibleHard || VisibleSoft;
        }

        // Возврат объекта слота планеты по его индексу
        public Landing SlotByIndex(int ASlot)
        {
            return FScript.SlotByIndex(ASlot);
        }

        // Активация и деактивация планетоида
        public void SetActive(bool AValue)
        {
            FScript.SetActive(AValue);
            FSubScript.SetActive(AValue);
        }

        // Обновление состояния планеты, видимое всем игрокам для БЧТ
        public void UpdateState(PlanetState AState)
        {
            // Обновим параметры
            State = AState;
            // Обновим состояние скрипта
            FScript.UpdateState();
            SetActive(AState != PlanetState.Inactive);

            FScript.UpdateMinimap(true, false, false);
        }

        // Обновление времени планеты, видимое подписчикам
        public void UpdateTimer(int ATime)
        {
            TimerUpdateTime = Time.time;
            TimerStateTime = ATime;
        }

        // Обновление состояния подписки
        public void UpdateSubscription(bool ASubscribed)
        {
            if (ASubscribed)
                Subscription = PlanetSubscription.Enabled;
            else
                Subscription = PlanetSubscription.Disabled;
            // Отправим уведомление о подписке
            SetActive(ASubscribed);
        }

        // Обновление владельца планеты
        public void UpdateOwner(int AOwner)
        {
            // Определим игрока владельца
            Owner = SSHShared.FindPlayer(AOwner);
            if (Owner.Role != SSHRole.Neutral)
                Name = Owner.Name;
            // Сохраним данные
            FScript.UpdateOwner();
            // Обновим миникарту при смене владельца
            FScript.UpdateMinimap(true, false, false);
        }

        // Обновление видимости и подсветки
        public void UpdateVisibility(bool AHardLight, bool AIncrement)
        {
            // Видимость целевой планеты или окраины
            if (AHardLight)
                VisibleHard = AIncrement;
            else
                VisibleSoft = AIncrement;
            // Обновим активность по видимости
            FScript.SetActive(IsVisible());
            // Обновим карту при смене типа видимости
            FScript.UpdateMinimap(true, false, false);
        }

        // Обновление зоны покрытия
        public void UpdateCoverage(bool AIncrement, SSHRole ARole)
        {
            // Для миникарты разный контроль имеет разный цвет
            if (ARole == SSHRole.Enemy)
                IsCoverageEnemy = AIncrement;
            else if (ARole == SSHRole.Friend)
                IsCoverageFriends = AIncrement;
            else if (ARole == SSHRole.Self)
                IsCoverageSelf = AIncrement;
            FScript.UpdateMinimap(false, true, false);
        }

        // Обновление торгового пути
        public void UpdateTradePath(Planet APlanet)
        {
            TradePlanet = APlanet;
            FScript.UpdateTradePath();
        }

        // Обновление количества энергии
        public void UpdateEnergy(int AEnergy)
        {
            EnergyCount = AEnergy;
            Engine.UIPlanetDetails.UpdateEnergy(this);
        }

        // Обновление количества верфей
        public void UpdateShipyards(int ACount)
        {
            /*ShipyardsCount = ACount;
            Shared.UIPlanetDetails.UpdateConstruct(this);*/
        }

        // Обновление значения захвата
        public void UpdateCapture(int AValue, SSHRole ARole)
        {
            FScript.UpdateCapture(AValue, ARole);
            CaptureValue = AValue;
        }

        // Обновление состояния боя на планете
        public void UpdateBattle(bool AValue)
        {
            InBattle = AValue;
        }

        // Изменение размера хранилища
        public void UpdateStorageSize(int ASize, bool AClear)
        {
            StorageCount = ASize;
            Engine.UIPlanetDetails.UpdateStorage(this, AClear);
        }

        // Изменение количества модулей
        public void UpdateModulesCount(int ACount)
        {
            ModulesCount = ACount;
            Engine.UIPlanetDetails.UpdateConstruct(this);
        }

        // Открытие портала
        public void PortalOpen(Planet ATarget, bool ABreakable, int ALimit, SSHRole ARole)
        {
            Portal = new Portal
            {
                Breakable = ABreakable,
                Source = this,
                Target = ATarget,
                Role = ARole,
                Limit = ALimit
            };
            // Сменим графику
            FScript.ShowPortal(true);
        }

        // Обновление параметров портала
        public void PortalUpdate(int ALimit)
        {
            Portal.Limit = ALimit;
        }

        // Закрытие портала
        public void PortalClose()
        {
            Portal = null;
            // Сменим графику
            FScript.ShowPortal(false);
        }

        public void LowGravity(bool AEnabled)
        {
            FScript.ShowLowGravity(AEnabled);
            if (AEnabled)
                Shared.PrefabManager.PlanetExplosion(Transform);
        }
    }
}