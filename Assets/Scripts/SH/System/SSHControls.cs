/*///////////////////////////////////////////////
{                                              }
{     Модуль управления общими UI контролами   }
{       Copyright (c) 2016 UAshota             }
{                                              }
{  Rev A  2016.12.17                           }
{  Rev B  2017.05.15                           }
{                                              }
/*///////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;

public static class SSHControls
{
    // Заставка загрузки
    private static GameObject FUILoading;

    // Показать заставку загрузки
    public static void ShowLoading(string AText)
    {
        if (!FUILoading)
            FUILoading = Shared.PrefabManager.UILoading(GameObject.Find("UI").transform);
        FUILoading.transform.Find("Text").GetComponent<Text>().text = AText;
        FUILoading.SetActive(true);
    }

    // Скрыть заставку загрузки
    public static void HideLoading()
    {
        FUILoading.SetActive(false);
    }
}