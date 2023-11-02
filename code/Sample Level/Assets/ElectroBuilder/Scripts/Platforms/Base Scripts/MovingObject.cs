using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ElectroBuilder.Enums;
using ElectroBuilder.Testmode;
using ElectroBuilder.ObjectActions;
using ElectroBuilder.SaveLoadSystem;
using System;

namespace ElectroBuilder
{
    namespace SupportedObjects
    {
        public class MovingObject : SupportedObject
        {
            #region variables
            [SerializeField]
            private Vector2 startPoint;
            [SerializeField]
            private Vector2 endPoint;

            private Vector3 targetPos;

            public float speed { get; set; }

            private bool movingStartToEnd;

            [SerializeField]
            private GameObject endPointPrefab;
            private GameObject line;
            [SerializeField] private List<GameObject> limits = new List<GameObject>();
            #endregion


            #region inherited methods
            public override ObjectType GetObjectType()
            {
                return ObjectType.MOVING;
            }

            protected override void Start()
            {
                base.Start();
                PrepareEvents();
                speed = 1;
                Setup();
            }

            protected override void RuntimeBehaviour()
            {
                Vector3 currentPos = RoundVector(transform.position);
                Vector3 startPointV3 = RoundVector(new Vector3(startPoint.x, startPoint.y, 0));
                Vector3 endPointV3 = RoundVector(new Vector3(endPoint.x, endPoint.y, 0));

                if (currentPos == startPointV3)
                {
                    targetPos = endPointV3;
                }
                else if (currentPos == endPointV3)
                {
                    targetPos = startPointV3;
                }

                Vector3 targetDirection = (targetPos - currentPos).normalized;
                transform.Translate(speed * Time.deltaTime * targetDirection);
            }

            protected override void AddRequiredScripts()
            {
                gameObject.AddComponent<ObjectSelection>();
                gameObject.AddComponent<MovingObjectMovement>();
                gameObject.AddComponent<MovingObjectResizing>();
            }

            protected override void ChangeTestModeBool()
            {
                base.ChangeTestModeBool();
                if (!testMode)
                {
                    foreach (Transform child in transform)
                    {
                        child.gameObject.SetActive(true);
                    }
                    MoveToCenterOfEndPoints();
                }
                else
                {
                    UpdateBothEndPointPositions();
                    Vector3 startPosV3 = new Vector3(startPoint.x, startPoint.y, 0);
                    targetPos = startPosV3;
                    foreach(Transform child in transform)
                    {
                        child.gameObject.SetActive(false);
                    }
                }
            }

            public override void Delete()
            {

                //saveLoadEvents.OnSaveLevel -= RemoveChildObjectsToSave;
                saveLoadEventHandler.OnExitSave -= Setup;
                DeleteRecursive();
                base.Delete();
            }

            public override void ConfirmFirstPlacement()
            {
                base.ConfirmFirstPlacement();
                UpdateBothEndPointPositions();
                RedrawLine();
            }
            #endregion

            // getters:
            public List<GameObject> GetLimitGameObjects() => limits;

            public Vector2 GetStartPointPos() => startPoint;

            public Vector2 GetEndPointPos() => endPoint;


            //runtime behaviour:
            private void OnTriggerEnter(Collider other)
            {
                if (testMode && other.gameObject.CompareTag("Player"))
                {
                    other.transform.SetParent(transform);
                }
            }

            private void OnTriggerExit(Collider other)
            {
                if (testMode && other.gameObject.CompareTag("Player"))
                {
                    other.transform.SetParent(null);
                }
            }


            //editor behaviour:
            void Setup()
            {
                bool firstSetup = false;
                if (transform.childCount == 0)
                {
                    firstSetup = true;
                    SetupStartAndEndPoints();
                    CreateStartAndEndPointObjects();
                }
                UpdateBothEndPointPositions();
                AddRigidBodyAndCollider();
                if (firstSetup)
                {
                    DrawNewLineBetweenStartAndEndPoint();
                }
                else
                {
                    foreach (Transform t in transform)
                    {
                        if (t.gameObject.GetComponent<MovingObjectLine>()!= null)
                        {
                            line = t.gameObject;
                        } else if (t.gameObject.GetComponent<MovingObjectLimit>().GetObjectType() == ObjectType.STARTLIMIT)
                        {
                            limits.Insert(0, t.gameObject);
                        }
                        else if (t.gameObject.GetComponent<MovingObjectLimit>().GetObjectType() == ObjectType.ENDLIMIT)
                        {
                            limits.Insert(1, t.gameObject);
                        }
                    }
                    RedrawLine();
                }
            }

            void SetupStartAndEndPoints()
            {
                if (startPoint  == Vector2.zero && endPoint == Vector2.zero)
                {
                    startPoint = new Vector2(transform.position.x - 1, transform.position.y);
                    endPoint = new Vector2(transform.position.x + 1, transform.position.y);
                }
            }

            void AddRigidBodyAndCollider()
            {
                if (GetComponent<Rigidbody>() == null)
                {
                    Rigidbody rb = gameObject.AddComponent(typeof(Rigidbody)) as Rigidbody;
                    rb.useGravity = false;
                    rb.isKinematic = true;
                }

                if (GetComponent<MeshCollider>() == null)
                {
                    MeshCollider mc = gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
                }
                
            }

            void CreateStartAndEndPointObjects()
            {
                if (limits.Count == 0)
                {
                    GameObject startPointSphere = Instantiate(endPointPrefab);
                    startPointSphere.transform.position = new Vector3(startPoint.x, startPoint.y, -1);
                    startPointSphere.transform.SetParent(transform, true);
                    startPointSphere.name = "Start Point";
                    startPointSphere.GetComponent<MovingObjectLimit>().SetLimitType(ObjectType.STARTLIMIT);
                    limits.Add(startPointSphere);

                    GameObject endPointSphere = Instantiate(endPointPrefab);
                    endPointSphere.transform.position = new Vector3(endPoint.x, endPoint.y, -1);
                    endPointSphere.transform.SetParent(transform, true);
                    endPointSphere.name = "End Point";
                    endPointSphere.GetComponent<MovingObjectLimit>().SetLimitType(ObjectType.ENDLIMIT);
                    limits.Add(endPointSphere);

                    UpdateBothEndPointPositions();
                }
            }

            public void DrawNewLineBetweenStartAndEndPoint()
            {
                line = new GameObject("Line");
                line.tag = "MovingObjectLine";
                line.AddComponent<MovingObjectLine>();
                line.AddComponent<ObjectSelection>();
                LineRenderer lr = line.AddComponent<LineRenderer>();
                lr.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
                lr.startColor = Color.red;
                lr.endColor = Color.red;
                lr.startWidth = 0.1f;
                lr.endWidth = 0.1f;
                lr.positionCount = 2;
                lr.useWorldSpace = false;

                line.transform.SetParent(null, true);
                lr.SetPosition(0, new Vector3(startPoint.x, startPoint.y, 0));
                lr.SetPosition(1, new Vector3(endPoint.x, endPoint.y, 0));
                line.transform.SetParent(transform, true);
                line.AddComponent<MeshCollider>();
            }

            /*public void RemoveChildObjectsToSave()
            {
                foreach (Transform child in transform)
                {
                    if (child.gameObject.GetComponent<MovingObjectLimit>() != null)
                    {
                        limits.Remove(child.gameObject);
                    }
                }
                DeleteRecursive();
            }*/

            // object movement behaviour:

            public void UpdateBothEndPointPositions()
            {
                startPoint = new Vector2(limits[0].transform.position.x, limits[0].transform.position.y);
                endPoint = new Vector2(limits[1].transform.position.x, limits[1].transform.position.y);
            }

            public void RedrawLine()
            {
                line.transform.position = transform.position;
                LineRenderer lr = line.GetComponent<LineRenderer>();
                if (lr.material != Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"))
                {
                    lr.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
                }
                lr.useWorldSpace = true;
                lr.positionCount = 0;
                lr.positionCount = 2;
                lr.SetPosition(0, new Vector3(startPoint.x, startPoint.y, 0));
                lr.SetPosition(1, new Vector3(endPoint.x, endPoint.y, 0));
            }

            public void UpdateEndPointCoordinatePosition(ObjectType type, Vector2 position)
            {
                switch (type)
                {
                    case ObjectType.STARTLIMIT:
                        startPoint = position;
                        break;
                    case ObjectType.ENDLIMIT:
                        endPoint = position;
                        break;
                }
            }

            public void MoveToCenterOfEndPoints()
            {
                Vector2 startPoint = GetStartPointPos();
                Vector2 endPoint = GetEndPointPos();

                Vector2 newPositionV2 = FindCenterOfVector(startPoint, endPoint);
                Vector3 newPositionV3 = new Vector3(newPositionV2.x, newPositionV2.y, 0);
                transform.position = newPositionV3;
            }


            // helper functions:
            Vector3 RoundVector(Vector3 v)
            {
                float oldx = v.x;
                float oldy = v.y;
                float oldz = v.z;

                float mult = Mathf.Pow(10.0f, 2.0f);
                float newx = Mathf.Round(oldx*mult)/mult;
                float newy = Mathf.Round(oldy*mult)/mult;
                float newz = Mathf.Round(oldz*mult)/mult;

                Vector3 newV = new Vector3(newx, newy, newz);
                return newV;
            }

            private Vector2 FindCenterOfVector(Vector2 v1, Vector2 v2) => (v1 + v2) / 2;

            void RemoveEvents()
            {
                //saveLoadEvents.OnSaveLevel -= RemoveChildObjectsToSave;
                saveLoadEventHandler.OnExitSave -= Setup;
            }

            void PrepareEvents()
            {
                //saveLoadEventHandler.OnSaveLevel += RemoveChildObjectsToSave;
                saveLoadEventHandler.OnExitSave += Setup;
                saveLoadEventHandler.OnNewLevel += RemoveEvents;
                saveLoadEventHandler.OnLoadLevel += RemoveEvents;
            }

            void DeleteRecursive()
            {
                Transform[] children = new Transform[3];
                int childCount = 0;
                foreach (Transform child in transform)
                {
                    children[childCount++] = child;
                }

                for (int i = 0; i < children.Length; i++)
                {
                    children[i].gameObject.GetComponent<SupportedObject>().Delete();
                }
            }
        }
    }
}
