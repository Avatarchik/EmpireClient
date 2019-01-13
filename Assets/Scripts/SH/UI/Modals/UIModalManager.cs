/////////////////////////////////////////////////
//
//  Модуль реализации модальных окошек
//  Copyright(c) 2016 UAshota
//
//  Rev D  2018.06.23
//
/////////////////////////////////////////////////

using UnityEngine;

namespace Shared
{
    // Класс реализации модальных окошек
    public class UIModalManager
    {
        // Признак активного модального окна
        public bool Active { get { return (FModalBackground) && FModalBackground.activeSelf; } }

        // Фон затемнения
        private GameObject FModalBackground;
        // Текущее открытое окно
        private UIModalCustom FCurrent;
        // Кэш окна показа количества с сохранением значения
        private UIModalChangeCount FModalChangeCount;

        // Пока фона затемнения
        private void ShowDialog(UIModalCustom AModal)
        {
            // Первое создание
            if (!FModalBackground)
            {
                FModalBackground = PrefabManager.CreateModalBackground();
                FModalBackground.transform.SetParent(SSHShared.UI, false);
            }            
            FCurrent = AModal;
            // Перемещаем на переднйи план
            FModalBackground.transform.SetAsLastSibling();
            FCurrent.transform.SetAsLastSibling();
            // Включаем
            FModalBackground.SetActive(true);
            FCurrent.gameObject.SetActive(true);
        }

        // Окно выбора количества
        public void ShowSeparateCount(string ACaption, int AMin, int AMax, int AValue, bool AShowSave, SSHShared.ModalActionInt ACallback)
        {
            // Первое создание
            if (!FModalChangeCount)
            {
                FModalChangeCount = PrefabManager.CreateModalChangeCount();
                FModalChangeCount.transform.SetParent(SSHShared.UI, false);
            }
            // Покажем наш диалог
            FModalChangeCount.Show(ACaption, AMin, AMax, AValue, AShowSave, ACallback);
            // Покажем фон
            ShowDialog(FModalChangeCount);
        }

        // Закрытие окна
        public void Close()
        {
            // Закроем само окно
            FCurrent.gameObject.SetActive(false);
            // Закроем тень
            FModalBackground.SetActive(false);
        }
    }
}