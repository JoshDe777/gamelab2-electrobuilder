using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;
using ElectroBuilder.SupportedObjects;
using ElectroBuilder.SaveLoadSystem;

namespace ElectroBuilder
{
    namespace UI
    {
        public class ObjectInstantiationManager : MonoBehaviour
        {
            // General variables
            public Camera cam;
            public GameObject errorMessagePanel;
            public TMP_Text errorText;
            private float errorMessageDuration = 10;

            // Object Instantiation
            public ObjectInstantiationButton[] ItemButtons;
            public GameObject[] itemPrefabs;
            public int currentButtonPressed;

            // test mode
            public bool testMode = false;
            public SaveLoadEventHandler saveLoadEvents;

            // Unity Functions:
            void Update()
            {
                // Object Instantiation related
                FinalObjectInstantiation();
            }

            private void Start()
            {
                foreach (Transform child in errorMessagePanel.transform)
                {
                    if (child.name == "ErrorText")
                    {
                        errorText = child.GetComponent<TMP_Text>();
                    }
                }

                saveLoadEvents = FindObjectOfType<SaveLoadEventHandler>();
                Action resetButtonCounters = () => ResetInstatiationButtonCounters();
                saveLoadEvents.OnNewLevel += resetButtonCounters;
            }

            //

            public void SetCurrentButtonPressed(int ID) => currentButtonPressed = ID;

            void FinalObjectInstantiation()
            {
                Vector3 screenPosition = Input.mousePosition;
                screenPosition.z = 2;
                Vector3 worldPosition = cam.ScreenToWorldPoint(screenPosition);
                worldPosition.z = 0;

                if (Input.GetMouseButtonDown(0) && ItemButtons[currentButtonPressed].clicked && ItemButtons[currentButtonPressed].GetQuantity() != 0)
                {
                    ItemButtons[currentButtonPressed].SetClicked(false);
                    GameObject newObject = ItemButtons[currentButtonPressed].GetLastSpawnedObject();
                    GameObject levelLayoutObject = GameObject.FindGameObjectWithTag("LevelLayoutObject");
                    newObject.transform.SetParent(levelLayoutObject.transform, true);
                    newObject.GetComponent<SupportedObject>().ConfirmFirstPlacement();
                    newObject.transform.position = worldPosition;
                    ItemButtons[currentButtonPressed].DecrementQuantity();
                }
            }

            public void SendErrorMessage(string message)
            {
                errorMessagePanel.SetActive(true);
                errorText.text = message;
                Invoke(nameof(CloseErrorMessage), 10);
            }

            public void CloseErrorMessage()
            {
                errorMessagePanel.SetActive(false);
            }

            private void ResetInstatiationButtonCounters()
            {
                foreach(ObjectInstantiationButton button in ItemButtons)
                {
                    button.ResetQuantity();
                }
            }
        }
    }
}
