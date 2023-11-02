using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ElectroBuilder
{
    namespace ObjectActions
    {
        public class YCubeScaling : MonoBehaviour
        {
            ResizingManager resizingManager;

            Vector3 defaultArrowScale = new Vector3(1, 1, 1);
            Vector3 startCubePosition;
            private GameObject yArrowRod;
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
                mainCamera = Camera.main;
                cameraDistanceToObject = mainCamera.WorldToScreenPoint(transform.position).z + 1;
                startCubePosition = transform.localPosition;
                FindCoupledGameObjects();
                distanceBetweenCubes = 2 * Vector3.Distance(transform.position, inverseCube.transform.position);
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
                scaleOffset.y = startScale.y;
                if (resizingManager.IsResizeModeUniform())
                {
                    scaleOffset.x = startScale.x;
                }
                else
                {
                    scaleOffset.x = 0;
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

                RepositionCubes(dist);
                ResizeCylinder();
            }

            private void OnMouseUp()
            {
                ResetAllPositions();
            }

            private float CalculateNewUniformScale(float xOld, float yOld, float yNew) => (xOld * yNew) / yOld;

            private void RepositionCubes(float dist)
            {
                Vector3 newCubePos = anchorPoint + new Vector3(0, dist, -1);
                transform.position = newCubePos;

                Vector3 newInverseCubePosition = -transform.localPosition;
                inverseCube.transform.localPosition = newInverseCubePosition;
            }
            private void ResizeCylinder()
            {
                Vector3 newArrowScale = transform.localScale;
                float newDistBetweenCubes = Mathf.Abs(transform.position.y - inverseCube.transform.position.y);

                newArrowScale.y = 2 * CalculateNewArrowScaleY(defaultArrowScale.y, distanceBetweenCubes, newDistBetweenCubes);
                yArrowRod.transform.localScale = newArrowScale;
            }

            void ResetAllPositions()
            {
                yArrowRod.transform.localScale = defaultArrowScale;
                transform.localPosition = startCubePosition;
                inverseCube.transform.localPosition = -startCubePosition;
            }

            private void FindCoupledGameObjects()
            {
                List<string> cubeNames = new List<string> { "plusY", "minusY" };
                foreach (Transform child in transform.parent)
                {
                    if (child.name.Equals("yCylinder"))
                    {
                        yArrowRod = child.gameObject;
                    }
                    else if (cubeNames.Contains(child.name) && child.name != name)
                    {
                        inverseCube = child.gameObject;
                    }
                }
            }

            private float CalculateNewArrowScaleY(float oldScale, float oldDist, float newDist) => (oldScale * newDist) / oldDist;

            private float GetSignedYDistanceBetweenVectors(Vector3 v1, Vector3 v2) => v2.y - v1.y;
        }
    }
}
