/*///////////////////////////////////////////////
{                                              }
{   Примитивный вариант чата на первое время   }
{       Copyright (c) 2016 UAshota             }
{                                              }
{                                              }
{  Rev A  2016.12.15                           }
{                                              }
/*///////////////////////////////////////////////
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Planetar
{

    public class MSHUIPanelChat : MonoBehaviour
    {
        // Поле ввода текста
        public InputField MessageField;
        // Текств  чате
        public Text Content;
        // Скроллер
        public Scrollbar Scroller;

        // Аккамулятор сообщений
        private string FText;

        void Start()
        {
            // Пока такой хак на постройку по клавише
            MessageField.onEndEdit.AddListener(Value =>
            {
                if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) && (Value.Length > 0))
                    DoSendMessage(Value);
                else
                    MessageField.DeactivateInputField();
            });
        }

        // Активация чата доступна по глобальному Enter хоткею
        void Update()
        {
            if ((!MessageField.isFocused) && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
                MessageField.ActivateInputField();
        }

        // После смены сообщения ждем обработки 5-ти кадров
        private IEnumerator DoCheckScroll()
        {
            bool LFlagScroll = (Scroller.value < 0.1f);
            Canvas.ForceUpdateCanvases();
            yield return new WaitForFixedUpdate();
            if ((Scroller.value != 0)
                && (LFlagScroll)) Scroller.value = 0;
        }

        // Отправка сообщения по хоткею
        void DoSendMessage(string AMessage)
        {
            Engine.SocketWriter.ChatMessage(AMessage);
            MessageField.text = "";
            MessageField.ActivateInputField();
        }

        // Отображение нового сообщения
        public void ShowMessage(string AMessage)
        {
            FText = FText + AMessage + "\n";
            Content.text = FText;
            // Если чат стыкован в конце - то прокрутим
            StartCoroutine(DoCheckScroll());
        }
    }
}