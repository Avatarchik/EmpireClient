/*///////////////////////////////////////////////
{                                              }
{ Planetar                                     }
{ Модуль кнопки выбора корабля технологии      }
{ Copyright (c) 2016 UAshota                   }
{                                              }
{ Rev A  2016.12.18                            }
{ Rev B  2017.06.06                            }
{                                              }
/*///////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;

namespace Planetar
{
    public class MonoUIObjectTechShipSelect : MonoBehaviour
    {
        // Кэш контрола
        private Transform FTransform;
        // Тип корабля, на который завязана кнопка
        private ShipType FShipType;
        // Спрайт корабля
        private Sprite FShipSprite;

        private void DoPrepare()
        {
            FTransform = transform;
            Button LButton = FTransform.GetComponent<Button>();
            LButton.onClick.AddListener(new UnityEngine.Events.UnityAction(DoSelect));
            LButton.image.sprite = FShipSprite;
        }

        // Каллбак кнопки выбора технологии
        private void DoSelect()
        {
            Engine.UITechShips.Show(FShipType);
        }

        // Задание иконки и свойств кнопки
        public void UpdateData(ShipType AShipType, Sprite AShipSprite)
        {
            FShipType = AShipType;
            FShipSprite = AShipSprite;
            DoPrepare();
        }
    }
}