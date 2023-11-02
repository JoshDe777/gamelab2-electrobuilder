using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ElectroBuilder.Enums;
using ElectroBuilder.SupportedObjects;


namespace ElectroBuilder
{
    namespace ObjectActions
    {
        public class ResizingManager : MonoBehaviour
        {
            SelectionManager selectionManager;

            [SerializeField]
            private GameObject resizeMenu;
            [SerializeField]
            private GameObject defaultMenu;
            [SerializeField]
            private GameObject movingObjectMenu;
            [SerializeField]
            private GameObject scaleArrows;

            private GameObject selectedObject;

            private bool uniformResize = false;


            // button functions:
            public void OpenResizeMode()
            {
                selectedObject = selectionManager.GetSelectedObject();
                selectionManager.SetUIState(UIState.RESIZE);
                selectedObject.GetComponent<ObjectResizing>().StartResizeMode();
                OpenResizeModeUI();
            }

            public void CloseResizeMode(bool saveProgress = true)
            {
                selectionManager.SetUIState(UIState.MAIN);
                selectedObject.GetComponent<ObjectResizing>().CloseResizeMode(saveProgress);
                CloseResizeModeUI();
            }

            public void ToggleUniformResize(bool uniformMode) => uniformResize = uniformMode;


            // getters & setters
            public bool IsResizeModeUniform() => uniformResize;

            public GameObject GetArrowsPrefab() => scaleArrows;


            // support functions
            private void OpenResizeModeUI()
            {
                defaultMenu.SetActive(false);
                movingObjectMenu.SetActive(false);
                resizeMenu.SetActive(true);
            }

            private void CloseResizeModeUI()
            {
                selectedObject = selectionManager.GetSelectedObject();
                SupportedObject obj = selectedObject.GetComponent<SupportedObject>();

                if (obj.GetObjectType() == ObjectType.MOVING)
                {
                    movingObjectMenu.SetActive(true);
                }
                else
                {
                    defaultMenu.SetActive(true);
                }
                resizeMenu.SetActive(false);
            }

            // Unity functions
            private void Start()
            {
                selectionManager = FindObjectOfType<SelectionManager>();
            }
        }
    }
}
