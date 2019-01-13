/*///////////////////////////////////////////////
{                                              }
{ Planetar                                     }
{ Модуль реализации кнопки покупки технологии  }
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
    public class MonoUIObjectTechShipBuy : MonoBehaviour
    {
        // Кэш контрола
        private Transform FTransform;
        // Информация о прикрепленной технологии
        private TechInfo FInfo;
        // Тип корабля
        private ShipType FShipType;
        // Тип технологии
        private ShipTech FTechType;
        // Координата для кнопки
        private float FVector;

        private bool FInner;

        void Start()
        {
            // Кнопки покупки размещаются кругом на модели
            FTransform = transform;
            if (!FInner)
                FTransform.localPosition = new Vector3(0, 169, 0);
            else
                FTransform.localPosition = new Vector3(0, 90, 0);
            FTransform.RotateAround(FTransform.parent.position, Vector3.up, FVector);
            FTransform.localEulerAngles = new Vector3(0, 0, 0);
            FTransform.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(DoBuyTech));
        }

        // Обновление прогресса прокачки технологии
        void DoUpdateValues()
        {
            /* Временный вариант отображения, пока непонятно как более грамотно предоставить прогресс прокачки технологии */

            transform.Find("ProgressText").GetComponent<Text>().text = FInfo.Name;

            Transform a = transform.Find("ProgressValue");
            a.GetComponent<RectTransform>().offsetMax = new Vector2(((float)FInfo.Level / (float)FInfo.Count * 49) - 49, 10);

            if (FInfo.Level == FInfo.Count)
                a.gameObject.SetActive(false);
            /**/
            // Выключение кнопки, если достигнут максимум технологий
            if (FInfo.Level == FInfo.Count)
                transform.GetComponent<Button>().interactable = false;
        }

        // Каллбак покупки технологии для кнопки
        private void DoBuyTech()
        {
            Engine.SocketWriter.TechShipBuy((int)FShipType, (int)FTechType);
        }

        // Установка координат расположения кнопки
        public void PlaceTo(int AIndex, int ACount)
        {
            FInner = AIndex < 7;
            if (FInner)
                FVector = AIndex * 360 / Mathf.Min(ACount, 7);
            else
                FVector = (AIndex - 7) * 360 / (ACount - 7);
        }

        // Проверка кнопки на принадлежность к обновляемой технологии
        public bool CheckUpdate(ShipType AShipType, ShipTech ATechType)
        {
            bool LCheck = ((FShipType == AShipType) && (FTechType == ATechType));
            if (LCheck)
                UpdateData(AShipType, ATechType);
            return LCheck;
        }

        // Загрузка данных по указанной технологии   
        public void UpdateData(ShipType AShipType, ShipTech ATechType)
        {
            FShipType = AShipType;
            FTechType = ATechType;
            FInfo = Engine.TechShip(FShipType, FTechType);
            DoUpdateValues();
        }

        // Удаление кнопки с полотна технологий
        public void Delete()
        {
            Destroy(FTransform.gameObject);
        }
    }
}