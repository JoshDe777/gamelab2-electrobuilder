using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditorInternal;
using UnityEngine;
using ElectroBuilder.Enums;
using ElectroBuilder.Testmode;
using ElectroBuilder.ObjectActions;
using ElectroBuilder.UI;
using ElectroBuilder.SaveLoadSystem;

namespace ElectroBuilder
{
    namespace SupportedObjects
    {
        public class SupportedObject : MonoBehaviour
        {
            #region variables
            [SerializeField]
            private ObjectStatus status;
            private GameObject itemImage;
            private Material itemImageMaterial;
            private bool beingPlaced = false;

            private ObjectInstantiationButton instantiationButton;

            protected bool testMode = false;
            protected TestModeEventHandler testModeEventHandler;
            protected SaveLoadEventHandler saveLoadEventHandler;
            #endregion

            #region status related
            public ObjectStatus GetStatus() => status;

            public void UpdateStatus(ObjectStatus newMode) => status = newMode;
            #endregion

            #region unity functions
            protected virtual void Start()
            {
                //Event when Testmode Switch Button is pressed
                testModeEventHandler = FindObjectOfType<TestModeEventHandler>();
                testModeEventHandler.OnTestmodeSwitch += ChangeTestModeBool;

                saveLoadEventHandler = FindObjectOfType<SaveLoadEventHandler>();
                // saveLoadEventHandler.OnLoadLevel += RemoveTestModeEvents;
                // saveLoadEventHandler.OnNewLevel += RemoveTestModeEvents;

                itemImageMaterial = (Material)Resources.Load("Materials/ObjectImageMaterial", typeof(Material));
            }

            protected virtual void Update()
            {
                if (testMode)
                {
                    RuntimeBehaviour();
                }
                else
                {
                    EditorBehaviour();
                }
            }
            #endregion

            #region virtual methods
            public virtual ObjectType GetObjectType()
            {
                return ObjectType.CUSTOM;
            }

            protected virtual void RuntimeBehaviour()
            {

            }

            protected virtual void EditorBehaviour()
            {
                if (beingPlaced)
                {
                    Vector3 screenPosition = Input.mousePosition;
                    screenPosition.z = 2;
                    Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
                    worldPosition.z = 0;
                    transform.position = worldPosition;
                }
            }

            protected virtual void AddRequiredScripts()
            {
                gameObject.AddComponent<ObjectSelection>();
                gameObject.AddComponent<DefaultObjectMovement>();
                gameObject.AddComponent<ObjectResizing>();
            }
            #endregion

            #region events
            protected virtual void ChangeTestModeBool()
            {
                testMode = testModeEventHandler.TestmodeToggle;
            }
            #endregion

            #region firstPlacement
            public void BeginFirstPlacement(ObjectInstantiationButton oib)
            {
                Renderer rndr = GetComponent<Renderer>();
                Material objectMaterial = rndr.material;
                if (objectMaterial != null)
                {
                    objectMaterial.EnableKeyword("_EMISSION");
                    objectMaterial.SetColor("_EmissionColor", new Color(164f / 255f, 64f / 255f, 64f / 255f));
                }

                SetMovementPermissions(true);

                instantiationButton = oib;
            }

            public virtual void ConfirmFirstPlacement()
            {
                Renderer rndr = GetComponent<Renderer>();
                Material objectMaterial = rndr.material;
                if (objectMaterial != null)
                {
                    objectMaterial.DisableKeyword("_EMISSION");
                }

                SetMovementPermissions(false);
            }

            public void SetMovementPermissions(bool b)
            {
                beingPlaced = b;
            }
            #endregion

            #region other functions
            public virtual void Delete()
            {
                instantiationButton?.IncrementQuantity();
                testModeEventHandler.OnTestmodeSwitch -= ChangeTestModeBool;
                Destroy(gameObject);
            }

            public virtual GameObject CreateItemImage()
            {
                itemImage = Instantiate(gameObject, null);
                itemImage.transform.position = transform.position +  new Vector3(0, 0, 1);
                itemImage.name = "Object Image";
                itemImage.tag = "ItemImage";
                Destroy(itemImage.GetComponent<SupportedObject>());
                Destroy(itemImage.GetComponent<ObjectSelection>());
                Destroy(itemImage.GetComponent<ObjectMovement>());
                Destroy(itemImage.GetComponent<ObjectResizing>());

                Renderer rndr = itemImage.GetComponent<Renderer>();
                rndr.material = itemImageMaterial;
                if (itemImageMaterial != null)
                {
                    itemImageMaterial.EnableKeyword("_EMISSION");
                    itemImageMaterial.SetColor("_EmissionColor", new Color(165f / 255f, 64f / 255f, 64f / 255f));
                }

                return itemImage;
            }

            public void RemoveItemImage()
            {
                if (itemImage != null)
                {
                    Destroy(itemImage);
                    itemImage = null;
                }
            }

            /*protected void RemoveTestModeEvents()
            {
                testModeEventHandler.OnTestmodeSwitch -= ChangeTestModeBool;
            }*/
            #endregion
        }
    }

    namespace Enums
    {
        public enum ObjectType
        {
            STATIC,
            MOVING,
            START,
            END,
            STARTLIMIT,
            ENDLIMIT,
            LINE,
            CUSTOM
        }

        public enum ObjectStatus
        {
            IDLE,
            SELECTED,
            MOVE,
            MOVELIMITS,
            RESIZE,
            DELETE,
            RUNNING,
            SAVING
        }

        public enum UIState
        {
            IDLE,
            MAIN,
            OBJECTCREATION,
            MOVEOBJECT,
            MOVELIMITS,
            RESIZE,
            TESTING,
            SAVING
        }
    }
}
