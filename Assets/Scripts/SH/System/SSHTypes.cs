/*///////////////////////////////////////////////
{                                              }
{ Модуль глобальных типов и констант всей игры }
{       Copyright (c) 2016 UAshota             }
{                                              }
{  Rev A  2016.12.17                           }
{  Rev B  2017.05.15                           }
{                                              }
/*///////////////////////////////////////////////
using UnityEngine;

// Расы
public enum SSHRace : int
{
    Empty,
    Human,
    Maloc,
    Peleng,
    Gaal,
    Feyan,
    Klisan
}

// Роли
public enum SSHRole : int
{
    Self,
    Enemy,
    Friend,
    Neutral
}

// Мусоросборник всяких функций
public static class SSHLocale
{
    // Количество в строку с сокращением, 100000 = 100к
    public static string CountToShortString(int ACount)
    {
        if (ACount >= 100000)
            return (double)(ACount / 1000) + "k";
        else
            return ACount.ToString();
    }

    // Количество в строку с разделением, 100000 = 100 000
    public static string CountToLongString(int ACount)
    {
        if (ACount >= 9999)
            return string.Format("{0:### ###}", ACount);
        else
            if (ACount >= 999)
                return string.Format("{0:# ###}", ACount);
            else
                return ACount.ToString();
    }

    // Hex цвет в Color
    public static Color IntToColor(uint AColor)
    {
        Color32 LResult = Color.clear;
        LResult.a = (byte)((AColor) & 0xFF);
        LResult.b = (byte)((AColor >> 8) & 0xFF);
        LResult.g = (byte)((AColor >> 16) & 0xFF);
        LResult.r = (byte)((AColor >> 24) & 0xFF);
        return LResult;
    }

    // Ограничение позиции точки в заданом радиусе
    public static Vector3 PointToCircle(Transform AObject, float ARadius)
    {
        Vector3 LMousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        LMousePos = Camera.main.ScreenToWorldPoint(LMousePos);
        LMousePos.y = AObject.position.y;
        // Шоб в трех соснах не заблудиться
        float LX2 = LMousePos.x;
        float LX1 = AObject.position.x;
        float LY2 = LMousePos.z;
        float LY1 = AObject.position.z;
        float LQuadr = Mathf.Sqrt(Mathf.Pow(LX2 - LX1, 2) + Mathf.Pow(LY2 - LY1, 2));
        // Точка возвращается в круг только если выходит за его пределы
        if (LQuadr > ARadius)
        {
            float LCoeff = ARadius / LQuadr;
            LMousePos.x = (LX2 - LX1) * LCoeff + LX1;
            LMousePos.z = (LY2 - LY1) * LCoeff + LY1;
        }
        return LMousePos;
    }

    public static string SecondsToString(int ASeconds)
    {
        string LText;
        bool LFrozen = ASeconds <= -1;

        if (LFrozen)
            ASeconds = -ASeconds;
        if (ASeconds < 60)
        {
            LText = string.Format("0:{0:00}", ASeconds);
        }
        else if (ASeconds < 3600)
        {
            int LMin = Mathf.RoundToInt(ASeconds / 60);
            int LSec = Mathf.RoundToInt(ASeconds % 60);
            LText = string.Format("{0:##}:{1:00}", LMin, LSec);
        }
        else
        {
            int LHour = Mathf.RoundToInt(ASeconds / 3600);
            int LMin = Mathf.RoundToInt((ASeconds - LHour * 3600) / 60);
            int LSec = Mathf.RoundToInt(ASeconds % 60);
            LText = string.Format("{0:##}:{1:00}:{2:00}", LHour, LMin, LSec);
        }
        if (LFrozen)
            LText = "<color=\"#00B5FFFF\">" + LText + "</color>";

        return LText;
    }
}