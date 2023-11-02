using System;
using UnityEngine;
using Cinemachine;
using ElectroBuilder.Testmode;
using ElectroBuilder.SaveLoadSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ElectroBuilder
{
    namespace VirtualCamera
    {
        public class VirtualCameraManager : MonoBehaviour
        {
            //Getting the affected VirtualCamera
            [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;


            // Camera
            private float cameraMoveSpeed = 30f;

            //Camera Middle Mouse
            private float middleMouseSpeed = 150f;


            //Turning Camerafunctions ON/OFF in Unity Editor
            private bool allowCameraZoom = true;


            //Zoom 
            private float minZoomValue = 1f; //Must be >0!
            private float zoomSpeed = 0.25f;
            private float orthographicStartSize;



            //Camera Modes
            private bool testMode = false;
            private TestModeEventHandler testModeEventHandler;

            //Camera disabel when Save Window open
            [SerializeField] private SaveLoadLevelUI sLLUI;

            //Camerafollow Values
            private Transform target;
            private Vector3 offSet;

            //Editor Foldout
            private bool testingModeFolout = false;
            private bool editingModeFolout = false;

            private void Start()
            {
                //Event when Testmode Switch Button is pressed
                testModeEventHandler = FindObjectOfType<TestModeEventHandler>();
                testModeEventHandler.OnTestmodeSwitch += changeTestModeBool;

                //sLLUI = GameObject.Find("UI_InputWindow_SaveLevel").GetComponent<SaveLoadLevelUI>();

                //set orthographicStartSize
                orthographicStartSize = cinemachineVirtualCamera.m_Lens.OrthographicSize;
            }


            private void Update()
            {

                if (testMode)
                    CharacterFollowCamera();
                else
                {
                    if (Input.GetMouseButton(2))
                    {
                        HandleMiddleMouseCameraMove();
                    }
                    else
                    {
                        HandleCameraMovmentBasic();
                    }

                    if (allowCameraZoom)
                    {
                        HandleCameraZoom();
                    }
                }
            }

            //Moving the Camera with WASD Keys
            private void HandleCameraMovmentBasic()
            {
                //Check for Input
                Vector3 inputDir = new Vector3(0, 0, 0);

                if (Input.GetKey(KeyCode.A) && !sLLUI.saveWindowOpen) inputDir.x = -1f;
                if (Input.GetKey(KeyCode.D) && !sLLUI.saveWindowOpen) inputDir.x = +1f;
                if (Input.GetKey(KeyCode.W) && !sLLUI.saveWindowOpen) inputDir.y = +1f;
                if (Input.GetKey(KeyCode.S) && !sLLUI.saveWindowOpen) inputDir.y = -1f;


                //Reset Camera Position
                if (Input.GetKey(KeyCode.F) && !sLLUI.saveWindowOpen) resetCameraPosition();

                //Input directions are translated to the according MovmentDirection regardles of the rotation of the Camera
                Vector3 moveDir = transform.right * inputDir.x + transform.up * inputDir.y;

                //Camera Movement depending in Input and Speed
                transform.position += moveDir * cameraMoveSpeed * Time.deltaTime;
            }

            private void HandleMiddleMouseCameraMove()
            {
                transform.position += new Vector3(Input.GetAxisRaw("Mouse X") * Time.deltaTime * middleMouseSpeed * (-1f), Input.GetAxisRaw("Mouse Y") * Time.deltaTime * middleMouseSpeed * (-1f), 0.0f);
            }

            //Changing the FOV to get a Zoom effekt
            private void HandleCameraZoom()
            {
                //Check for Input and Zoom in or out
                if (Input.mouseScrollDelta.y > 0 && cinemachineVirtualCamera.m_Lens.OrthographicSize > minZoomValue)
                {
                    cinemachineVirtualCamera.m_Lens.OrthographicSize -= zoomSpeed;
                }
                if (Input.mouseScrollDelta.y < 0)
                {
                    cinemachineVirtualCamera.m_Lens.OrthographicSize += zoomSpeed;
                }

                //Zoom Reset
                if (Input.GetKey(KeyCode.R) && !sLLUI.saveWindowOpen)
                {
                    cinemachineVirtualCamera.m_Lens.OrthographicSize = orthographicStartSize;
                }
                
            }

            private void CharacterFollowCamera()
            {
                if (target != null)
                {
                    //If Object is found then Camera follows Player 
                    transform.position = target.position + offSet;
                }
            }

            public void changeTestModeBool()
            {
                testMode = testModeEventHandler.TestmodeToggle;
            }

            public void SetTargetObject(GameObject obj)
            {
                target = obj.transform;
            }

            public void switchCameraMode()
            {
                Camera.main.orthographic = !Camera.main.orthographic;
            }

            private void resetCameraPosition()
            {
                GameObject respawnObject = GameObject.FindGameObjectWithTag("Respawn");

                if (respawnObject != null)
                {
                    transform.position = respawnObject.transform.position;
                }
                else
                {
                    transform.position = Vector3.zero;
                }
            }

            //Custom Editor Layout
            #region Editor
#if UNITY_EDITOR

            [CustomEditor(typeof(VirtualCameraManager))]
            public class VirtualCameraEditor : Editor
            {

                public override void OnInspectorGUI()
                {
                    //Shows everything that would usually be shown
                    base.OnInspectorGUI();

                    //Instance of the VirtualCameraManager Script
                    VirtualCameraManager manager = (VirtualCameraManager)target;

                    EditorGUILayout.Space();

                    //testing Mode Toggel with Foldout Window for accoring Values
                    //Foldout
                    manager.testingModeFolout = EditorGUILayout.Foldout(manager.testingModeFolout, "Testing Mode Values", true);
                    if (manager.testingModeFolout)
                    {
                        //offSet Values
                        manager.offSet = EditorGUILayout.Vector3Field("Camera Player offSet", manager.offSet);
                    }

                    EditorGUILayout.Space();

                    //Editing Mode Foldout Window For according possibilities           
                    //Foldout
                    manager.editingModeFolout = EditorGUILayout.Foldout(manager.editingModeFolout, "Editing Mode Possibilities", true);
                    if (manager.editingModeFolout)
                    {
                        //Indent for cleaner Appearance 
                        EditorGUI.indentLevel++;

                        manager.cameraMoveSpeed = EditorGUILayout.FloatField("Camera Move Speed", manager.cameraMoveSpeed);

                        manager.middleMouseSpeed = EditorGUILayout.FloatField("Middle Mouse Move Speed", manager.middleMouseSpeed);

                        // Camera Zoom Toggel with according Values
                        manager.allowCameraZoom = EditorGUILayout.Toggle("Allow Camera Zoom", manager.allowCameraZoom);
                        if (manager.allowCameraZoom)
                        {
                            EditorGUI.indentLevel++;

                            EditorGUILayout.BeginVertical("box");

                            manager.minZoomValue = EditorGUILayout.FloatField("Zoom in Stop Value", manager.minZoomValue);
                            manager.zoomSpeed = EditorGUILayout.FloatField("Zoom Speed", manager.zoomSpeed);

                            EditorGUILayout.EndVertical();

                            EditorGUI.indentLevel--;

                            EditorGUILayout.Space();
                        }

                        //reset the Indent
                        EditorGUI.indentLevel--;
                    }
                }

            }


#endif
            #endregion
        }
    }
}


