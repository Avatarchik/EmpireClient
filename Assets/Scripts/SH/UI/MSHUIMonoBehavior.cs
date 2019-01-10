/*///////////////////////////////////////////////
{                                              }
{   Модуль базовой обработки UI панелей        }
{       Copyright (c) 2016 UAshota             }
{                                              }
{                                              }
/*///////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MSHUIMonoBehavior : MonoBehaviour
{
    // Текущая система событий
    private EventSystem FEventSystem;

    protected virtual void Start()
    {
        FEventSystem = EventSystem.current;
    }

    protected virtual void Update()
    {
        DoCheckTabNavigation();
    }

    // Переход по элементам через Tab
    private void DoCheckTabNavigation()
    {
        // Табуляция по контролам
        if (!Input.GetKeyDown(KeyCode.Tab))
            return;
        // Поиск следующего контрола
        Selectable LNextObject = FEventSystem.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
        if (LNextObject == null)
            return;
        // Проверка на наличие текстового поля
        InputField LNextInputField = LNextObject.GetComponent<InputField>();
        if (LNextInputField != null)
            LNextInputField.OnPointerClick(new PointerEventData(FEventSystem));
        FEventSystem.SetSelectedGameObject(LNextObject.gameObject, new BaseEventData(FEventSystem));
    }
}