using UnityEngine;
using UnityEngine.UI;

namespace Galaxy
{
    public class MGAObjectCluster : MonoBehaviour
    {
        public Image Selection;

        void OnMouseEnter()
        {
            Selection.enabled = true;
        }

        void OnMouseExit()
        {
            Selection.enabled = false;
        }

        void OnMouseDown()
        {
            SGAShared.SocketWriter.PlanetarAvailable(4);
        }
    }
}