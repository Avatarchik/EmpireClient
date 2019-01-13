/*///////////////////////////////////////////////
{                                              }
{  Базовый класс любого интерактивного объекта }
{       Copyright (c) 2016 UAshota             }
{                                              }
{  Rev B  2017.05.15                           }
{                                              }
/*///////////////////////////////////////////////
using UnityEngine;

namespace Shared
{
    public class Interactive
    {
        // Идентификатор объекта
        public int UID;
        // Игровой трансфоррм
        public Transform Transform;

        public MonoInteractive Script;

        public MonoUIHintCustom Hint { get { return DoGetHint(); } }

        public MonoUIPopupCustom Popup { get { return DoGetPopup(); } }

        // Лок объекта по времени
        private float FLockTime;

        // Разгрузка ддос на сервер при зажатии клавиши
        public bool Locked()
        {
            return (Time.time - FLockTime < 1);
        }

        // Блокировка объекта
        public void Lock()
        {
            FLockTime = Time.time;
        }

        protected virtual MonoUIHintCustom DoGetHint()
        {
            return null;
        }

        protected virtual MonoUIPopupCustom DoGetPopup()
        {
            return null;
        }

        public void ShowHint()
        {
            if (Hint)
                Hint.Show(this);
        }

        public void HideHint()
        {
            if (Hint)
                Hint.Hide();
        }

        public void ShowPopup()
        {
            if (Popup)
                Popup.Show(this);
        }

        public void HidePopup()
        {
            if (Popup)
                Popup.Hide();
        }
    }
}