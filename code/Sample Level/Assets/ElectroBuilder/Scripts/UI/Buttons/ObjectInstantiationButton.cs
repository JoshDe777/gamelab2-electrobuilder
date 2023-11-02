using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ElectroBuilder.ObjectActions;
using ElectroBuilder.SupportedObjects;

namespace ElectroBuilder
{
    namespace UI
    {
        public class ObjectInstantiationButton : MonoBehaviour
        {
            // variables:
            public int ID;
            [SerializeField] private int quantity;
            private int defaultQuantity;
            public bool clicked = false;

            GameObject obj;

            private ObjectInstantiationManager objectInstantiationManager;
            private SelectionManager selectionManager;


            // unity functions:
            private void Start()
            {
                objectInstantiationManager = FindObjectOfType<ObjectInstantiationManager>();
                selectionManager = objectInstantiationManager.gameObject.GetComponent<SelectionManager>();
                defaultQuantity = quantity;
            }


            // public functions:
            public void IncrementQuantity()
            {
                quantity++;
            }

            public void DecrementQuantity()
            {
                quantity--;
            }

            public int GetQuantity() => quantity;

            public void ResetQuantity() => quantity = defaultQuantity;


            // button function:
            public void ButtonClicked()
            {
                clicked = true;
                GameObject selectedObject = selectionManager.GetSelectedObject();
                if (selectedObject != null)
                {
                    selectionManager.DeselectCurrentObject();
                }
                if (quantity != 0)
                {
                    Vector3 screenPosition = Input.mousePosition;
                    screenPosition.z = 2;
                    Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
                    obj = Instantiate(objectInstantiationManager.itemPrefabs[ID], new Vector3(worldPosition.x, worldPosition.y, 0), Quaternion.identity);
                    SupportedObject itemImage = obj.GetComponent<SupportedObject>();
                    itemImage.BeginFirstPlacement(this);
                    objectInstantiationManager.SetCurrentButtonPressed(ID);
                }
                else
                {
                    ShowErrorMessage();
                }
            }

            private void ShowErrorMessage()
            {
                string message = "Maximum amount of this item in scene!";
                objectInstantiationManager.SendErrorMessage(message);
            }

            public void SetClicked(bool b) => clicked = b;

            public GameObject GetLastSpawnedObject() => obj;
        }
    }
}
