using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Planetar
{
    public class MonoUIPopupShipItem : MonoBehaviour
    {
        public Text Caption;

        private UnityAction FCallback;
        private string FCaption;

        public void Init(string ACaption, UnityAction ACallback)
        {
            FCallback = ACallback;
            FCaption = ACaption;
        }

        public void Change(string ACaption)
        {
            FCaption = ACaption;
            Caption.text = ACaption;
        }
    }
}