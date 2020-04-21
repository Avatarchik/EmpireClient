/*///////////////////////////////////////////////
{                                              }
{ Planetar                                     }
{ Модуль создания планетарных префабов         }
{ Copyright (c) 2016 UAshota                   }
{                                              }
{ Rev A  2016.12.18                            }
{ Rev B  2017.06.06                            }
{                                              }
/*///////////////////////////////////////////////
using UnityEngine;

namespace Planetar
{
    public class PrefabManager : Shared.PrefabManager
    {
        // Создание префаба ядра корабля
        private static GameObject CreateShipCore()
        {
            // Путь к каталогу префабов
            const string LPrefabPath = "PL/Ship/";
            string LPrefabShipName = "/pfPLShip";
            // Создадим объекты с префабов
            return Create(LPrefabPath + "Shared" + LPrefabShipName, Vector3.zero);
        }

        // Создание префаба модульки корабля
        public static GameObject CreateShipModel(Transform AParent, SSHRace ARace, ShipType AShipType)
        {
            // Путь к каталогу префабов
            const string LPrefabPath = "PL/Ship/";
            string LPrefabShipName = "/pfPLShip";
            string LPrefabModelName = "";
            string LRaceName = ARace.ToString();
            // Выбор префаба на основе корабля
            if (AShipType == ShipType.Flagship)
                LPrefabModelName = string.Concat(AShipType.ToString(), LPrefabShipName, AShipType.ToString());
            else
                LPrefabModelName = string.Concat(LRaceName, LPrefabShipName, AShipType.ToString(), LRaceName);
            GameObject LModel = Create(LPrefabPath + LPrefabModelName, Vector3.zero);
            LModel.transform.SetParent(AParent, false);
            LModel.name = "Model";
            return LModel;
        }

        // Создание префаба планетарного кораблика
        public static GameObject CreateShip(Transform AParent, SSHRace ARace, ShipType AShipType)
        {
            GameObject LShipCore = CreateShipCore();
            GameObject LShipModel = CreateShipModel(AParent, ARace, AShipType);
            LShipModel.transform.SetParent(LShipCore.transform, false);
            LShipCore.transform.SetParent(AParent, false);

            return LShipCore;
        }

        // Создание префаба панели хинта кораблика
        public static MonoUIHintShip CreateShipHint()
        {
            GameObject LPrefab = Create("PL/Ship/Shared/pfPlShipHint", Vector3.zero);
            LPrefab.SetActive(false);
            LPrefab.transform.SetParent(Engine.UI, false);
            // Вернем ссылку на скрипт
            return LPrefab.GetComponent<MonoUIHintShip>();
        }

        // Создание префаба панели меню кораблика
        public static MonoUIPopupShip CreateShipPopup()
        {
            GameObject LPrefab = Create("PL/Ship/Shared/pfPlShipPopup", Vector3.zero);
            LPrefab.SetActive(false);
            LPrefab.transform.SetParent(Engine.UI, false);
            // Вернем ссылку на скрипт
            return LPrefab.GetComponent<MonoUIPopupShip>();
        }

        // Создание префаба элемента меню кораблика
        public static MonoUIPopupShipItem CreateShipPopupItem(Transform AParent)
        {
            GameObject LPrefab = Create("PL/UI/Popups/pfPopupShipItem", Vector3.zero);
            LPrefab.transform.SetParent(AParent, false);
            // Вернем ссылку на скрипт
            return LPrefab.GetComponent<MonoUIPopupShipItem>();
        }

        // Создание префаба панели хинта планеты
        public static MonoUIHintPlanet CreatePlanetHint()
        {
            GameObject LPrefab = Create("PL/Planet/Shared/pfPlPlanetHint", Vector3.zero);
            LPrefab.SetActive(false);
            LPrefab.transform.SetParent(Engine.UI, false);
            // Вернем ссылку на скрипт
            return LPrefab.GetComponent<MonoUIHintPlanet>();
        }

        // Создание префаба каркаса планетарной планеты
        public static GameObject CreatePlanet(Vector3 APosition)
        {
            string LPrefabPath = "PL/Planet/";
            string LPrefabPlanetName = "Shared/pfPLPlanet";
            // Сперва создать планету
            return Create(LPrefabPath + LPrefabPlanetName, APosition);
        }

        // Создание префаба сферы планетарной планеты
        public static GameObject CreatePlanetSphere(PlanetType APlanetType, PlanetMode aMode)
        {
            string LPrefabPath = "PL/Planet/";
            int LSkinID;
            string LPrefabPlanetSphereType = APlanetType.ToString();
            // Временное украшательство
            if (APlanetType == PlanetType.Earth && aMode == PlanetMode.Big)
            {
                LPrefabPlanetSphereType = "Big";
                LSkinID = Random.Range(1, 4);
            }
            else if (APlanetType == PlanetType.Earth && aMode == PlanetMode.Normal)
            {
                LPrefabPlanetSphereType = "Small";
                LSkinID = Random.Range(1, 4);
            }
            else
                    if (APlanetType == PlanetType.Hydro)
                LSkinID = Random.Range(1, 3);
            else
                        if (APlanetType == PlanetType.Sun)
                LSkinID = 2;
            else
                LSkinID = 1;
            string LPrefabPlanetSphere = LPrefabPlanetSphereType + "/pfPLPlanetSphere" + LPrefabPlanetSphereType + LSkinID.ToString();
            return Create(LPrefabPath + LPrefabPlanetSphere, Vector3.zero);
        }

        // Создание префаба снаряда планетарного корабля
        public static GameObject CreateShell(Transform AParent, Vector3 APosition, ShipShellType AShellType)
        {
            string LPrefabName = "PL/Shell/" + AShellType.ToString() + "/pfPLShell" + AShellType.ToString();
            // И создадим с выключенной анимацией
            return Create(LPrefabName, APosition);
        }

        // Создание префаба посадочного слота планеты
        public static GameObject CreateLandingSlot(Transform AParent)
        {
            const string LPrefabResName = "PL/Planet/Shared/pfPLPlanetSlot";
            // И создадим 
            return Create(LPrefabResName, Vector3.zero);
        }

        // Создание префаба слота постройки планетарного кораблика
        public static GameObject CreateConstructionSlot(Transform AParent)
        {
            const string LPrefabResName = "PL/Construct/pfPLConstructionSlot";
            // И создадим 
            return Create(LPrefabResName, Vector3.zero);
        }

        // Создание префаба слота кнопки выбора кораблика на панели технологий корабликов
        public static GameObject CreateTechSlotSelect(Transform AParent, Vector3 APosition)
        {
            const string LPrefabResName = "PL/TechWarShip/pfPLTechShipSelect";
            // И создадим 
            return Create(LPrefabResName, APosition);
        }

        // Создание префаба слота кнопки покупки технологии на панели технологий корабликов
        public static GameObject CreateTechSlotBuy(Transform AParent, Vector3 APosition)
        {
            const string LPrefabResName = "PL/TechWarShip/pfPLTechShipBuy";
            // И создадим 
            return Create(LPrefabResName, APosition);
        }

        // Создание префаба слота планетарной постройки
        public static GameObject CreateBuildingSlot(Transform AParent, Vector3 APosition)
        {
            const string LPrefabResName = "PL/Building/Shared/pfPLBuilding";
            // И создадим 
            return Create(LPrefabResName, APosition);
        }

        // Создание префаба слота технологии планетарной постройки
        public static GameObject CreateBuildingBuySlot(Transform AParent, Vector3 APosition)
        {
            const string LPrefabResName = "PL/Building/Shared/pfPLBuildingBuy";
            // И создадим 
            return Create(LPrefabResName, APosition);
        }

        // Создание префаба планетарной постройки
        public static GameObject CreateBuilding(Transform AParent, Vector3 APosition, BuildingType AType, int ALevel)
        {
            string LPrefabResName = "PL/Building/" + AType.ToString() + "/pfPLBuilding" + AType.ToString() + "Lvl" + ALevel.ToString();
            // И создадим 
            return Create(LPrefabResName, APosition);
        }

        // Создание префаба слота ангара
        public static GameObject CreateHangarSlot(Transform AParent)
        {
            const string LPrefabResName = "PL/Hangar/pfPlHangarSlot";
            // И создадим 
            return Create(LPrefabResName, Vector3.zero);
        }

        // Создание префаба панели хинта ангара
        public static MonoUIHintHangar CreateHangarHint()
        {
            GameObject LPrefab = Create("PL/Hangar/pfPLHangarHint", Vector3.zero);
            LPrefab.SetActive(false);
            LPrefab.transform.SetParent(Engine.UI, false);
            // Вернем ссылку на скрипт
            return LPrefab.GetComponent<MonoUIHintHangar>();
        }

        // Создание префаба координатной сетки
        public static GameObject CreatePlaneGrid(float ACoordX, float ACoordY)
        {
            const string LPrefabResName = "PL/Place/pfPLPlaceMapGrid";
            // И создадим 
            return Create(LPrefabResName, new Vector3(ACoordX, 0, ACoordY));
        }
    }
}