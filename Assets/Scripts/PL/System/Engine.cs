/*///////////////////////////////////////////////
{                                              }
{ Planetar                                     }
{ Модуль глобальных переменных для планетарок  }
{ Copyright (c) 2016 UAshota                   }
{                                              }
{ Rev A  2016.12.15                            }
{ Rev B  2017.06.06                            }
{                                              }
/*///////////////////////////////////////////////
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Planetar
{

    // Информация о технологии какого-либо объекта
    public struct TechInfo
    {
        public string Name;
        public bool Supported;
        public int Count;
        public int[] Levels;
        public int Level;
        public int Value;
    }

    public class Engine : SSHShared
    {
        // Список всех кораблей системы
        public static Dictionary<int, Ship> MapShips;
        // Список всех планет системы
        public static List<Planet> MapPlanets;
        // Описание строений
        public static BuildingInfo[] InfoBuildings;
        // Описание корабликов
        public static ShipInfo[] InfoShips;
        // Активная группа кораблей
        public static ShipGroup ShipGroup;
        // Менеджер сцены
        public static MonoSceneManager SceneManager;
        // Сокет чтения
        public static SocketReader SocketReader;
        // Сокет записи
        public static SocketWriter SocketWriter;
        // UI панель описания игрока (ангар, валюта)
        public static MonoUIPanelPlayer UIPlayerDetails;
        // UI панель технологий корабликов
        public static MonoUIPanelTechShips UITechShips;
        // UI панель информации о планете
        public static MonoUIPanelPlanet UIPlanetDetails;
        // UI объект поля боя
        public static Transform UIBattlefield;
        // UI объект родителя координатной сетки
        public static Transform UIPanelGrid;

        // Кэш последней обработанной планеты
        private static Planet FLastPlanet = null;
        private static int FLastPlanetID = -1;

        // Технологии корабликов
        private static TechInfo[,] FTechShips;
        // Технологии строений
        private static TechInfo[,] FTechBuildings;


        // Загрузка объектов сцены
        public static void Load()
        {
            MapShips = new Dictionary<int, Ship>();
            MapPlanets = new List<Planet>();
            FTechShips = new TechInfo[Enum.GetValues(typeof(ShipType)).Length, Enum.GetValues(typeof(ShipTech)).Length];
            FTechBuildings = new TechInfo[Enum.GetValues(typeof(BuildingType)).Length, Enum.GetValues(typeof(BuildingTech)).Length];
            InfoBuildings = new BuildingInfo[(int)BuildingType.Empty];
            InfoShips = new ShipInfo[(int)ShipType.Empty];
            ShipGroup = new ShipGroup();
            SocketReader = new SocketReader();
            SocketWriter = new SocketWriter();
            MapPlanets.Clear();
            MapShips.Clear();
        }

        public static TechInfo TechShip(ShipType AShipType, ShipTech ATechType)
        {
            return FTechShips[(int)AShipType, (int)ATechType];
        }

        public static void TechShip(ShipType AShipType, ShipTech ATechType, TechInfo ATech)
        {
            FTechShips[(int)AShipType, (int)ATechType] = ATech;
        }

        public static TechInfo TechBuilding(BuildingType ABuildingType, BuildingTech ATechType)
        {
            return FTechBuildings[(int)ABuildingType, (int)ATechType];
        }

        public static void TechBuilding(BuildingType ABuildingType, BuildingTech ATechType, TechInfo ATech)
        {
            FTechBuildings[(int)ABuildingType, (int)ATechType] = ATech;
        }

        // Получение объекта планеты по порядковому идентификатору
        public static Planet PlanetByUID(int AUID)
        {
            if (FLastPlanetID == AUID)
                return FLastPlanet;
            else if (AUID == -1)
                return null;
            else
            {
                FLastPlanetID = AUID;
                FLastPlanet = MapPlanets[AUID];
                return FLastPlanet;
            }
        }

        // Получение объекта корабля по уникальному идентификатору
        public static Ship ShipByUID(int AUID)
        {
            if (AUID == -1)
                return null;
            else
                return MapShips[AUID];
        }
    }
}