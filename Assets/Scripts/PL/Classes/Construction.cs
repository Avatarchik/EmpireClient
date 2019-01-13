/*///////////////////////////////////////////////
{                                              }
{ Planetar                                     }
{ Интерактивный класс слота постройки флота    }
{ Copyright (c) 2016 UAshota                   }
{                                              }
{ Rev A  2016.12.26                            }
{ Rev B  2017.06.06                            }
{                                              }
/*///////////////////////////////////////////////
using UnityEngine;

namespace Planetar
{
    // Тип внешнего класса слота постройки кораблей
    public class Construction : Shared.Interactive
    {
        // Ссылка на игровой скрипт
        private MonoUIObjectConstructSlot FScript;
        // Тип корабля, закрепленного за кнопкой
        public ShipType ShipType;
        // Каллбак при нажатии на кнопку
        public MonoUIPanelConstruction.OnSelect OnSelect;

        // Конструктор сразу определяет тип данных
        public Construction(Transform AParent, ShipType AShipType)
        {
            ShipType = AShipType;
            Transform = PrefabManager.CreateConstructionSlot(AParent).transform;
            Transform.SetParent(AParent, false);
            FScript = Transform.GetComponent<MonoUIObjectConstructSlot>();
            FScript.Init(this);
        }

        // Значение стоимости объекта
        public void UpdatePrice(string AValue = "")
        {
            FScript.UpdatePrice(AValue);
        }

        // Подсветить слот
        public void FadeOn()
        {
            FScript.FadeOn();
        }

        // Убрать подсветку слота
        public void FadeOff()
        {
            FScript.FadeOff();
        }
    }
}