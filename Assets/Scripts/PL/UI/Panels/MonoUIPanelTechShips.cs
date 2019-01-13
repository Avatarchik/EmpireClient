/*///////////////////////////////////////////////
{                                              }
{ Planetar                                     }
{ Панель технологий планетарных корабликов     }
{ Copyright (c) 2016 UAshota                   }
{                                              }
{ Rev A  2016.12.18                           }
{ Rev B  2017.06.06                           }
{                                              }
/*///////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace Planetar
{
    public class MonoUIPanelTechShips : MonoBehaviour
    {
        // Кэш контрола
        private Transform FTransform;
        // Местоположение корабликов выбора технологий
        private Transform FPlace;
        // Местоположение модельки выбранной технологии
        private Transform FModel;
        // Массив трейтов покупки технологии
        private List<MonoUIObjectTechShipBuy> FButtons;

        /* временный вариант отображения технологий корабликов */
        private Sprite[] sprites;

        // Предподготовка
        void DoPrepare()
        {
            FTransform = transform;
            FPlace = FTransform.Find("UITechShips");
            FModel = FTransform.Find("UITechModel");
            FButtons = new List<MonoUIObjectTechShipBuy>();
            DoCreateShips();
            FTransform.Find("UITechControls").Find("UITechBtnClose").GetComponent<Button>().onClick.AddListener(Close);
            DoChangeTech(ShipType.Millitary);
        }

        // Создание плашки корабликов
        private void DoCreateShips()
        {
            foreach (ShipType LShipType in Enum.GetValues(typeof(ShipType)))
            {
                if (LShipType == ShipType.Empty)
                    continue;
                // создадим сам слот покупки 
                Transform LShipTech = PrefabManager.CreateTechSlotSelect(FPlace, Vector3.zero).transform;
                LShipTech.SetParent(FPlace, false);
                LShipTech.GetComponent<MonoUIObjectTechShipSelect>().UpdateData(LShipType, SpriteManager.Ship(Engine.Player.Race, LShipType));
            }
        }

        // Создание плашек трейтов
        private void DoCreateTraits(ShipType AShipType)
        {
            sprites = Resources.LoadAll<Sprite>("Untitled-2");

            foreach (ShipTech LTech in Enum.GetValues(typeof(ShipTech)))
            {
                // Пропуск неподдерживаемых технологий
                if (!Engine.TechShip(AShipType, LTech).Supported)
                    continue;
                // Создание слота трейта технологии
                GameObject LTechSlot = PrefabManager.CreateTechSlotBuy(FModel, Vector3.zero);
                LTechSlot.transform.SetParent(FModel, false);
                MonoUIObjectTechShipBuy LTechScript = LTechSlot.transform.GetComponent<MonoUIObjectTechShipBuy>();
                /* временный вариант */
                LTechSlot.transform.GetComponent<Image>().sprite = sprites[0];

                LTechScript.UpdateData(AShipType, LTech);
                FButtons.Add(LTechScript);
            }

            // Размещение трейтов по окружности
            int LCount = FButtons.Count;
            for (int LIndex = 0; LIndex < LCount; LIndex++)
                FButtons[LIndex].PlaceTo(LIndex, LCount);
        }

        // Создание модельки корабликов
        private void DoCreateModel(ShipType AShipType)
        {
            // Создание главной модельки кораблика
            FModel.GetComponent<Image>().sprite = SpriteManager.Ship(Engine.Player.Race, AShipType);
        }

        // Замена текущего набора технологий на набор выбранной технологии
        private void DoChangeTech(ShipType AShipType)
        {
            // Очистка предыдущих технологий
            foreach (MonoUIObjectTechShipBuy LButton in FButtons)
                LButton.Delete();
            FButtons.Clear();
            // Создание трейтов и модельки корабликов
            DoCreateTraits(AShipType);
            DoCreateModel(AShipType);
        }

        // Обновление уровня технологии
        public void UpdateTech(ShipType AShipType, ShipTech ATechID)
        {
            // Обновление кнопки технологии
            foreach (MonoUIObjectTechShipBuy FButton in FButtons)
                if (FButton.CheckUpdate((ShipType)AShipType, (ShipTech)ATechID)) return;
        }

        // Скрыть форму
        public void Close()
        {
            FTransform.gameObject.SetActive(false);
        }

        // Показать форму покупки технологий
        public void Show(ShipType AShipType)
        {
            DoChangeTech(AShipType);
        }

        // Показать форму из UI, по дефолту военку пость смотрят
        public void Show()
        {
            if (FTransform == null)
                DoPrepare();
            FTransform.gameObject.SetActive(!FTransform.gameObject.activeSelf);
        }
    }
}