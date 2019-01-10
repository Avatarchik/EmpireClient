/*///////////////////////////////////////////////
//                                            // 
// Панель авторизации                         //  
// Copyright (c) 2016 UAshota                 //  
//                                            //  
// Rev A  2016.12.28                          //  
// Rev B  2017.05.15                          //  
//                                            // 
/*///////////////////////////////////////////////

using UnityEngine;
using UnityEngine.UI;
using System;

public class MWEUIPanelAuth : MSHUIMonoBehavior
{
    // Логин
    public InputField EditLogin;
    // Пароль
    public InputField EditPassword;
    // Запомнить логин
    public Toggle RememberLogin;
    // Запомнить пароль
    public Toggle RememberPassword;
    // Подключение к тестовому серверу
    public Toggle TestServer;
    // Сообщение о возникшей ошибке
    public Text ErrorText;
    // Кнопка входа
    public Button ButtonLogin;
    // Панель входа
    public Transform UILogin;
    // Панель ожидания соединения
    public Transform UIConnect;

    // Кукис
    private string FCookie;
    // Сообщение об ошибке
    private string FLastError = "";

    protected override void Start()
    {
        base.Start();
        // Коннектор
        SSHConnection.Socket.OnConnected = DoOnConnect;
        SSHConnection.Socket.OnError = DoOnError;
        // Настроим UI
        ButtonLogin.onClick.AddListener(DoAuth);
        if (!RememberLogin.isOn)
            EditLogin.Select();
        // Загрузим настройки
        DoLoadSettings();
        // Попробуем сразу войти по кукисам
        if (GetAutoAuth())
            DoAuth();
    }

    protected override void Update()
    {
        base.Update();
        // Хоткей на вход
        if (Input.GetKeyDown(KeyCode.Return))
            DoAuth();
        // Обработаем последнюю ошибку
        if (FLastError != "")
        {
            DoShowLogin(false);
            FLastError = "";
        }
    }

    private bool GetAutoAuth()
    {
        return ((FCookie != "")
            && RememberLogin.isOn
            && RememberPassword.isOn);
    }

    private void DoShowLogin(bool AValue)
    {
        ErrorText.text = FLastError;
        ErrorText.enabled = !AValue;
        UILogin.gameObject.SetActive(!AValue);
        UIConnect.gameObject.SetActive(AValue);
    }

    private void DoLoadSettings()
    {
        FCookie = PlayerPrefs.GetString("Cookie");
        EditLogin.text = PlayerPrefs.GetString("Login");
        EditPassword.text = PlayerPrefs.GetString("Password");
        RememberLogin.isOn = Convert.ToBoolean(PlayerPrefs.GetInt("RememberLogin"));
        RememberPassword.isOn = Convert.ToBoolean(PlayerPrefs.GetInt("RememberPassword"));
        TestServer.isOn = Convert.ToBoolean(PlayerPrefs.GetInt("TestServer"));
        // Добавим листенеры после считывания значений
        RememberLogin.onValueChanged.AddListener(DoSaveSettings);
        RememberPassword.onValueChanged.AddListener(DoSaveSettings);
    }

    private void DoSaveSettings(string APassword, string ACookie)
    {
        // Сохраним логин
        if (RememberLogin.isOn)
            PlayerPrefs.SetString("Login", EditLogin.text);
        else
            PlayerPrefs.SetString("Login", "");
        // Сохраним пароль
        if (RememberPassword.isOn)
            PlayerPrefs.SetString("Password", APassword);
        else
            PlayerPrefs.SetString("Password", "");
        // Сохраним кукисы и параметры сохранения
        PlayerPrefs.SetString("Cookie", ACookie);
        PlayerPrefs.SetInt("RememberLogin", Convert.ToInt32(RememberLogin.isOn));
        PlayerPrefs.SetInt("RememberPassword", Convert.ToInt32(RememberPassword.isOn));
        PlayerPrefs.SetInt("TestServer", Convert.ToInt32(TestServer.isOn));
        // Сбросим дамп
        PlayerPrefs.Save();
    }

    private void DoSaveSettings(bool AValue)
    {
        DoSaveSettings(EditPassword.text, FCookie);
    }

    private void DoOnError(string AMessage)
    {
        FLastError = AMessage;
        FCookie = "";
    }

    private void DoOnConnect()
    {        
        SWEShared.SocketWriter.Login(EditLogin.text, EditPassword.text);
    }

    private void DoAuth()
    {
        DoSaveSettings(EditPassword.text, FCookie);
        DoShowLogin(true);
        SSHConnection.TestServer = TestServer.isOn;
        SSHConnection.ServerConnect();
    }

    public void ShowError(int AErrorCode)
    {
        if (AErrorCode == 1)
            FLastError = "Неверный Email или пароль";
        else if (AErrorCode == 0x1F01)
            FLastError = "Созвездие не загружено";
        else if (AErrorCode == 0x1F02)
            FLastError = "Созвездие в процессе запуска";
        else
            FLastError = "Неизвестная ошибка " + AErrorCode;
    }

    public void SavePassword(String APassword)
    {
        DoSaveSettings(APassword, Guid.NewGuid().ToString());
    }
}