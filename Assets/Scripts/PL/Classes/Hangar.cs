/////////////////////////////////////////////////
//
//  Модуль класса слота панели флота       
//  Copyright (c) 2016 UAshota                   
//                                              
//  Rev A  2016.12.07                            
//  Rev B  2017.06.06     
//  Rev D  2018.06.22
//                                              
/////////////////////////////////////////////////
using UnityEngine;

namespace Planetar
{
    // Класс слота панели флота   
    public class Hangar : Shared.Interactive
    {
        // Тип корабля в панели флота
        public ShipType ShipType;
        // Количество кораблей в слоте
        public int Count;

        // Привязка к игровому скрипту
        private MonoHangar FScript;
        // Общий компонент хинта
        private static MonoUIHintHangar FHint;

        protected override Shared.MonoUIHintCustom DoGetHint()
        {
            if (!FHint)
                FHint = PrefabManager.CreateHangarHint();
            return FHint;
        }

        // Конструктор сразу определяет тип данных
        public Hangar(int AUID, Transform AParent)
        {
            UID = AUID;            
            ShipType = ShipType.Empty;
            Transform = PrefabManager.CreateHangarSlot(AParent).transform;
            Transform.SetParent(AParent, false);
            FScript = Transform.GetComponent<MonoHangar>();
            FScript.Init(this);
        }
        
        // Смена типа или количества кораблика
        public void Change(ShipType AShipType, int ACount)
        {
            FScript.Change(AShipType, ACount);
        }
    }
}