/////////////////////////////////////////////////
//
//  Интерактивный класс слота посадки кораблей   
//  Copyright (c) 2016 UAshota                   
//
//  Rev A  2016.11.15                            
//  Rev B  2017.06.06                            
//  Rev D  2018.06.22
//
/////////////////////////////////////////////////
using UnityEngine;

namespace Planetar
{
    // Тип внешнего класса слота посадки кораблей
    public class Landing : Shared.Interactive
    {
        // Планета слота
        public Planet Planet;
        // Объект слота
        public Ship Ship { get { return FShip; } set { SetShip(value); } }
        // Признак внутреннего слота
        public bool IsLowOrbit;

        // Привязка к игровому скрипту
        private MonoLanding FScript;
        // Объект слота
        public Ship FShip;

        // Установка объекта
        private void SetShip(Ship AShip)
        {
            FShip = AShip;
            FScript.Show();
        }

        // Конструктор сразу определяет тип данных
        public Landing(int AUID, Transform AParent, Planet APlanet, bool AIsLowOrbit)
        {
            UID = AUID;
            Planet = APlanet;
            IsLowOrbit = AIsLowOrbit;
            Transform = PrefabManager.CreateLandingSlot(AParent).transform;
            Transform.SetParent(AParent, false);
            FScript = Transform.GetComponent<MonoLanding>();
            FScript.Init(this);
        }

        // Показ или скрытие слота для посадки
        public void Show(bool AShowOuterSlots, bool AShowInnerSlots)
        {
            FScript.Show(AShowOuterSlots, AShowInnerSlots);
        }
    }
}