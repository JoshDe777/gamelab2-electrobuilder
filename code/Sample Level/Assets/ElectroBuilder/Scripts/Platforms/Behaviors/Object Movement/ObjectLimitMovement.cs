using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEditor;
using UnityEngine;
using ElectroBuilder.SupportedObjects;
using ElectroBuilder.UI;
using ElectroBuilder.Enums;

namespace ElectroBuilder
{
    namespace ObjectActions
    {
        public class ObjectLimitMovement : ObjectMovement
        {
            // Scripts:
            private MoveObjectMenuManager moveMenuManager;
            MovingObjectLimit objectLimit;
            MovingObject movingObject;
            GameObject instantiatedItemImage;
            MovingObjectMovement movingObjectMovement;
            // Vectors:
            private Vector3 lastSavedPosition = Vector3.zero;
            private Vector3 mousePos;
            // Bools:
            private bool dragAndDrop = false;
            private bool isMoving = false;


            // unity functions:
            void Start()
            {
                moveMenuManager = GameObject.FindWithTag("UIManager").GetComponent<MoveObjectMenuManager>();
                objectLimit = gameObject.GetComponent<MovingObjectLimit>();
                movingObject = objectLimit.GetParentMovingObject();
                movingObjectMovement = movingObject.gameObject.GetComponent<MovingObjectMovement>();
            }


            // inherited functions:
            // opening/closing mode
            public override void StartMoveObjectMode()
            {
                //CreateObjectImage();
                SavePosition();
                objectLimit.UpdateStatus(ObjectStatus.MOVELIMITS);
                SetMovePermission(true);
            }

            public override void CloseMoveObjectMode(bool saveChanges)
            {
                //DestroyObjectImage();
                if (!saveChanges)
                {
                    RevertPosition();
                    movingObject.RedrawLine();
                }
                SavePosition();
                objectLimit.UpdateStatus(ObjectStatus.SELECTED);
                UpdateLimitPositionForMovingObject();
                SetMovePermission(false);
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
                    UpdateLimitPositionForMovingObject();
                    movingObject.MoveToCenterOfEndPoints();
                    movingObject.RedrawLine();
                }
            }

            public void UpdateMovingObjectOnPassiveMovement()
            {
                movingObject.UpdateBothEndPointPositions();
                movingObject.MoveToCenterOfEndPoints();
            }


            //helper functions:
            private Vector3 GetMousePos()
            {
                return Camera.main.WorldToScreenPoint(transform.position);
            }

            // positioning related
            private void SavePosition()
            {
                lastSavedPosition = transform.position;
            }

            private void RevertPosition()
            {
                Vector2 oldPosition = new Vector2(lastSavedPosition.x, lastSavedPosition.y);
                movingObject.UpdateEndPointCoordinatePosition(objectLimit.GetObjectType(), oldPosition);
                transform.position = lastSavedPosition;
            }

            private void UpdateLimitPositionForMovingObject()
            {
                movingObject.UpdateBothEndPointPositions();
            }
        }
    }
}
