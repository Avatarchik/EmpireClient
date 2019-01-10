/*///////////////////////////////////////////////
{                                              }
{ Planetar                                     }
{ Модуль реализации кнопки постройки флота     }
{ Copyright (c) 2016 UAshota                   }
{                                              }
{ Rev A  2016.12.26                            }
{ Rev B  2017.06.06                            }
{                                              }
/*///////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;

namespace Planetar
{
    public class MonoUIObjectConstructSlot : Shared.MonoInteractive
    {
        // Фон кнопки
        public Image ButtonBg;
        // Кнопка выбора
        public Button ButtonSelect;
        // Текст стоимости единицы отряда
        public Text TextPrice;
        // Цвет активной кнопки
        public Color ActiveColor;

        // Класс данных
        private Construction FSelf;

        // Отправка сообщения о покупке кораблика
        private void DoSelect()
        {
            FadeOn();
            FSelf.OnSelect(FSelf);
        }

        // Раскрасить кнопку
        public void FadeOn()
        {
            ButtonBg.color = ActiveColor;
        }

        // Вернуть прежний цвет
        public void FadeOff()
        {
            ButtonBg.color = Color.white;
        }

        // Установка значения цены, на случай если обновлена теха
        public void UpdatePrice(string AValue)
        {
            if (AValue == "")
                TextPrice.text = Engine.TechShip(FSelf.ShipType, ShipTech.Cost).Value.ToString();
            else
                TextPrice.text = AValue;
        }

        // Задание параметров для кнопки покупки
        protected override void DoInit(Shared.Interactive ASubject)
        {
            FSelf = (Construction)ASubject;
            // Событие на клик кнопки
            ButtonSelect.onClick.AddListener(DoSelect);
            ButtonSelect.image.sprite = SpriteManager.Ship(Engine.Player.Race, FSelf.ShipType);
        }
    }
}