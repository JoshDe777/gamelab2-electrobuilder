using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ElectroBuilder.Enums;


namespace ElectroBuilder
{
    namespace SupportedObjects
    {
        public class MovingObjectLine : SupportedObject
        {
            // inherited methods:
            public override ObjectType GetObjectType()
            {
                return ObjectType.LINE;
            }
        }
    }
}
