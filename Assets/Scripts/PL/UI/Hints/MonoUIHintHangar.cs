/*///////////////////////////////////////////////
{                                              }
{ Shared                                       }
{ Панель подсказки для планетарного ангара     }
{ Copyright (c) 2016 UAshota                   }
{                                              }
/*///////////////////////////////////////////////
using UnityEngine.UI;

namespace Planetar
{
    // Панель подсказки для планетарного ангара
    public class MonoUIHintHangar : Shared.MonoUIHintCustom
    {
        public Text OwnerName;
        public Text OwnerAliance;
        public Text Type;
        public Text Description;

        protected override void DoChange(Shared.Interactive AObject)
        {
            Hangar LHangar = (Hangar)AObject;
            OwnerName.text = "Hangar";
            Type.text = LHangar.ShipType.ToString();
            Description.text = "Описание " + LHangar.ToString();
        }
    }
}