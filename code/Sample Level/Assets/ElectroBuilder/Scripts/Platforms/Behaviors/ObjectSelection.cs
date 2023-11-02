using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ElectroBuilder.SupportedObjects;
using ElectroBuilder.Enums;
using ElectroBuilder.UI;


namespace ElectroBuilder
{
    namespace ObjectActions
    {
        public class ObjectSelection : MonoBehaviour
        {
            private Renderer rndr;
            private Material objectMaterial;
            private ObjectMovement objectMovement;

            // managers
            private SupportedObject objManager;
            private GameObject uiManager;

            public bool selected = false;


            // Start is called before the first frame update
            void Start()
            {
                rndr = GetComponent<Renderer>();
                objectMaterial = rndr.material;
                objectMovement = GetComponent<ObjectMovement>();
                objManager = GetComponent<SupportedObject>();
                uiManager = GameObject.FindGameObjectWithTag("UIManager");
            }

            void OnMouseEnter()
            {
                HighlightObject();
            }

            void OnMouseExit()
            {
                if (!selected)
                {
                    UnHighlightObject();
                }
            }

            public void Select()
            {
                HighlightObject();
                selected = true;
                try
                {
                    if (objManager.GetStatus() == ObjectStatus.IDLE)
                    {
                        objManager.UpdateStatus(ObjectStatus.SELECTED);
                    }
                }
                catch (NullReferenceException e)
                {
                    objManager = GetComponent<SupportedObject>();
                    objManager.UpdateStatus(ObjectStatus.SELECTED);
                }
            }

            public void Deselect()
            {
                UnHighlightObject();
                // abort movement mode
                MoveObjectMenuManager uiMoveModeManager = uiManager.GetComponent<MoveObjectMenuManager>();
                if (objManager.GetStatus() == ObjectStatus.MOVE)
                {
                    uiMoveModeManager.CloseMoveObjectMode(true);
                }
                else if (objManager.GetStatus() == ObjectStatus.MOVELIMITS)
                {
                    uiMoveModeManager.CloseMoveLimitsMode(true);
                }
                selected = false;
                objManager.UpdateStatus(ObjectStatus.IDLE);
                // check if children -> close move limits mode
            }

            void HighlightObject()
            {
                if (objectMaterial != null)
                {
                    objectMaterial.EnableKeyword("_EMISSION");
                    objectMaterial.SetColor("_EmissionColor", new Color(64f / 255f, 64f / 255f, 64f / 255f));
                }
                if (transform.childCount > 0)
                {
                    foreach (Transform child in transform)
                    {
                        ObjectSelection os = child.gameObject.GetComponent<ObjectSelection>();
                        os.HighlightObject();
                    }
                }
            }

            void UnHighlightObject()
            {
                objectMaterial.DisableKeyword("_EMISSION");
                // unhighlight all children --> only for moving objects a.o. now
                if (transform.childCount > 0)
                {
                    foreach (Transform child in transform)
                    {
                        ObjectSelection os = child.gameObject.GetComponent<ObjectSelection>();
                        os.Deselect();
                    }
                }
            }
        }
    }
}
