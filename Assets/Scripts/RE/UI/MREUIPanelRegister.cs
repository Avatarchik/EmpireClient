/*///////////////////////////////////////////////
{                                              }
{      Заготовка панели регистрации            }
{       Copyright (c) 2016 UAshota             }
{                                              }
{                                              }
/*///////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;
using System;

public class MREUIPanelRegister : MSHUIMonoBehavior
{
    public InputField EditLogin;
    public InputField EditPassword;
    public Toggle RememberLogin;
    public Toggle RememberPassword;    
    public Text ErrorText;
    public Button ButtonLogin;
    public Transform UILogin;
    public Transform UIConnect;

    // Кукис
    private string FCookie;
    // Сообщение об ошибке
    private string FLastError = "";
    // Буфер команды
    SSHConnectionCommandBuffer FData;

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
        SSHConnection.ServerConnect();
    }

    public void ShowError(int AErrorCode)
    {
        if (AErrorCode == 1)
            FLastError = "Неверный Email или пароль";
    }

    public void Authorize(String APassword)
    {
        DoSaveSettings(APassword, Guid.NewGuid().ToString());
    }
}