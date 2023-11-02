using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ElectroBuilder.Enums;


namespace ElectroBuilder
{
    namespace SupportedObjects
    {
        public class StaticObject : SupportedObject
        {
            Rigidbody rb;
            MeshCollider mc;

            // Start:
            protected override void Start()
            {
                base.Start();

                AddRigidBodyAndCollider();
            }


            // inherited methods:
            public override ObjectType GetObjectType()
            {
                return ObjectType.STATIC;
            }


            // runtime behaviour:
            void AddRigidBodyAndCollider()
            {
                if (GetComponent<Rigidbody>() == null)
                {
                    rb = gameObject.AddComponent(typeof(Rigidbody)) as Rigidbody;
                    rb.useGravity = false;
                    rb.isKinematic = true;
                }

                if (GetComponent<MeshCollider>() == null)
                {
                    mc = gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
                }
                
            }
        }
    }
}
