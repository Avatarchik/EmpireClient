/*///////////////////////////////////////////////
{                                              }
{ Planetar                                     }
{ Панель управления планетарным ангаром        }
{ Copyright (c) 2016 UAshota                   }
{                                              }
{ Rev A  2016.12.15                            }
{ Rev B  2017.06.06                            }
{                                              }
/*///////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;

namespace Planetar
{
    public class MonoUIPanelHangar : MonoBehaviour
    {
        // Кэш панели
        public Transform UIHangarSlots;
        // Заголовок
        public Transform UIHeader;
        // Кнопка сворачивания
        public Button UIMinimize;

        private float FTop;
        private float FHeight;
        private float FTargetTop;
        private float FDeltaSpeed;
        private bool FSizeable;

        private RectTransform FRectHangar;
        // Массив слотов ангара
        public Hangar[] FSlots;

        private void Start()
        {
            UIMinimize.onClick.AddListener(Minimize);
            FHeight = UIHeader.GetComponent<RectTransform>().sizeDelta.y;
            FRectHangar = transform.GetComponent<RectTransform>();
            FTop = FRectHangar.anchoredPosition.y;
            FTargetTop = FTop;
            FDeltaSpeed = Time.deltaTime * 300;
        }

        private void Update()
        {
            if (FSizeable)
            {
                float nv = FTargetTop;
                if (FRectHangar.anchoredPosition.y < FTargetTop)
                {
                    nv = Mathf.Round(FRectHangar.anchoredPosition.y + FDeltaSpeed);
                    if (nv > FTargetTop)
                    {
                        nv = FTargetTop;
                        FSizeable = false;
                    }
                }
                else
                {
                    nv = Mathf.Round(FRectHangar.anchoredPosition.y - FDeltaSpeed);
                    if (nv < FTargetTop)
                    {
                        nv = FTargetTop;
                        FSizeable = false;
                    }
                }

                FRectHangar.anchoredPosition = new Vector2(0, nv);
            }

        }

        private void Minimize()
        {
            if (FTargetTop != FTop)
                FTargetTop = FTop;
            else
                FTargetTop = FTop - FRectHangar.sizeDelta.y + FHeight;

            FSizeable = true;
        }

        // Задания параметров слота
        public void UpdateData(int ASlot, ShipType AShipType, int ACount)
        {
            FSlots[ASlot].Change(AShipType, ACount);
        }

        // По умолчанию 7 слотов
        public void Create()
        {
            /* в константы и оно тут не нужно нафиг, ангар вынести в шару*/
            FSlots = new Hangar[7];

            for (int LIndex = 0; LIndex < FSlots.Length; LIndex++)
                FSlots[LIndex] = new Hangar(LIndex, UIHangarSlots);
        }
    }
}