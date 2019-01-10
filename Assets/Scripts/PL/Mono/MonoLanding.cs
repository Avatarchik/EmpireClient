/////////////////////////////////////////////////
//
//  Модуль обработки действий планетарного слота 
//  Copyright (c) 2016 UAshota                   
//                                              
//  Rev A  2016.12.15                            
//  Rev B  2017.06.06                            
//  Rev D  2018.06.22
//
/////////////////////////////////////////////////

using UnityEngine;

namespace Planetar
{
    // Класс обработки действий планетарного слота 
    public class MonoLanding : Shared.MonoInteractive
    {
        // Показываемый спрайт
        public SpriteRenderer _Model;
        // Статический спрайт
        public Sprite _SpritePassive;
        // Динамический спрайт
        public Sprite _SpriteActive;
        // Коллайдер для отключения в нективном режиме
        public Collider _Collider;

        // Внутренний объект описывающий слот
        private Landing FSelf;
        // Показ внешних слотов
        private bool FShowOuterSlots;
        // Показ внутренних слотов
        private bool FShowInnerSlots;

        // Загрузка данных слота
        protected override void DoInit(Shared.Interactive ASubject)
        {
            FSelf = (Landing)ASubject;
            _Model.sprite = _SpritePassive;
        }

        // В слот можно входить всегда
        protected override bool DoEnterTarget()
        {
            return true;
        }

        // Показ фокуса
        protected override void DoShowFocus(bool AApproove)
        {
            _Model.sprite = _SpriteActive;
        }

        // Скрытие фокуса
        protected override void DoHideFocus()
        {
            _Model.sprite = _SpritePassive;
        }

        // Показ или скрытие слота по заданным значениям
        public void Show(bool AShowOuterSlots, bool AShowInnerSlots)
        {
            FShowOuterSlots = AShowOuterSlots;
            FShowInnerSlots = AShowInnerSlots;
            Show();
        }

        // Показ или скрытие слота по кэшированным значениям
        public void Show()
        {
            _Collider.enabled = (FSelf.Ship == null) && FShowOuterSlots && ((!FSelf.IsLowOrbit) || (FShowInnerSlots && FSelf.IsLowOrbit));
            _Model.gameObject.SetActive(_Collider.enabled);
        }
    }
}