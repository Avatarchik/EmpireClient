/*///////////////////////////////////////////////
{                                              }
{   Панель подсказки для общего ресурса        }
{       Copyright (c) 2016 UAshota             }
{                                              }
{                                              }
/*///////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;

namespace Shared
{
    public class MSHUIHintResource : MonoBehaviour
    {
        public Text OwnerName;
        public Text OwnerAliance;
        public Text Type;
        public Text Description;

        private Transform FTransform;
        private Resource FResource;

        public void Show(Resource AResource)
        {
            if (!FTransform)
                FTransform = transform;
            FResource = AResource;

            OwnerName.text = "locals";
            Type.text = AResource.ResType.ToString();
            Description.text = "Описание: пока нету";

            UpdateActive();
            UpdatePassive();

            FTransform.position = AResource.Transform.position;
            FTransform.gameObject.SetActive(true);
            FTransform.localPosition = new Vector3(FTransform.localPosition.x + 15, FTransform.localPosition.y - 15, -50);
        }

        public void UpdateActive()
        {
            if (FResource == null)
                return;

        }

        public void UpdatePassive()
        {
            if (FResource == null)
                return;
        }

        public void Hide()
        {
            if (FResource != null)
            {
                FTransform.gameObject.SetActive(false);
                FResource = null;
            }
        }
    }
}