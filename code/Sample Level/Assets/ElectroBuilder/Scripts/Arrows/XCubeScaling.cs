using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.SceneManagement;


namespace ElectroBuilder
{
    namespace ObjectActions
    {
        public class XCubeScaling : MonoBehaviour
        {
            ResizingManager resizingManager;

            Vector3 defaultArrowScale = new Vector3(1, 1, 1);
            Vector3 startCubePosition;
            private GameObject xArrowRod;
            private GameObject inverseCube;
            private float distanceBetweenCubes;

            Vector3 anchorPoint = Vector3.zero;
            private GameObject objectToScale;
            private Camera mainCamera;
            private float cameraDistanceToObject;
            Vector3 startScale = Vector3.one;

            Vector3 scaleOffset = Vector3.zero;

            // unity functions:
            void Start()
            {
                resizingManager = FindObjectOfType<ResizingManager>();
                startCubePosition = transform.localPosition;
                mainCamera = Camera.main;
                cameraDistanceToObject = mainCamera.WorldToScreenPoint(transform.position).z + 1;
                FindCoupledGameObjects();
                distanceBetweenCubes = 2*Vector3.Distance(transform.position, inverseCube.transform.position);
            }


            // setup functions:
            public void Setup(GameObject obj)
            {
                objectToScale = obj;
                anchorPoint = obj.transform.position;
                startScale = obj.transform.localScale;
            }


            // scaling:
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
            }

            private void OnMouseDrag()
            {
                Vector3 MouseScreenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, cameraDistanceToObject);
                Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(MouseScreenPos);

                float dist = Vector3.Distance(anchorPoint, mouseWorldPos);
                /*float dist = GetSignedXDistanceBetweenVectors(anchorPoint, mouseWorldPos);
                if (name.Equals("minusX"))
                {
                    dist = dist * -1;
                }*/
                Vector3 newScale = objectToScale.transform.localScale;
                if (resizingManager.IsResizeModeUniform())
                {
                    newScale.y = CalculateNewUniformScale(startScale.x, startScale.y, dist);
                }
                newScale.x = dist;
                // newScale += scaleOffset;

                objectToScale.transform.localScale = newScale;

                RepositionCubes(dist);
                ResizeCylinder();
            }

            private void OnMouseUp()
            {
                //RepositionAndResizeCylinderAndArrowhead(defaultArrowScale);
                ResetAllPositions();
            }

            private float CalculateNewUniformScale(float xOld, float yOld, float xNew) => (yOld * xNew) / xOld;
            
            private void RepositionCubes(float dist)
            {
                Vector3 newCubePos = anchorPoint + new Vector3(dist, 0, -1);
                transform.position = newCubePos;

                Vector3 newInverseCubePosition = -transform.localPosition;
                inverseCube.transform.localPosition = newInverseCubePosition;
            }

            private void ResizeCylinder()
            {
                Vector3 newArrowScale = transform.localScale;
                float newDistBetweenCubes = Mathf.Abs(transform.position.x - inverseCube.transform.position.x);

                newArrowScale.x = 2*CalculateNewArrowScaleX(defaultArrowScale.x, distanceBetweenCubes, newDistBetweenCubes);
                xArrowRod.transform.localScale = newArrowScale;
            }

            void ResetAllPositions()
            {
                xArrowRod.transform.localScale = defaultArrowScale;
                transform.localPosition = startCubePosition;
                inverseCube.transform.localPosition = -startCubePosition;
            }

            private void FindCoupledGameObjects()
            {
                List<string> cubeNames = new List<string> { "plusX", "minusX" };
                foreach (Transform child in transform.parent)
                {
                    if (child.name.Equals("xCylinder"))
                    {
                        xArrowRod = child.gameObject;
                    }
                    else if (cubeNames.Contains(child.name) && child.name != name)
                    {
                        inverseCube = child.gameObject;
                    }
                }
            }

            private float CalculateNewArrowScaleX(float oldScale, float oldDist, float newDist) => (oldScale*newDist)/oldDist;

            private float GetSignedXDistanceBetweenVectors(Vector3 v1, Vector3 v2) => v2.x - v1.x;
        }
    }
}
