/*///////////////////////////////////////////////
{                                              }
{ Planetar                                     }
{ Модуль обработки действий планетарной планеты}
{ Copyright (c) 2016 UAshota                   }
{                                              }
{ Rev A  2016.12.17                            }
{ Rev B  2017.06.06                            }
{ Rev C  2017.09.26                            }
{                                              }
/*///////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;

namespace Planetar
{
    public class MonoPlanet : Shared.MonoInteractive
    {
        #region Паблик

        // Коллайдер планеты для отключения у ЧТ
        public SphereCollider _Collider;
        // Картинка выделения планеты
        public SpriteRenderer _ImageFocus;
        // Спрайт пассивного выделения
        public Sprite _SpriteFocusActive;
        // Спрайт активного выделения
        public Sprite _SpriteFocusInactive;
        // Интерактивный массив объектов
        public GameObject _UI;
        // Сам объект таймера
        public GameObject _Timer;
        // Фон таймера
        public SpriteRenderer _TimerBackground;
        // Текст таймера
        public TextMesh _TimerText;
        // Маска осветления
        public SpriteRenderer _FogActive;
        // Маска затемнения
        public SpriteRenderer _FogPassive;
        // Крутилки на краях осветления
        public GameObject _FogCorner;
        // Объект ядра миникарты
        public SpriteRenderer _MinimapCore;
        // Объект оболочки миникарты
        public SpriteRenderer _MinimapControl;
        // Объект оболочки миникарты
        public SpriteRenderer _MinimapWar;
        // Объект статического торгового пути
        public Transform _PathTrade;
        // Спрайт статического торгового пути
        public SpriteRenderer _PathTradeImage;
        // Объект динамического торгового пути
        public LineRenderer _PathSelection;
        // Массив слотов
        public Transform _SlotsArray;
        // Метка имени владельца
        public TextMesh _OwnerName;
        // Картинка захвата владельца
        public Image _OwnerLevel;
        // Картинка захвата врага
        public Image _OwnerCapture;
        // Объект портала планеты
        public GameObject _Portal;
        // Объект аннигиляции
        public GameObject _Annihilation;
        // Сфера планеты для отключения у ЧТ
        public GameObject _Sphere;

        #endregion

        #region Константы

        // Расстояние до боевого слота
        private const float C_FightSlotRange = 1.4f;
        // Расстояние до торгового слотв
        private const float C_TradeSlotRange = 0.7f;
        // Количество боевых слотов
        private const int C_FightSlotCount = 14;
        // Количество слотов с экономическими
        private const int C_TotalSlotCount = C_FightSlotCount + 5;
        // Скорость вращения слотов
        private const float C_SlotsSpeed = 0.005f;
        // Дельта смещения активации установки торгового пути
        private const float C_PathDelta = 5f;
        // Радиус длины выбора транспортного пути
        private const float C_TradePathRadius = 8.3f;
        // Размер полной освещаемой области
        private const float C_FogSizeActive = 6.6f;
        // Размер частичной освещаемой области
        private const float C_FogSizePassive = 2.0f;

        #endregion

        #region Переменные

        // Ядро планеты описывающее игровой объект
        private Planet FSelf;

        // Массив слотов
        private Landing[] FLanding;
        // Признак отображения внутренних слотов
        private bool FIsShowInnerSlots;
        // Угол вращения слотов
        private float FSlotsAngle = 0;

        private bool showingfromany;

        private bool FLandingVisible;

        private bool LCoreVisible;

        private bool LHangarApproove;

        private bool qwe;

        #endregion

        #region События

        private void Update()
        {
            // Неподписанные и видимые в камере планеты игнорируются
            if (FSelf.Subscription != PlanetSubscription.Enabled)
                return;
            // Обновим таймер времени активности
            if (FSelf.IsTiming)
                _TimerText.text = SSHLocale.SecondsToString(Mathf.RoundToInt(FSelf.TimerStateTime - (Time.time - FSelf.TimerUpdateTime)));
            // Повернем слоты вокруг сферы
            DoSlotsRotate();
        }

        // Определение объекта планеты
        protected override void DoInit(Shared.Interactive ASubject)
        {
            FSelf = (Planet)ASubject;
            // Покажем таймер для тайминговых планет
            if (FSelf.IsTiming)
                _Timer.SetActive(true);
            // Скроем сферу ЧТ
            if (FSelf.PlanetType == PlanetType.Hole)
                DoShowSphere(false);
            // Обновим имя на карте
            DoUpdateName();
            // Обновим миникарту
            UpdateMinimap(true, true, true);
        }

        protected override void DoOnBecameVisible()
        {

            FSelf.SetActive(true);
        }

        protected override void DoOnBecameInvisible()
        {
            FSelf.SetActive(false);
        }

        #endregion

        #region Операции с планетой

        protected override void DoOnActivationChanged()
        {
            bool LActiveOrDrag = (SSHShared.MonoSource != null) && (SSHShared.MonoSource.Script.IsActive || SSHShared.MonoSource.Script.IsDrag);
            // Если это ангар
            if ((SSHShared.MonoSource is Hangar) && LActiveOrDrag)
            {
                // Ландинг только на активных планетоидах
                if (FSelf.State == PlanetState.Active)
                {
                    if ((FSelf.IsBigHole) || ((!FSelf.IsTiming) && (!FSelf.InBattle)
                        && ((!FSelf.IsCoverageEnemy) || (FSelf.Owner.Role == SSHRole.Friend) || (FSelf.Owner.Role == SSHRole.Self))))
                    {
                        LHangarApproove = true;
                        if (!SSHShared.MonoSource.Script.IsDrag)
                        {
                            FLandingVisible = true;
                            FIsShowInnerSlots = false;
                            ShowLanding(true);
                        }
                        else
                        {
                            LCoreVisible = true;
                            DoShowFocus(LHangarApproove);
                        }
                        return;
                    }
                }

                LCoreVisible = true;
                LHangarApproove = (!FSelf.IsTiming) && (!FSelf.IsCoverageEnemy);
                DoShowFocus(LHangarApproove);
            }
            // Если это корабль
            else if ((SSHShared.MonoSource is Ship) && LActiveOrDrag)
            {
                Ship LShip = (Ship)SSHShared.MonoSource;
                if ((FSelf == LShip.Planet) || (!LShip.Tech(ShipTech.Stationary).Supported && FSelf.Links.Contains(LShip.Planet)))
                {
                    FLandingVisible = true;
                    FIsShowInnerSlots = false;
                    ShowLanding(true);
                    // Показ радиуса для текущей планеты
                    ShowRadius(FSelf == LShip.Planet);
                }
            }
            else
            // Не показываем ячейки если на планету никто не претендует или если планета неактивна
            {
                LHangarApproove = false;
                FIsShowInnerSlots = true;

                if (FLandingVisible)
                {

                    ShowRadius(false);
                    ShowLanding(false);
                    FLandingVisible = false;
                }
                if (LCoreVisible)
                {
                    LCoreVisible = false;
                    DoHideFocus();
                }
                return;
            }
        }

        // Показ фокуса
        protected override void DoShowFocus(bool AApproove)
        {
            if (AApproove)
                _ImageFocus.sprite = _SpriteFocusActive;
            else
                _ImageFocus.sprite = _SpriteFocusInactive;

            _ImageFocus.gameObject.SetActive(true);
        }

        // Скрытие фокуса
        protected override void DoHideFocus()
        {
            if (!LCoreVisible)
                _ImageFocus.gameObject.SetActive(false);
        }

        // Разрешение входа в элемент-источник
        protected override bool DoEnterSource()
        {
            return ((!FSelf.IsTiming) && (FSelf.State == PlanetState.Active));
        }

        // Разрешение входа в элемент-цель
        protected override bool DoEnterTarget()
        {
            if (LHangarApproove)
                return true;

            if (SSHShared.MonoSource is Ship)
            {
                Ship LShip = (Ship)SSHShared.MonoSource;
                // Драг на планету за пределами круга
                if (LShip.Script.IsDrag)
                {
                    if (LShip.Planet == FSelf)
                        return true;
                    if ((!LShip.Tech(ShipTech.Stationary).Supported) && LShip.Planet.Links.Contains(FSelf))
                        return true;
                }
                else
                    return true;
            }
            return false;
        }

        // Разрешение выхода из элемента-источника
        protected override bool DoLeaveSource()
        {

            return true;
        }

        // Сообщение завершенного клика
        protected override bool DoOnClick()
        {
            Debug.Log("planet click");
            return true;
        }

        // Сообщение активации
        protected override bool DoActivate()
        {
            if (Engine.ShipGroup.Ships.Count > 0)
            {
                // Если зажат контрол - сменим вхождение
                if (Engine.HotKey(KeyCode.LeftControl, true))
                {
                    if (Engine.ShipGroup.Planets.Contains(FSelf))
                        Engine.ShipGroup.RemovePath(FSelf);
                    else
                        Engine.ShipGroup.AddPath(FSelf);
                }
                else
                    // Иначе выстроим путь и отправим 
                    Engine.ShipGroup.Run(FSelf);
                return false;
            }
            return true;
        }










        // Показ и скрытие слотов
        private void ShowLanding(bool AShow)
        {

            // Создадим слоты
            if (FLanding == null)
                DoSlotsCreate();
            foreach (Landing LSlot in FLanding)
                LSlot.Show(AShow, FIsShowInnerSlots);
        }








        // Запуск механизма показа планетоида
        private void DoShow()
        {
            // Не подписываемся на невидимые, с запущенной подпиской или неактивные ЧТ
            if ((!FSelf.IsVisible())
                || (FSelf.Subscription == PlanetSubscription.Queryed)
                || (FSelf.PlanetType == PlanetType.Hole && FSelf.State == PlanetState.Inactive)
             )
                return;
            // Подпишемся если нет подписки и есть видимость
            if (FSelf.Subscription == PlanetSubscription.Disabled)
            {
                FSelf.Subscription = PlanetSubscription.Queryed;
                Engine.SocketWriter.PlanetSubscribe(FSelf);
            }
            else
                DoShowingAction(true);
        }

        // Запуск механизма сокрытия планетоида
        private void DoHide()
        {
            DoShowingAction(false);

            enabled = false;
        }



        // Выполнение механизма поточной обработки компонент планетоида
        private void DoShowingAction(bool AVisible)
        {
            // Создадим слоты
            if (FLanding == null)
                DoSlotsCreate();

            if (AVisible)
            {
                // Скрипт для обработки действий включаем всегда
                enabled = true;
            }
            bool LVisible = AVisible;
            // Обработаем показ
            if (LVisible)
            {
                // Для центра или бчт всегда полное освещение
            }
            else
            {
                if (FLandingVisible)
                    ShowLanding(false);
            }
            // Покажем сферу ЧТ
            if (FSelf.PlanetType == PlanetType.Hole)
                DoShowSphere(LVisible);
            // Переключим туманы        

            // Сменим видимость интерактивных элементов
            _UI.gameObject.SetActive(LVisible);
        }

        private void DoShowSphere(bool AValue)
        {
            _Sphere.SetActive(AValue);
            _Collider.enabled = AValue;
        }

        // Определение возможности выбора планеты для действий
        private void DoCheckSelection()
        {
            /*Interactive.ObjectType LSource = Interactive.ObjectType.Empty;/* Interactive.GetType(Shared.ObjectSource); */
            // Определим тип закраски
            /*if (
                // При закраске противника можно только прокладывать путь, смотреть инфо, высаживаться на свою планету или отправлять кораблик
                (FSelf.ControlEnemy > 0
                    && !FSelf.IsBigHole
                    && LSource != Interactive.ObjectType.PortalShip
                    && LSource != Interactive.ObjectType.Ship
                    && LSource != Interactive.ObjectType.FlyGroup
                    && LSource != Interactive.ObjectType.Empty
                    && FSelf.Owner.Role != SSHRole.Friend
                    && FSelf.Owner.Role != SSHRole.Self
                )
                // На невидимые планетоиды можно только высаживаться из ангара или прокладывать путь
                || (!FSelf.IsVisible()
                    && LSource != Interactive.ObjectType.FlyGroup
                    && LSource != Interactive.ObjectType.Hangar)
                // Нельзя высаживаться на неактивные БЧТ и простые ЧТ или делать торговый путь на ЧТ
                || (FSelf.Type == PlanetType.Hole && (
                    (FSelf.State != PlanetState.Active)
                    || ((!FSelf.IsBigHole)
                        && (LSource != Interactive.ObjectType.Ship)
                        && (LSource != Interactive.ObjectType.FlyGroup)))
                 )
                // Нельзя высаживаться на пульсар
                || (FSelf.Type == PlanetType.Pulsar
                    && LSource == Interactive.ObjectType.Hangar)
                // Нельзя скидывать ресурс на не свою планету
                || (FSelf.Owner.Role != SSHRole.Self
                    && LSource == Interactive.ObjectType.Storage)
            )
                LShowingAccept = TShowingAction.Hide;
            else
                LShowingAccept = TShowingAction.Show;
            // Проверим необходимость обновить спрайт
            if (LShowingAccept == FShowingAction)
                return;
            // Сменим состояние выделения
            FShowingAccept = LShowingAccept;
            /*
            // Выставим нужный спрайт
            if (FShowingAccept == TShowingAction.Show)
            {
                SSHShared.ObjectTarget = FSelf;
                _Selection.sprite = _SelectionActive;
            }
            else
            {
                SSHShared.ObjectTarget = null;
                _Selection.sprite = _SelectionPassive;
            }*/
        }

        // Обработка клика по планете
        private void DoPlanetClick()
        {
            // Определим тип объекта
            /*Interactive.ObjectType LSource = Interactive.ObjectType.Empty; /*Interactive.GetType(Shared.ObjectSource);
            // Обработаем группу полетов
            if (LSource == Interactive.ObjectType.FlyGroup)
            {
                if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                    Engine.ShipGroup.AddPath(FSelf);
                else
                    Engine.ShipGroup.Run(FSelf);
                return;
            }
            // Обработаем высадку в свободный слот
            else if (LSource == Interactive.ObjectType.Hangar)
                ((Hangar)SSHShared.MonoSource).MoveToLanding();
            // Установим путь, если протягивали
            else if (LSource == Interactive.ObjectType.Planet)
                DoSetTradePath();
            // Проверим установку портала
            else if (LSource == Interactive.ObjectType.PortalShip)
                DoPortalPrepare();
            // Иначе проверим на возможность клика
            else
                DoPlanetDrag(true);*/
        }

        // Отправка сведений для открытия портала
        private void DoPortalPrepare()
        {
            // Выключим UI
            bool LFound = false;
            /*PortalShip LPortal = (PortalShip)SSHShared.MonoSource;*/
            /*LPortal.Initiator.SelectionEnable(false);
            // Проверим что портал не зацикливается сам на себя
            if (LPortal.Initiator.Planet == FSelf)
                return;*/
            // Найдем научку для портала
            foreach (Ship LShip in FSelf.Ships)
            {
                if ((LShip.ShipType == ShipType.Scient)
                    && (LShip.Owner.Role != SSHRole.Enemy))
                {
                    LFound = true;
                    break;
                }
            }
            // Если нет научки - открывать нечего
            if (!LFound)
                return;
            // Отправим данные портала
            /*SSHShared.ObjectSource = null;*/
            /*Shared.SocketWriter.ShipPortalOpen(LPortal, FSelf);*/
        }

        // Активация функционала установки торгового пути или свойства планеты
        private void DoPlanetDrag(bool AMouseUp)
        {
            // При активном выборе пути торговли нужно протянуть до курсора мышки
            if ((SSHShared.MonoSource != null) && (_PathSelection.enabled))
            {
                Vector3 LPosition = SSHLocale.PointToCircle(FSelf.Transform, C_TradePathRadius);
                LPosition.y = 0;
                _PathSelection.SetPosition(1, LPosition);
                return;
            }
            // Если планета недоступна или уже выбрана для протягивания - не запускаем функционал повторно
            /*if ((FShowingAccept != TShowingAction.Show) || (SSHShared.MonoSource != null))
                return;*/
            // Покажем детали планеты
            /*if (AMouseUp)
                Engine.UIPlanetDetails.Show(FSelf);
            // Или начнем протягивание пути
            else if ((FSelf.ControlEnemy == 0)
                && (FSelf.Type != PlanetType.Hole)
                && (Vector3.Distance(Input.mousePosition, FClickPoint) > C_PathDelta))
            {
                /*SSHShared.MonoSource = FSelf;
                _PathSelection.enabled = true;
                ShowRadius(true);
                DoPlanetDrag(AMouseUp);
            }*/
        }

        // Установка торгового пути с другой планетой
        private void DoSetTradePath()
        {
            // При наличии таргетной планеты, протянем путь до нее или обнулим текущий
            //Planet LPlanet;
            /*if (Interactive.IsPlanet(Shared.ObjectTarget, out LPlanet) || (LPlanet == FSelf))
                Shared.SocketWriter.SendPlanetTradePath(FSelf.UID, LPlanet.UID);*/
            // Выключим эффекты
            _PathSelection.enabled = false;
            ShowRadius(false);
            // Нужно обнулить все активные объекты
            /*SSHShared.ObjectSource = null;*/
        }

        // Установка флагов отображения массива слотов для высадки корабликов
        private bool DoCheckMouseOver()
        {
            return true;


            // Если претендует корабль
            //Ship LShip;
            /*if (Interactive.IsShip(Shared.ObjectSource, out LShip))
            {
                // Также не показываем если объект стационарный и планеты разные
                if ((LShip.Stationary) && (FSelf != LShip.Planet))
                    return false;
                // И не показываем если большое расстояние
                if ((FSelf != LShip.Planet) && !FSelf.Links.Contains(LShip.Planet))
                    return false;
                FIsShowInnerSlots = LShip.UseLowOrbit;
            }
            else
            {
                // Высадка с ангара
                Hangar LPark;

                else
                    return false;
            }*/
        }

        // Создание массива слотов
        private void DoSlotsCreate()
        {
            FLanding = new Landing[C_TotalSlotCount + 1];
            //  Слоты бывают внутренние и внешние, создаем сразу оба типа
            for (int LIndex = 0; LIndex <= C_TotalSlotCount; LIndex++)
            {
                Landing LLanding = new Landing(LIndex, _SlotsArray, FSelf, LIndex > C_FightSlotCount);
                FLanding[LIndex] = LLanding;
                // Расположем на орбите
                if (LLanding.IsLowOrbit)
                    LLanding.Transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            }
        }

        // Вращение слотов
        private void DoSlotsRotate()
        {
            float LOrbitCount;
            float LRange;
            float LAngle;
            // Вращение конкретного слота в обратную сторону для видимости стабильного юнита
            foreach (Landing LSlot in FLanding)
            {
                // Расположем на орбите
                if (!LSlot.IsLowOrbit)
                {
                    LOrbitCount = C_FightSlotCount + 1;
                    LRange = C_FightSlotRange;
                }
                else
                {
                    LOrbitCount = (C_TotalSlotCount - C_FightSlotCount);
                    LRange = C_TradeSlotRange;
                }
                // Определим скорость смещения
                FSlotsAngle += Time.deltaTime * C_SlotsSpeed;
                // Определим угол вращения
                LAngle = LSlot.UID / LOrbitCount * 2 * Mathf.PI + FSlotsAngle;
                // Перенесем слот
                LSlot.Transform.localPosition = new Vector3(Mathf.Sin(LAngle) * LRange, 0, Mathf.Cos(LAngle) * LRange);
            }
        }

        // Обновление надписи владельца планеты
        private void DoUpdateName()
        {
            FSelf.Name = FSelf.UID.ToString();
            // Для пустого имени убираем меш
            // if (FSelf.Name.Length == 0)
            //  {
            //      if (_OwnerName.gameObject.activeSelf)
            //          _OwnerName.gameObject.SetActive(false);
            //  }
            // else
            {
                // Смену имени проводим если планета называется по ругму или кем-то захвачена
                //  if ((FSelf.PlanetType == PlanetType.Big) || (FSelf.PlanetType == PlanetType.Small))
                _OwnerName.text = FSelf.Class.ToString() + "\r\n<color=\"#" + ColorUtility.ToHtmlStringRGB(_OwnerLevel.color) + "\">" + FSelf.Name + "</color>";
                // else
                //     _OwnerName.text = FSelf.Name;
                _OwnerName.gameObject.SetActive(true);
            }
        }

        // Обновление роли планеты
        private void DoUpdateRole()
        {
            /* цвета вынести */
            switch (FSelf.Owner.Role)
            {
                case SSHRole.Self:
                    _OwnerLevel.color = SSHLocale.IntToColor(0x00CAFFB2); break;
                case SSHRole.Friend:
                    _OwnerLevel.color = Color.green; break;
                case SSHRole.Neutral:
                    _OwnerLevel.color = SSHLocale.IntToColor(0xF0F0F0AE); break;
                case SSHRole.Enemy:
                    _OwnerLevel.color = SSHLocale.IntToColor(0xFF8E0090); break;
            }
        }

        #endregion

        #region Внешние вызовы

        // Возврат объекта слота планеты по его индексу
        public Landing SlotByIndex(int ASlot)
        {
            // Создадим слоты
            if (FLanding == null)
                DoSlotsCreate();

            return FLanding[ASlot];
        }

        // Показ границы перелета
        public void ShowRadius(bool AValue)
        {
            _FogCorner.SetActive(AValue);
        }

        // Обновление параметров миникарты
        public void UpdateMinimap(bool ACore, bool AControl, bool AWar)
        {
            // Меняем детали миникарты
            if (ACore)
                _MinimapCore.sprite = SpriteManager.MinimapPlanetCore(FSelf);
            if (AControl)
                _MinimapControl.sprite = SpriteManager.MinimapPlanetControl(FSelf);
            if (AWar)
                _MinimapWar.sprite = SpriteManager.MinimapPlanetWar(false, false, false);

            if (FSelf.Owner.Role == SSHRole.Self)
                _FogPassive.color = Color.white;
            else
                _FogPassive.color = Color.gray;

            _FogActive.enabled = FSelf.VisibleHard;
            _FogPassive.enabled = !FSelf.VisibleHard && (FSelf.VisibleSoft || !FSelf.IsCoverageEnemy);

        }

        // Обновление владельца планеты
        public void UpdateOwner()
        {
            DoUpdateRole();
            DoUpdateName();
        }

        // Обновление уровня лояльности
        public void UpdateCapture(int AValue, SSHRole ARole)
        {
            // Закрасим кружок
            _OwnerCapture.fillAmount = AValue / 100000f;
            // Под цвет владельца
            if (ARole == SSHRole.Self)
                _OwnerCapture.color = SSHLocale.IntToColor(0x00CAFF79);
            else if (ARole == SSHRole.Friend)
                _OwnerCapture.color = SSHLocale.IntToColor(0x00770079);
            else
                _OwnerCapture.color = SSHLocale.IntToColor(0xFF8E0090);
            // Выключим графику захвата
            if (AValue == 0)
            {
                if (FSelf.CaptureValue > 0)
                {
                    _OwnerCapture.enabled = false;
                    _OwnerLevel.enabled = false;
                }
            }
            // Включим графику захвата
            else if (FSelf.CaptureValue == 0)
            {
                _OwnerCapture.enabled = true;
                _OwnerLevel.enabled = true;
            }
        }

        // Обновление расположения торгового пути
        public void UpdateTradePath()
        {
            // Выключение пути, если таргетной планеты более не существует
            if (FSelf.TradePlanet == null)
            {
                _PathTradeImage.enabled = false;
                return;
            }
            // Переменные чтобы не путаться
            float LSourceX = FSelf.Transform.position.x;
            float LSourceY = FSelf.Transform.position.z;
            float LTargetX = FSelf.TradePlanet.Transform.position.x;
            float LTargetY = FSelf.TradePlanet.Transform.position.z;
            // Дистанция между двумя планетами
            float LDistance = Vector3.Distance(FSelf.Transform.position, FSelf.TradePlanet.Transform.position);
            // Знаковый угол для поворота стрелки
            float LAngle = Mathf.Atan2(LTargetX - LSourceX, -LTargetY + LSourceY) * 180 / Mathf.PI;
            // Установка палки между двумя точками квадратом координат
            _PathTrade.localPosition = new Vector3((LTargetX - LSourceX) / 2f, (LTargetY - LSourceY) / 2f, 0);
            _PathTrade.localEulerAngles = new Vector3(0, 0, LAngle);
            _PathTradeImage.size = new Vector2(_PathTradeImage.size.x, LDistance - 1.5f);
            _PathTradeImage.enabled = true;
        }

        // Обновление состояния планеты, видимое всем игрокам
        public void UpdateState()
        {
            // Обновим таймер для тайминговых планет
            if (FSelf.IsTiming)
            {
                if (FSelf.State == PlanetState.Active)
                    _TimerBackground.color = SSHLocale.IntToColor(0x003807FF);
                else
                    _TimerBackground.color = SSHLocale.IntToColor(0x636363FF);
            }
        }

        // Показ или скрытие иконки портала и пути на миникарте
        public void ShowPortal(bool AVisible)
        {
            _Portal.SetActive(AVisible);
        }

        public void ShowLowGravity(bool AEnabled)
        {
            Debug.Log(AEnabled);
            _Annihilation.SetActive(AEnabled);
        }

        // Смена активности планетоида
        public void SetActive(bool AValue)
        {
            if (!AValue)
                DoHide();
            else
                DoShow();
        }

        #endregion
    }
}