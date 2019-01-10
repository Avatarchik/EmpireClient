/*///////////////////////////////////////////////
{                                              }
{ Модуль обработки скроллинга и зума планетарки}
{       Copyright (c) 2016 UAshota             }
{                                              }
{  Rev A  2016.12.15                           }
{  Rev B  2017.07.21                           }
{                                              }
/*///////////////////////////////////////////////
using UnityEngine;

public class MSHCommonMapControl : MonoBehaviour
{
    // Камера миникарты
    public Camera Mimimap;
    // Материал для паралакса
    public Material Paralax;
    // Объект выделения сеткой
    public Transform UISelection;

    // Множитель размера панели растягивающей паралакс
    private const float C_ParalaxDepth = 350;
    // Дельта дистанции прекращения доката миникарты
    private const float C_MiniMapStopDistance = 0.01f;
    // Скорость доката
    private const float C_MiniMapFlySpeed = 180f;
    // Скорость перемещения по карте
    private const float C_MapSpeed = 0.0131f;
    // Разрешение от которого отталкиваются все расчеты
    private const float C_Resolution = 768f;
    // Минимальный зум камеры
    private const float C_ZoomMin = 4f;
    // Максимальный зум камеры
    private const float C_ZoomMax = 12;
    // Шаг зума
    private const float C_ZoomStep = 0.5f;
    // Скорость прокрутки карты клавишами
    private const float C_KeyMapSpeed = 10f;
    // Ускорение прокрутки карты
    private const float C_KeyMapAcceleration = 10f;
    // Дистанция до фона
    private const float C_ParalaxDistanceBack = 300;
    // Дистанция до мелких звезд
    private const float C_ParalaxDistanceSmall = 150;
    // Дистанция до средних звезд
    private const float C_ParalaxDistanceMedium = 100;
    // Дистанция до больших звезд
    private const float C_ParalaxDistanceBig = 50;
    // Текущий зум камеры
    private float FZoomValue;
    // Позиция камеры сцены
    private Transform FGeneralCamera;
    // Позиция камеры миникарты
    private Transform FMinimapCamera;
    // Позиция панели карты
    private Transform FTransform;
    // Разрешение камере миникарты докатиться до места просмотра
    private bool FMinimapFollow;
    // Стартовый угол камеры
    private float FCameraDefaultSize;
    // Вычисляемый угол камеры
    private float FCameraSize;
    // Высота экрана для определения его изменения
    private int FScreenHeight;
    // Разрешение на перетаскивание карты
    private bool FMapMoveEnabled;
    // Последняя координата перемещения по карте
    private Vector3 FMapMovePos;


    private Vector3 FMapSelectionPos;

    void Start()
    {
        FTransform = transform;
        FMinimapCamera = Mimimap.transform;
        FGeneralCamera = Camera.main.transform;
        FZoomValue = Camera.main.orthographicSize;
        FCameraDefaultSize = FZoomValue;
        FMinimapFollow = true;
        DoSetResolution();
        DoStableZoom();
        DoStableParalax();
    }
    void Update()
    {
        DoSetResolution();
        DoFollowMinimap();
        DoCheckKeyboard();
        DoChangeMoving();
        DoChangeZoom();
    }

    // Расчет коэфициента сдвига угла камеры при изменении высоты клиента
    void DoSetResolution()
    {
        if (FScreenHeight == Screen.height)
            return;
        FScreenHeight = Screen.height;
        FCameraSize = Screen.height / C_Resolution * FCameraDefaultSize;
        Camera.main.orthographicSize = FCameraSize;
    }

    // Передвижение миникарты к указанной точке
    void DoFollowMinimap()
    {
        if (!FMinimapFollow)
            return;
        if (Vector3.Distance(FMinimapCamera.position, FGeneralCamera.position) > C_MiniMapStopDistance)
            FMinimapCamera.position = Vector3.MoveTowards(FMinimapCamera.position, FGeneralCamera.position, C_MiniMapFlySpeed * Time.deltaTime);
        else
            FMinimapFollow = false;
    }

    // Горячие клавиши перемещения по карту
    void DoCheckKeyboard()
    {
        float LLeftRight = 0;
        float LForwardBack = 0;
        float LAcceleration = 1;
        // Лево
        if (SSHShared.HotKey(KeyCode.A, true) || SSHShared.HotKey(KeyCode.LeftArrow, true))
            LLeftRight = C_KeyMapSpeed;
        // Право
        if (SSHShared.HotKey(KeyCode.D, true) || SSHShared.HotKey(KeyCode.RightArrow, true))
            LLeftRight = -C_KeyMapSpeed;
        // Верх 
        if (SSHShared.HotKey(KeyCode.W, true) || SSHShared.HotKey(KeyCode.UpArrow, true))
            LForwardBack = -C_KeyMapSpeed;
        // Вниз
        if (SSHShared.HotKey(KeyCode.S, true) || SSHShared.HotKey(KeyCode.DownArrow, true))
            LForwardBack = C_KeyMapSpeed;
        // Ускорение
        if (SSHShared.HotKey(KeyCode.LeftShift, true))
            LAcceleration = C_KeyMapAcceleration;
        // Смена позиции только если хоть одна кнопка нажата
        if ((LLeftRight != 0) || (LForwardBack != 0))
            ChangePosition(LLeftRight * LAcceleration * -C_MapSpeed, LForwardBack * LAcceleration * -C_MapSpeed, true);
    }

    // Прокрутка карты мышкой
    private void DoChangeMoving()
    {
        // Определим старт или продолжение перемещения по карте
        if ((SSHShared.MonoFocus == null) && Input.GetMouseButtonDown(0))
        {
            FMapMovePos = Input.mousePosition;
            FMapMoveEnabled = true;
        } else
        // Все отvеняем если нет перемещения
        if (!FMapMoveEnabled)
            return;
        // Отмена перемещения при отклике
        else if (Input.GetMouseButtonUp(0))
            FMapMoveEnabled = false;
        // Перетаскивание карты
        else if (Input.GetMouseButton(0) && (Input.mousePosition != FMapMovePos))
        {
            float LSpeed = C_MapSpeed * Camera.main.orthographicSize / FCameraDefaultSize;
            // Сбор координат дельты
            ChangePosition((FMapMovePos.x - Input.mousePosition.x) * LSpeed, (FMapMovePos.y - Input.mousePosition.y) * LSpeed, true);
            // Запомним позицию мыши
            FMapMovePos = Input.mousePosition;
        }
    }

    // Приближение и отдаление карты
    private void DoChangeZoom()
    {
        float LScrollWheel = Input.GetAxis("Mouse ScrollWheel");
        // Отдаление
        if ((LScrollWheel < 0) && (FZoomValue < C_ZoomMax))
        {
            Camera.main.orthographicSize += C_ZoomStep;
            FZoomValue = Camera.main.orthographicSize;
        }
        // Приближение
        else if ((LScrollWheel > 0) && (FZoomValue > C_ZoomMin))
        {
            Camera.main.orthographicSize -= C_ZoomStep;
            FZoomValue = Camera.main.orthographicSize;
        }
        // Стабилизируем паралакс на одной глубине
        if (LScrollWheel != 0)
            DoStableZoom();
    }

    // Паралакс должен всегда быть на одной глубине
    void DoStableZoom()
    {
        // Сменим масштаб паралакса
        float LSize = (Screen.width > Screen.height ? Screen.width : Screen.height) / C_ParalaxDepth * Camera.main.orthographicSize / FCameraDefaultSize;
        FTransform.localScale = new Vector3(LSize, 0, LSize);
    }

    // Сдвиг спрайтов паралакса
    void DoStableParalax()
    {
        Paralax.SetTextureOffset("_Background", new Vector2(
            FGeneralCamera.position.x / C_ParalaxDistanceBack,
            FGeneralCamera.position.z / C_ParalaxDistanceBack));
        Paralax.SetTextureOffset("_SmallStars", new Vector2(
            FGeneralCamera.position.x / C_ParalaxDistanceSmall,
            FGeneralCamera.position.z / C_ParalaxDistanceSmall));
        Paralax.SetTextureOffset("_MediumStars", new Vector2(
            FGeneralCamera.position.x / C_ParalaxDistanceMedium,
            FGeneralCamera.position.z / C_ParalaxDistanceMedium));
        Paralax.SetTextureOffset("_BigStars", new Vector2(
            FGeneralCamera.position.x / C_ParalaxDistanceBig,
            FGeneralCamera.position.z / C_ParalaxDistanceBig));
    }

    // Передвижение фона планетарной сетки
    void DoMovePlane(float AX, float AY)
    {
        /* магическая константа */
        if (AX < 0)
            AX = -0;
        if (AY > 0)
            AY = 0;
        if (AX > SSHShared.MapSize.x * 6.6f)
            AX = SSHShared.MapSize.x * 6.6f;
        if (AY < -SSHShared.MapSize.y * 6.6f)
            AY = -SSHShared.MapSize.y * 6.6f;
        FGeneralCamera.position = new Vector3(AX, FGeneralCamera.position.y, AY);
        // Передвинем палакс на указанную дельту
        DoStableParalax();
    }

    // Перемещение камеры в указанную точку
    public void ChangePosition(float AX, float AY, bool AMinimapFollow)
    {
        AX += Camera.main.transform.position.x;
        AY += Camera.main.transform.position.z;
        // Сдвиг камеры на ровную дельту
        DoMovePlane(AX, AY);
        // Переместим мини-карту
        FMinimapFollow = AMinimapFollow;
    }

    // Перемещение камеры в указанную точку с другого угла
    public void ChangePosition(float AX, float AY, float AOffsetX, float AOffsetY)
    {
        AX += AOffsetX * (FCameraSize / Camera.main.orthographicSize);
        AY += AOffsetY * (FCameraSize / Camera.main.orthographicSize);
        // Сдвиг камеры на ровную дельту
        DoMovePlane(AX, AY);
    }
}