/////////////////////////////////////////////////
//
//  Модуль обработки действий боевого корабля    
//  Copyright (c) 2016 UAshota                 
//                                              
//  Rev A  2016.11.15                            
//  Rev B  2017.06.06                            
//  Rev D  2018.06.19
//
/////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.UI;

namespace Planetar
{
    public class MonoShip : Shared.MonoInteractive
    {
        #region Объекты

        // Объект флага: панелька
        public Transform _CountGroup;
        // Объект флага: количество корабликов
        public Text _CountValue;
        // Объект флага: рамка выделения
        public Image _CountCorner;
        // Объект флага: фон
        public Image _CountBackground;
        // Объект линии привязки к планете и указания цели
        public LineRenderer _ShipLineAttach;
        // Объект линии прокладки пути
        public LineRenderer _ShipLinePath;
        // Объект уголков выделения кораблика
        public GameObject _ShipSelection;
        // Время постройки
        public TextMesh _Timer;

        #endregion

        #region Константы

        // Цвет флага: уголок свой
        private const uint C_ColorSelfCorner = 0xBFD2FFFF;
        // Цвет флага: фон свой
        private const uint C_ColorSelfBack = 0x0D48FFCC;
        // Цвет флага: уголок чужой
        private const uint C_ColorEnemyCorner = 0xB56868FF;
        // Цвет флага: фон чужой
        private const uint C_ColorEnemyBack = 0x841100FF;
        // Цвет флага: уголок нейтральный
        private const uint C_ColorNeutralCorner = 0xC0C0C0FF;
        // Цвет флага: фон нейтральный
        private const uint C_ColorNeutralBack = 0xB0B0B0FF;
        // Скорость полета флота
        private const float C_FlySpeed = 2.2f;
        // Дельта дистанции прекращения перелета флота при приземлении на точку
        private const float C_FlyStopDistance = 0.01f;
        // Интервал мигания рамки
        private const float C_ChangeStateInterval = 0.4f;
        // Скорость перелета на одном планетоиде
        private const float C_SpeedInternal = 3.5f;
        // Скорость перелета на другой планетоид
        private const float C_SpeedExternalL = 6f;

        #endregion

        #region Переменные

        // Внутренний объект описывающий кораблик
        private Ship FSelf;
        // Орудия: тип патронов
        private ShipShellType FShipShellType;
        // Таймер мигания уголков выделения
        private float FTimerCornerState;
        // Итоговая скорость полета
        private float FFLySpeed;
        // Объект панели операции
        private GameObject FOperationsPanel;
        // Объект двигателя корабля
        private GameObject FShipEngine;
        // Объект модельки корабля для вращения
        private Transform FShipModel;

        #endregion

        #region Наследование

        // Инициализация
        protected override void DoInit(Shared.Interactive ASubject)
        {
            FSelf = (Ship)ASubject;
            // Найдем модельку
            FShipModel = FSelf.Transform.Find("Model");
            // Найдем двигатель
            FShipEngine = FShipModel.Find("Engine").gameObject;
            // Выберем тип владельца для раскраски
            ShowCornerColor();
            // Проверка на необходимость включения UI постройки
            UpdateState();
            // Обновим структуру
            UpdateHP();
            // Обновим аттач
            UpdateAttach();
            // Орудия
            SetWeapons();
        }

        // Обновляемые действия
        protected override void DoUpdate()
        {
            // Анимация полета
            if (FSelf.IsMove())
                MoveToTarget();
            // Перемещение линии пути    
            if (_ShipLinePath.enabled)
                MovePathLine();
            // Линия аттача
            if (_ShipLineAttach.enabled)
                _ShipLineAttach.SetPosition(0, FSelf.Transform.position);
            // Смена состояние рамки
            ShowCornerState();
            // Учет хоткеев
            CheckHotkeyUpdate();
            // Атака нацеленных орудий
            AttackTarget();
            // Вращение паровозиком
            RotateAroundPlanet();
        }

        // Учет хоткея
        protected override void DoOnOver()
        {
            if ((FSelf.State == ShipState.Available) && (!FSelf.Planet.InBattle))
                CheckHotkeyOver();
        }

        // Отображение фокуса
        protected override void DoShowFocus(bool AApproove)
        {
            if (FSelf.AttachedPlanet != null)
                _ShipLineAttach.enabled = true;
            if (AApproove)
                _ShipSelection.SetActive(true);
        }

        // Скрытие фокуса
        protected override void DoHideFocus()
        {
            if (FSelf.AttachedPlanet != null)
                _ShipLineAttach.enabled = false;
            _ShipSelection.SetActive(false);
        }

        // Разрешение стать источником
        protected override bool DoEnterSource()
        {
            return (FSelf.IsAvail() && (FSelf.Owner.Role == SSHRole.Self)
                && ((SSHShared.MonoSource == null) || (Engine.ShipGroup.Ships.Count > 0)));
        }

        // Разрешение перестать быть источником
        protected override bool DoLeaveSource()
        {
            return (!IsDrag) && (Engine.ShipGroup.Ships.Count != 1);
        }

        // Разрешение стать целью
        protected override bool DoEnterTarget()
        {
            // Первое вхождение
            if (SSHShared.MonoSource == null)
                return true;
            else
            // Если это кораблик
            if (SSHShared.MonoSource is Ship)
            {
                // Проверим доп параметры
                Ship LShip = (Ship)SSHShared.MonoSource;
                // Выбран вражеский кораблик
                if (FSelf.Owner != LShip.Owner)
                    return false;
                // Перетаскивание или клик
                return (!LShip.Script.IsDrag) || ((LShip.ShipType == FSelf.ShipType) && (LShip.Planet == FSelf.Planet));
            }
            return false;
        }

        // Сообщение активации
        protected override bool DoActivate()
        {
            // Проверим хоткей
            if (Engine.HotKey(KeyCode.LeftControl, true))
            {
                // Если кораблик есть в группе - удалим
                if (Engine.ShipGroup.Ships.Contains(FSelf))
                {
                    RemoveFromGroup();
                    return false;
                }
                else
                // Если кораблика нет в группе - добавим
                {
                    Engine.ShipGroup.AddShip(FSelf);
                    if (Engine.ShipGroup.Ships.Count > 1)
                        return false;
                }
            }
            // Иначе если просто смена кораблика
            else
            {
                Engine.ShipGroup.Clear();
                Engine.ShipGroup.AddShip(FSelf);
            }
            return true;
        }

        // Сообщение деактивации
        protected override bool DoDeactivate()
        {
            if ((Engine.ShipGroup.Ships.Count != 1) || !(SSHShared.MonoTarget is Ship) || (SSHShared.MonoTarget == FSelf))
                RemoveFromGroup();
            return true;
        }

        // Сообщение завершенного клика
        protected override bool DoOnClick()
        {
            // Клик по ландингу
            if (SSHShared.MonoTarget is Landing)
            {
                Engine.ShipGroup.RemoveShip(FSelf);
                MoveToLanding();
                return true;
            }
            else
            // Клик по планете
            if (SSHShared.MonoTarget is Planet)
            {
                Planet LPlanet = (Planet)SSHShared.MonoTarget;
                if ((FSelf.Planet != LPlanet) || (Engine.ShipGroup.LastPlanet != null))
                {
                    // Если зажат контрол - сменим вхождение
                    if (Engine.HotKey(KeyCode.LeftControl, true))
                    {
                        if (Engine.ShipGroup.Planets.Contains(LPlanet))
                            Engine.ShipGroup.RemovePath(LPlanet);
                        else
                            Engine.ShipGroup.AddPath(LPlanet);
                    }
                    else
                        // Иначе выстроим путь и отправим 
                        Engine.ShipGroup.Run(LPlanet);
                    return false;
                }
                else
                    Engine.SocketWriter.ShipAttachTo(FSelf, LPlanet.UID);
            }
            // Ничего
            return true;
        }

        // Событие даблклика по элементу
        protected override bool DoOnDblClick()
        {
            // Проверим хоткей на выбор всех юнитов
            if (Engine.HotKey(KeyCode.LeftControl, true))
            {
                foreach (Ship LShip in FSelf.Planet.Ships)
                {
                    if ((FSelf.Owner == LShip.Owner)
                        && (!LShip.Tech(ShipTech.Stationary).Supported)
                        && (LShip.IsAvail() || LShip.IsMove())
                        && (!Engine.ShipGroup.Ships.Contains(LShip)))
                        Engine.ShipGroup.AddShip(LShip);
                }
            }
            else
                // Иначе уравняем стеки если есть хотя бы два корабля одного типа
                foreach (Ship LShip in FSelf.Planet.Ships)
                {
                    if ((FSelf.ShipType == LShip.ShipType) && (FSelf.Owner == LShip.Owner) && (FSelf != LShip))
                    {
                        Engine.SocketWriter.ShipHypodispersion(FSelf);
                        return false;
                    }
                }
            return false;
        }

        // Разрешение перетаскивания
        protected override bool DoOnDragBegin()
        {
            _ShipLinePath.enabled = true;
            return true;
        }

        // Завершение перетаскивания
        protected override bool DoOnDragEnd()
        {
            _ShipLinePath.enabled = false;
            // Клик по ландингу
            if (SSHShared.MonoTarget is Landing)
            {
                MoveToLanding();
                return true;
            }
            else
            // Клик по кораблю
            if (SSHShared.MonoTarget is Ship)
            {
                Ship LShip = (Ship)SSHShared.MonoTarget;
                // При зажатом контроле - показываем запрос
                if (Engine.HotKey(KeyCode.LeftControl, true))
                {
                    SSHShared.UIModalManager.ShowSeparateCount("Merge", 1, FSelf.Count - 1, FSelf.Count / 2, false, (int ACount, bool ASave) =>
                    {
                        Engine.SocketWriter.ShipMerge(FSelf, LShip, ACount);
                    });
                }
                else
                    // Иначе просто объеденяем
                    Engine.SocketWriter.ShipMerge(FSelf, LShip, FSelf.Count);
                return true;
            }
            else
            // Клик по планете
            if (SSHShared.MonoTarget is Planet)
            {
                Planet LPlanet = (Planet)SSHShared.MonoTarget;
                if (LPlanet.PlanetType != PlanetType.Hole)
                    Engine.SocketWriter.ShipAttachTo(FSelf, LPlanet.UID);
            }
            else
            // Отмена привязки к планете
            if (FSelf.AttachedPlanet != null)
            {
                Engine.SocketWriter.ShipAttachTo(FSelf, SSHShared.MonoTarget != null ? SSHShared.MonoTarget.UID : -1);
                return true;
            }
            // Пусто
            return false;
        }

        // Запуск уничтожения корабля со взрывом
        protected override void DoDelete(bool AExplosion)
        {
            RemoveFromGroup();
            FSelf.Planet.Ships.Remove(FSelf);
            FSelf.Landing = null;
            // При уничтожении показать взрыв
            if (AExplosion)
                Shared.PrefabManager.ShipExplosion(FSelf.Transform.parent);
        }

        #endregion

        #region Операции с кораблем

        // Создание орудийных систем
        private void SetWeapons()
        {
            ShipShellType LShellType;
            // Центральное
            if (FSelf.Tech(ShipTech.WeaponeLaser).Supported)
                LShellType = ShipShellType.Laser;
            else
                LShellType = ShipShellType.Bullet;
            FSelf.Weapons[(int)ShipWeaponType.Center] = new ShipWeapon(FShipModel.Find("WeaponCenter"), LShellType, FSelf.Owner);
            // Боковые
            if (FSelf.Tech(ShipTech.WeaponeDoubleLaser).Supported)
                LShellType = ShipShellType.Laser;
            else
                LShellType = ShipShellType.Bullet;
            FSelf.Weapons[(int)ShipWeaponType.Left] = new ShipWeapon(FShipModel.Find("WeaponLeft"), LShellType, FSelf.Owner);
            FSelf.Weapons[(int)ShipWeaponType.Right] = new ShipWeapon(FShipModel.Find("WeaponRight"), LShellType, FSelf.Owner);
            // Ракета
            FSelf.Weapons[(int)ShipWeaponType.Rocket] = new ShipWeapon(FShipModel.Find("WeaponRocket"), ShipShellType.Rocket, FSelf.Owner);
        }

        // Перемещение в слот
        private void MoveToLanding()
        {
            Landing LLanding = (Landing)SSHShared.MonoTarget;
            // Если корабль не один, проверим хоткеи
            if (FSelf.Count > 1)
            {
                // При зажатом шифте просто делим пополам
                if (Engine.HotKey(KeyCode.LeftShift, true))
                {
                    Engine.SocketWriter.ShipSeparate(FSelf, LLanding.UID, FSelf.Count / 2);
                    return;
                }
                else
                // При зажатом контроле - показываем запрос
                if (Engine.HotKey(KeyCode.LeftControl, true))
                {
                    SSHShared.UIModalManager.ShowSeparateCount("Separate", 1, FSelf.Count - 1, FSelf.Count / 2, false, (int ACount, bool ASave) =>
                    {
                        Engine.SocketWriter.ShipSeparate(FSelf, LLanding.UID, ACount);
                    });
                    return;
                }
            }
            // Иначе просто переместим
            Engine.SocketWriter.ShipMoveTo(FSelf, LLanding.Planet, LLanding.UID);
        }

        // Проверка хоткея при наведении на кораблик
        private void CheckHotkeyOver()
        {
            // Пробел в ангар
            if (GetAction(ShipAction.MoveToHangar) && Engine.HotKey(KeyCode.Space, true))
                FSelf.ActionCall(ShipAction.MoveToHangar);
            else
            // Отправка / отмена походки
            if (GetAction(ShipAction.ChangeMode) && Engine.HotKey(KeyCode.Q, false))
                FSelf.ActionCall(ShipAction.ChangeMode);
            else
            // Отправка / отмена портал
            if (GetAction(ShipAction.PortalJump) && Engine.HotKey(KeyCode.BackQuote, false))
                FSelf.ActionCall(ShipAction.PortalJump);
        }

        // Проверка хоткея при ненаведении
        private void CheckHotkeyUpdate()
        {
            // Сброс выделения группы
            if (Engine.ShipGroup.Ships.Count > 0)
            {
                bool LCheckPopup = Input.GetMouseButtonUp(1);
                // Нажатие Esc или ПКМ на пустой области - сброс выделения
                if (Engine.HotKey(KeyCode.Escape, true) || (LCheckPopup && SSHShared.MonoFocus == null))
                {
                    if (Engine.ShipGroup.Ships.Count > 1)
                        Engine.ShipGroup.Clear();
                    else
                    if (Engine.ShipGroup.Ships[0] == FSelf)
                        Cancel();
                }
            }
        }

        private bool GetAction(ShipAction AAction)
        {
            return (FSelf.ActionStatus(AAction) == ActionState.Enabled);
        }

        // Действие - отправить в походку
        private void ActionPassiveMode()
        {
            Engine.SocketWriter.ShipChangeActive(FSelf);
        }

        // Активация последнего корабля в группе
        private void RemoveFromGroup()
        {
            Engine.ShipGroup.RemoveShip(FSelf);
            // И если это предпоследний - сделаем последний активным
            if (Engine.ShipGroup.Ships.Count == 1)
            {
                Engine.ShipGroup.Ships[0].Script.IsActive = true;
                SSHShared.MonoSource = Engine.ShipGroup.Ships[0];
                SSHShared.OnActiveElementChanged.Invoke();
                // Попытка найти новый источник
                if (SSHShared.MonoFocus != null)
                    SSHShared.MonoFocus.Script.CheckEnterTarget();
            }
        }

        // Отображение указателя перемещения корабля
        private void MovePathLine()
        {
            Vector3 LPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            LPos = Camera.main.ScreenToWorldPoint(LPos);
            LPos.y = FSelf.Transform.position.y + 2;
            _ShipLinePath.SetPosition(0, FSelf.Transform.position);
            _ShipLinePath.SetPosition(_ShipLinePath.positionCount - 1, LPos);
        }

        // UI графика: владелец корябля
        private void ShowCornerColor()
        {
            if (FSelf.Owner.Role == SSHRole.Self)
            {
                _CountBackground.color = SSHLocale.IntToColor(C_ColorSelfBack);
                _CountCorner.color = SSHLocale.IntToColor(C_ColorSelfCorner);
            }
            else if (FSelf.Owner.Role == SSHRole.Enemy)
            {
                _CountBackground.color = SSHLocale.IntToColor(C_ColorEnemyBack);
                _CountCorner.color = SSHLocale.IntToColor(C_ColorEnemyCorner);
            }
            else
            {
                _CountBackground.color = SSHLocale.IntToColor(C_ColorNeutralBack);
                _CountCorner.color = SSHLocale.IntToColor(C_ColorNeutralCorner);
            }
        }

        // UI графика: состояние корабля
        private void ShowCornerState()
        {
            // При стройке или в походке нечего мигать, ибо ваистину
            if ((FSelf.Mode == ShipMode.Blocked) || (FSelf.Mode == ShipMode.Offline) || (FSelf.State == ShipState.Disabled))
            {
                _CountCorner.gameObject.SetActive(false);
                return;
            }
            // Не мигаем только если избыток или не в покое
            if ((FSelf.State == ShipState.Available) && (FSelf.Mode != ShipMode.Full))
            {
                _CountCorner.gameObject.SetActive(true);
                return;
            }
            // Мигаем
            if (Time.time >= FTimerCornerState + C_ChangeStateInterval)
            {
                _CountCorner.gameObject.SetActive(!_CountCorner.isActiveAndEnabled);
                FTimerCornerState = Time.time;
            }
        }

        // Атака нацеленного юнита
        private void AttackTarget()
        {
            foreach (ShipWeapon LWeapon in FSelf.Weapons)
                LWeapon.Attack();
        }

        // Получение угла для следования в виде паровозика
        private void RotateAroundPlanet()
        {
            // Пропускаем кто не в походке и не стационарный
            if (!FSelf.IsAvail())
                return;
            // Установка угла вращения
            Vector3 LDirection = FSelf.Planet.Transform.position - FSelf.Transform.position;
            FShipModel.rotation = Quaternion.FromToRotation(Vector3.forward, LDirection) * Quaternion.Euler(0, 90, 0);
        }

        // Движение корабля до указанной цели
        private void MoveToTarget()
        {
            // Передвижение
            if (Vector3.Distance(FSelf.Landing.Transform.position, FSelf.Transform.position) > C_FlyStopDistance)
            {
                Vector3 LDirection = FSelf.Landing.Transform.position - FSelf.Transform.position;
                FSelf.Transform.position = Vector3.MoveTowards(FSelf.Transform.position, FSelf.Landing.Transform.position, FFLySpeed * Time.deltaTime);
                if (LDirection != Vector3.zero)
                {
                    Quaternion LTarget = Quaternion.LookRotation(LDirection);
                    FShipModel.rotation = Quaternion.RotateTowards(FShipModel.rotation, LTarget, -200 * Time.deltaTime);
                }
            }
            // Доворот на месте
            else
            {
                Vector3 LDirection = FSelf.Planet.Transform.position - FSelf.Transform.position;
                Quaternion LRotation = Quaternion.FromToRotation(Vector3.forward, LDirection) * Quaternion.Euler(0, 90, 0);
                FShipModel.rotation = Quaternion.RotateTowards(FShipModel.rotation, LRotation, 200 * Time.deltaTime);
            }
        }

        // Отображение отсчета таймера
        private void UpdateTimer()
        {
            int LTime;
            // Определим с какого таймера брать данные
            if (FSelf.Timer(ShipTimer.PortalJump) > 0)
                LTime = FSelf.Timer(ShipTimer.PortalJump);
            else if (FSelf.Timer(ShipTimer.Annihilation) > 0)
                LTime = FSelf.Timer(ShipTimer.Annihilation);
            else if (FSelf.Timer(ShipTimer.Construction) > 0)
                LTime = FSelf.Timer(ShipTimer.Construction);
            else
                return;
            // Определим оставшееся время, прячем расхождение секунды с сервером
            if (LTime < 0)
                _Timer.text = SSHLocale.SecondsToString(LTime);
            else if (LTime - Mathf.RoundToInt(Time.time) >= 0)
                _Timer.text = SSHLocale.SecondsToString(LTime - Mathf.RoundToInt(Time.time));

        }

        #endregion

        #region Внешние вызовы

        // Добавление пути линии перемещения
        public void GroupPathAdd(Planet APlanet)
        {
            _ShipLinePath.SetPosition(_ShipLinePath.positionCount - 1, APlanet.Transform.position);
            _ShipLinePath.positionCount++;
        }

        // Удаление пути линии перемещения
        public void GroupPathRemove()
        {
            if (_ShipLinePath.positionCount > 2)
                _ShipLinePath.positionCount--;
        }

        // Перемещение на указанный слот планеты
        public void MoveToLanding(Planet APlanet, int ASlot)
        {
            FSelf.Landing = APlanet.SlotByIndex(ASlot);
            FSelf.Planet.Ships.Remove(FSelf);
            FSelf.Planet = APlanet;
            FSelf.Transform.SetParent(FSelf.Landing.Transform, false);
            FSelf.Planet.Ships.Add(FSelf);
            foreach (ShipWeapon LWeapon in FSelf.Weapons)
                LWeapon.Retarget(null);
        }

        // Команда физического перемещения кораблика
        public void MoveToPlanet(Planet APlanet, int ASlot)
        {
            // Определение точки перелета
            Vector3 LSavePosition = FSelf.Transform.position;
            // Переместим в новый слот
            MoveToLanding(APlanet, ASlot);
            // Определим скорость полета
            float LRange = Vector3.Distance(LSavePosition, FSelf.Transform.position);
            /* if (FSelf.State == ShipState.MovingLocal)*/
            FFLySpeed = C_FlySpeed * (LRange / C_SpeedInternal);
            /*else
                FFLySpeed = C_FlySpeed * (LRange / C_SpeedExternalL);*/
            // Выставим на предыдущее место для имитации полета
            FSelf.Transform.position = LSavePosition;
        }

        // Отображение пути перемещения
        public void ShowPath(bool AEnabled)
        {
            _ShipLinePath.enabled = AEnabled;
        }

        // Обновление признака привязки к планете
        public void UpdateAttach()
        {
            bool LEnabled = (FSelf.AttachedPlanet != null);
            // Выставим позиции
            if (LEnabled)
                _ShipLineAttach.SetPosition(1, FSelf.AttachedPlanet.Transform.position);
            // Для фокуса обновим состояние
            if (IsFocused)
                _ShipLineAttach.enabled = LEnabled;
        }

        // Обновление количества корабликов
        public void UpdateHP()
        {
            _CountValue.text = FSelf.Count.ToString();
        }

        // Обновление состояния корабля
        public void UpdateState()
        {
            // Включена постройка
            if (FSelf.Timer(ShipTimer.Constructor) > 0)
            {
                FOperationsPanel = FSelf.Transform.Find("Skills").Find("Construct").gameObject;
                FOperationsPanel.gameObject.SetActive(true);
                _Timer.transform.gameObject.SetActive(true);
            }
            // Включен прыжок в портал
            else if (FSelf.Timer(ShipTimer.PortalJump) > 0)
            {
                FOperationsPanel = FSelf.Transform.Find("Skills").Find("Portal").gameObject;
                FOperationsPanel.gameObject.SetActive(true);
                _Timer.transform.gameObject.SetActive(true);
            }
            // Включена аннигиляция
            else if (FSelf.Timer(ShipTimer.Annihilation) > 0)
            {
                FOperationsPanel = FSelf.Transform.Find("Skills").Find("Annihilation").gameObject;
                FOperationsPanel.gameObject.SetActive(true);
                _Timer.transform.gameObject.SetActive(true);
            }
            // Выключаем если была включена
            else
            {
                // Скроем панель если была показана
                if (FOperationsPanel != null)
                {
                    FOperationsPanel.gameObject.SetActive(false);
                    _Timer.transform.gameObject.SetActive(false);
                }
                // Если движение - включим двигатели, иначе - выключим
                FShipEngine.SetActive(FSelf.IsMove());
            }
        }

        #endregion
    }
}