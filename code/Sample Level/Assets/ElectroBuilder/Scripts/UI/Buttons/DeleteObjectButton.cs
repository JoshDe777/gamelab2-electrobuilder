using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ElectroBuilder.SupportedObjects;
using ElectroBuilder.ObjectActions;


namespace ElectroBuilder
{
    namespace UI
    {
        public class DeleteObjectButton : MonoBehaviour
        {
            SelectionManager selectionManager;

            // Start is called before the first frame update
            void Start()
            {
                selectionManager = FindObjectOfType<SelectionManager>();
            }

            // plan for archive method to enable ctrl+z?
            public void DeleteObject()
            {
                GameObject obj = selectionManager.GetSelectedObject();
                if (WarnUserAndConfirmDeletion())
                {
                    obj.GetComponent<SupportedObject>().Delete();
                }
            }

            // message "are you sure about that?", if cancel then return false, else return true.
            bool WarnUserAndConfirmDeletion()
            {
                return true;
            }
        }
    }
}
