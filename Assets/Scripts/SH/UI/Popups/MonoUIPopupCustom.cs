/////////////////////////////////////////////////
//                                              
//  Класс базового объекта моно меню
//  Copyright (c) 2016 UAshota                   
//              
//  Rev D  2018.06.19
//                                              
/////////////////////////////////////////////////

using UnityEngine;

namespace Shared
{
    // Класс базового объекта моно меню
    public abstract class MonoUIPopupCustom : MonoBehaviour
    {
        // Ссылка на свой объект
        private GameObject FSelf;
        // Ссылка на свой трансформ
        private Transform FTransform;

        // Показ меню
        public void Show(Interactive AObject)
        {
            // При первом показе определим параметры
            if (FSelf == null)
            {
                FSelf = gameObject;
                FTransform = transform;
            }
            // Обновим данные в хинте
            DoChange(AObject);
            // Отпозиционируем относительно объекта
            FTransform.position = AObject.Transform.position;
            FTransform.localPosition = new Vector3(FTransform.localPosition.x, FTransform.localPosition.y, -50);/*magic*/
            // Покажем
            FSelf.SetActive(true);
        }

        // Скрытие меню
        public void Hide()
        {
            FSelf.SetActive(false);
        }

        // Обновление параметров меню
        protected abstract void DoChange(Interactive AObject);
    }
}