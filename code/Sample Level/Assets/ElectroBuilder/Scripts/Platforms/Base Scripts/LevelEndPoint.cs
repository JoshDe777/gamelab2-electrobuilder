using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ElectroBuilder.Enums;
using ElectroBuilder.Testmode;

namespace ElectroBuilder
{
    namespace SupportedObjects
    {
        public class LevelEndPoint : SupportedObject
        {

            //GameObject Varaible for Object found with Tag "Player" 
            private GameObject player;

            //Respawn
            private Transform respawn;


            // inherited functions:
            public override ObjectType GetObjectType()
            {
                return ObjectType.END;
            }

            protected override void Start()
            {
                base.Start();
            }

            protected override void RuntimeBehaviour()
            {
                // hide mesh if settings apply

                if (player == null)
                {
                    //Searching for GameObject with Tag Player
                    player = GameObject.FindWithTag("Player");
                }
                //Finding RespawnPoint - optimizable in case start point created after end point
                if (respawn == null)
                {
                    GameObject respawnObject = GameObject.FindGameObjectWithTag("Respawn");
                    if (respawnObject != null)
                    {
                        respawn = respawnObject.transform;
                    }
                }
            }


            // flag functions:
            private void OnTriggerEnter(Collider other)
            {
                if (testMode && other.gameObject.CompareTag("Player"))
                {
                    other.transform.position = respawn.position;
                }
            }
        }
    }
}
