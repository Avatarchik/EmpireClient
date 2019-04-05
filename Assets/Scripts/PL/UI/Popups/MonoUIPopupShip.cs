using System.Collections.Generic;
using UnityEngine;

namespace Planetar
{
    public class MonoUIPopupShip : Shared.MonoUIPopupCustom
    {
        // Объект, для которого вызвано меню
        private Ship FShip;
        private Transform FTransform;

        private Dictionary<ShipAction, MonoUIPopupShipItem> FItems;

        private void ItemAdd(ShipAction AShipAction)
        {
            MonoUIPopupShipItem tmpItem = new MonoUIPopupShipItem();
            tmpItem = PrefabManager.CreateShipPopupItem(FTransform);
            tmpItem.Init(AShipAction.ToString(), () => { FShip.ActionCall(AShipAction); });
            FItems.Add(AShipAction, tmpItem);
        }

        private void Start()
        {
            FTransform = transform;
            FItems = new Dictionary<ShipAction, MonoUIPopupShipItem>();
            ItemAdd(ShipAction.ChangeMode);
            ItemAdd(ShipAction.PortalJump);
            ItemAdd(ShipAction.PortalOpen);
            ItemAdd(ShipAction.MoveToHangar);
            ItemAdd(ShipAction.Annihilation);
            ItemAdd(ShipAction.Constructor);
            ItemAdd(ShipAction.Delete);
        }

        // Обновление параметров меню
        protected override void DoChange(Shared.Interactive AObject)
        {
            FShip = (Ship)AObject;
        }
    }
}