/*///////////////////////////////////////////////
{                                              }
{ Planetar                                     }
{ Модуль отправки запросов на сервер           }
{ Copyright (c) 2016 UAshota                   }
{                                              }
{ Rev A  2016.11.22                            }
{ Rev B  2017.06.06                            }
{                                              }
/*///////////////////////////////////////////////
using System.IO;
using System.Collections.Generic;

namespace Planetar
{
    public class SocketWriter : SSHSocketWriter
    {
        #region Константы
        // Объединение стеков
        private const int C_SHIP_MERGE = 0x1000;
        // Перемещение кораблика между планетами
        private const int C_SHIP_MOVE_TO = 0x1001;
        // Смена аттача к планете
        private const int C_SHIP_ATTACH_TO = 0x1002;
        // Смена транспортного пути
        private const int C_PLANET_TRADE_PATH = 0x1003;
        // Выгрузка кораблика с ангара на планетоид
        private const int C_SHIP_FROM_HANGAR = 0x1004;
        // Подписка на события на планете
        private const int C_PLANET_SUBSCRIBE = 0x1005;
        // Отправка кораблика с планеты в ангар
        private const int C_SHIP_TO_HANGAR = 0x1006;
        // Запрос показа деталей планеты
        private const int C_PLANET_SHOW_DETAILS = 0x1007;
        // Перемещение ресурса в хранилище
        private const int C_RESOURCE_MOVE = 0x1008;
        // Постройка корабля
        private const int C_SHIP_CONSTRUCT = 0x1009;
        // Покупка технологии флота
        private const int C_TECH_SHIP_BUY = 0x100A;
        // Постройка здания
        private const int C_PLANET_BUILDING_CONSTRUCT = 0x100B;
        // Апгрейд здания
        private const int C_PLANET_BUILDING_UPGRADE = 0x100C;
        // Смена походки кораблика
        private const int C_SHIP_CHANGE_ACTIVE = 0x100D;
        // Уравнивание стекоа
        private const int C_SHIP_HYPODISPERSION = 0x100E;
        // Отправка группы кораблей
        private const int C_SHIP_MOVE_TO_GROUP = 0x100F;
        // Разделение стеков стеков
        private const int C_SHIP_SEPARATE = 0x1010;
        // Сцена готова принять данные
    /*    private const int CmdPlanetarReadyToLoad = 0x1011;*/
        // Создать портал между двумя объектами
        private const int C_SHIP_PORTAL_OPEN = 0x1012;
        // Уничтожить стек
        private const int C_SHIP_DELETE = 0x1013;
        // Закрыть портал
        private const int C_SHIP_PORTAL_CLOSE = 0x1014;
        // Прыжок в портал
        private const int C_SHIP_PORTAL_JUMP = 0x1015;
        // Аннигиляция
        private const int C_SHIP_ANNIHILATION = 0x1016;
        // Конструктор
        private const int C_SHIP_CONSTRUCTOR = 0x1017;
        // Поменять местами слоты в ангаре
        private const int C_SHIP_HANGAR_SWAP = 0x1018;
        #endregion

        public void PlanetarSubscribe()
        {
            BinaryWriter LWriter = DoOpen(CmdPlanetarSubscribe);
            LWriter.Write(Engine.PlanetarID);
            DoClose(LWriter);
        }

        public void ShipMerge(Ship ASource, Ship ATarget, int ACount)
        {
            BinaryWriter LWriter = DoOpen(C_SHIP_MERGE);
            LWriter.Write(ASource.Planet.UID);
            LWriter.Write(ASource.UID);
            LWriter.Write(ATarget.UID);
            LWriter.Write(ACount);
            DoClose(LWriter);
        }

        public void ShipSeparate(Ship ASource, int ATarget, int ACount)
        {
            BinaryWriter LWriter = DoOpen(C_SHIP_SEPARATE);
            LWriter.Write(ASource.Planet.UID);
            LWriter.Write(ASource.UID);
            LWriter.Write(ATarget);
            LWriter.Write(ACount);
            DoClose(LWriter);
        }

        public void ShipMoveTo(Ship AShip, Planet ATarget, int ATargetSlot)
        {
            BinaryWriter LWriter = DoOpen(C_SHIP_MOVE_TO);
            LWriter.Write(AShip.Planet.UID);
            LWriter.Write(AShip.UID);
            LWriter.Write(ATarget.UID);
            LWriter.Write(ATargetSlot);
            DoClose(LWriter);
        }

        public void ShipAttachTo(Ship AShip, int AUID)
        {
            BinaryWriter LWriter = DoOpen(C_SHIP_ATTACH_TO);
            LWriter.Write(AShip.Planet.UID);
            LWriter.Write(AShip.UID);
            LWriter.Write(AUID);
            DoClose(LWriter);
        }

        public void ShipFromHangar(int AHangarSlot, int ATargetPlanet, int ATargetSLot, int ACount)
        {
            BinaryWriter LWriter = DoOpen(C_SHIP_FROM_HANGAR);
            LWriter.Write(AHangarSlot);
            LWriter.Write(ATargetPlanet);
            LWriter.Write(ATargetSLot);
            LWriter.Write(ACount);
            DoClose(LWriter);
        }

        public void ShipToHangar(int AHangarSlot, Ship AShip)
        {
            BinaryWriter LWriter = DoOpen(C_SHIP_TO_HANGAR);
            LWriter.Write(AShip.Planet.UID);
            LWriter.Write(AShip.UID);
            LWriter.Write(AHangarSlot);
            DoClose(LWriter);
        }

        public void ShipHypodispersion(Ship AShip)
        {
            BinaryWriter LWriter = DoOpen(C_SHIP_HYPODISPERSION);
            LWriter.Write(AShip.Planet.UID);
            LWriter.Write(AShip.UID);
            DoClose(LWriter);
        }

        public void ShipMoveToGroup(ShipGroup AGroup)
        {
            BinaryWriter LWriter = DoOpen(C_SHIP_MOVE_TO_GROUP);
            LWriter.Write(AGroup.Planets.Count);
            LWriter.Write(AGroup.Ships.Count);
            foreach (Planet LPLanet in AGroup.Planets)
                LWriter.Write(LPLanet.UID);
            foreach (Ship LShip in AGroup.Ships)
            {
                LWriter.Write(LShip.Planet.UID);
                LWriter.Write(LShip.UID);
            }
            DoClose(LWriter);
        }

        public void HangarSwap(int AHangarFrom, int AHangarTo)
        {
            BinaryWriter LWriter = DoOpen(C_SHIP_HANGAR_SWAP);
            LWriter.Write(AHangarFrom);
            LWriter.Write(AHangarTo);
            DoClose(LWriter);
        }

        public void ShipSkillConstructor(Ship ASource, Ship ATarget)
        {
            BinaryWriter LWriter = DoOpen(C_SHIP_CONSTRUCTOR);
            LWriter.Write(ASource.Planet.UID);
            LWriter.Write(ASource.UID);
            LWriter.Write(ATarget.UID);
            DoClose(LWriter);
        }

        public void PlanetTradePath(int ASource, int ATarget)
        {
            BinaryWriter LWriter = DoOpen(C_PLANET_TRADE_PATH);
            LWriter.Write(ASource);
            LWriter.Write(ATarget);
            DoClose(LWriter);
        }

        public void PlanetSubscribe(Planet APlanet)
        {
            // Отправим запрос
            BinaryWriter LWriter = DoOpen(C_PLANET_SUBSCRIBE);
            LWriter.Write(APlanet.UID);
            DoClose(LWriter);
        }

        public void PlanetShowDetails(int APlanet)
        {
            BinaryWriter LWriter = DoOpen(C_PLANET_SHOW_DETAILS);
            LWriter.Write(APlanet);
            DoClose(LWriter);
        }

        public void ResourceMove(int ASourcePlanetID, int ASourceSlot, int ATargetPlanetID, int ATargetSlot, int ACount, bool AOnePlace)
        {
            BinaryWriter LWriter = DoOpen(C_RESOURCE_MOVE);
            LWriter.Write(ASourcePlanetID);
            LWriter.Write(ASourceSlot);
            LWriter.Write(ATargetPlanetID);
            LWriter.Write(ATargetSlot);
            LWriter.Write(ACount);
            LWriter.Write(AOnePlace);
            DoClose(LWriter);
        }

        public void ShipConstruct(int APlanetID, int AType, int ACount)
        {
            BinaryWriter LWriter = DoOpen(C_SHIP_CONSTRUCT);
            LWriter.Write(APlanetID);
            LWriter.Write(AType);
            LWriter.Write(ACount);
            DoClose(LWriter);
        }

        public void TechShipBuy(int AShipID, int ATechID)
        {
            BinaryWriter LWriter = DoOpen(C_TECH_SHIP_BUY);
            LWriter.Write(AShipID);
            LWriter.Write(ATechID);
            DoClose(LWriter);
        }

        public void PlanetBuildingConstruct(int APlanetID, int APosition, int ABuildingType)
        {
            BinaryWriter LWriter = DoOpen(C_PLANET_BUILDING_CONSTRUCT);
            LWriter.Write(APlanetID);
            LWriter.Write(APosition);
            LWriter.Write(ABuildingType);
            DoClose(LWriter);
        }

        public void PlanetBuildingUpgrade(int ABuildingType)
        {
            BinaryWriter LWriter = DoOpen(C_PLANET_BUILDING_UPGRADE);
            LWriter.Write(ABuildingType);
            DoClose(LWriter);
        }

        public void ShipChangeActive(Ship AShip)
        {
            BinaryWriter LWriter = DoOpen(C_SHIP_CHANGE_ACTIVE);
            LWriter.Write(AShip.Planet.UID);
            LWriter.Write(AShip.UID);
            DoClose(LWriter);
        }

        public void ShipPortalOpen(PortalShip APortal, Planet ADestination)
        {
            BinaryWriter LWriter = DoOpen(C_SHIP_PORTAL_OPEN);
            LWriter.Write(APortal.Initiator.Planet.UID);
            LWriter.Write(APortal.Initiator.UID);
            LWriter.Write(ADestination.UID);
            DoClose(LWriter);
        }

        public void ShipPortalClose(Portal APortal)
        {
            BinaryWriter LWriter = DoOpen(C_SHIP_PORTAL_CLOSE);
            LWriter.Write(APortal.Source.UID);
            DoClose(LWriter);
        }

        public void ShipPortalJump(Ship AShip)
        {
            BinaryWriter LWriter = DoOpen(C_SHIP_PORTAL_JUMP);
            LWriter.Write(AShip.Planet.UID);
            LWriter.Write(AShip.UID);
            DoClose(LWriter);
        }

        public void ShipDelete(Ship AShip)
        {
            BinaryWriter LWriter = DoOpen(C_SHIP_DELETE);
            LWriter.Write(AShip.Planet.UID);
            LWriter.Write(AShip.UID);
            DoClose(LWriter);
        }

        public void ShipAnnihilation(Ship AShip)
        {
            BinaryWriter LWriter = DoOpen(C_SHIP_ANNIHILATION);
            LWriter.Write(AShip.Planet.UID);
            LWriter.Write(AShip.UID);
            DoClose(LWriter);
        }
    }
}