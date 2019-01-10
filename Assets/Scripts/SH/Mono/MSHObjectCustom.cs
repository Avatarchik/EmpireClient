/////////////////////////////////////////////////
//
//  Базовый класс любого интерактивного моно 
//  объекта
//  Copyright (c) 2016 UAshota
//
//  Rev B  2017.05.15
//  Rev D  2018.06.22
//
/////////////////////////////////////////////////

using UnityEngine;

namespace Shared
{
    // Базовый класс моно интерактивных элементов
    public abstract class MonoInteractive : MonoBehaviour
    {
        // Объект, который используется для этого элемента
        public Interactive Object;
        // Признак актиности элемента
        public bool IsActive;
        // Признак перемещения элемента
        public bool IsDrag;
        // Признак фокуса элемента
        public bool IsFocused;

        // Активация элемента
        protected abstract void DoInit(Interactive ASubject);
        // Активация элемента
        protected virtual bool DoActivate() { return false; }
        // Деактивация элемента
        protected virtual bool DoDeactivate() { return true; }
        // Событие клика по элементу
        protected virtual bool DoOnClick() { return true; }
        // Событие даблклика по элементу
        protected virtual bool DoOnDblClick() { return false; }
        // Событие начала перемещения элемента
        protected virtual bool DoOnDragBegin() { return false; }
        // Событие перемещения элемента
        protected virtual void DoOnDrag() { }
        // Событие завершения перемещения элемента
        protected virtual bool DoOnDragEnd() { return false; }
        // Разрешение входа в элемент-источник
        protected virtual bool DoEnterSource() { return false; }
        // Разрешение выхода из элемента-источника
        protected virtual bool DoLeaveSource() { return true; }
        // Разрешение входа в элемент-цель
        protected virtual bool DoEnterTarget() { return false; }
        // Разрешение выхода из элемента-цели
        protected virtual bool DoLeaveTarget() { return true; }
        // Показ разрешительного или запрещающего фокуса
        protected virtual void DoShowFocus(bool AApproove) { }
        // Скрытие фокуса
        protected virtual void DoHideFocus() { }
        // Активация элемента
        protected virtual void DoOnOver() { }
        // Событие смены состояния
        protected virtual void DoOnActivationChanged() { }
        // Событие показа элемента в камере
        protected virtual void DoOnBecameVisible() { }
        // Событие ухода камеры от элемента
        protected virtual void DoOnBecameInvisible() { }
        // Событие удаления объекта
        protected virtual void DoDelete(bool AExplosion) { }
        // Событие обновления фрейма
        protected virtual void DoUpdate() { }

        // Дельта смещения активации перемещения
        private const float C_PathDelta = 15f;
        // Дельта даблклика
        private const float C_DoubleClickTime = 12f;
        // Точка клика
        private Vector3 FClickPoint;
        // Запрет перетаскивания
        private bool FDragDisabled;
        // Время клика
        private float FSingleClickTime;

        // Проверка на инициализацию объектов
        private void Start()
        {
            if (Object == null)
                Debug.LogError("MonoObject not set: " + name);
        }

        private void Update()
        {
            // Обновим клиентские данные
            DoUpdate();
        }

        // Нахождение мыши над объектом
        private void OnMouseOver()
        {
            if (Input.GetMouseButtonUp(1))
                Object.ShowPopup();
            else
                DoOnOver();
        }

        // Наведение мыши на элемент
        private void OnMouseEnter()
        {
            SSHShared.MonoFocus = Object;
            IsFocused = true;
            CheckEnterAny();
            Object.ShowHint();
        }

        // Увод мыши с элемента
        private void OnMouseExit()
        {
            CheckLeave();
            IsFocused = false;
            Object.HideHint();
            SSHShared.MonoFocus = null;
        }

        // Нажатие на элемент
        private void OnMouseDown()
        {
            // Выставим дефолтные параметры
            FDragDisabled = false;
            FClickPoint = Input.mousePosition;
            Object.Script.IsDrag = false;
            Object.HideHint();
        }

        // Завершение клика на элементе
        private void OnMouseUp()
        {
            // Проверяем двойной клик
            if (Time.time < FSingleClickTime + C_DoubleClickTime * Time.deltaTime)
            {
                FSingleClickTime = 0;
                ReleaseClick();
                ReleaseDoubleClick();
            } else
            // Если нету - засекем время
            {
                FSingleClickTime = Time.time;
                ReleaseClick();
            }
        }

        // Событие драга на элементе
        private void OnMouseDrag()
        {
            // Перетаскивание запрещено или элемент не источник
            if ((FDragDisabled) || (SSHShared.MonoSource != Object) || (Object.Script.IsActive))
                return;
            // Если стартовали - отправим событие о перемещении
            if (Object.Script.IsDrag)
                DoOnDrag();
            else
            // Иначе проверим необходимость анализа дельта смещения
            if (Vector3.Distance(Input.mousePosition, FClickPoint) > C_PathDelta)
            {
                if (DoOnDragBegin())
                {
                    Object.Script.IsDrag = true;
                    CheckEnterTarget();
                    SSHShared.OnActiveElementChanged.Invoke();
                }
                else
                    FDragDisabled = true;
            }
        }

        // Объект попапл в обзор камеры
        private void OnBecameVisible()
        {
            SSHShared.OnActiveElementChanged += DoOnActivationChanged;
            DoOnBecameVisible();
            DoOnActivationChanged();
        }

        // Объект ушел из обзора камеры
        private void OnBecameInvisible()
        {
            DoOnBecameInvisible();
            SSHShared.OnActiveElementChanged -= DoOnActivationChanged;
        }

        private void ReleaseClick()
        {
            // Режим переноса
            if (Object.Script.IsDrag)
            {
                // Если перемещения небыло - отменим
                if (DoOnDragEnd())
                    Cancel();
                else
                // Иначе обнулим все и дождемся окончания операции
                {
                    SSHShared.MonoTarget = null;
                    SSHShared.MonoSource = null;
                }
                Object.Script.IsDrag = false;
                SSHShared.OnActiveElementChanged.Invoke();
            }
            else
            // Кликнули на этот элемент
            if (SSHShared.MonoSource == Object)
            {
                // Если первый клик - включаем
                if (SSHShared.MonoTarget == null)
                    Activate();
                else
                // Если повторный на себя - выключаем
                if (SSHShared.MonoTarget == Object)
                    Deactivate();
                else
                // Если драг от себя к другому - переключаем
                {
                    SSHShared.MonoSource.Script.Cancel();
                    SSHShared.MonoSource.Script.Activate();
                }
            }
            else
            // Кликнули на другой элемент
            if (SSHShared.MonoTarget != null)
            {
                // При клике не меняем активацию
                if (SSHShared.MonoSource.Script.DoOnClick())
                {
                    SSHShared.MonoSource.Script.Cancel();
                    if (SSHShared.MonoSource != null)
                        SSHShared.MonoSource.Script.Activate();
                }
            }
            else
                Debug.Log("Monoobject is disabled");
        }

        private void ReleaseDoubleClick()
        {
            DoOnDblClick();
        }

        // Инициализация управляемого объекта
        public void Init(Interactive ASubject)
        {
            if (Object != null)
                Debug.LogError("MonoObject is already set: " + name);
            Object = ASubject;
            Object.Script = this;
            DoInit(Object);
        }

        // Активация функционала и установки источника или цели
        public void Activate()
        {
            if (!DoActivate())
                return;
            // Активируем объект
            Object.Script.IsActive = true;
            SSHShared.OnActiveElementChanged.Invoke();
            // Попробуем найти новую цель
            if (SSHShared.MonoFocus != null)
                SSHShared.MonoFocus.Script.CheckEnterTarget();
            // Покажем фокус
            if (!IsFocused)
                DoShowFocus(true);
        }

        // Деактивация функционала
        public void Deactivate()
        {
            if (!DoDeactivate())
                return;
            // Деактивируем объект
            Object.Script.IsActive = false;
            SSHShared.OnActiveElementChanged.Invoke();
            // Попробуем найти новый источник
            if (SSHShared.MonoTarget == Object)
                SSHShared.MonoTarget = null;
            // Попробуем найти новый источник
            if (SSHShared.MonoFocus != null)
                SSHShared.MonoFocus.Script.CheckEnterSource();
            // Скроем фокус
            if (!IsFocused)
                DoHideFocus();
        }

        // Отмена любой текущей операции
        public void Cancel()
        {
            // Выключение источника
            SSHShared.MonoSource.Script.Deactivate();
            SSHShared.MonoSource = null;
            // Выключение 
            Object.Script.IsDrag = false;
            FDragDisabled = true;
            // Выключение цели
            if (SSHShared.MonoTarget != null)
                SSHShared.MonoTarget = null;
            // Попытка найти новый источник
            if (SSHShared.MonoFocus != null)
                SSHShared.MonoFocus.Script.CheckEnterSource();
        }

        // Проверка на возможность стать источником
        public bool CheckEnterSource()
        {
            // Первое вхождение - источник
            bool LApproove = (SSHShared.MonoSource == null) && (DoEnterSource());
            if (LApproove)
                SSHShared.MonoSource = Object;
            // Выставим фокус
            if (IsFocused)
                DoShowFocus(LApproove);
            return LApproove;
        }

        // Проверка на возможность стать целью
        public bool CheckEnterTarget()
        {
            // Повторное вхождение - цель
            bool LApproove = (SSHShared.MonoTarget == null) && (DoEnterTarget());
            if (LApproove)
                SSHShared.MonoTarget = Object;
            // Выставим фокус
            if (IsFocused)
                DoShowFocus(LApproove);
            return LApproove;
        }

        // Проверка на возможность стать кем нибудь в этой сраной жизни
        public void CheckEnterAny()
        {
            if (!CheckEnterSource() && (SSHShared.MonoSource != null))
                CheckEnterTarget();
        }

        // Проверка на возможность сброса элемента с источника или цели
        public void CheckLeave()
        {
            // Если разрешен уход как от источника
            if (SSHShared.MonoSource == Object)
            {
                if (DoLeaveSource())
                    SSHShared.MonoSource = null;
            }
            // Если разрешен уход как от цели
            if (SSHShared.MonoTarget == Object)
            {
                if (DoLeaveTarget())
                    SSHShared.MonoTarget = null;
            }
            // Уберем фокус
            DoHideFocus();
        }

        // Удаление объекта
        public void Delete(bool AExplosion)
        {
            // Проверим фокус
            if (SSHShared.MonoFocus == Object)
                SSHShared.MonoFocus = null;
            // Убрать активность
            if (IsActive)
                Cancel();
            else
            {
                // Проверим источник
                if (SSHShared.MonoSource == Object)
                    SSHShared.MonoSource = null;
                // Проверим цель
                if (SSHShared.MonoTarget == Object)
                    SSHShared.MonoTarget = null;
            }
            // Отключить пользовательские объекты
            DoDelete(AExplosion);
            // Удалить сам объект
            Destroy(gameObject);
        }
    }
}