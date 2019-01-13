using UnityEngine;

namespace Register
{
    public class MonoModels : MonoBehaviour
    {
        private Transform FTransform;

        void Start()
        {
            FTransform = transform;
        }

        void OnMouseEnter()
        {
            FTransform.GetComponent<Renderer>().materials[0].color = Color.white;
        }

        void OnMouseExit()
        {
            if (Shared.SelectedRace != transform)
                FTransform.GetComponent<Renderer>().materials[0].color = Color.black;
        }

        void OnMouseDown()
        {
            if (Shared.SelectedRace == transform)
                return;
            if (Shared.SelectedRace != null)
                Shared.SelectedRace.GetComponent<Renderer>().materials[0].color = Color.black;
            Shared.SelectedRace = transform;
        }
    }
}