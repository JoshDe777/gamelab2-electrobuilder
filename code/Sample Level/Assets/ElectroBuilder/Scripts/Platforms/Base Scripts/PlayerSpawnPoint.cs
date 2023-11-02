using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;
using ElectroBuilder.Enums;
using ElectroBuilder.Testmode;
using ElectroBuilder.VirtualCamera;

namespace ElectroBuilder
{
    namespace SupportedObjects
    {
        public class PlayerSpawnPoint : SupportedObject
        {
            //Loading in Character from Resource Folder
            private GameObject PlayerPrefab;

            private GameObject SpawnedPlayer;

            private VirtualCameraManager vcm;

            MeshCollider collider;

            // Unity functions:
            protected override void Start()
            {
                base.Start();

                //Searching for Player in Resource Folder
                PlayerPrefab = Resources.Load<GameObject>("Prefabs/Character/Character");

                // camera reference
                vcm = FindObjectOfType<VirtualCameraManager>();

                collider = GetComponent<MeshCollider>();
            }

            // inherited functions:
            public override ObjectType GetObjectType()
            {
                return ObjectType.START;
            }

            protected override void ChangeTestModeBool()
            {
                collider.enabled = !collider.enabled;
                if (testModeEventHandler.TestmodeToggle)
                {
                    SpawnedPlayer = Instantiate(PlayerPrefab, transform);
                    //SpawnedPlayer.transform.parent = GameObject.FindGameObjectWithTag("LevelLayoutObject").transform;
                    vcm.SetTargetObject(SpawnedPlayer);
                }
                else
                {
                    Destroy(SpawnedPlayer);
                    SpawnedPlayer = null;
                }
               
            }
        }
    }
}
