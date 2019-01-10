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

        private void ItemCreate(ShipAction AShipAction)
        {
            /*FItems[(int)AShipAction] = PrefabManager.CreateShipPopupItem(FTransform);
            FItems[(int)AShipAction].Init(AShipAction.ToString(), () => { FShip.ActionCall(AShipAction); });*/
        }

        private void Start()
        {
            FTransform = transform;
            /*FItems = new MonoUIPopupShipItem[10];*/

            ItemCreate(ShipAction.ChangeMode);
            ItemCreate(ShipAction.PortalJump);
            ItemCreate(ShipAction.PortalOpen);
            ItemCreate(ShipAction.MoveToHangar);
            ItemCreate(ShipAction.Annihilation);
            ItemCreate(ShipAction.Constructor);
            ItemCreate(ShipAction.Delete);
        }

        // Обновление параметров меню
        protected override void DoChange(Shared.Interactive AObject)
        {
            FShip = (Ship)AObject;
        }
    }
}