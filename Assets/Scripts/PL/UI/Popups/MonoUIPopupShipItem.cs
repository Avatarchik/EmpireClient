using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Planetar
{
    public class MonoUIPopupShipItem : MonoBehaviour
    {
        public Text Caption;

        private UnityAction FCallback;

        private void OnMouseDown()
        {
            FCallback();
        }

        public void Init(string ACaption, UnityAction ACallback)
        {
            FCallback = ACallback;
            Caption.text = ACaption;
        }

        public void Change(string ACaption)
        {
            Caption.text = ACaption;
        }
    }
}