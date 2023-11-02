using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ElectroBuilder
{
    namespace ObjectActions
    {
        public class XCylinderScaling : MonoBehaviour
        {
            #region variables
            ResizingManager resizingManager;

            Vector3 defaultArrowScale = new Vector3(1, 1, 1);
            private Vector3[] xCubePositions;
            private GameObject[] xCubes;
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
                xCubes = new GameObject[2];
                xCubePositions = new Vector3[2];
                FindCoupledGameObjects();
                SetCubeStartPositions();
                distanceBetweenCubes = 2 * Vector3.Distance(xCubePositions[0], xCubePositions[1]);
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
                    if (child.name.Equals("plusX"))
                    {
                        xCubes[0] = child.gameObject;
                    }
                    else if (child.name.Equals("minusX"))
                    {
                        xCubes[1] = child.gameObject;
                    }
                }
            }

            private void SetCubeStartPositions()
            {
                xCubePositions[0] = xCubes[0].transform.position;
                xCubePositions[1] = xCubes[1].transform.position;
            }
            #endregion

            #region scaling
            public void OnMouseDown()
            {
                startScale = objectToScale.transform.localScale;
                scaleOffset.x = startScale.x;
                if (resizingManager.IsResizeModeUniform())
                {
                    scaleOffset.y = startScale.y;
                }
                else
                {
                    scaleOffset.y = 0;
                }
                Vector3 MouseScreenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, cameraDistanceToObject - 1);
                Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(MouseScreenPos);
                foreach (GameObject cube in xCubes)
                {
                    if (Mathf.Abs(cube.transform.position.x - mouseWorldPos.x) < mouseOffsetFromCube)
                    {
                        mouseOffsetFromCube = Mathf.Abs(cube.transform.position.x - mouseWorldPos.x);
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
                    newScale.y = CalculateNewUniformScale(startScale.x, startScale.y, dist);
                }
                newScale.x = dist;

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
                Vector3 newCubePos = anchorPoint + new Vector3(dist, 0, -1);
                xCubes[0].transform.position = newCubePos;

                Vector3 newInverseCubePosition = -xCubes[0].transform.localPosition;
                xCubes[1].transform.localPosition = newInverseCubePosition;
            }

            private void ResizeCylinder()
            {
                Vector3 newArrowScale = transform.localScale;
                float newDistBetweenCubes = Mathf.Abs(xCubes[0].transform.position.x - xCubes[1].transform.position.x);

                newArrowScale.x = 2 * CalculateNewArrowScaleY(defaultArrowScale.y, distanceBetweenCubes, newDistBetweenCubes);
                transform.localScale = newArrowScale;
            }

            void ResetAllPositions()
            {
                transform.localScale = defaultArrowScale;
                xCubes[0].transform.position = xCubePositions[0];
                xCubes[1].transform.position = xCubePositions[1];
            }

            private float CalculateNewArrowScaleY(float oldScale, float oldDist, float newDist) => (oldScale * newDist) / oldDist;
            #endregion
        }
    }
}
