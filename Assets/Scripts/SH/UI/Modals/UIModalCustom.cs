/////////////////////////////////////////////////
//
//  Модуль реализации кастомного модального окна
//  Copyright(c) 2016 UAshota
//
//  Rev D  2018.06.23
//
/////////////////////////////////////////////////

using UnityEngine;

namespace Shared
{
    // Класс реализации кастомного модального окна
    public abstract class UIModalCustom : MonoBehaviour
    {
        // Обработка горячих клавиш
        protected virtual void Update()
        {
            // Принятие изменений
            if (Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.KeypadEnter))
                DoApply();
            else
            // Отмена изменений
            if (Input.GetKeyUp(KeyCode.Escape))
                Close();
         }

        // Применение изменений для конкретного класса
        protected abstract void DoApply();

        // Закрытие окна
        public void Close()
        {
            SSHShared.UIModalManager.Close();
        }
    }
}