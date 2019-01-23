/*///////////////////////////////////////////////
{                                              }
{ Planetar                                     }
{ Модуль обработки сообщений сервера           }
{ Copyright (c) 2016 UAshota                   }
{                                              }
{ Rev A  2016.12.18                            }
{ Rev B  2017.06.06                            }
{                                              }
/*///////////////////////////////////////////////
using System;
using System.IO;

namespace Planetar
{
    public class SocketReader : SSHSocketReader
    {
        // Команда запуска загрузки созвездия клиенту
        private const int C_LOAD_SYSTEM_BEGIN = 0x1001;
        // Команда перемещения корабля
        private const int C_SHIP_MOVE_TO = 0x1002;
        // Команда создания корабля
        private const int C_SHIP_CREATE = 0x1003;
        // Команда удаления корабля
        private const int C_SHIP_DELETE = 0x1005;
        // Команда обновления ресурса корабля
        private const int C_PLANET_UPDATE_TIMER = 0x1006;
        // Команда обновления HP корабля
        private const int C_SHIP_UPDATE_HP = 0x1007;
        // Команда смены аттача корабля
        private const int C_SHIP_CHANGE_ATTACH = 0x1008;
        // Команда смены цели корабля
        private const int C_SHIP_CHANGE_TARGET = 0x1009;
        // Команда обновления состояния корабля
        private const int C_SHIP_UPDATE_STATE = 0x100A;
        // Команда открытия портала
        private const int C_PLANET_PORTAL_OPEN = 0x100B;
        // Команда настройки хранилища игрока
        private const int C_PLAYER_STORAGE_CHANGE = 0x100C;
        // Команда обновления данных панели флота
        private const int C_PLAYER_HANGAR_UPDATE = 0x100D;
        // Команда показа окна деталей планеты
        private const int C_PLANET_DETAILS_SHOW = 0x100E;
        // Команда обновления данных хранилища планеты
        private const int C_PLANET_STORAGE_UPDATE = 0x100F;
        // Команда очистки слота хранилища планеты
        private const int C_PLANET_STORAGE_CLEAR = 0x1011;
        // Команда обновления данных хранилища игрока
        private const int C_PLAYER_STORAGE_UPDATE = 0x1012;
        // Команда загрузки технологий корабликов
        private const int C_PLAYER_TECH_SHIP_CREATE = 0x1013;
        // Команда обновления технологий корабликов
        private const int C_PLAYER_TECH_SHIP_UPDATE = 0x1014;
        // Команда завершения загрузки созвездия клиенту
        private const int C_LOAD_SYSTEM_COMPLETE = 0x1015;
        // Команда загрузки технологий строений
        private const int C_PLAYER_TECH_BUILDING_CREATE = 0x1016;
        // Команда обновления данных строения
        private const int C_PLANET_BUILDING_UPDATE = 0x1017;
        // Команда обновления технологий строений
        private const int C_PLAYER_TECH_BUILDING_UPDATE = 0x1018;
        // Команда обновления динамических данных игрока
        private const int C_PLAYER_INFO_UPDATE = 0x1019;
        // Команда загрузки данных строений
        private const int C_INFO_BUILDINGS_LOAD = 0x101A;
        // Команда загрузки данных корабликов
        private const int C_INFO_WARSHIPS_LOAD = 0x101B;
        // Команда обновления состояния планетоида
        private const int C_PLANET_STATE_UPDATE = 0x101C;
        // Команда обновления времени таймера
        private const int C_PLANET_STATE_TIME = 0x101D;
        // Команда обновления видимости планетоида
        private const int C_PLANET_VISIBILITY_UPDATE = 0x101E;
        // Команда обновления данных о подписке
        private const int C_PLANET_SUBSCRIPTION_CHANGED = 0x101F;
        // Команда обновления владельца планетоида
        private const int C_PLANET_OWNER_CHANGED = 0x1020;
        // Команда обновления покрытия планетоида
        private const int C_PLANET_COVERAGE_UPDATE = 0x1021;
        // Команда обновления торгового пути
        private const int C_PLANET_TRADEPATH_UPDATE = 0x1022;
        // Команда закрытия портала
        private const int C_PLANET_PORTAL_CLOSE = 0x1023;
        // Команда обновления количества энергии планетоида
        private const int C_PLANET_ELECTRO_UPDATE = 0x1024;
        // Команда обновления данных захвата планетоида
        private const int C_PLANET_CAPTURE_UPDATE = 0x1025;
        // Комманда обновления признака идущего боя на планетоиде
        private const int C_PLANET_BATTLE_UPDATE = 0x1026;
        // Комманда обновления размера хранилища
        private const int C_PLANET_STORAGE_RESIZE = 0x1027;
        // Комманда обновления размера хранилища
        private const int C_PLANET_MODULES_UPDATE = 0x1028;
        // Комманда обновления размера хранилища
        private const int C_SHIP_TIMER_UPDATE = 0x1029;
        // Команда обновления портала
        private const int C_PLANET_PORTAL_UPDATE = 0x1030;
        // Команда обновления гравитационного потенциала
        private const int C_PLANET_LOWGRAVITY_UPDATE = 0x1031;
        // Команда моментального прыжка на планету
        private const int C_SHIP_JUMP_TO = 0x1032;
        // Команда обновления количества топлива
        private const int C_SHIP_REFILL = 0x1033;
        // Загрузка планетоидов системы
        private const int PlanetarLoadPlanets = 0x1034;

        // Чтение буфера комманд
        protected override void DoRead(int ACommand, MemoryStream AReader)
        {
            // Пока такое решение, перебор
            if (ACommand == C_SHIP_UPDATE_HP)
                DoReadShipUpdateHP();
            else if (ACommand == C_SHIP_CREATE)
                DoReadShipCreate();
            else if (ACommand == C_SHIP_DELETE)
                DoReadShipRemove();
            else if (ACommand == C_SHIP_MOVE_TO)
                DoReadShipMoveTo();
            else if (ACommand == C_SHIP_JUMP_TO)
                DoReadShipJumpTo();
            else if (ACommand == C_SHIP_CHANGE_TARGET)
                DoReadShipRetarget();
            else if (ACommand == C_SHIP_CHANGE_ATTACH)
                DoReadShipAttach();
            else if (ACommand == C_SHIP_UPDATE_STATE)
                DoReadShipFlight();
            else if (ACommand == C_SHIP_TIMER_UPDATE)
                DoReadShipTimerUpdate();
            else if (ACommand == C_SHIP_REFILL)
                DoReadShipRefill();
            else if (ACommand == C_PLANET_DETAILS_SHOW)
                DoReadPlanetDetailsShow();
            else if (ACommand == C_PLANET_OWNER_CHANGED)
                DoReadPlanetOwnerChanged();
            else if (ACommand == C_PLANET_STATE_UPDATE)
                DoReadPlanetStateUpdate();
            else if (ACommand == C_PLANET_STATE_TIME)
                DoReadPlanetStateTime();
            else if (ACommand == C_PLANET_VISIBILITY_UPDATE)
                DoReadPlanetVisibilityUpdate();
            else if (ACommand == C_PLANET_COVERAGE_UPDATE)
                DoReadPlanetCoverageUpdate();
            else if (ACommand == C_PLANET_STORAGE_RESIZE)
                DoReadPlanetStorageResize();
            else if (ACommand == C_PLANET_STORAGE_UPDATE)
                DoReadPlanetStorageUpdate();
            else if (ACommand == C_PLANET_STORAGE_CLEAR)
                DoReadPlanetStorageClear();
            else if (ACommand == C_PLANET_BUILDING_UPDATE)
                DoReadPlanetBuildingUpdate();
            else if (ACommand == C_PLANET_SUBSCRIPTION_CHANGED)
                DoReadPlanetSubscriptionChanged();
            else if (ACommand == C_PLANET_TRADEPATH_UPDATE)
                DoReadPlanetTradePathUpdate();
            else if (ACommand == C_PLANET_ELECTRO_UPDATE)
                DoReadPlanetElectroUpdate();
            else if (ACommand == C_PLANET_CAPTURE_UPDATE)
                DoReadPlanetCaptureUpdate();
            else if (ACommand == C_PLANET_BATTLE_UPDATE)
                DoReadPlanetBattleUpdate();
            else if (ACommand == C_PLANET_MODULES_UPDATE)
                DoReadPlanetModulesUpdate();
            else if (ACommand == C_PLANET_PORTAL_OPEN)
                DoReadPortalOpen();
            else if (ACommand == C_PLANET_PORTAL_UPDATE)
                DoReadPortalUpdate();
            else if (ACommand == C_PLANET_PORTAL_CLOSE)
                DoReadPortalClose();
            else if (ACommand == C_PLANET_LOWGRAVITY_UPDATE)
                DoReadLowGravityUpdate();
            else if (ACommand == C_PLANET_UPDATE_TIMER)
                DoReadPlanetUpdateTimer();
            else if (ACommand == C_LOAD_SYSTEM_BEGIN)
                DoReadLoadSystemBegin();
            else if (ACommand == PlanetarLoadPlanets)
                DoReadLoadPlanetarPlanets();
            else if (ACommand == C_LOAD_SYSTEM_COMPLETE)
                DoReadLoadSystemComplete();
            else if (ACommand == C_PLAYER_HANGAR_UPDATE)
                DoReadPlayerHangarUpdate();
            else if (ACommand == C_PLAYER_STORAGE_CHANGE)
                DoReadPlayerStorageResize();
            else if (ACommand == C_PLAYER_STORAGE_UPDATE)
                DoReadPlayerStorageUpdate();
            else if (ACommand == C_PLAYER_TECH_SHIP_CREATE)
                DoReadPlayerTechShipCreate();
            else if (ACommand == C_PLAYER_TECH_SHIP_UPDATE)
                DoReadPlayerTechShipUpdate();
            else if (ACommand == C_PLAYER_TECH_BUILDING_CREATE)
                DoReadPlayerTechBuildingCreate();
            else if (ACommand == C_PLAYER_TECH_BUILDING_UPDATE)
                DoReadPlayerTechBuildingUpdate();
            else if (ACommand == C_PLAYER_INFO_UPDATE)
                DoReadPlayerInfo();
            else if (ACommand == C_INFO_BUILDINGS_LOAD)
                DoReadInfoBuildings();
            else if (ACommand == C_INFO_WARSHIPS_LOAD)
                DoReadInfoWarships();
            else
                base.DoRead(ACommand, AReader);
        }

        private void DoReadShipCreate()
        {
            int LUID = FReader.ReadInt32();
            Player LOwner = SSHShared.FindPlayer(FReader.ReadInt32());
            Planet LPlanet = Engine.PlanetByUID(FReader.ReadInt32());
            Landing LSlot = LPlanet.SlotByIndex(FReader.ReadInt32());
            ShipType LShipType = (ShipType)FReader.ReadInt32();
            Ship LShip = new Ship(LOwner, LUID, LSlot, LShipType);
            LShip.State = (ShipState)FReader.ReadInt32();
            LShip.Mode = (ShipMode)FReader.ReadInt32();
            LShip.AttachedPlanet = Engine.PlanetByUID(FReader.ReadInt32());
            LShip.Count = FReader.ReadInt32();
            LShip.HP = FReader.ReadInt32();
            LShip.Fuel = FReader.ReadInt32();
            LShip.IsCapture = FReader.ReadBoolean();
            LShip.IsAutoTarget = FReader.ReadBoolean();
            LShip.Landing = LSlot;
            LShip.Planet = LPlanet;
            LShip.Planet.Ships.Add(LShip);
            LShip.Init();
            Engine.MapShips.Add(LUID, LShip);
        }

        private void DoReadShipRemove()
        {
            int LShipID = FReader.ReadInt32();
            bool LDestroy = FReader.ReadBoolean();

            Engine.ShipByUID(LShipID).Delete(LDestroy);
        }

        private void DoReadShipUpdateHP()
        {
            int LShipID = FReader.ReadInt32();
            int LCount = FReader.ReadInt32();
            int LHP = FReader.ReadInt32();
            int LDestructed = FReader.ReadInt32();

            Ship LShip = Engine.ShipByUID(LShipID);
            LShip.UpdateHP(LCount, LHP, LDestructed);
        }

        private void DoReadShipMoveTo()
        {
            int LShipID = FReader.ReadInt32();
            int LTargetPlanet = FReader.ReadInt32();
            int LTargetSlot = FReader.ReadInt32();

            Ship LShip = Engine.ShipByUID(LShipID);
            Planet LPlanet = Engine.PlanetByUID(LTargetPlanet);

            LShip.MoveTo(LPlanet, LTargetSlot);
        }

        private void DoReadShipJumpTo()
        {
            int LShipID = FReader.ReadInt32();
            int LTargetPlanet = FReader.ReadInt32();
            int LTargetSlot = FReader.ReadInt32();

            Ship LShip = Engine.ShipByUID(LShipID);
            Planet LPlanet = Engine.PlanetByUID(LTargetPlanet);

            LShip.JumpTo(LPlanet, LTargetSlot);
        }

        private void DoReadShipRetarget()
        {
            Ship LShip = Engine.ShipByUID(FReader.ReadInt32());
            ShipWeaponType LWeaponeType = (ShipWeaponType)FReader.ReadInt32();
            Planet LPlanetTarget = Engine.PlanetByUID(FReader.ReadInt32());
            Ship LShipTarget = Engine.ShipByUID(FReader.ReadInt32());

            LShip.Weapon(LWeaponeType).Retarget(LPlanetTarget, LShipTarget);
        }

        private void DoReadShipAttach()
        {
            Ship LShip = Engine.ShipByUID(FReader.ReadInt32());
            Planet LPlanet = Engine.PlanetByUID(FReader.ReadInt32());
            bool LCapture = FReader.ReadBoolean();
            bool LAutoTarget = FReader.ReadBoolean();

            LShip.UpdateAttach(LPlanet, LCapture, LAutoTarget);
        }

        private void DoReadShipFlight()
        {
            Ship LShip = Engine.ShipByUID(FReader.ReadInt32());
            int LState = FReader.ReadInt32();
            int LMode = FReader.ReadInt32();
            bool LCapture = FReader.ReadBoolean();

            LShip.UpdateState((ShipState)LState, (ShipMode)LMode, LCapture);
        }

        private void DoReadShipTimerUpdate()
        {
            int LShipID = FReader.ReadInt32();
            int LTimer = FReader.ReadInt32();
            int LSeconds = FReader.ReadInt32();

            Engine.ShipByUID(LShipID).UpdateTimer(LTimer, LSeconds);
        }

        private void DoReadShipRefill()
        {
            int LShipID = FReader.ReadInt32();
            int LFuel = FReader.ReadInt32();

            Engine.ShipByUID(LShipID).UpdateFuel(LFuel);
        }

        private void DoReadPlanetOwnerChanged()
        {
            int LPlanetUID = FReader.ReadInt32();
            int LOwner = FReader.ReadInt32();

            Engine.PlanetByUID(LPlanetUID).UpdateOwner(LOwner);
        }

        private void DoReadPlanetStateUpdate()
        {
            int LPlanet = FReader.ReadInt32();
            PlanetState LPlanetState = (PlanetState)FReader.ReadInt32();
            bool LIsBigHole = FReader.ReadBoolean();

            Engine.PlanetByUID(LPlanet).UpdateState(LPlanetState, LIsBigHole);
        }

        private void DoReadPlanetStateTime()
        {
            int LPlanet = FReader.ReadInt32();
            int LTime = FReader.ReadInt32();

            Engine.PlanetByUID(LPlanet).UpdateTimer(LTime);
        }

        private void DoReadPlanetVisibilityUpdate()
        {
            int LPlanet = FReader.ReadInt32();
            bool LHardLight = FReader.ReadBoolean();
            bool LIncrement = FReader.ReadBoolean();

            Engine.PlanetByUID(LPlanet).UpdateVisibility(LHardLight, LIncrement);
        }

        private void DoReadPlanetCoverageUpdate()
        {
            int LPlanet = FReader.ReadInt32();
            bool LIncrement = FReader.ReadBoolean();
            SSHRole LRole = (SSHRole)FReader.ReadInt32();

            Engine.PlanetByUID(LPlanet).UpdateCoverage(LIncrement, LRole);
        }

        private void DoReadPlanetSubscriptionChanged()
        {
            int LPLanet = FReader.ReadInt32();
            bool LSubscribed = FReader.ReadBoolean();

            Engine.PlanetByUID(LPLanet).UpdateSubscription(LSubscribed);
        }

        private void DoReadPlanetDetailsShow()
        {
            Engine.UIPlanetDetails.Show();
        }

        private void DoReadPlanetBuildingUpdate()
        {
            int LPlanet = FReader.ReadInt32();
            int LIndex = FReader.ReadInt32();
            int LBuidingType = FReader.ReadInt32();
            int LLevel = FReader.ReadInt32();
            int LHP = FReader.ReadInt32();

            Engine.UIPlanetDetails.Buildings.BuildingUpdate(LPlanet, LIndex, (BuildingType)LBuidingType, LLevel, LHP);
        }

        private void DoReadPlanetTradePathUpdate()
        {
            int LPLanet = FReader.ReadInt32();
            Planet LTarget = Engine.PlanetByUID(FReader.ReadInt32());

            Engine.PlanetByUID(LPLanet).UpdateTradePath(LTarget);
        }

        private void DoReadPlanetElectroUpdate()
        {
            int LPLanet = FReader.ReadInt32();
            int LEnergy = FReader.ReadInt32();

            Engine.PlanetByUID(LPLanet).UpdateEnergy(LEnergy);
        }
        private void DoReadPlanetCaptureUpdate()
        {
            int LPLanet = FReader.ReadInt32();
            int LCaptureValue = FReader.ReadInt32();
            SSHRole LCaptureRole = (SSHRole)FReader.ReadInt32();

            Engine.PlanetByUID(LPLanet).UpdateCapture(LCaptureValue, LCaptureRole);
        }

        private void DoReadPlanetBattleUpdate()
        {
            int LPLanet = FReader.ReadInt32();
            bool LInBattle = FReader.ReadBoolean();

            Engine.PlanetByUID(LPLanet).UpdateBattle(LInBattle);
        }

        private void DoReadPlanetModulesUpdate()
        {
            int LPLanet = FReader.ReadInt32();
            int LCount = FReader.ReadInt32();

            Engine.PlanetByUID(LPLanet).UpdateModulesCount(LCount);
        }

        private void DoReadPortalOpen()
        {
            Planet LSource = Engine.PlanetByUID(FReader.ReadInt32());
            Planet LTarget = Engine.PlanetByUID(FReader.ReadInt32());
            bool LBreakable = FReader.ReadBoolean();
            int LLimit = FReader.ReadInt32();
            SSHRole LRole = SSHRole.Neutral;
            // Если не БЧТ - то нужна роль портала
            if (LSource.PlanetType != PlanetType.Hole)
                LRole = (SSHRole)FReader.ReadInt32();
            // Откроем портал
            LSource.PortalOpen(LTarget, LBreakable, LLimit, LRole);
        }

        private void DoReadPortalUpdate()
        {
            Planet LSource = Engine.PlanetByUID(FReader.ReadInt32());
            int LLimit = FReader.ReadInt32();
            // Обновим портал
            LSource.PortalUpdate(LLimit);
        }

        private void DoReadPortalClose()
        {
            Planet LSource = Engine.PlanetByUID(FReader.ReadInt32());
            // Закроем портал
            LSource.PortalClose();
        }

        private void DoReadLowGravityUpdate()
        {
            Planet LSource = Engine.PlanetByUID(FReader.ReadInt32());
            bool LEnabled = FReader.ReadBoolean();
            // Включим или выключим гравитационный потенциал
            LSource.LowGravity(LEnabled);
        }

        private void DoReadPlanetUpdateTimer()
        {
            /*int LPlanet = FReader.ReadInt32();
            int LTimer = FReader.ReadInt32();
            int LSeconds = FReader.ReadInt32();*/
        }

        private void DoReadPlanetStorageResize()
        {
            int LPLanet = FReader.ReadInt32();
            int LSize = FReader.ReadInt32();
            bool LClear = FReader.ReadBoolean();

            Engine.PlanetByUID(LPLanet).UpdateStorageSize(LSize, LClear);
        }

        private void DoReadPlanetStorageUpdate()
        {
            int LPlanet = FReader.ReadInt32();
            int LIndex = FReader.ReadInt32();
            int LResID = FReader.ReadInt32();
            int LCount = FReader.ReadInt32();
            int LFlags = FReader.ReadInt32();
            bool LActive = FReader.ReadBoolean();

            Engine.UIPlanetDetails.Storage.Change(LPlanet, LIndex, LResID, LCount, LFlags, LActive);
        }

        private void DoReadPlanetStorageClear()
        {
            int LPlanet = FReader.ReadInt32();
            int LIndex = FReader.ReadInt32();

            Engine.UIPlanetDetails.Storage.Clear(LPlanet, LIndex);
        }

        private void DoReadLoadPlanetarPlanets()
        {
            int LCount = FReader.ReadInt32();
            // Создадим координатную сетку
            for (int LSectorX = 0; LSectorX < Engine.MapSize.x; LSectorX++)
                for (int LSectorY = 0; LSectorY < Engine.MapSize.y; LSectorY++)
                    new Grid(Engine.UIPanelGrid, LSectorX, LSectorY);
            // Загрузка планет
            for (int LIndex = 0; LIndex < LCount; LIndex++)
            {
                int LUID = FReader.ReadInt32();
                int LPosX = FReader.ReadInt32();
                int LPosY = FReader.ReadInt32();
                int LType = FReader.ReadInt32();
                Planet LPlanet = new Planet(LUID, Engine.UIBattlefield, LPosX, LPosY, (PlanetType)LType)
                {
                    Owner = SSHShared.FindPlayer(FReader.ReadInt32()),
                    State = (PlanetState)FReader.ReadInt32(),
                    VisibleHard = FReader.ReadBoolean(),
                    VisibleSoft = FReader.ReadBoolean(),
                    ControlSelf = FReader.ReadInt32(),
                    ControlFriend = FReader.ReadInt32(),
                    ControlEnemy = FReader.ReadInt32(),
                    IsBigHole = FReader.ReadBoolean()
                };
                Engine.MapPlanets.Add(LPlanet);
            }
            // Загрузка ссылок
            for (int LIndex = 0; LIndex < LCount; LIndex++)
            {
                int LLinkCount = FReader.ReadInt32();
                for (int LLink = 0; LLink < LLinkCount; LLink++)
                    Engine.MapPlanets[LIndex].Links.Add(Engine.MapPlanets[FReader.ReadInt32()]);
            }
        }

        private void DoReadLoadSystemBegin()
        {
            // Размеры созвездия
            Engine.MapSize.x = FReader.ReadInt32();
            Engine.MapSize.y = FReader.ReadInt32();
            // Установим свойства созвездия
            Engine.SceneManager.Load();
        }

        private void DoReadLoadSystemComplete()
        {
            Engine.IsSystemLoaded = true;
        }

        private void DoReadPlayerHangarUpdate()
        {
            int LSlot = FReader.ReadInt32();
            int LShipType = FReader.ReadInt32();
            int LCount = FReader.ReadInt32();

            Engine.UIPlayerDetails.Hangar.UpdateData(LSlot, (ShipType)LShipType, LCount);
        }

        private void DoReadPlayerStorageResize()
        {
            int LStorageSize = FReader.ReadInt32();

            Engine.Player.StorageLevel = LStorageSize;
            Engine.UIPlayerDetails.Storage.Resize(LStorageSize, false);
        }

        private void DoReadPlayerStorageUpdate()
        {
            int LSlot = FReader.ReadInt32();
            int LResID = FReader.ReadInt32();
            int LCount = FReader.ReadInt32();

            Engine.UIPlayerDetails.Storage.Change(LSlot, LResID, LCount);
        }

        private void DoReadPlayerTechShipCreate()
        {
            TechInfo LTechInfo;
            // Перебор всех корабликов
            foreach (ShipType LShip in Enum.GetValues(typeof(ShipType)))
            {
                if (LShip == ShipType.Empty)
                    continue;
                // Перебор всех технологий
                foreach (ShipTech LTech in Enum.GetValues(typeof(ShipTech)))
                {
                    if (LTech == ShipTech.Empty)
                        continue;
                    // Если теха не поддерживается - нет смысла ее считывать
                    LTechInfo.Supported = FReader.ReadBoolean();
                    if (!LTechInfo.Supported)
                        continue;
                    // Сбор сведений
                    LTechInfo.Name = FReader.ReadString();
                    LTechInfo.Description = FReader.ReadString();fg
                    LTechInfo.Level = FReader.ReadInt32();
                    LTechInfo.Count = FReader.ReadInt32();
                    
                    LTechInfo.Levels = new int[6];
                    // Перебор уровней технлогий
                    for (int LIndex = 0; LIndex <= 5; LIndex++)
                        LTechInfo.Levels[LIndex] = FReader.ReadInt32();
                    // Установим текущее значение техи
                    LTechInfo.Value = LTechInfo.Levels[LTechInfo.Level];
                    Engine.TechShip(LShip, LTech, LTechInfo);
                }
            }
        }

        private void DoReadPlayerTechBuildingCreate()
        {
            TechInfo LTechInfo;
            // Перебор всех строений
            foreach (BuildingType LBuildingType in Enum.GetValues(typeof(BuildingType)))
            {
                if (LBuildingType == BuildingType.Empty)
                    continue;
                // Перебор всех технологий
                foreach (BuildingTech LTech in Enum.GetValues(typeof(BuildingTech)))
                {
                    if (LTech == BuildingTech.Empty)
                        continue;
                    // Сбор сведений
                    LTechInfo.Name = LTech.ToString();
                    LTechInfo.Level = FReader.ReadInt32();
                    LTechInfo.Count = FReader.ReadInt32();
                    LTechInfo.Supported = FReader.ReadBoolean();
                    LTechInfo.Levels = new int[6];
                    // Перебор уровней технологий
                    for (int LIndex = 0; LIndex <= 5; LIndex++)
                        LTechInfo.Levels[LIndex] = FReader.ReadInt32();
                    // Установим текущее значение техи
                    LTechInfo.Value = LTechInfo.Levels[LTechInfo.Level];
                    Engine.TechBuilding(LBuildingType, LTech, LTechInfo);
                }
            }
        }

        private void DoReadPlayerTechShipUpdate()
        {
            ShipType LShipType = (ShipType)FReader.ReadInt32();
            ShipTech LTechType = (ShipTech)FReader.ReadInt32();
            // Обновим значение самой категории
            TechInfo LTechInfo = Engine.TechShip(LShipType, LTechType);
            LTechInfo.Level++;
            LTechInfo.Value = LTechInfo.Levels[LTechInfo.Level];
            Engine.TechShip(LShipType, LTechType, LTechInfo);
            // Обновим данные в UI
            Engine.UITechShips.UpdateTech(LShipType, LTechType);
            Engine.UIPlanetDetails.Construct.UpdateData();
        }

        private void DoReadPlayerTechBuildingUpdate()
        {
            BuildingType LBuildingType = (BuildingType)FReader.ReadInt32();
            // Пересчитаем все значения технологий для указанного типа строения
            foreach (BuildingTech LBuildingTech in Enum.GetValues(typeof(BuildingTech)))
            {
                if (LBuildingTech == BuildingTech.Empty)
                    continue;
                TechInfo LTechInfo = Engine.TechBuilding(LBuildingType, LBuildingTech);
                LTechInfo.Level++;
                LTechInfo.Value = LTechInfo.Levels[LTechInfo.Level];
                Engine.TechBuilding(LBuildingType, LBuildingTech, LTechInfo);
            }
            // Обновим данные в UI
            Engine.UIPlanetDetails.Buildings.PanelTech.UpdateTech();
        }

        private void DoReadPlayerInfo()
        {
            int LGold = FReader.ReadInt32();
            int LCredits = FReader.ReadInt32();
            int LFuel = FReader.ReadInt32();

            Engine.Player.Credits = LCredits;
            Engine.Player.Fuel = LFuel;

            //        SRPlanetarShared.UserCredits.text = LCredits.ToString();
            //        SRPlanetarShared.UserFuel.text = LFuel.ToString();
        }

        private void DoReadInfoBuildings()
        {
            foreach (BuildingType LBuildingType in Enum.GetValues(typeof(BuildingType)))
            {
                /*if (LBuildingType == BuildingType.Empty)
                    continue;
                // Загрузим основные параметры
                BuildingInfo LInfo = Engine.InfoBuildings[(int)LBuildingType];
                LInfo.BuildingType = LBuildingType;
                LInfo.Name = DoReadString();
                LInfo.Description = DoReadString();
                LInfo.ResInType = new TSHClassResourceType[BuildingInfo.C_WORK_COUNT, BuildingInfo.C_WORK_COUNT];
                LInfo.ResOutType = new TSHClassResourceType[BuildingInfo.C_WORK_COUNT];
                LInfo.ResOutCount = new int[BuildingInfo.C_WORK_COUNT];
                // Загрузим линии производства
                for (int LIndex = 0; LIndex < BuildingInfo.C_WORK_COUNT; LIndex++)
                {
                    LInfo.ResInType[0, LIndex] = (TSHClassResourceType)FReader.ReadInt32();
                    LInfo.ResInType[1, LIndex] = (TSHClassResourceType)FReader.ReadInt32();
                    LInfo.ResOutType[LIndex] = (TSHClassResourceType)FReader.ReadInt32();
                    LInfo.ResOutCount[LIndex] = FReader.ReadInt32();
                }
                Engine.InfoBuildings[(int)LBuildingType] = LInfo;*/
            }
        }

        private void DoReadInfoWarships()
        {
            /**/
        }
    }
}