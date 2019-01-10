/*///////////////////////////////////////////////
{                                              }
{ Planetar                                     }
{ Панель планетарного радара                   }
{ Copyright (c) 2016 UAshota                   }
{                                              }
{ Rev B 2017.07.20                             }
{                                              }
/*///////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;

public class MonoUIPanelRadar : MonoBehaviour
{
    // Камера миникарты
    public Camera Camera;
    // Кнопка смены отображения
    public Button Maximize;
    // Кнопка уменьшения масштаба
    public Button ZoomIn;
    // Кнопка увеличения масштаба
    public Button ZoomOut;
    // Тип карты - перетаскивается или елозить
    public bool Interactive;

    // Отступ снизу при максимальной карте
    private const int C_MAP_OFFSET_BOTTOM = 55;
    // Скорость прокрути карты при выходе за ее пределы
    private const float C_RollOffSpeed = 0.548f;
    // Плавность зума
    private const float C_ZoomSpeed = 150f;
    // Шаг зума от исходного
    private const float C_ZoomOffset = 140f;
    // Шаг зума
    private const float C_ZoomStep = 20f;
    // Дефолтное значение камеры для расчета смещения
    private float FCameraZoom;
    // Нужное значение зума, до которого докатывается миникарта
    private float FCameraZoomTarget;
    // Точка последнего клика мышкой
    private Vector3 FMousePoint;
    // Признак отмены клика мышки при перетаскивании карты
    private bool FMouseClick;
    // Панель радара
    private RectTransform FRadarRect;
    // Базовый размер
    private Vector2 FRadarRectSize;
    // Кэш коллайдера
    private BoxCollider FCollider;
    // Центр коллайдера по умолчанию
    private Vector2 FColliderCenter;
    // Размер коллайдера по умолчанию
    private Vector3 FColliderSize;

    Vector3 zx;

    void Start()
    {
        Maximize.onClick.AddListener(DoShowMap);
        ZoomOut.onClick.AddListener(DoZoomIn);
        ZoomIn.onClick.AddListener(DoZoomOut);
        FCameraZoomTarget = Camera.orthographicSize;
        FCameraZoom = FCameraZoomTarget;
        FCollider = transform.GetComponent<BoxCollider>();
        FColliderCenter = FCollider.center;
        FColliderSize = FCollider.size;
        FRadarRect = transform.GetComponent<RectTransform>();
        FRadarRectSize = FRadarRect.offsetMin;
    }

    void OnMouseDrag()
    {
        if (Interactive)
            DoPointMap();
        else
            DoDragMap();
    }

    void OnMouseOver()
    {
        DoCheckMouse();
    }

    void Update()
    {
        if (SSHShared.HotKey(KeyCode.M, false))
            DoShowMap();
        if (SSHShared.HotKey(KeyCode.Escape, false))
            DoHideMap();
        if (FCameraZoomTarget != Camera.orthographicSize)
            DoRollZoom();
    }

    private void DoCheckMouse()
    {
        // Обработаем клик вниз
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            FMousePoint = Input.mousePosition;
            FMouseClick = true;
        }
        // Обработаем клик вверх
        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
        {
            if (FMouseClick)
                DoPointMap();
        }
        // Обработаем зажатые клавиши
        if (Input.GetMouseButton(1) && (Input.mousePosition != FMousePoint))
            DoDragMap();
        else
            DoScrollZoom();
        FMousePoint = Input.mousePosition;
    }

    // Увеличение карты
    private void DoShowMap()
    {
        if (FCollider.size != FColliderSize)
        {
            DoHideMap();
            return;
        }
        float LMaxSize = Mathf.Min(Camera.targetTexture.height, Screen.height - C_MAP_OFFSET_BOTTOM);
        FRadarRect.offsetMin = new Vector2(-LMaxSize, -LMaxSize - Mathf.Abs(FColliderCenter.y - FColliderCenter.x));
        FCollider.size = new Vector3(LMaxSize, LMaxSize);
        FCollider.center = new Vector3(-LMaxSize / 2, -LMaxSize / 2 - Mathf.Abs(FColliderCenter.y - FColliderCenter.x), 0);
    }

    // Уменьшение карты
    private void DoHideMap()
    {
        FRadarRect.offsetMin = FRadarRectSize;
        FCollider.center = FColliderCenter;
        FCollider.size = FColliderSize;
        SSHShared.MapControl.ChangePosition(0, 0, true);
    }

    // Перетаскивание области через смещение
    private void DoDragMap(float AX, float AY)
    {
        // Подсчитаем координаты смещения
        /* магическая константа */
        float LOffset = (FCameraZoom / Camera.orthographicSize * 3.65f);
        float LLeft = Camera.transform.position.x + AX / LOffset;
        float LTop = Camera.transform.position.z + AY / LOffset;

        if (LLeft < -3)
            LLeft = -3;
        if (LLeft > SSHShared.MapSize.x * 6.6f + 3)
            LLeft = SSHShared.MapSize.x * 6.6f + 3;
        if (LTop > 3)
            LTop = 3;
        if (LTop < -(SSHShared.MapSize.y * 6.6f+3))
            LTop = -(SSHShared.MapSize.y * 6.6f +3);

        // Переместим камеру
        Camera.transform.position = new Vector3(LLeft, Camera.transform.position.y, LTop);
        // Сохраним последнее положении
        FMouseClick = false;
    }

    // Перетаскивание области мышкой
    private void DoDragMap()
    {
        DoDragMap(FMousePoint.x - Input.mousePosition.x, FMousePoint.y - Input.mousePosition.y);
    }

    // Переход на точку на карте с миникарты
    private void DoPointMap()
    {
        RaycastHit LRayHit;
        Ray LRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(LRay, out LRayHit))
        {
            float LCoeff = (Camera.orthographicSize / FCameraZoom);
            float LOffsetLeft = (LRayHit.point.x - LRayHit.collider.bounds.center.x);
            float LOffsetTop = (LRayHit.point.z - LRayHit.collider.bounds.center.z);

            /* магическая константа */
            if ((LRayHit.collider != FCollider) || (Mathf.Abs(LRayHit.collider.bounds.extents.x / LOffsetLeft) <= 1.2f) || (Mathf.Abs(LRayHit.collider.bounds.extents.z / LOffsetTop) <= 1.15f))
            {
                float lcx = (Input.mousePosition.x - zx.x) / 25;
                float lcy = (Input.mousePosition.y - zx.y) / 25;
                SSHShared.MapControl.ChangePosition(lcx * C_RollOffSpeed, C_RollOffSpeed * lcy, false);
                DoDragMap(2f / LCoeff * lcx, 2f / LCoeff * lcy);
                return;
            }

            zx = Input.mousePosition;


            float LOffset = (LCoeff * 23.1f);

            LOffsetLeft *= LOffset;
            LOffsetTop *= LOffset;
            SSHShared.MapControl.ChangePosition(Camera.transform.position.x, Camera.transform.position.z, LOffsetLeft, LOffsetTop);
        }
    }

    // Докат зума камеры
    private void DoRollZoom()
    {
        // Размер инкремента камеры
        float LSize = Time.deltaTime * C_ZoomSpeed;
        // Смотрим сколько осталось докатить чтобы не перекатить
        float LBorder = Mathf.Abs(Camera.orthographicSize - FCameraZoomTarget);
        if (LBorder < LSize)
            LSize = LBorder;
        // Смотрим не нужно ли мотать в другую сторону
        if (FCameraZoomTarget < Camera.orthographicSize)
            LSize = -LSize;
        // Сменим угол камеры
        Camera.orthographicSize += LSize;
    }

    // Приближение или отдаление роликом
    private void DoScrollZoom()
    {
        float LScrollWheel = Input.GetAxis("Mouse ScrollWheel");
        // Отдаление
        if (LScrollWheel < 0)
            DoZoomIn();
        // Приближение
        else if (LScrollWheel > 0)
            DoZoomOut();
    }

    // Отдалить миникарту
    private void DoZoomIn()
    {
        if (FCameraZoomTarget < FCameraZoom + C_ZoomOffset)
            FCameraZoomTarget += C_ZoomStep;
    }

    // Приблизить миникарту
    private void DoZoomOut()
    {
        if (FCameraZoomTarget > FCameraZoom - C_ZoomOffset)
            FCameraZoomTarget -= C_ZoomStep;
    }
}