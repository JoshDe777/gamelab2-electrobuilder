using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ElectroBuilder.Enums;
using ElectroBuilder.SupportedObjects;

namespace ElectroBuilder
{
    namespace ObjectActions
    {
        public class MovingObjectResizing : ObjectResizing
        {
            MovingObject movingObject;

            // unity functions:
            private void Start()
            {
                movingObject = GetComponent<MovingObject>();
                obj = GetComponent<SupportedObject>();
            }

            public override void StartResizeMode()
            {
                DecoupleChildObjects();
                base.StartResizeMode();
            }

            public override void CloseResizeMode(bool saveChanges)
            {
                base.CloseResizeMode(saveChanges);
                RecoupleChildObjects();
                movingObject.RedrawLine();
            }

            private void DecoupleChildObjects()
            {
                List<GameObject> limits = movingObject.GetLimitGameObjects();
                foreach (GameObject limit in limits)
                {
                    limit.transform.SetParent(null, true);
                }
            }

            private void RecoupleChildObjects()
            {
                List<GameObject> limits = movingObject.GetLimitGameObjects();
                foreach (GameObject limit in limits)
                {
                    if (limit.transform.parent == null)
                    {
                        limit.transform.SetParent(transform, true);
                    }
                }
            }
        }
    }
}
