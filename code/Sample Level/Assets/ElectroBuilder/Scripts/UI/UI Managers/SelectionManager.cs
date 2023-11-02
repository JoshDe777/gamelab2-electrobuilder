using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ElectroBuilder.Enums;
using ElectroBuilder.SupportedObjects;
using ElectroBuilder.UI;
using ElectroBuilder.Testmode;
using ElectroBuilder.SaveLoadSystem;
using System;
using System.Reflection;

namespace ElectroBuilder
{
    namespace ObjectActions
    {
        public class SelectionManager : MonoBehaviour
        {
            #region variables
            // General variables
            [SerializeField]
            private GameObject selectedObject;
            [SerializeField]
            private UIState uiState;

            public Camera cam;
            private bool isInMovingObjectMenu = false;

            // UI Panels:
            [SerializeField]
            private GameObject objectSelectedPanel;
            [SerializeField]
            private GameObject noObjectSelectedPanel;
            [SerializeField]
            private GameObject defaultObjectActionMenu;
            [SerializeField]
            private GameObject movingObjectActionMenu;
            [SerializeField]
            private TMP_Text objectNameText;
            [SerializeField]
            private Slider speedSlider;

            // Events:
            private bool testMode = false;
            private TestModeEventHandler testModeEventHandler;
            private SaveLoadEventHandler saveLoadEvents;
            #endregion

            #region public functions
            public GameObject GetSelectedObject() => selectedObject;

            public void SetMovingObjectMenuBool(bool b) => isInMovingObjectMenu = b;

            public UIState GetUIState() => uiState;

            public void SetUIState(UIState newState) => uiState = newState;

            public void DeselectCurrentObject()
            {
                DeselectCurrentObjectUnlessSameObjectSelected(null);
                ChangeObjectActionUI(null);
                selectedObject = null;
            }
            #endregion

            #region unity functions
            void Update()
            {
                ObjectSelectionCheck();
            }

            private void Start()
            {
                SetUIState(UIState.IDLE);

                //Event when Testmode Switch Button is pressed
                testModeEventHandler = FindObjectOfType<TestModeEventHandler>();
                testModeEventHandler.OnTestmodeSwitch += ChangeTestModeBool;

                SaveLoadEventHandler saveLoadEventHandler = FindObjectOfType<SaveLoadEventHandler>();
                saveLoadEventHandler.OnNewLevel += AddToTestMode;
                saveLoadEventHandler.OnLoadLevel += AddToTestMode;

                saveLoadEvents = FindObjectOfType<SaveLoadEventHandler>();
                Action closeAnyObjectMode = () => CloseAnyObjectMode();
                saveLoadEvents.OnSaveLevel += closeAnyObjectMode;
                Action deselectCurrentObject = () => DeselectCurrentObject();
                saveLoadEvents.OnNewLevel += deselectCurrentObject;
                saveLoadEvents.OnLoadLevel += deselectCurrentObject;
            }
            #endregion

            #region events
            private void ChangeTestModeBool()
            {
                testMode = testModeEventHandler.TestmodeToggle;
                if (testMode)
                {
                    DeselectCurrentObject();
                    SetUIState(UIState.TESTING);
                }
                else
                {
                    SetUIState(UIState.IDLE);
                    ChangeObjectActionUI(null);
                }
            }

            void AddToTestMode()
            {
                testModeEventHandler.OnTestmodeSwitch += ChangeTestModeBool;
            }
            #endregion

            #region selection
            void ObjectSelectionCheck()
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = cam.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out RaycastHit hitInfo))
                    {
                        GameObject obj = hitInfo.collider.gameObject;
                        if (obj != selectedObject && obj.GetComponent<ObjectSelection>() != null && !EventSystem.current.IsPointerOverGameObject())
                        {
                            SupportedObject objectManager = obj.GetComponent<SupportedObject>();
                            if (objectManager != null)
                            {
                                GameObject levelLayoutObject = GameObject.FindGameObjectWithTag("LevelLayoutObject");
                                if (obj.transform.parent != levelLayoutObject.transform)
                                {
                                    obj = obj.transform.parent?.gameObject;
                                }
                                if (!(obj.GetComponent<SupportedObject>()?.GetStatus() == ObjectStatus.MOVELIMITS))
                                {
                                    DeselectCurrentObjectUnlessSameObjectSelected(obj);
                                    SelectObject(obj);
                                    ChangeObjectActionUI(obj);
                                }
                            }
                        }
                        else
                        {
                            bool isArrow = false;
                            if (obj != selectedObject)
                            {
                                if (obj.transform.parent != null)
                                {
                                    if (obj.transform.parent.parent != null)
                                    {
                                        if (obj.transform.parent.parent.CompareTag("Arrows"))
                                        {
                                            isArrow = true;
                                        }
                                        else if (obj.transform.parent.parent != null)
                                        {
                                            if (obj.transform.parent.parent.CompareTag("Arrows"))
                                            {
                                                isArrow = true;
                                            }
                                        }
                                    }
                                }
                            }
                            else if (!EventSystem.current.IsPointerOverGameObject() && obj != selectedObject && !isArrow)         // <- prevents from deselecting objects if clicking at UI
                            {
                                DeselectCurrentObject();
                            }
                        }
                    }
                    else
                    {
                        if (!EventSystem.current.IsPointerOverGameObject())             // <- prevents from deselecting objects if clicking at UI
                        {
                            DeselectCurrentObject();
                        }
                    }
                }
            }

            void SelectObject(GameObject obj)
            {
                selectedObject = obj;

                ObjectSelection objectSelectionComponent = obj.GetComponent<ObjectSelection>();
                if (objectSelectionComponent != null)
                {
                    objectSelectionComponent.Select();
                }
            }

            void DeselectCurrentObjectUnlessSameObjectSelected(GameObject nextSelection = null)
            {
                /* no changes if:
                 * no obj previously selected, or
                 * newly selected the exact same obj */
                if (selectedObject == null | selectedObject == nextSelection)
                {
                    return;
                }


                if (selectedObject != null && selectedObject.GetComponent<SupportedObject>().GetStatus() != ObjectStatus.SELECTED)
                {
                    CloseAnyObjectMode();
                }

                ObjectSelection selectedObjectSelectionComponent = selectedObject.GetComponent<ObjectSelection>();
                if (selectedObjectSelectionComponent != null)
                {
                    selectedObjectSelectionComponent.Deselect();
                }
            }
            #endregion

            #region ui management
            void ChangeObjectActionUI(GameObject obj)
            {
                if (obj == null)
                {
                    noObjectSelectedPanel.SetActive(true);
                    objectSelectedPanel.SetActive(false);
                    isInMovingObjectMenu = false;
                    return;
                }
                else
                {
                    SupportedObject objectManager = obj.GetComponent<SupportedObject>();
                    if (objectManager?.GetStatus() == ObjectStatus.SELECTED)
                    {
                        ObjectType objectType = objectManager.GetObjectType();
                        // check if 
                        switch (objectType)
                        {
                            // if newly selected object is a start point, end point, or static object (identical menus):
                            case ObjectType.START:
                            case ObjectType.END:
                            case ObjectType.STATIC:
                            case ObjectType.CUSTOM:
                                OpenObjectActionMenu(obj.name);
                                OpenDefaultObjectActionMenu();
                                break;
                            //if newly selected object is a moving object:
                            case ObjectType.MOVING:
                                OpenObjectActionMenu(obj.name);
                                OpenMovingObjectMenu();
                                break;
                        }
                    }
                }
            }

            void OpenObjectActionMenu(string name)
            {
                noObjectSelectedPanel.SetActive(false);
                objectSelectedPanel.SetActive(true);
                objectNameText.text = name.Substring(0, name.Length - 7);
            }

            void OpenDefaultObjectActionMenu()
            {
                defaultObjectActionMenu.SetActive(true);
                movingObjectActionMenu.SetActive(false);
                isInMovingObjectMenu = false;
            }

            void OpenMovingObjectMenu()
            {
                defaultObjectActionMenu.SetActive(false);
                movingObjectActionMenu.SetActive(true);
                SpeedSlider slider = movingObjectActionMenu.GetComponentInChildren<SpeedSlider>();
                slider.AssignSliderValueOnSelection();
                isInMovingObjectMenu = true;
            }

            private void CloseAnyObjectMode()
            {
                if (selectedObject != null)
                {
                    ObjectStatus os = selectedObject.GetComponent<SupportedObject>().GetStatus();
                    MoveObjectMenuManager moveObjectMenuManager = GetComponent<MoveObjectMenuManager>();
                    switch (os)
                    {
                        case ObjectStatus.MOVE:
                            moveObjectMenuManager.CloseMoveObjectMode(true);
                            break;
                        case ObjectStatus.MOVELIMITS:
                            moveObjectMenuManager.CloseMoveLimitsMode(true);
                            break;
                        case ObjectStatus.RESIZE:
                            ResizingManager resizingManager = GetComponent<ResizingManager>();
                            resizingManager.CloseResizeMode(true);
                            break;
                    }
                }
            }
            #endregion
        }
    }
}
