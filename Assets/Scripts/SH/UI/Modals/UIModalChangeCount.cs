/////////////////////////////////////////////////
//
//  Модуль модального окна задания количества
//  Copyright(c) 2016 UAshota
//
//  Rev D  2018.06.23
//
/////////////////////////////////////////////////

using UnityEngine.UI;

namespace Shared
{
    //  Класс модального окна задания количества
    public class UIModalChangeCount : UIModalCustom
    {
        // Заголовок окна
        public Text _Caption;
        // Значение
        public InputField _Value;
        // Кнопка подтверждения
        public Button _ButtonYes;
        // Кнопка отмены
        public Button _ButtonNo;
        // Кнопка инкремента
        public Button _ButtonInc;
        // Кнопка декремента
        public Button _ButtonDec;
        // Кнопка сохранения изменений
        public Toggle _SaveChanges;

        // Признак необходимости показа фокуса 
        private bool FNeedFocus;
        // Минимально разрешенное значение
        private int FMin;
        // Максимально разрешенное значение
        private int FMax;
        // Текущее значение
        private int FValue;
        // Каллбак положительного решения
        private SSHShared.ModalActionInt FCallback;
        
        // Первый запуск
        private void Start()
        {
            _ButtonNo.onClick.AddListener(Close);
            _ButtonInc.onClick.AddListener(IncValue);
            _ButtonDec.onClick.AddListener(DecValue);
            _Value.onValueChanged.AddListener(OnChanged);
        }
                
        // Первое обновление выставляет фокус
        protected override void Update()
        {
            if (FNeedFocus)
            {
                FNeedFocus = false;
                _Value.Select();
            } else
                base.Update();
        }

        // Сохранение изменений
        protected override void DoApply()
        {
            FCallback(FValue, _SaveChanges.isOn);
            Close();
        }

        // Обработка смены количества
        private void OnChanged(string AValue)
        {
            if (!int.TryParse(_Value.text, out FValue))
                _Value.text = FMax.ToString();
        }

        // Обновление текущего значения
        private int ChangeValue(int ACount)
        {
            FValue = ACount;
            if (int.TryParse(_Value.text, out FValue))
            {
                FValue += ACount;
                if (FValue > FMax)
                    FValue = FMax;
                else
                if (FValue < FMin)
                    FValue = FMin;
            }
            if (ACount != 0)
                _Value.text = FValue.ToString();
            return FValue;
        }

        // Инкремент значения
        private void IncValue()
        {
            ChangeValue(+1);
        }

        // Декремент значения
        private void DecValue()
        {
            ChangeValue(-1);
        }

        // Показ самого модального окна
        public void Show(string ACaption, int AMin, int AMax, int AValue, bool AShowSave, SSHShared.ModalActionInt ACallback)
        {
            FCallback = ACallback;
            FMax = AMax;
            FMin = AMin;
            FValue = AValue;
            FNeedFocus = true;
            _Value.text = AValue.ToString();
            _Caption.text = ACaption;
            _ButtonYes.onClick.RemoveAllListeners();
            _ButtonYes.onClick.AddListener(DoApply);
            _SaveChanges.gameObject.SetActive(AShowSave);
            _SaveChanges.isOn = false;
        }
    }
}