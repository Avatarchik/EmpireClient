/*///////////////////////////////////////////////
{                                              }
{ Planetar                                     }
{ Панель технологий планетарных строений       }
{ Copyright (c) 2016 UAshota                   }
{                                              }
{ Rev A  2016.12.18                            }
{ Rev B  2017.06.06                            }
{                                              }
/*///////////////////////////////////////////////
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Planetar
{
    public class MonoUIPanelTechBuildings : MonoBehaviour
    {
        // Кэш панели
        private Transform FTransform;
        // Список доступных строений
        private List<MonoUIObjectBuildingBuy> FBuildings;
        // Позиция здания
        private int FPosition;
        // Тип здания, для которого открыта панель
        private BuildingType FBuildingType;
        // Уровень здания, для которого открыта панель
        private int FLevel;

        // Создание списка строений
        private void DoCreateBuildings()
        {
            Transform LBuildingSlot;
            MonoUIObjectBuildingBuy LBuilding;
            FBuildings = new List<MonoUIObjectBuildingBuy>();
            // На каждый тип строения по зданию
            foreach (BuildingType LBuildingType in Enum.GetValues(typeof(BuildingType)))
            {
                if (LBuildingType > BuildingType.Modules)
                    continue;
                // Сперва слот строения
                LBuildingSlot = PrefabManager.CreateBuildingBuySlot(FTransform, Vector3.zero).transform;
                LBuildingSlot.SetParent(FTransform, false);
                // И в него само строение
                LBuilding = LBuildingSlot.GetComponent<MonoUIObjectBuildingBuy>();
                LBuilding.InitData(FPosition, LBuildingType);
                // Добавим в список строений
                FBuildings.Add(LBuilding);
            }
            UpdateTech();
        }

        // Обновление уровня технологии
        public void UpdateTech()
        {
            // Обновление кнопки возможности покупки
            foreach (MonoUIObjectBuildingBuy LBuilding in FBuildings)
                LBuilding.UpdateBuildingTech(FBuildingType, FLevel, FPosition);
        }

        // Показ панели строений
        public void Show(int APosition, BuildingType AType, int ALevel)
        {
            FLevel = ALevel;
            FPosition = APosition;
            FBuildingType = AType;
            // Предварительное создание всех строений
            if (FTransform == null)
            {
                FTransform = transform;
                DoCreateBuildings();
            }
            UpdateTech();
            FTransform.parent.gameObject.SetActive(true);
        }

        // Скрытие панели строений
        public void Close()
        {
            FTransform.parent.gameObject.SetActive(false);
        }
    }
}