/*///////////////////////////////////////////////
{                                              }
{    Модуль реализации объекта ресурса         }
{       Copyright (c) 2016 UAshota             }
{                                              }
{  Rev A  2016.12.15                           }
{  Rev B  2017.05.15                           }
{                                              }
/*///////////////////////////////////////////////
using UnityEngine;

namespace Shared
{
    public class MSHObjectResource : MonoInteractive
    {
        // Угол скорости вращения 
        private const float C_RotateAngle = 218f;
        // Класс описатель
        private Resource FSelf;
        // Признак того, что ресурс перетаскивают
        private bool FInMovie = false;
        // Признак необходимости покрутить ресурс
        private bool FInRotate = false;
        // Коллайдер для выключения при переносе
        private Collider FOCollider;
        // Стартовая позиция ресурса
        private Vector3 FPosition;

        void Update()
        {
            // Перенос ресурса за курсором
            if (FInMovie)
                DoMovie();
            // Вращение вокруг оси
            if ((FInRotate) || (FInMovie))
                FSelf.Model.Rotate(Vector3.up, C_RotateAngle * Time.deltaTime);
        }

        void OnMouseDown()
        {
            // Взять ресурс в руку
            Cursor.visible = false;
            FInMovie = true;
            FOCollider.enabled = false;
            FPosition = FSelf.Transform.position;
            /*SSHShared.MonoSource = FSelf.Slot;*/
        }

        void OnMouseUp()
        {
            // Положить ресурс
            Cursor.visible = true;
            FInMovie = false;
            FOCollider.enabled = true;
            // Перемещение между хранилищами
            FSelf.OnResourceMove(FSelf, false);
            // Возврат на прежнюю позицию до ответа сервера
            FSelf.Transform.position = FPosition;
            /*SSHShared.ObjectSource = null;*/
        }

        /* protected override void OnMouseEnter()
         {
             base.OnMouseEnter();
             DoFadeIn();
             // Если что-то выбрано, то ресурс становится таргетом
             /*if (SSHShared.ObjectSource != null)
                 SSHShared.ObjectTarget = FSelf;
         }

         protected override void OnMouseExit()
         {
             base.OnMouseExit();
             DoFadeOut();
             // Если действий нет - сбросить таргет
             /*SSHShared.ObjectTarget = null;
         }

         protected override void OnMouseOver()
         {
             base.OnMouseOver();
             // Пробелинг ресурса с планеты или на планету
             if (Input.GetKeyDown(KeyCode.Space))
                 FSelf.OnResourceMove(FSelf, true);
         }*/

        // Запуск анимации ресурса
        private void DoFadeIn()
        {
            FInRotate = true;
            FSelf.Slot.FadeIn();
        }

        // Остановка анимации ресурса
        private void DoFadeOut()
        {
            FInRotate = false;
            FSelf.Model.localRotation = Quaternion.identity;
            FSelf.Slot.FadeOut();
        }

        // Перетаскивание ресурсора мышкой
        private void DoMovie()
        {
            Vector3 LMousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            LMousePos = Camera.main.ScreenToWorldPoint(LMousePos);
            LMousePos.y = FSelf.Transform.position.y;
            FSelf.Transform.position = LMousePos;
        }

        // Создание класса ресурса
        protected override void DoInit(Interactive ASubject)
        {
            FSelf = (Resource)ASubject;
            FOCollider = FSelf.Transform.GetComponent<BoxCollider>();
        }
    }
}