using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using ElectroBuilder.UI;
using ElectroBuilder.SupportedObjects;
using ElectroBuilder.Enums;
using System.Runtime.Serialization;


namespace ElectroBuilder
{
    namespace ObjectActions
    {
        public class MovingObjectMovement : ObjectMovement
        {
            //variables:
            // Scripts:
            private MoveObjectMenuManager moveMenuManager;
            private MovingObject movingObject;

            // booleans:
            bool dragAndDrop = true;

            // vectors
            private Vector3 lastSavedPosition = Vector3.zero;
            private Vector3 mousePos;
            private Vector2 lastSavedStartPointPosition = Vector2.zero;
            private Vector2 lastSavedEndPointPosition = Vector2.zero;

            private GameObject instantiatedItemImage;


            // unity functions:
            void Start()
            {
                moveMenuManager = GameObject.FindWithTag("UIManager").GetComponent<MoveObjectMenuManager>();
                movingObject = gameObject.GetComponent<MovingObject>();
            }


            // inherited functions:
            // opening/closing mode
            public override void StartMoveObjectMode()
            {
                instantiatedItemImage = movingObject.CreateItemImage();
                SavePosition();
                movingObject.UpdateStatus(ObjectStatus.MOVE);
                SetMovePermission(true);
            }

            public override void CloseMoveObjectMode(bool saveChanges)
            {
                DestroyObjectImage();
                if (saveChanges)
                {
                    movingObject.UpdateBothEndPointPositions();
                    movingObject.RedrawLine();
                }
                else
                {
                    RevertPosition();
                }
                SavePosition();
                movingObject.UpdateStatus(ObjectStatus.SELECTED);
                SetMovePermission(false);
                
                GameObject currentParent = transform.parent?.gameObject;
                if (currentParent == null)
                {
                    Transform intendedParent = GameObject.FindWithTag("LevelLayoutObject").transform;
                    transform.SetParent(intendedParent, true);
                }
            }

            // Object Movement
            protected override void OnMouseDown()
            {
                if (GetMovePermission())
                {
                    mousePos = Input.mousePosition - GetMousePos();
                }
            }

            protected override void OnMouseDrag()
            {
                if (GetMovePermission())
                {
                    dragAndDrop = moveMenuManager.IsDragAndDropModeOn();
                    if (dragAndDrop)
                    {
                        Vector3 screenPosition = Input.mousePosition;
                        screenPosition.z = 2;
                        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
                        worldPosition.z = 0;
                        transform.position = worldPosition;
                        
                    }
                    movingObject.UpdateBothEndPointPositions();
                    movingObject.RedrawLine();
                }
            }


            // Object Limit Movement
            public void OpenMoveLimitMode()
            {
                List<GameObject> limits = movingObject.GetLimitGameObjects();

                movingObject.UpdateStatus(ObjectStatus.MOVELIMITS);
                instantiatedItemImage = movingObject.CreateItemImage();
                ReleaseChildren();
                foreach (GameObject limit in limits)
                {
                    limit.GetComponent<ObjectLimitMovement>().StartMoveObjectMode();
                }
            }

            public void CloseMoveLimitMode(bool save)
            {
                List<GameObject> limits = movingObject.GetLimitGameObjects();

                movingObject.UpdateStatus(ObjectStatus.SELECTED);
                DestroyObjectImage();
                foreach (GameObject limit in limits)
                {
                    limit.GetComponent<ObjectLimitMovement>().CloseMoveObjectMode(save);
                }

                if (save)
                {
                    movingObject.RedrawLine();
                }
                movingObject.MoveToCenterOfEndPoints();

                ReadoptChildren();
            }

            private void ReleaseChildren()
            {
                GameObject levelLayoutObject = GameObject.FindWithTag("LevelLayoutObject");
                List<GameObject> limits = movingObject.GetLimitGameObjects();
                foreach (GameObject limit in limits)
                {
                    limit.transform.SetParent(levelLayoutObject.transform, true);
                }
                movingObject.UpdateBothEndPointPositions();
            }

            private void ReadoptChildren()
            {
                List<GameObject> limits = movingObject.GetLimitGameObjects();
                foreach (GameObject limit in limits)
                {
                    limit.transform.SetParent(transform, true);
                }
            }

            // helper functions:

            private void DestroyObjectImage()
            {
                //Assertions:
                Debug.Assert(instantiatedItemImage != null);

                Destroy(instantiatedItemImage.gameObject);
                instantiatedItemImage = null;
            }


            // position related:
            public void SavePosition()
            {
                if (transform.position != lastSavedPosition)
                {
                    lastSavedPosition = transform.position;
                }

                if (movingObject.GetStartPointPos() != lastSavedStartPointPosition)
                {
                    lastSavedStartPointPosition = movingObject.GetStartPointPos();
                }

                if (movingObject.GetEndPointPos() != lastSavedEndPointPosition)
                {
                    lastSavedEndPointPosition = movingObject.GetEndPointPos();
                }
            }

            private void RevertPosition()
            {
                transform.position = lastSavedPosition;
            }

            private Vector3 GetMousePos()
            {
                return Camera.main.WorldToScreenPoint(transform.position);
            }
        }
    }
}
