/*///////////////////////////////////////////////
{                                              }
{     Внешний класс слота хранилища            }
{       Copyright (c) 2016 UAshota             }
{                                              }
{  Rev A  2016.11.15                           }
{  Rev B  2017.05.15                           }
{                                              }
/*///////////////////////////////////////////////
using UnityEngine;

// Тип внешнего класса слота хранилища
public class TSHClassStorageSlot : Shared.Interactive
{
    // Подкреплен ли слот складом
    public bool Active;
    // Ссылка на хранилище в котором лежит слот
    public bool PlayerStorage;
    // Ссылка на ресурс если есть
    public Shared.Resource Resource;

    // Ссылка на игровой скрипт
    private  Shared.MSHObjectStorageSlot FScript;

    // Конструктор сразу определяет тип данных
    public TSHClassStorageSlot(Transform AParent, int AUID, bool APlayerStorage)
    {
        UID = AUID;
        PlayerStorage = APlayerStorage;
        Transform = Shared.PrefabManager.CreateStorageSlot(AParent).transform;
        Transform.SetParent(AParent, false);
        FScript = Transform.GetComponent<Shared.MSHObjectStorageSlot>();
        FScript.Init(this);
    }

    // Повышение яркости слота
    public void FadeIn()
    {
        FScript.FadeIn();
    }

    // Возвращение яркости слота
    public void FadeOut()
    {
        FScript.FadeOut();
    }

    // Включение или выключение надписи количества ресурсов
    public void ChangeEnable(bool AEnabled)
    {
        FScript.ChangeEnable(AEnabled);
    }

    // Смена количества елиниц ресурсов у слота
    public void ChangeCount(int AValue)
    {
        FScript.ChangeCount(AValue);
    }

    // Включение или выключение самого слота
    public void ChangeState(bool AActive, Sprite ABackground)
    {
        FScript.ChangeState(AActive, ABackground);
    }
}