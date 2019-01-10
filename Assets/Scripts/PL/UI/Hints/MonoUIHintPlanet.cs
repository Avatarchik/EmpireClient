﻿/*///////////////////////////////////////////////
{                                              }
{   Панель подсказки для планетарной планеты   }
{       Copyright (c) 2016 UAshota             }
{                                              }
{                                              }
/*///////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;

namespace Planetar
{
    public class MonoUIHintPlanet : MonoBehaviour
    {
        public Text OwnerName;
        public Text OwnerAliance;
        public Text Type;
        public Text Description;

        private Transform FTransform;
        private Planet FPlanet;

        public void Show(Planet APlanet)
        {
            if (!FTransform)
                FTransform = transform;
            FPlanet = APlanet;

            if (APlanet.IsVisible())
            {
                if (APlanet.Owner.Role != SSHRole.Neutral)
                    OwnerName.text = APlanet.Name;
                else
                    OwnerName.text = "Неизвестно";

                string LDescription = "Номер в реестре: " + APlanet.UID.ToString() + "\r\n";

                LDescription += "В бою: " + APlanet.InBattle.ToString() + "\r\n";
                LDescription += "\r\n";
                if (APlanet.VisibleHard)
                    LDescription += "Видимость: полная \r\n";
                if (APlanet.VisibleSoft)
                    LDescription += "Видимость: окраина\r\n";
                if (APlanet.IsBigHole)
                    LDescription += "Видимость: черная дыра\r\n";
                if (APlanet.IsBigEdge)
                    LDescription += "Видимость: окраина черной дыры\r\n";
                LDescription += "\r\n";
                LDescription += "Роль: " + APlanet.Owner.Role.ToString() + "\r\n";
                LDescription += "Подписка: " + APlanet.Subscription.ToString() + "\r\n";
                LDescription += "\r\n";
                LDescription += "Контроль свой: - " + APlanet.ControlSelf.ToString() + "\r\n";
                LDescription += "Контроль союз: - " + APlanet.ControlFriend.ToString() + "\r\n";
                LDescription += "Контроль враг: - " + APlanet.ControlEnemy.ToString() + "\r\n";
                LDescription += "\r\n";
                LDescription += "Верфи: " + APlanet.ShipyardsCount.ToString() + "\r\n";
                LDescription += "Модули: " + APlanet.ModulesCount.ToString() + "\r\n";

                Description.text = LDescription;
            }
            else
                OwnerName.text = "Покрыто мраком";

            Type.text = APlanet.PlanetType.ToString();

            UpdateActive();
            UpdatePassive();

            FTransform.gameObject.SetActive(true);
            FTransform.position = APlanet.Transform.position;
            FTransform.localPosition = new Vector3(FTransform.localPosition.x + 30, FTransform.localPosition.y - 30, -50);

        }

        public void UpdateActive()
        {
            if (FPlanet == null)
                return;

        }

        public void UpdatePassive()
        {
            if (FPlanet == null)
                return;
            if (FPlanet.IsVisible())
            {
                if (FPlanet.Owner.Role == SSHRole.Enemy)
                    OwnerAliance.text = "Вражеская";
                else if (FPlanet.Owner.Role == SSHRole.Self)
                    OwnerAliance.text = "Своя";
                else if (FPlanet.Owner.Role == SSHRole.Friend)
                    OwnerAliance.text = "Дружеская";
                else
                    OwnerAliance.text = "Ничейная";
            }
            else
                OwnerAliance.text = "Скрыто в тумане";

        }

        public void Hide()
        {
            if (FPlanet != null)
            {
                FTransform.gameObject.SetActive(false);
                FPlanet = null;
            }
        }
    }
}