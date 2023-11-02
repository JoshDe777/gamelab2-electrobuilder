using System;
using UnityEngine;

namespace ElectroBuilder
{
    namespace SaveLoadSystem
    {
        //Generall UI functioanlity used by every SaveAndLoad UI Window 
        public class SaveLoadLevelUI : MonoBehaviour
        {
            private SaveLoadEventHandler saveLoadEvents;

            public bool saveWindowOpen { get; private set; }

            private void Awake()
            {
                saveLoadEvents = FindObjectOfType<SaveLoadEventHandler>();
                Hide();
            } 

            public void Hide()
            {
                if (name.Equals("UI_InputWindow_SaveLevel"))
                {
                    saveWindowOpen = false;
                    saveLoadEvents?.InvokeExitSaveEvent();
                }
                gameObject.SetActive(false);
            }

            public void Show() 
            {
                if (name.Equals("UI_InputWindow_SaveLevel"))
                {
                    saveWindowOpen = true;
                }
                gameObject.SetActive(true);

            } 

            public void ShowHideSwitch()
            {
                if (gameObject.activeSelf)
                {
                    Hide();
                }
                else
                {
                    Show();
                }
            }
            
        }
    }
}


