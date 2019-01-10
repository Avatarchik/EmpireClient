/////////////////////////////////////////////////
//
//  Модуль реализации кнопки планетарного ангара
//  Copyright (c) 2016 UAshota
//
//  Rev A  2016.12.18                            
//  Rev B  2017.06.06                            
//  Rev D  2018.06.22
//
/////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.UI;

namespace Planetar
{
    // Скрипт префаба слота ангара
    public class MonoHangar : Shared.MonoInteractive
    {
        // Картинка фона
        public Image _ImageBackground;
        // Текст типа объекта
        public Text _TextType;
        // Текст количества объекта
        public Text _TextCount;
        // Контейнер информационных контролов
        public GameObject _PanelInfo;
        // Панель префаба объекта
        public Transform _PanelModel;
        // Точка старта линии выделения пути
        public Transform _ActivePoint;
        // Линия установки пути
        public LineRenderer _Line;
        // Спрайт обычного фона
        public Sprite _SpriteDefault;
        // Спрайт выделенного фона
        public Sprite _SpriteFocused;

        // Настройки количества высаживаемых
        private const string S_LANDING_COUNT = SSHShared.SettingsName + "Hangar.Landing.";

        // Объект сопоставленный элементу
        private Hangar FSelf;
        // Префаб флота
        private Transform FPrefab;
        // Признак активации по хоткею
        private bool FIsHotkey;

        // Создание контрола
        protected override void DoInit(Shared.Interactive ASubject)
        {
            FSelf = (Hangar)ASubject;
        }

        // Игровой тик
        protected override void DoUpdate()
        {
            if (IsActive)
                SetLinePosition();
            CheckHotkey();
        }

        // Включение активного режима слота
        protected override bool DoActivate()
        {
            FIsHotkey = false;
            SetLinePosition();
            _Line.enabled = true;
            return true;
        }

        // Выключение активного режима слота
        protected override bool DoDeactivate()
        {
            if (IsDrag)
            {
                FPrefab.position = _PanelModel.position;
                _PanelInfo.SetActive(true);
            }
            else
                _Line.enabled = false;
            return true;
        }

        // Разрешение использовать элемент как источник
        protected override bool DoEnterSource()
        {
            return (FSelf.ShipType != ShipType.Empty) && (FSelf.Count > 0);
        }

        // Разрешение деактивации при выходе с элемента
        protected override bool DoLeaveSource()
        {
            return (!IsDrag && !_Line.enabled);
        }

        // Разрешение использовать элемент как назначение
        protected override bool DoEnterTarget()
        {
            // С ангара в ангар
            if (SSHShared.MonoFocus is Hangar)
            {
                Hangar LHangar = (Hangar)SSHShared.MonoFocus;
                return (SSHShared.MonoSource.Script.IsDrag) || ((LHangar.ShipType != ShipType.Empty) && (LHangar.Count > 0));
            }
            else
                return false;
        }

        // Показ фокуса
        protected override void DoShowFocus(bool AApproove)
        {
            if (AApproove)
                _ImageBackground.sprite = _SpriteFocused;
            else
                _ImageBackground.sprite = _SpriteDefault;
        }

        // Скрытие фокуса
        protected override void DoHideFocus()
        {
            if (!IsActive)
                _ImageBackground.sprite = _SpriteDefault;
        }

        // Событие начала перетаскивания
        protected override bool DoOnDragBegin()
        {
            _PanelInfo.SetActive(false);
            return true;
        }

        // Перенос модельки в режиме драга
        protected override void DoOnDrag()
        {
            FPrefab.position = GetMousePos();
        }

        // Событие завершения перетаскивания
        protected override bool DoOnDragEnd()
        {
            // Кинули в пустое место или на себя
            if ((SSHShared.MonoTarget == null) || (SSHShared.MonoTarget == FSelf))
            {
                DoDeactivate();
                return true;
            }
            else
            // Кинули на смену слота
            if (SSHShared.MonoTarget is Hangar)
                Engine.SocketWriter.HangarSwap(FSelf.UID, SSHShared.MonoTarget.UID);
            else
            // Кинули на планету
            if (SSHShared.MonoTarget is Planet)
            {
                SendFromHangar(SSHShared.MonoTarget.UID, -1, true);
                return true;
            }
            return false;
        }

        // Событие клика по таргету
        protected override bool DoOnClick()
        {
            // Высадка на планету
            if (SSHShared.MonoTarget is Planet)
            {
                SendFromHangar(SSHShared.MonoTarget.UID, -1, false);
                return false;
            }
            else
            // Высадка на слот
            if (SSHShared.MonoTarget is Landing)
            {
                SendFromHangar(((Landing)SSHShared.MonoTarget).Planet.UID, SSHShared.MonoTarget.UID, false);
                return false;
            }
            return true;
        }

        // Прямая или отложенная высадка юнитов
        private void SendFromHangar(int ATargetUID, int ASlot, bool AEmulateHotKey)
        {
            // Сколько можно максимум и сколько сохранено в настройках
            string LOptionName = S_LANDING_COUNT + FSelf.ShipType.ToString() + "." + FIsHotkey.ToString();
            int LMax = Engine.TechShip(FSelf.ShipType, ShipTech.Count).Value;
            int LCount = PlayerPrefs.GetInt(LOptionName, LMax);
            // С хоткеем только через окно
            if (AEmulateHotKey || Engine.HotKey(KeyCode.LeftControl, true))
            {
                SSHShared.UIModalManager.ShowSeparateCount("Land from hangar mode " + (FIsHotkey ? "A" : "B"), 1, LMax, LCount, true, (int ACount, bool ASave) =>
                {
                    if (ASave)
                        PlayerPrefs.SetInt(LOptionName, ACount);
                    Engine.SocketWriter.ShipFromHangar(FSelf.UID, ATargetUID, ASlot, ACount);
                });
            }
            else
                Engine.SocketWriter.ShipFromHangar(FSelf.UID, ATargetUID, ASlot, LCount);
            // Сбросим текущее выделение
            FSelf.Script.Cancel();
        }

        // Координаты курсора мыши
        private Vector3 GetMousePos()
        {
            Vector3 LPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            LPos = Camera.main.ScreenToWorldPoint(LPos);
            LPos.y = _ActivePoint.position.y;
            return LPos;
        }

        // Перемещение указателя выбора 
        private void SetLinePosition()
        {
            _Line.SetPosition(0, _ActivePoint.position);
            _Line.SetPosition(1, GetMousePos());
        }

        // Показ линии высадки по хоткею
        private void CheckHotkey()
        {
            // Выключить путь
            if (SSHShared.HotKey(KeyCode.Escape, false) || Input.GetMouseButtonDown(1))
            {
                if (IsActive || IsDrag)
                    Cancel();
                return;
            }
            // Для драга игнорируем другие хоткеи
            if (IsDrag)
                return;
            // Каждый слот соответствует клавише (1, 2, ... 7) игнорится если выбран какой-то объект
            if (!SSHShared.HotKey((FSelf.UID + 1).ToString()))
                return;
            // Выключим предыдущее состояние
            if (IsActive)
            {
                Cancel();
                return;
            }
            // Если нельзя включить
            else if (!DoEnterSource())
                return;
            // Выключим предыдущий объект
            if (SSHShared.MonoSource != null)
            {
                // Отмена если идет драг
                if (SSHShared.MonoSource.Script.IsDrag)
                    return;
                else
                    SSHShared.MonoSource.Script.Cancel();
            }
            FIsHotkey = true;
            // Активируем текущий слот
            if (!IsFocused)
                SSHShared.MonoSource = FSelf;
            Activate();
        }

        // Смена типа или количества кораблика
        public void Change(ShipType AShipType, int ACount)
        {
            FSelf.Count = ACount;
            // Обновление строчки количества
            _TextCount.text = SSHLocale.CountToShortString(ACount);
            // Тип кораблика не изменился
            if (AShipType == FSelf.ShipType)
                return;
            // Меняем тип кораблика
            FSelf.ShipType = AShipType;
            if (FPrefab != null)
                Destroy(FPrefab.gameObject);
            // Для нового кораблика задаем спрайт
            if (AShipType != ShipType.Empty)
            {
                FPrefab = PrefabManager.CreateShipModel(_PanelModel, Engine.Player.Race, AShipType).transform;
                _TextType.text = AShipType.ToString();
                _PanelInfo.SetActive(true);
                if (IsFocused)
                    CheckEnterAny();
            }
            // Для пустого слота выключаем кнопку
            else
            {
                _PanelInfo.SetActive(false);
                if (IsFocused)
                    Deactivate();
            }
        }
    }
}