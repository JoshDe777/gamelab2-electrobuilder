using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ElectroBuilder
{
    namespace SaveLoadSystem
    {
        public class ReadSelectedLevel : MonoBehaviour, IPointerClickHandler
        {
            public event Action OnSelectionChanged;

            //External Classes are nt allowed to change this value
            public string selcetedLevelName { get; private set; }

            //On klick Event in the Load-UI Scroll-View-Window
            public void OnPointerClick(PointerEventData eventData)
            {
                selcetedLevelName = eventData.pointerEnter.name;
                OnSelectionChanged.Invoke();
            }

            public void ResetSelectedLevelName()
            {
                selcetedLevelName = null;
                OnSelectionChanged.Invoke();
            }
        }
    }
}



