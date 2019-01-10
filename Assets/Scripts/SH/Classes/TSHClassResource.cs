/*///////////////////////////////////////////////
{                                              }
{     Внешний класс единицы ресурса            }
{       Copyright (c) 2016 UAshota             }
{                                              }
{  Rev A  2016.11.15                           }
{  Rev B  2017.05.15                           }
{                                              }
/*///////////////////////////////////////////////
using UnityEngine;

namespace Shared
{
    // Типы ресурсов
    public enum ResourceType
    {
        Empty,
        Vodorod,
        Xenon,
        Modules,
        Fuel,
        Gold,
        Titan,
        Kremniy,
        Antikristals,
        Antimatery,
        Metall,
        Electronic,
        Eda,
        Protoplazma,
        Plazma
    }

    // Тип внешнего класса описания ресурса
    public class Resource : Interactive
    {
        // Делегат действий с ресурсом
        public delegate void TSHCallbackResourceMove(Resource AResource, bool AHotkey);
        // Модель ресурса
        public Transform Model;
        // Слот, на котором лежит ресурс
        public TSHClassStorageSlot Slot;
        // Тип ресурса
        public ResourceType ResType;
        // Количество ресурса
        public int Count;
        // Каллбак действия с ресурсом
        public TSHCallbackResourceMove OnResourceMove;

        // Привязка к игровому скрипту
        private MSHObjectResource FScript;

        // Конструктор сразу определяет тип данных
        public Resource(Transform AParent, ResourceType AResType, TSHClassStorageSlot ASlot)
        {
            UID = (int)AResType;
            ResType = AResType;
            Slot = ASlot;
            Model = PrefabManager.CreateResourceModel(AResType).transform;
            Transform = PrefabManager.CreateResourceCore().transform;
            Transform.SetParent(AParent, false);
            Model.SetParent(Transform, false);
            FScript = Transform.GetComponent<MSHObjectResource>();
            FScript.Init(this);
        }
    }
}