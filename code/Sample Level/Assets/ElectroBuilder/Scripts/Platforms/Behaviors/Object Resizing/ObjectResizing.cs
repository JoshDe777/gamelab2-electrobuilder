using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ElectroBuilder.SupportedObjects;
using ElectroBuilder.Enums;


namespace ElectroBuilder
{
    namespace ObjectActions
    {
        public class ObjectResizing : MonoBehaviour
        {
            protected SupportedObject obj;

            protected Vector3 lastSavedScale = Vector3.zero;

            private GameObject resizeArrowsPrefab;

            protected GameObject resizeArrows;

            private GameObject levelLayoutObject;


            // unity functions:
            private void Start()
            {
                obj = GetComponent<SupportedObject>();
            }


            // public functions:
            public virtual void StartResizeMode()
            {
                obj.UpdateStatus(ObjectStatus.RESIZE);
                SaveScale();
                SetupScaleArrows();
            }

            public virtual void CloseResizeMode(bool saveChanges)
            {
                obj.UpdateStatus(ObjectStatus.SELECTED);
                if (!saveChanges)
                {
                    ResetScale();
                }
                RemoveScaleArrows();
            }


            // helper functions:
            private void SaveScale()
            {
                if (lastSavedScale != transform.localScale)
                {
                    lastSavedScale = transform.localScale;
                }
            }

            private void ResetScale()
            {
                if (lastSavedScale != transform.localScale)
                {
                    transform.localScale = lastSavedScale;
                }
            }

            private void SetupScaleArrows()
            {
                ResizingManager rm = FindObjectOfType<ResizingManager>();
                resizeArrowsPrefab = rm.GetArrowsPrefab();
                resizeArrows = Instantiate(resizeArrowsPrefab);
                resizeArrows.transform.position = transform.position + new Vector3(0,0,-1);
                foreach (Transform child in resizeArrows.transform)
                {
                    if (child.name.Equals("xScaleArrows"))
                    {
                        foreach (Transform grandchild in child)
                        {
                            if (!grandchild.name.Equals("xCylinder"))
                            {
                                grandchild.gameObject.GetComponent<XCubeScaling>().Setup(gameObject);
                            }
                            else
                            {
                                grandchild.gameObject.GetComponent<XCylinderScaling>().Setup(gameObject);
                            }
                        }
                    }
                    else if (child.name.Equals("yScaleArrows"))
                    {
                        foreach (Transform grandchild in child)
                        {
                            if (!grandchild.name.Equals("yCylinder"))
                            {
                                grandchild.gameObject.GetComponent<YCubeScaling>().Setup(gameObject);
                            }
                            else
                            {
                                grandchild.gameObject.GetComponent<YCylinderScaling>().Setup(gameObject); 
                            }
                        }
                    }
                }
            }

            private void RemoveScaleArrows()
            {
                levelLayoutObject = GameObject.FindGameObjectWithTag("LevelLayoutObject");
                gameObject.transform.SetParent(levelLayoutObject.transform);
                Destroy(resizeArrows);
            }
        }
    }
}
