using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


namespace ElectroBuilder
{
    namespace ObjectActions
    {
        public abstract class ObjectMovement : MonoBehaviour
        {
            private bool mayMove = false;

            // getters & setters
            public bool GetMovePermission() => mayMove;

            public void SetMovePermission(bool decision) => mayMove = decision;


            // abstract functions:
            public abstract void StartMoveObjectMode();

            public abstract void CloseMoveObjectMode(bool saveChanges);

            protected abstract void OnMouseDown();

            protected abstract void OnMouseDrag();
        }
    }
}
