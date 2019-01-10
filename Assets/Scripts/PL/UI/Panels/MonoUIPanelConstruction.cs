/*///////////////////////////////////////////////
{                                              }
{ Planetar                                     }
{ Модуль функционала строительства флота       }
{ Copyright (c) 2016 UAshota                   }
{                                              }
{ Rev A  2016.12.26                            }
{ Rev B  2017.06.06                            }
{                                              }
/*///////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace Planetar
{
    public class MonoUIPanelConstruction : MonoBehaviour
    {
        // Поле количества корабликов
        public InputField InputCount;
        // Кнопка установки максимума количества
        public Button ButtonMax;
        // Кнопка покупки выбранного кораблика
        public Button ButtonBuy;
        // Панель выбора количества корабликов
        public GameObject PanelPrice;
        // Каллбак выбора кнопки
        public delegate void OnSelect(Construction AButton);

        // Базовый флот, который строится только при наличии верфи
        private ShipType[] FShipTypesBase = new ShipType[] {
            ShipType.Cruiser,
            ShipType.Dreadnought,
            ShipType.Devastator,
            ShipType.Corvete,
            ShipType.Invader,
            ShipType.Transport
        };
        // Расширенный флот, который строится на любой своей планете
        private ShipType[] FShipTypesExt = new ShipType[] {
            ShipType.Millitary,
            ShipType.Shipyard,
            ShipType.Scient,
            ShipType.Service
        };
        // Кэш панель
        private Transform FTransform;
        // Массив слотов для быстрого переключения при смене планеты
        private Dictionary<ShipType, Construction> FSlots;
        // Предыдущий выбранный слот
        private Construction FLastSlot;
        // Выбранное количество корабликов для постройки
        private int FShipCount;
        // Максимальное количество корабликов для постройки
        private int FShipMax;
        // Цена выбранного стека
        private int FShipCost;
        // Количество модулей для постройки
        private int FModulesCount;
        // Уровень доступа
        private int FLevel;

        // Предподготовка данных
        void DoPrepareData()
        {
            FTransform = transform;
            FSlots = new Dictionary<ShipType, Construction>();
            DoCreateSlot(FShipTypesBase);
            DoCreateSlot(FShipTypesExt);
            ButtonBuy.onClick.AddListener(DoConstruct);
            ButtonMax.onClick.AddListener(DoSetMax);
            // Пока такой хак на постройку по клавише
            InputCount.onEndEdit.AddListener(Value =>
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                    DoConstruct();
            });
            // Получение ввода количества корабликов
            InputCount.onValueChanged.AddListener(Value =>
            {
                int LResult;
                if (!int.TryParse(Value, out LResult) || (LResult < 0))
                    InputCount.text = FShipMax.ToString();
                else
                    DoButtonChangePrice(LResult);
            });
        }

        // Создание массива слотов, основанных на наборе корабликов
        void DoCreateSlot(ShipType[] AShipTypes)
        {
            Construction LSlot;
            foreach (ShipType LShipType in AShipTypes)
            {
                LSlot = new Construction(FTransform, LShipType)
                {
                    OnSelect = DoButtonSelect
                };
                FSlots.Add(LShipType, LSlot);
            }
        }

        // Показ определенного набора корабликов по условию
        void DoShowSlot(ShipType[] AShipTypes, bool AVisible)
        {
            foreach (ShipType LShipType in AShipTypes)
            {
                // Обновим цены
                if (FSlots[LShipType] == FLastSlot)
                    DoButtonChangePrice(FShipCount);
                else
                    FSlots[LShipType].UpdatePrice();
                // Включим и выключим слоты
                FSlots[LShipType].Transform.gameObject.SetActive(
                    AVisible && Engine.TechShip(LShipType, ShipTech.Active).Value == 1);
            }
        }

        // Кэширование значений для дальнейшей покупки корабликов
        void DoCalculateValues()
        {
            // Смотрим сколько можно построить
            int LCount = Engine.TechShip(FLastSlot.ShipType, ShipTech.Count).Value;
            FShipCost = Engine.TechShip(FLastSlot.ShipType, ShipTech.Cost).Value;
            FShipMax = Mathf.Min(Mathf.FloorToInt(FModulesCount / FShipCost), LCount);
        }

        // Выделение текста и активация поля ввода
        void DoSelectInput()
        {
            InputCount.ActivateInputField();
            InputCount.Select();
        }

        // Смены цены покупки стека
        void DoButtonChangePrice(int ACount)
        {
            FShipCount = Mathf.Min(FShipMax, ACount);
            // Если можно купить несколько кораблей, распишем формулу покупки
            if (FShipCount > 0)
                FLastSlot.UpdatePrice(FShipCount.ToString() + " x " + FShipCost.ToString() + " = " + SSHLocale.CountToLongString(ACount * FShipCost));
            else
                FLastSlot.UpdatePrice();
            InputCount.text = FShipCount.ToString();
            DoSelectInput();
        }

        // Установка максимального количества корабликов для покупки
        void DoSetMax()
        {
            DoButtonChangePrice(FShipMax);
        }

        // Обработка нажатия на кнопку выбора корабля
        void DoButtonSelect(Construction ASlot)
        {
            if (FLastSlot != null)
            {
                FLastSlot.UpdatePrice();
                FLastSlot.FadeOff();
            }
            else
                PanelPrice.gameObject.SetActive(true);
            FLastSlot = ASlot;

            DoCalculateValues();
            DoSetMax();
            ASlot.FadeOn();
        }

        // Отправка команды серверу на постройку кораблика
        void DoConstruct()
        {
            if (FShipCount > 0)
                Engine.SocketWriter.ShipConstruct(Engine.UIPlanetDetails.ActivePlanetId(), (int)FLastSlot.ShipType, FShipCount);
            DoSelectInput();
        }

        // Внешний метод обновления сведений
        public void ChangeData(int AModulesCount, bool ASelf, bool AHaveShipyards, bool AHaveShips)
        {
            if (FTransform == null)
                DoPrepareData();
            FModulesCount = AModulesCount;
            FLevel = Convert.ToInt32(ASelf) + Convert.ToInt32(AHaveShipyards && AHaveShips);
            UpdateData();
        }

        // Внешний метод смены состояния за счет пересчета технологий
        public void UpdateData()
        {
            if (Engine.UIPlanetDetails.ActivePlanet() != null)
            {
                DoShowSlot(FShipTypesBase, FLevel > 1);
                DoShowSlot(FShipTypesExt, FLevel > 0);
                if (FLastSlot != null)
                    DoCalculateValues();
            }
        }

        // Обнуление ранее выбранных параметров
        public void Close()
        {
            PanelPrice.gameObject.SetActive(false);
            if (FLastSlot != null)
            {
                FLastSlot.UpdatePrice();
                FLastSlot.FadeOff();
                FLastSlot = null;
            }
        }
    }
}