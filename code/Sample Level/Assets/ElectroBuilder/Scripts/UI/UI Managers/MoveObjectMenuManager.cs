using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ElectroBuilder.ObjectActions;
using ElectroBuilder.Enums;
using ElectroBuilder.SupportedObjects;

namespace ElectroBuilder
{
    namespace UI
    {
        public class MoveObjectMenuManager : MonoBehaviour
        {
            // script references
            private SelectionManager selectionManager;

            // bools
            private bool dragAndDrop = true;

            // GameObjects & related components:
            private GameObject obj;
            private ObjectMovement objectMovement;

            // UI Elements:
            [SerializeField]
            private GameObject MoveMenu;
            [SerializeField]
            private GameObject DefaultMenu;
            [SerializeField]
            private GameObject MovingObjectMenu;
            [SerializeField]
            private TMP_Text objectCoordinatesText;
            [SerializeField]
            private GameObject LimitsMenu;
            [SerializeField]
            private GameObject moveArrowsPrefab;

            // Start
            void Start()
            {
                selectionManager = FindObjectOfType<SelectionManager>();
                obj = selectionManager.GetSelectedObject();
            }

            // Getters & Setters:

            public bool IsDragAndDropModeOn()
            {
                return dragAndDrop;
            }


            // button functions:
            public void OpenMoveObjectMode()
            {
                // Assertions
                obj = selectionManager.GetSelectedObject();
                Debug.Assert(obj != null);

                selectionManager.SetUIState(UIState.MOVEOBJECT);
                obj = selectionManager.GetSelectedObject();
                objectMovement = obj.GetComponent<ObjectMovement>();
                objectMovement.StartMoveObjectMode();
                if (!dragAndDrop)
                {
                    SpawnArrows();
                }
                OpenMoveModeUI();
            }

            public void CloseMoveObjectMode(bool save)
            {
                if (!dragAndDrop)
                {
                    DestroyArrows();
                }
                objectMovement.CloseMoveObjectMode(save);
                selectionManager.SetUIState(UIState.MAIN);
                CloseMoveModeUI();
            }

            public void ToggleDragAndDropMode(bool b)
            {
                dragAndDrop = b;
                SpawnOrDespawnArrows();
            }

            public void OpenMoveLimitsMode()
            {
                obj = selectionManager.GetSelectedObject();
                MovingObjectMovement movingObjectComponent = obj.GetComponent<MovingObjectMovement>();
                selectionManager.SetUIState(UIState.MOVELIMITS);
                Debug.Assert(movingObjectComponent != null);

                movingObjectComponent.OpenMoveLimitMode();
                if (!dragAndDrop)
                {
                    SpawnArrows();
                }
                OpenMoveLimitModeUI();
            }

            public void CloseMoveLimitsMode(bool save)
            {
                MovingObjectMovement movingObjectComponent = selectionManager.GetSelectedObject().GetComponent<MovingObjectMovement>();
                Debug.Assert(movingObjectComponent != null);

                if (!dragAndDrop)
                {
                    DestroyArrows();
                }
                movingObjectComponent.CloseMoveLimitMode(save);
                selectionManager.SetUIState(UIState.MAIN);
                CloseMoveLimitModeUI();
            }


            // opening functions
            private void OpenMoveModeUI()
            {
                MoveMenu.SetActive(true);
                DefaultMenu.SetActive(false);
                MovingObjectMenu.SetActive(false);
                selectionManager.SetMovingObjectMenuBool(false);
            }

            private void OpenMoveLimitModeUI()
            {
                LimitsMenu.SetActive(true);
                DefaultMenu.SetActive(false);
                MovingObjectMenu.SetActive(false);
                selectionManager.SetMovingObjectMenuBool(false);
            }


            // closing functions:
            private void CloseMoveModeUI()
            {
                if (obj.CompareTag("MovingObject"))
                {
                    MovingObjectMenu.SetActive(true);
                    selectionManager.SetMovingObjectMenuBool(true);
                }
                else
                {
                    DefaultMenu.SetActive(true);
                }
                MoveMenu.SetActive(false);
            }

            private void CloseMoveLimitModeUI()
            {
                obj = selectionManager.GetSelectedObject();
                if (obj.CompareTag("MovingObject"))
                {
                    MovingObjectMenu.SetActive(true);
                    selectionManager.SetMovingObjectMenuBool(true);
                }
                else
                {
                    DefaultMenu.SetActive(true);
                }
                LimitsMenu.SetActive(false);
            }


            // arrows:
            private void SpawnOrDespawnArrows()
            {
                obj = selectionManager.GetSelectedObject();
                if (!dragAndDrop)
                {
                    SpawnArrows();
                }
                else
                {
                    DestroyArrows();
                }
            }

            private void SpawnArrows()
            {
                if (selectionManager.GetUIState() == UIState.MOVEOBJECT && GameObject.FindWithTag("Arrows") == null)
                {
                    GameObject arrows = Instantiate(moveArrowsPrefab);
                    arrows.transform.position = obj.transform.position + new Vector3(0,0,-1);
                    foreach (Transform child in arrows.transform)
                    {
                        if (!child.name.Equals("center"))
                        {
                            foreach (Transform grandchild in child)
                            {
                                XArrowMovement xArrow = grandchild.GetComponent<XArrowMovement>();
                                if (xArrow != null)
                                {
                                    xArrow.Setup(obj);
                                }
                                else
                                {
                                    YArrowMovement yArrow = grandchild.GetComponent<YArrowMovement>();
                                    yArrow.Setup(obj);
                                }
                            }
                        }
                    }
                    obj.transform.SetParent(arrows.transform, true);
                }
                else if (selectionManager.GetUIState() == UIState.MOVELIMITS)
                {
                    MovingObject movingObject = obj.GetComponent<MovingObject>();
                    Debug.Assert(movingObject != null);

                    // get start point & end point objects.
                    List<GameObject> limits = movingObject.GetLimitGameObjects();

                    // spawn 2 sets of coordinate arrows.
                    // set objects to children of respective arrows.
                    foreach (GameObject limit in limits)
                    {
                        GameObject arrows = Instantiate(moveArrowsPrefab);
                        arrows.transform.position = limit.transform.localPosition + new Vector3(0,0,-1);
                        foreach (Transform child in arrows.transform)
                        {
                            if (!child.name.Equals("center"))
                            {
                                foreach (Transform grandchild in child.transform)
                                {

                                    XArrowMovement xArrow = grandchild.GetComponent<XArrowMovement>();
                                    if (xArrow != null)
                                    {
                                        xArrow.Setup(limit);
                                    }
                                    else
                                    {
                                        YArrowMovement yArrow = grandchild.GetComponent<YArrowMovement>();
                                        yArrow.Setup(limit);
                                    }
                                }
                            }
                        }
                        limit.transform.SetParent(arrows.transform, true);
                    }
                }
            }

            private void DestroyArrows()
            {
                if (selectionManager.GetUIState() == UIState.MOVEOBJECT)
                {
                    GameObject arrows = null;
                    if (obj.transform.parent.gameObject.CompareTag("Arrows"))
                    {
                        arrows = obj.transform.parent.gameObject;
                    }
                    Transform levelLayoutObject = GameObject.FindWithTag("LevelLayoutObject").transform;
                    obj.transform.SetParent(levelLayoutObject, true);
                    Destroy(arrows);
                }
                else
                {
                    MovingObject movingObject = obj.GetComponent<MovingObject>();
                    Debug.Assert(movingObject != null);

                    List<GameObject> limits = movingObject.GetLimitGameObjects();

                    foreach (GameObject limit in limits)
                    {
                        GameObject arrows = limit.transform.parent.gameObject;
                        limit.transform.SetParent(null, true);
                        Destroy(arrows);
                    }
                }
            }


            // runtime functions:
            void Update()
            {
                if (selectionManager.GetUIState() == UIState.MOVEOBJECT)
                {
                    UpdateObjectCoordinatesInUI();
                }
            }

            private void UpdateObjectCoordinatesInUI()
            {
                objectCoordinatesText.text = "Object Coordinates:\nx: " + obj.transform.position.x.ToString("n2") + "\ny: " + obj.transform.position.y.ToString("n2");
            }
        }
    }
}
