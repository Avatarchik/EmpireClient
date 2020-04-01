/*///////////////////////////////////////////////
{                                              }
{   Панель подсказки для планетарного корабля  }
{       Copyright (c) 2016 UAshota             }
{                                              }
{                                              }
/*///////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;

namespace Planetar
{
    public class MonoUIHintShip : Shared.MonoUIHintCustom
    {
        public Text OwnerName;
        public Text OwnerAliance;
        public Text Type;
        public Text Description;
        public Text Count;
        public Text Power;
        public Text Armor;
        public Text HP;
        public Text Damage;

        private Transform FTransform;
        private Ship FShip;
        private float FTime;

        private void Update()
        {
            if ((FShip != null) && (FTime + 0.5f >= Time.time))
                doshow();
        }

        private void doshow()
        {
            OwnerName.text = FShip.Owner.Name;
            Type.text = FShip.ShipType.ToString();
            FTime = Time.time;

            string LText = "Описание " + FShip.ShipType.ToString() + "\r\n";
            LText += "Номер в реестре: " + FShip.UID + "\r\n\r";

            if (FShip.AttachedPlanet != null)
            {
                LText += "Присоединен: " + FShip.AttachedPlanet.UID + ", " + FShip.AttachedPlanet.PlanetType + " : ";
                if (FShip.IsAutoTarget)
                    LText += "Автоприцел\r\n\r\n";
                else
                    LText += "Вручную\r\n\r\n";
            }
            LText += "Топливо: " + FShip.Fuel;
            if (FShip.Timer(ShipTimer.Refill) > 0)
                LText += " (" + SSHLocale.SecondsToString(FShip.Timer(ShipTimer.Refill) - Mathf.RoundToInt(Time.time)) + ")\r\n";
            else
                LText += "\r\n";

            if (FShip.Timer(ShipTimer.Constructor) > 0)
                LText += "Constructor: (" + SSHLocale.SecondsToString(FShip.Timer(ShipTimer.Constructor) - Mathf.RoundToInt(Time.time)) + ")\r\n";

            LText += "Убито: " + FShip.Destructed + "\r\n";
            LText += "В группе: " + FShip.Group + "\r\n";
            LText += "Инициатор: " + FShip.Script.IsActive + "\r\n";
            LText += "Состояние: " + FShip.State + ", Режим: " + FShip.Mode + "\r\n\r\n";

            if (FShip.Weapon(ShipWeaponType.Left).Ship != null)
                LText += "Левое орудие: " + FShip.Weapon(ShipWeaponType.Left).Ship.UID + ", " + FShip.Weapon(ShipWeaponType.Left).Ship.ShipType + "\r\n";
            if (FShip.Weapon(ShipWeaponType.Right).Ship != null)
                LText += "Правое орудие: " + FShip.Weapon(ShipWeaponType.Right).Ship.UID + ", " + FShip.Weapon(ShipWeaponType.Right).Ship.ShipType + "\r\n";
            if (FShip.Weapon(ShipWeaponType.Center).Ship != null)
                LText += "Центральное орудие: " + FShip.Weapon(ShipWeaponType.Center).Ship.UID + ", " + FShip.Weapon(ShipWeaponType.Center).Ship.ShipType + "\r\n";
            if (FShip.Weapon(ShipWeaponType.Rocket).Ship != null)
                LText += "Ракетное орудие: " + FShip.Weapon(ShipWeaponType.Rocket).Ship.UID + ", " + FShip.Weapon(ShipWeaponType.Rocket).Ship.ShipType + "\r\n";

            Description.text = LText;

            UpdateActive();
            UpdatePassive();
        }

        protected override void DoChange(Shared.Interactive AObject)
        {
            if (!FTransform)
                FTransform = transform;
            FShip = (Ship)AObject;
            FTime = Time.time;

            doshow();
        }

        public void UpdateActive()
        {
            if (FShip == null)
                return;
            int LHP = FShip.Tech(ShipTech.Hp).Value;
            HP.text = SSHLocale.CountToLongString(LHP);
            Damage.text = SSHLocale.CountToLongString(LHP - FShip.HP);
            Count.text = SSHLocale.CountToLongString(FShip.Count);
        }

        public void UpdatePassive()
        {
            if (FShip == null)
                return;
            OwnerAliance.text = "Нейтрал";
            Power.text = FShip.Tech(ShipTech.Damage).Value.ToString();
            Armor.text = FShip.Tech(ShipTech.Armor).Value.ToString() + "%";
        }
    }
}