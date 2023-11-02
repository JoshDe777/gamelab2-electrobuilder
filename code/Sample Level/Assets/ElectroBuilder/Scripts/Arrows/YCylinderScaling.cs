using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ElectroBuilder
{
    namespace ObjectActions
    {
        public class YCylinderScaling : MonoBehaviour
        {
            #region variables
            ResizingManager resizingManager;

            Vector3 defaultArrowScale = new Vector3(1, 1, 1);
            private Vector3[] yCubePositions;
            private GameObject[] yCubes;
            private float distanceBetweenCubes;
            private float mouseOffsetFromCube = 100000000;

            Vector3 anchorPoint = Vector3.zero;
            private GameObject objectToScale;
            private Camera mainCamera;
            private float cameraDistanceToObject;
            Vector3 startScale = Vector3.one;

            Vector3 scaleOffset = Vector3.zero;
            #endregion


            #region unity functions
            void Start()
            {
                resizingManager = FindObjectOfType<ResizingManager>();
                mainCamera = Camera.main;
                cameraDistanceToObject = mainCamera.WorldToScreenPoint(transform.position).z + 1;
                yCubes = new GameObject[2];
                yCubePositions = new Vector3[2];
                FindCoupledGameObjects();
                SetCubeStartPositions();
                distanceBetweenCubes = 2 * Vector3.Distance(yCubes[0].transform.position, yCubes[1].transform.position);
            }
            #endregion

            #region setup
            public void Setup(GameObject obj)
            {
                objectToScale = obj;
                anchorPoint = obj.transform.position;
                startScale = obj.transform.localScale;
            }

            private void FindCoupledGameObjects()
            {
                foreach (Transform child in transform.parent)
                {
                    if (child.name.Equals("plusY"))
                    {
                        yCubes[0] = child.gameObject;
                    }
                    else if (child.name.Equals("minusY"))
                    {
                        yCubes[1] = child.gameObject;
                    }
                }
            }

            private void SetCubeStartPositions()
            {
                yCubePositions[0] = yCubes[0].transform.position;
                yCubePositions[1] = yCubes[1].transform.position;
            }
            #endregion

            #region scaling
            public void OnMouseDown()
            {
                startScale = objectToScale.transform.localScale;
                scaleOffset.y = startScale.y;
                if (resizingManager.IsResizeModeUniform())
                {
                    scaleOffset.x = startScale.x;
                }
                else
                {
                    scaleOffset.x = 0;
                }
                Vector3 MouseScreenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, cameraDistanceToObject-1);
                Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(MouseScreenPos);
                foreach(GameObject cube in yCubes)
                {
                    if (Mathf.Abs(cube.transform.position.y - mouseWorldPos.y) < mouseOffsetFromCube)
                    {
                        mouseOffsetFromCube = Mathf.Abs(cube.transform.position.y - mouseWorldPos.y);
                    }
                }
            }

            private void OnMouseDrag()
            {
                Vector3 MouseScreenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, cameraDistanceToObject);
                Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(MouseScreenPos);

                float dist = Vector3.Distance(anchorPoint, mouseWorldPos);
                Vector3 newScale = objectToScale.transform.localScale;
                if (resizingManager.IsResizeModeUniform())
                {
                    newScale.x = CalculateNewUniformScale(startScale.x, startScale.y, dist);
                }
                newScale.y = dist;

                objectToScale.transform.localScale = newScale;

                RepositionCubes(dist + mouseOffsetFromCube);
                ResizeCylinder();
            }

            private void OnMouseUp()
            {
                ResetAllPositions();
            }
            #endregion

            #region support functions
            private float CalculateNewUniformScale(float xOld, float yOld, float yNew) => (xOld * yNew) / yOld;

            private void RepositionCubes(float dist)
            {
                Vector3 newCubePos = anchorPoint + new Vector3(0, dist, -1);
                yCubes[0].transform.position = newCubePos;

                Vector3 newInverseCubePosition = -yCubes[0].transform.localPosition;
                yCubes[1].transform.localPosition = newInverseCubePosition;
            }

            private void ResizeCylinder()
            {
                Vector3 newArrowScale = transform.localScale;
                float newDistBetweenCubes = Mathf.Abs(yCubes[0].transform.position.y - yCubes[1].transform.position.y);

                newArrowScale.y = 2 * CalculateNewArrowScaleY(defaultArrowScale.y, distanceBetweenCubes, newDistBetweenCubes);
                transform.localScale = newArrowScale;
            }

            void ResetAllPositions()
            {
                transform.localScale = defaultArrowScale;
                yCubes[0].transform.position = yCubePositions[0];
                yCubes[1].transform.position = yCubePositions[1];
            }

            private float CalculateNewArrowScaleY(float oldScale, float oldDist, float newDist) => (oldScale * newDist) / oldDist;
            #endregion
        }
    }
}

