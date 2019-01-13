/*///////////////////////////////////////////////
{                                              }
{     Модуль реализации слота хранилища        }
{       Copyright (c) 2016 UAshota             }
{                                              }
{  Rev A  2016.11.15                           }
{  Rev B  2017.05.15                           }
{                                              }
/*///////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;

namespace Shared
{
    public class MSHObjectStorageSlot : MonoInteractive
    {
        // Интерактивная оболочка
        private TSHClassStorageSlot FSelf;
        // Объект надписи количества
        private Transform FInfoCount;
        // Компонент надписи количества
        private Text FCount;
        // Цвет слота при наведении
        private Color FImageColorActive;
        // Стандартный цвет слота
        private Color FImageColorPassive;
        // Картинка для цветного оверлея
        private Image FImage;
        // Картинка слота задается менеджером склада
        private Image FBackground;
        // Коллайдер
        private Collider FCollider;

        // Таргет на слот если выбран какой-торесурс
        /* protected override void OnMouseEnter()
         {
             base.OnMouseEnter();
             if (SSHShared.MonoSource != null)
             {
                 FadeIn();
                 /*SSHShared.ObjectTarget = FSelf;
             }
         }

         // Обнуление таргета
         protected override void OnMouseExit()
         {
             base.OnMouseExit();
             FadeOut();
             /*SSHShared.ObjectTarget = null;
         }*/

        // Повышение яркости слота
        public void FadeIn()
        {
            FImage.color = FImageColorActive;
        }

        // Возвращение яркости слота
        public void FadeOut()
        {
            FImage.color = FImageColorPassive;
        }

        // Включение или выключение надписи количества ресурсов
        public void ChangeEnable(bool AEnabled)
        {
            FInfoCount.gameObject.SetActive(AEnabled);
        }

        // Смена количества елиниц ресурсов у слота
        public void ChangeCount(int AValue)
        {
            FCount.text = SSHLocale.CountToShortString(AValue);
            FCollider.enabled = (AValue == 0);
        }

        // Включение или выключение самого слота
        public void ChangeState(bool AActive, Sprite ABackground)
        {
            gameObject.SetActive(AActive);
            if (AActive)
                FBackground.sprite = ABackground;
            FSelf.Active = AActive;
        }

        // Создание и настройка слота
        protected override void DoInit(Interactive ASubject)
        {
            FSelf = (TSHClassStorageSlot)ASubject;
            FImage = FSelf.Transform.GetComponent<Image>();
            FImageColorPassive = FImage.color;
            FImageColorActive = FImageColorPassive;
            FImageColorActive.a = 125;
            FInfoCount = FSelf.Transform.Find("Info");
            FCount = FInfoCount.Find("Count").GetComponent<Text>();
            FBackground = GetComponent<Image>();
            FCollider = GetComponent<Collider>();
            ChangeEnable(false);
        }
    }
}