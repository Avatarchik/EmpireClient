/*///////////////////////////////////////////////
{                                              }
{ Planetar                                     }
{ Модуль управления массивами спрайтов         }
{ Copyright (c) 2016 UAshota                   }
{                                              }
{ Rev A  2016.12.18                            }
{ Rev B  2017.06.06                            }
{                                              }
/*///////////////////////////////////////////////
using UnityEngine;

namespace Planetar
{
    public class SpriteManager
    {
        // Спрайты корабликов
        private static Sprite[] FSpritesShips;
        // Спрайты строений
        private static Sprite[] FSpritesBuildings;
        // Спрайты подложки строений
        private static Sprite[] FSpritesBuildingPlace;

        // Спрайт кораблика
        public static Sprite Ship(SSHRace ARace, ShipType AShipType)
        {
            if (FSpritesShips == null)
                FSpritesShips = Resources.LoadAll<Sprite>("PL/Ship/Shared/Textures/PLShipPanel");
            // Смещение спрайтов
            int LSpriteOffset = (((short)ARace - 1) * ((int)ShipType.Flagship + 1)) + (short)AShipType - 1;
            // Вернуть иконку корабля
            return FSpritesShips[LSpriteOffset];
        }

        // Спрайт строения
        public static Sprite Building(BuildingType AType, int ALevel)
        {
            if (FSpritesBuildings == null)
                FSpritesBuildings = Resources.LoadAll<Sprite>("PL/Building/Shared/Textures/PLBuildingPanel");
            // Смещение спрайтов
            int LSpriteOffset = (int)AType * 5 + ALevel - 1;
            // Вернуть иконку строения
            return FSpritesBuildings[LSpriteOffset];
        }

        // Спрайт подложки строения
        public static Sprite BuildingPlace(PlanetType AType, bool AActive)
        {
            if (FSpritesBuildingPlace == null)
                FSpritesBuildingPlace = Resources.LoadAll<Sprite>("PL/Building/Shared/Textures/PLBuildingPlace");
            // Смещение спрайтов
            int LSpriteOffset;
            if (AActive)
                LSpriteOffset = (int)AType * 6 + Random.Range(0, 5);
            else
                LSpriteOffset = (int)AType * 6 + 5;
            // Вернуть иконку строения
            return FSpritesBuildingPlace[LSpriteOffset];
        }

        // Спрайт миникарты для владельца планеты
        public static Sprite MinimapPlanetCore(Planet APlanet)
        {
            string LSprite = null;
            // Временное решение сокрытия роли без отписки
            SSHRole LRole;
            if (!APlanet.IsVisible() && APlanet.PlanetType != PlanetType.Hole)
                LRole = SSHRole.Neutral;
            else
                LRole = APlanet.Owner.Role;
            // Не показываем неактивные ЧТ
            if ((APlanet.PlanetType == PlanetType.Hole) && (APlanet.State == PlanetState.Inactive))
                return null;
            else if (APlanet.PlanetType == PlanetType.Earth && APlanet.PlanetMode == PlanetMode.Big)
                LSprite = "PL/Radar/Textures/PLRadarMapCore" + LRole.ToString() + "Big";
            else if (APlanet.PlanetType == PlanetType.Pulsar)
                LSprite = "PL/Planet/" + APlanet.PlanetType.ToString() + "/Textures/PLPlanet" + APlanet.PlanetType.ToString() + "Minimap";
            else if (APlanet.PlanetType == PlanetType.Hole)
            {
                if (APlanet.PlanetMode == PlanetMode.Big)
                {
                    if (APlanet.State == PlanetState.Active)
                        LSprite = "PL/Planet/" + APlanet.PlanetType.ToString() + "/Textures/PLPlanet" + APlanet.PlanetType.ToString() + "MinimapActive";
                    else
                        LSprite = "PL/Planet/" + APlanet.PlanetType.ToString() + "/Textures/PLPlanet" + APlanet.PlanetType.ToString() + "MinimapPassive";
                }
                else
                    LSprite = "PL/Planet/" + APlanet.PlanetType.ToString() + "/Textures/PLPlanet" + APlanet.PlanetType.ToString() + "MinimapSimple";
            }
            else
                LSprite = "PL/Radar/Textures/PLRadarMapCore" + LRole.ToString() + "Small";
            // Вернем спрайт
            return Resources.Load<Sprite>(LSprite);
        }

        // Спрайт миникарты для закраски контроля
        public static Sprite MinimapPlanetControl(Planet APlanet)
        {
            string LSprite = null;
            if ((APlanet.PlanetType == PlanetType.Hole) && (APlanet.State == PlanetState.Inactive))
                return null;
            // Враг и наложение
            else if (APlanet.IsCoverageEnemy)
            {
                if (APlanet.IsCoverageSelf || APlanet.IsCoverageFriends)
                    LSprite = "PL/Radar/Textures/PLRadarMapControlBoth";
                else
                    LSprite = "PL/Radar/Textures/PLRadarMapControlEnemy";
            }
            // Свой
            else if (APlanet.IsCoverageSelf)
                LSprite = "PL/Radar/Textures/PLRadarMapControlSelf";
            // Союзник
            else if (APlanet.IsCoverageFriends)
                LSprite = "PL/Radar/Textures/PLRadarMapControlFriend";
            // Вернем спрайт
            return Resources.Load<Sprite>(LSprite);
        }

        // Спрайт миникарты для обозначения наличия корабликов
        public static Sprite MinimapPlanetWar(bool ASelf, bool AEnemy, bool AFriend)
        {
            string LSprite;
            if (ASelf && AEnemy)
                LSprite = "PL/Radar/Textures/PLRadarShipsInBattle";
            else if (ASelf)
                LSprite = "PL/Radar/Textures/PLRadarShipsSelf";
            else if (AEnemy)
                LSprite = "PL/Radar/Textures/PLRadarShipsEnemy";
            else
                return null;
            return Resources.Load<Sprite>(LSprite);
        }
    }
}