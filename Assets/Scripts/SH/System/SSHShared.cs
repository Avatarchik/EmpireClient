/////////////////////////////////////////////////
//
//     Модуль общеигрового функционала          
//       Copyright (c) 2016 UAshota             
//                                              
//  Rev A  2016.12.28                           
//  Rev B  2017.05.15           
//                                             
/////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Text;

public enum ActionState
{
    Hidden,
    Disabled,
    Enabled
}


// Вспомогательный класс, помогающий enum паковать в байт-массив
public class TByteArray
{
    protected byte[] Combine(params byte[][] arrays)
    {
        byte[] rv = new byte[arrays.Sum(a => a.Length)];
        int offset = 0;
        foreach (byte[] array in arrays)
        {
            System.Buffer.BlockCopy(array, 0, rv, offset, array.Length);
            offset += array.Length;
        }
        return rv;
    }

    virtual protected byte[] getData()
    {
        byte[] LLink = new byte[0];

        return LLink;
    }

    public byte[] Data()
    {
        return getData();
    }

    public byte[] StringToByteArray(string str, int length)
    {
        return Encoding.UTF8.GetBytes(str.PadRight(length, ' '));
    }
}

// Игрок
public class Player
{
    public int UID;
    public int StorageLevel;
    public int CurrentSystem;
    public SSHRace Race;
    public int Credits;
    public int Fuel;
    public string Name;
    public SSHRole Role;
}


public class SSHShared
{

    public static int PlanetarID;

    public const string SettingsName = "Planetar.";
    /* временные игроки */
    private static Player StubNeutral;
    private static Player StubEnemy;

    private static Shared.Interactive FMonoSource;
    private static Shared.Interactive FMonoTarget;

    public delegate void ModalActionInt(int AResult, bool ASave);

    public delegate void OnMonoInteractiveChanged();

    // Параметры игрока
    public static Player Player;

    public static string monotouid(Shared.Interactive a)
    {
        if (a != null)
            return a.UID.ToString();
        else
            return "null";
    }

    // Объект, с которым начали какое-то действие
    public static Shared.Interactive MonoSource
    {
        get { return FMonoSource; }
        set
        {
            if (FMonoSource != value)
            {
                //if (Debug.isDebugBuild) Debug.Log("Source changed " + monotouid(FMonoSource) + " to " + monotouid(value));
                FMonoSource = value;
            }
            else if (Debug.isDebugBuild) Debug.Log("Dup mono source");
        }
    }

    // Объект, на котором действие завершилось
    public static Shared.Interactive MonoTarget
    {
        get { return FMonoTarget; }
        set
        {
            if (FMonoTarget != value)
            {
                //if (Debug.isDebugBuild) Debug.Log("Target changed " + monotouid(FMonoTarget) + " to " + monotouid(value));
                FMonoTarget = value;
            }
            else if (Debug.isDebugBuild) Debug.Log("Dup mono target");
        }
    }

    public static OnMonoInteractiveChanged OnActiveElementChanged;

    // Объект, на котором был последний фокус
    public static Shared.Interactive MonoFocus;

    // Моно контроллера карты
    public static MSHCommonMapControl MapControl;
    // Размер созвездия в секторах
    public static Vector2 MapSize;
    // UI базовая для всех панель
    public static Transform UI;
    // UI объект чата
    public static Planetar.MSHUIPanelChat UIChat;
    // UI объект мини-карты
    public static Transform UIMiniMap;

    public static Shared.UIModalManager UIModalManager;

    // Выпилить
    public static bool IsSystemLoaded;
    public static bool IsMainSceneStarted;

    static SSHShared()
    {
        Player = new Player
        {
            Role = SSHRole.Self,
            Name = "Player",
            Race = SSHRace.Gaal
        };
        StubNeutral = new Player
        {
            UID = 1,
            Name = "Locals",
            Role = SSHRole.Neutral,
            Race = SSHRace.Human
        };
        StubEnemy = new Player
        {
            Name = "Enemy ",
            Role = SSHRole.Enemy,
            Race = SSHRace.Human
        };

        UIModalManager = new Shared.UIModalManager();
    }

    private static bool DoCheckHotKey()
    {
        GameObject LGameObject = EventSystem.current.currentSelectedGameObject;
        return (!UIModalManager.Active && ((!LGameObject) || (LGameObject.tag != "INPUT")));
    }

    // Пока такой вариант блокировки хоткея
    public static bool HotKey(KeyCode AKeyCode, bool AHold)
    {
        return DoCheckHotKey() && (
            (AHold && Input.GetKey(AKeyCode)) || (!AHold && Input.GetKeyDown(AKeyCode)));
    }

    // Пока такой вариант блокировки хоткея
    public static bool HotKey(string AName)
    {
        return DoCheckHotKey() && Input.GetKeyDown(AName);
    }

    public static void ShowGalaxy()
    {
        SSHControls.ShowLoading("Вход в галактику");
        SceneManager.LoadSceneAsync("Galaxy");
    }

    public static void ShowPlanetar(int APlanetarID)
    {
        PlanetarID = APlanetarID;
        SSHControls.ShowLoading("Система найдена, загрузка...");
        SceneManager.LoadSceneAsync("Planetar");
    }

    public static void ShowWelcome(bool AClearCookie)
    {
        if (AClearCookie)
            PlayerPrefs.SetString("Cookie", "");
        SceneManager.LoadScene("Welcome");
        SSHConnection.Socket.Disconnect();
    }

    public static Player FindPlayer(int AUID)
    {
        if (AUID == Player.UID)
            return Player;
        else
        if (AUID == 1)
            return StubNeutral;
        else
            return StubEnemy;
    }
}