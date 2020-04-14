/////////////////////////////////////////////////
//                                              
//  Интерактивный класс планетарного флота       
//  Copyright (c) 2016 UAshota                   
//                                              
//  Rev A  2016.11.15                            
//  Rev B  2017.06.06                            
//  Rev D  2018.06.19
//                                              
/////////////////////////////////////////////////

using System;
using UnityEngine;

namespace Planetar
{
    // Пункты меню
    public enum ShipAction
    {
        Annihilation,
        ChangeMode,
        Constructor,
        Delete,
        MoveToHangar,
        PortalOpen,
        PortalJump
    }

    // Типы доступных таймеров
    public enum ShipTimer
    {
        /// <summary>
        /// Операция постройки
        /// </summary>
        Construction,
        /// <summary>
        /// Операция прыжка в портал
        /// </summary>
        PortalJump,
        /// <summary>
        /// Операция дозаправки
        /// </summary>
        Refill,
        /// <summary>
        /// Операция перелета
        /// </summary>
        FlightLocal,
        /// <summary>
        /// Операция полета
        /// </summary>
        FlightGlobal,
        /// <summary>
        /// Операция аннигиляции
        /// </summary>
        Annihilation,
        /// <summary>
        /// Операция самопочинки
        /// </summary>
        Fix,
        /// <summary>
        /// Операция ремонта союзника
        /// </summary>
        Repair,
        /// <summary>
        /// Кулдаун скилла разбора юнита
        /// </summary>
        Constructor
    }

    // Типы боевых снарядов
    public enum ShipShellType
    {
        // Пуля
        Bullet,
        // Лазер
        Laser,
        // Ракета
        Rocket
    }

    // Типы технологий боевых кораблей
    public enum ShipTech
    {
        // Пустая
        Empty,
        // Оружие пули
        WeaponeBullet,
        // Возможность постройки
        Active,
        // Аннигиляция
        Annihilation,
        // Броня
        Armor,
        // Запрет разбора конструктором
        SolidBody,
        // Цена
        Cost,
        // Количество в стеке
        Count,
        // Множитель урона
        Damage,
        // Самопочинка
        Repair,
        // Скрытое перемещение
        Hidden,
        // Структура
        Hp,
        // Свободная технология
        Mirror,
        // Безлимитный портал
        StablePortal,
        // Выработка ресурса
        Production,
        // Починка других юнитов
        Fix,
        // Переносное хранилище
        Storage,
        // Вес юнита
        Weight,
        // Скилл разбора
        SkillConstructor,
        // Элитка разбор дополнительных противников
        SkillConstructorEnemy,
        // Элитка разбор в дополнительных союзников
        SkillConstructorFriend,
        // Стационарность
        Stationary,
        // Приоритет атаки
        Priority,
        // Мспользование нижней орбиты
        LowOrbit,
        // Слет с планетоида
        Faster,
        // Защита от атак артилерии с другой планеты
        RangeDefence,
        // Оружие прострела через юнит
        WeaponeOvershot,
        // Возможность влета в тыл
        IntoBackzone,
        // Возможность блокировать с краев
        CornerBlock,
        // Оружие двойная пуля
        WeaponeDoubleBullet,
        // Цель для прострела
        OvershotTarget,
        // Оружие лазер
        WeaponeLaser,
        // Оружие ракета
        WeaponeRocket,
        // Захват лояльности
        Capturer,
        // Блокиратор прострела
        OvershotBlocker,
        // Оружие двойной лазер
        WeaponeDoubleLaser,
        // Блокиратор бытрых юнитовы
        SpeedBlocker,
        // Стабилизатор ЧТ
        WormholeGuard,
        // Строитель юнитов
        Construction
    }

    // Типы боевых кораблей
    public enum ShipType
    {
        // Пустой
        Empty,
        // Транспорт
        Transport,
        // Крыса
        Cruiser,
        // Дредноут
        Dreadnought,
        // Корвет
        Corvete,
        // Девастатор
        Devastator,
        // Штурмовик
        Invader,
        // Военная база
        Millitary,
        // Верфь
        Shipyard,
        // Научная станция
        Scient,
        // Сервисная платформа
        Service,
        // Флагман
        Flagship
    }

    // Состояние корабля
    public enum ShipState
    {
        /// <summary>
        /// Заблокирован для использования
        /// </summary>
        Disabled,
        /// <summary>
        /// Активен
        /// </summary>
        Available,
        /// <summary>
        /// Доступен к активации
        /// </summary>
        Interactive
    }

    // Режим состояния корабля
    public enum ShipMode
    {
        /// <summary>
        /// Активен
        /// </summary>
        Active,
        /// <summary>
        /// Блокирован с краев
        /// </summary>
        Blocked,
        /// <summary>
        /// Лимит для активации
        /// </summary>
        Full,
        /// <summary>
        /// Походный режим
        /// </summary>
        Offline
    }

    internal enum ShipFlyType
    {
        Parking,
        Local,
        Global
    }

    // Описание деталей корабля
    public struct ShipInfo
    {
        // Тип юнита
        public ShipType ShipType;
        // Название
        public string Name;
        // Описание
        public string Description;
    }

    // Типы вооружения
    public enum ShipWeaponType
    {
        // Центральное
        Center,
        // Левое
        Left,
        // Правое
        Right,
        // Ракета
        Rocket
    }

    /* заглушка скилла конструктора*/
    public class ShipSkillConstructor : Shared.Interactive
    {
        public Ship Initiator;

        public ShipSkillConstructor(Ship AInitiator)
        {
            Initiator = AInitiator;
            Initiator.ShowPath(true);
        }

        public void Execute(Ship ATarget)
        {
            Initiator.ShowPath(false);
            Engine.SocketWriter.ShipSkillConstructor(Initiator, ATarget);
        }
    }

    // Тип внешнего класса боевого флота
    public class Ship : Shared.Interactive
    {
        // Владелец юнита
        public Player Owner;
        // Планета флота
        public Planet Planet;
        // Тип корабля
        public ShipType ShipType;
        // Привязка к планете
        public Planet AttachedPlanet;
        // Слот корабля
        public Landing Landing { get { return FLanding; } set { SetLanding(value); } }
        // Количество кораблей
        public int Count;
        // Количество уничтоженных
        public int Destructed;
        // Количество HP
        public int HP;
        // Количество зарядов
        public int Fuel;
        // Признак состояния корабля
        public ShipState State;
        // Признак режима состояния корабля
        public ShipMode Mode;
        // Признак захвата планеты
        public bool IsCapture;
        // Признак автоприцела
        public bool IsAutoTarget;
        /* Группа корабля */
        public ShipGroup Group;
        // Орудийная система
        public ShipWeapon[] Weapons;

        // Технологии
        private readonly TechInfo[] FTech;
        // Слот посадки
        private Landing FLanding;
        // Массив активных таймеров
        private readonly int[] FTimers;
        // Привязка к игровому скрипту
        private MonoShip FScript;
        // Общий компонент хинта
        private static MonoUIHintShip FHint;
        // Общий компонент хинта
        private static MonoUIPopupShip FPopup;

        // Возвращение общего хинта
        protected override Shared.MonoUIHintCustom DoGetHint()
        {
            if (!FHint)
                FHint = PrefabManager.CreateShipHint();
            return FHint;
        }

        // Возвращение общего меню
        protected override Shared.MonoUIPopupCustom DoGetPopup()
        {
            if (!FPopup)
                FPopup = PrefabManager.CreateShipPopup();
            return FPopup;
        }

        // Установка зоны посадки
        private void SetLanding(Landing ALanding)
        {
            // Сбросим старый объект
            if (FLanding != null)
                FLanding.Ship = null;
            // Выставим новый
            if (ALanding != null)
            {
                FLanding = ALanding;
                FLanding.Ship = this;
            }
        }

        // Проверка возможности показа меню ухода в походку
        private ActionState CheckActionChangeMode()
        {
            return IsAvail() ? ActionState.Enabled : ActionState.Disabled;
        }

        private ActionState CheckActionDelete()
        {
            return IsAvail() ? ActionState.Enabled : ActionState.Disabled;
        }

        private ActionState CheckActionMoveToHangar()
        {
            if (Tech(ShipTech.Stationary).Supported)
                return ActionState.Hidden;
            else
                return (!Planet.InBattle) ? ActionState.Enabled : ActionState.Disabled;
        }

        private ActionState CheckActionPortalJump()
        {
            if (Planet.Portal == null)
                return ActionState.Hidden;
            else
                return IsAvail() ? ActionState.Enabled : ActionState.Disabled;
        }

        private ActionState CheckActionPortalOpen()
        {
            if (!Tech(ShipTech.Stationary).Supported)
                return ActionState.Hidden;
            else
                return ActionState.Enabled;
        }

        private ActionState CheckActionAnnihilation()
        {
            if (!Tech(ShipTech.Annihilation).Supported)
                return ActionState.Hidden;
            else
                return IsAvail() ? ActionState.Enabled : ActionState.Disabled;
        }

        private ActionState CheckActionConstructor()
        {
            if (!Tech(ShipTech.SkillConstructor).Supported)
                return ActionState.Hidden;
            else
                return (Timer(ShipTimer.Constructor) == 0) ? ActionState.Enabled : ActionState.Disabled;
        }

        private void CallActionMoveToHangar()
        {
            // Отправим в ангар
            Hangar LTargetHangar = null;
            // Найдем нужный или первый пустой ангар
            foreach (Hangar LHangar in Engine.UIPlayerDetails.Hangar.FSlots)
            {
                // Ангар подходит
                if (LHangar.ShipType == ShipType)
                {
                    LTargetHangar = LHangar;
                    break;
                }
                // Ангар пустой
                if ((LTargetHangar == null) && (LHangar.ShipType == ShipType.Empty))
                    LTargetHangar = LHangar;
            }
            // Если место есть - отправим
            if (LTargetHangar != null)
            {
                Lock();
                Engine.SocketWriter.ShipToHangar(LTargetHangar.UID, this);
            }
        }

        private void CallActionChangeMode()
        {
            Engine.SocketWriter.ShipChangeActive(this);
        }

        private void CallActionAnnihilation()
        {
            Engine.SocketWriter.ShipAnnihilation(this);
        }

        private void CallActionPortalJump()
        {
            Engine.SocketWriter.ShipPortalJump(this);
        }

        private void CallActionDelete()
        {
            Engine.SocketWriter.ShipDelete(this);
        }

        private void CallActionConstructor()
        {
            Debug.Log("empty");
        }

        private void CallActionPortalOpen()
        {
            Debug.Log("empty");
        }

        // Конструктор сразу определяет тип данных
        public Ship(int AID, Player AOwner, Landing ALanding, ShipType AShipType)
        {
            UID = AID;
            Owner = AOwner;
            ShipType = AShipType;
            Landing = ALanding;
            Planet = ALanding.Planet;
            FTimers = new int[Enum.GetValues(typeof(ShipTimer)).Length];
            Weapons = new ShipWeapon[Enum.GetValues(typeof(ShipWeaponType)).Length];
            Transform = PrefabManager.CreateShip(ALanding.Transform, Owner.Race, AShipType).transform;
            Transform.SetParent(ALanding.Transform, false);
            FScript = Transform.GetComponent<MonoShip>();
        }

        // Отложенная инициализация
        public void Allocate()
        {
            Planet.Ships.Add(this);
            FScript.Init(this);
        }

        // Возвращение технологии по типу
        public TechInfo Tech(ShipTech ATechType)
        {
            return Engine.TechShip(ShipType, ATechType);
        }

        // Значение указанного таймера
        public int Timer(ShipTimer AShipTimer)
        {
            return FTimers[(int)AShipTimer];
        }

        // Возврат орудия по типу
        public ShipWeapon Weapon(ShipWeaponType AWeaponType)
        {
            return Weapons[(int)AWeaponType];
        }

        // Признак того, что кораблем можно управлять
        public bool IsAvail()
        {
            return (State == ShipState.Available) || (State == ShipState.Interactive);
        }

        // Признак того, что корабль в полете
        public bool IsMove()
        {
            return Timer(ShipTimer.FlightGlobal) > 0 || Timer(ShipTimer.FlightLocal) > 0;
        }

        // Включение или выключение линии пути следования
        public void ShowPath(bool AEnabled)
        {
            FScript.ShowPath(AEnabled);
        }

        // Добавление планеты в путь следования корабля
        public void PathAdd(Planet APlanet)
        {
            FScript.GroupPathAdd(APlanet);
        }

        // Очистка пути следования корабля
        public void PathRemove()
        {
            FScript.GroupPathRemove();
        }

        // Команда физического перемещения кораблика
        public void MoveTo(Planet APlanet, int ASlot)
        {
            UID = APlanet.UID << 16 | ASlot;
            FScript.MoveToPlanet(APlanet, ASlot);
        }

        // Команда моментального перемещения кораблика
        public void JumpTo(Planet APlanet, int ASlot)
        {
            // Если прыжок на планету без подписки - удалим кораблик
            if (APlanet.Subscription != PlanetSubscription.Enabled)
                Delete(false);
            else
                FScript.MoveToLanding(APlanet, ASlot);
        }

        // Запуск уничтожения корабля со взрывом
        public void Delete(bool AExplosion)
        {
            Hint.Hide();
            FScript.Delete(AExplosion);
        }

        // Обновление структуры корабля
        public void UpdateHP(int ACount, int AHP, int ADestructed)
        {
            Count = ACount;
            HP = AHP;
            Destructed = ADestructed;
            FScript.UpdateHP();
        }

        // Обновление значения таймера
        public void UpdateTimer(int ATimer, int ASeconds)
        {
            if (ASeconds > 0)
                ASeconds += Mathf.RoundToInt(Time.time);
            // Установим значение таймеру
            FTimers[ATimer] = ASeconds;
        }

        // Обновление значения топлива
        public void UpdateFuel(int ACount)
        {
            Fuel = ACount;
        }

        // Обновление аттача к планете
        public void UpdateAttach(Planet APlanet, bool ACapture, bool AAutoTarget)
        {
            AttachedPlanet = APlanet;
            IsCapture = ACapture;
            IsAutoTarget = AAutoTarget;
            FScript.UpdateAttach();
        }

        // Обновление состояния кораблика
        public void UpdateState(ShipState AState, ShipMode AMode)
        {
            State = AState;
            Mode = AMode;
        /*    IsCapture = ACapture;*/
            FScript.UpdateState();
        }

        public ActionState ActionStatus(ShipAction AAction)
        {
            if (Locked() || (Owner.Role != SSHRole.Self))
                return ActionState.Hidden;
            switch (AAction)
            {
                case ShipAction.Annihilation:
                    return CheckActionAnnihilation();
                case ShipAction.ChangeMode:
                    return CheckActionChangeMode();
                case ShipAction.Constructor:
                    return CheckActionConstructor();
                case ShipAction.Delete:
                    return CheckActionDelete();
                case ShipAction.MoveToHangar:
                    return CheckActionMoveToHangar();
                case ShipAction.PortalOpen:
                    return CheckActionPortalOpen();
                case ShipAction.PortalJump:
                    return CheckActionPortalJump();
                default:
                    Debug.Log("Undefined status " + AAction);
                    break;
            }
            return ActionState.Hidden;
        }

        public void ActionCall(ShipAction AAction)
        {
            switch (AAction)
            {
                case ShipAction.Annihilation:
                    CallActionAnnihilation();
                    break;
                case ShipAction.ChangeMode:
                    CallActionChangeMode();
                    break;
                case ShipAction.Constructor:
                    CallActionConstructor();
                    break;
                case ShipAction.Delete:
                    CallActionDelete();
                    break;
                case ShipAction.MoveToHangar:
                    CallActionMoveToHangar();
                    break;
                case ShipAction.PortalOpen:
                    CallActionPortalOpen();
                    break;
                case ShipAction.PortalJump:
                    CallActionPortalJump();
                    break;
                default:
                    Debug.Log("Undefined call " + AAction);
                    break;
            }
        }
    }
}