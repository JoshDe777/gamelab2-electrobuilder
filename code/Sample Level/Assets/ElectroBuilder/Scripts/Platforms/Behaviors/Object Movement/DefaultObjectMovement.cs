using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using ElectroBuilder.SupportedObjects;
using ElectroBuilder.UI;
using ElectroBuilder.Enums;

namespace ElectroBuilder
{
    namespace ObjectActions
    {
        public class DefaultObjectMovement : ObjectMovement
        {
            //variables:
            // Scripts:
            private MoveObjectMenuManager moveMenuManager;
            SupportedObject objectManager;

            // booleans:
            bool dragAndDrop = true;

            // vectors
            private Vector3 lastSavedPosition = Vector3.zero;
            private Vector3 mousePos;

            private GameObject instantiatedItemImage;


            // unity functions:
            void Start()
            {
                moveMenuManager = GameObject.FindWithTag("UIManager").GetComponent<MoveObjectMenuManager>();
                objectManager = gameObject.GetComponent<SupportedObject>();
            }


            // inherited functions:
            // opening/closing mode
            public override void StartMoveObjectMode()
            {
                instantiatedItemImage = objectManager.CreateItemImage();
                SavePosition();
                objectManager.UpdateStatus(ObjectStatus.MOVE);
                SetMovePermission(true);
            }

            public override void CloseMoveObjectMode(bool saveChanges)
            {
                DestroyObjectImage();
                if (!saveChanges)
                {
                    RevertPosition();
                }
                SavePosition();
                objectManager.UpdateStatus(ObjectStatus.SELECTED);
                GameObject levelLayoutObject = GameObject.FindWithTag("LevelLayoutObject");
                transform.SetParent(levelLayoutObject.transform, true);
                SetMovePermission(false);
            }

            // Object Movement
            protected override void OnMouseDown()
            {
                if (GetMovePermission())
                {
                    mousePos = Input.mousePosition - GetObjectScreenPosition();
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
                }
            }

            // helper functions:
            // object images:

            private void DestroyObjectImage()
            {
                //Assertions:
                Debug.Assert(instantiatedItemImage != null);

                // Destroy(instantiatedItemImage.gameObject);
                objectManager.RemoveItemImage();
                instantiatedItemImage = null;
            }


            // position related:
            public void SavePosition()
            {
                if (transform.position != lastSavedPosition)
                {
                    lastSavedPosition = transform.position;
                }
            }

            private void RevertPosition()
            {
                transform.position = lastSavedPosition;
            }

            private Vector3 GetObjectScreenPosition()
            {
                return Camera.main.WorldToScreenPoint(transform.position);
            }
        }
    }
}
