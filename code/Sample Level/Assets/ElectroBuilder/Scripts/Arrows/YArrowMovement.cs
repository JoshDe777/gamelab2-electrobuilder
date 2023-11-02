using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ElectroBuilder.Enums;
using ElectroBuilder.SupportedObjects;

namespace ElectroBuilder
{
    namespace ObjectActions
    {
        public class YArrowMovement : MonoBehaviour
        {
            private ObjectLimitMovement potentialObjectLimit;
            private SelectionManager selectionManager;

            private GameObject objectToMove;
            private float clickOffset;

            private void Start()
            {
                selectionManager = FindObjectOfType<SelectionManager>();
            }


            // setup functions:
            public void Setup(GameObject obj)
            {
                objectToMove = obj;
            }

            void OnMouseDown()
            {
                Vector3 screenPosition = Input.mousePosition;
                screenPosition.z = 2;
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
                worldPosition.z = 0;
                clickOffset = worldPosition.y - objectToMove.transform.position.y;
            }

            private void OnMouseDrag()
            {
                GameObject parent = transform.parent.parent.gameObject;
                Vector3 screenPosition = Input.mousePosition;
                screenPosition.z = 2;
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
                worldPosition.z = -1;
                worldPosition.y -= clickOffset;
                worldPosition.x = parent.transform.position.x;
                parent.transform.position = worldPosition;

                Transform movingArrowsParent = transform.parent.parent;

                MovingObject movingObject = null;
                foreach (Transform child in movingArrowsParent)
                {
                    if (child.gameObject.GetComponent<MovingObject>() != null)
                    {
                        movingObject = child.gameObject.GetComponent<MovingObject>();
                    }
                }
                if (movingObject != null)
                {
                    movingObject.UpdateBothEndPointPositions();
                    movingObject.RedrawLine();
                }

                if (selectionManager.GetUIState() == UIState.MOVELIMITS)
                {
                    if (potentialObjectLimit == null)
                    {
                        foreach (Transform child in parent.transform)
                        {
                            ObjectLimitMovement objectLimitMovement = child.GetComponent<ObjectLimitMovement>();
                            if (objectLimitMovement != null)
                            {
                                potentialObjectLimit = objectLimitMovement;
                                break;
                            }
                        }
                    }

                    potentialObjectLimit.UpdateMovingObjectOnPassiveMovement();
                    MovingObjectLimit limit = potentialObjectLimit.gameObject.GetComponent<MovingObjectLimit>();
                    movingObject = limit.GetParentMovingObject();
                    movingObject.UpdateBothEndPointPositions();
                    movingObject.RedrawLine();
                }
            }
        }
    }
}
