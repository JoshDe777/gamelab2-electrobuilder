using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ElectroBuilder.Enums;


namespace ElectroBuilder
{
    namespace SupportedObjects
    {
        public class MovingObjectLimit : SupportedObject
        {
            ObjectType objectType;

            [SerializeField]
            MovingObject parentMovingObject;

            // inherited methods:
            public override ObjectType GetObjectType()
            {
                return objectType;
            }

            public void SetLimitType(ObjectType type)
            {
                objectType = type;
            }

            protected override void Start()
            {
                base.Start();
                parentMovingObject = transform.parent.GetComponent<MovingObject>();
            }

            // public methods
            public MovingObject GetParentMovingObject() => parentMovingObject;
        }
    }
}
