/*///////////////////////////////////////////////
{                                              }
{ Модуль создания глобальных префабов          }
{       Copyright (c) 2016 UAshota             }
{                                              }
{  Rev A  2016.12.18                           }
{  Rev B  2017.05.15                           }
{                                              }
/*///////////////////////////////////////////////
using UnityEngine;

namespace Shared
{
    public class PrefabManager
    {
        // Загрузка объекта с ресурса
        public static GameObject Create(string APrefabName, Vector3 APosition)
        {
            try
            {
                // Загрузка префаба
                return Object.Instantiate(Resources.Load<GameObject>(APrefabName), APosition, Quaternion.identity);
            }
            catch
            {
                Debug.LogError("Prefab don't exists " + APrefabName);
            }
            return null;
        }

        // Взрыв кораблика
        public static GameObject ShipExplosion(Transform AParent)
        {
            GameObject LObject = Create("SH/Explosion/pfSharedShipExplosion", Vector3.zero);
            LObject.transform.SetParent(AParent, false);
            return LObject;
        }

        // Взрыв планеты
        public static GameObject PlanetExplosion(Transform AParent)
        {
            GameObject LObject = Create("SH/Explosion/pfSharedPlanetExplosion", new Vector3(0, 0, -1));
            LObject.transform.SetParent(AParent, false);
            return LObject;
        }

        // UI Загрузка сцены
        public static GameObject UILoading(Transform AParent)
        {
            GameObject LObject = Create("SH/Loading/pfSharedLoading", new Vector3(0, 0, -50));
            LObject.transform.SetParent(AParent, false);
            return LObject;
        }

        // Создание оболочки префаба ресурса
        public static GameObject CreateResourceCore()
        {
            return Create("SH/Resource/pfSHResource", Vector3.zero);
        }

        // Создание модели префаба ресурса
        public static GameObject CreateResourceModel(ResourceType AResType)
        {
            return Create("SH/Resource/" + AResType.ToString() + "/pfSHResource" + AResType.ToString(), Vector3.zero);
        }

        // Создание префаба слота хранилища
        public static GameObject CreateStorageSlot(Transform AParent)
        {
            const string LPrefabResName = "SH/Storage/pfSHStorageSlot";
            // И создадим 
            return Create(LPrefabResName, Vector3.zero);
        }

        // Создание префаба панели хинта ресурса
        public static MSHUIHintResource CreateResourceHint()
        {
            GameObject LPrefab = Create("SH/Resource/pfSHResourceHint", Vector3.zero);
            LPrefab.SetActive(false);
            LPrefab.transform.SetParent(SSHShared.UI, false);
            // Вернем ссылку на скрипт
            return LPrefab.GetComponent<MSHUIHintResource>();
        }

        public static GameObject CreateModalBackground()
        {
            const string LPrefabResName = "SH/UI/Modals/Background/pfUIModalBackground";
            // И создадим 
            return Create(LPrefabResName, Vector3.zero);
        }

        public static UIModalChangeCount CreateModalChangeCount()
        {
            const string LPrefabResName = "SH/UI/Modals/ChangeCount/pfUIModalChangeCount";
            // И создадим 
            return Create(LPrefabResName, Vector3.zero).GetComponent<UIModalChangeCount>();
        }
    }
}