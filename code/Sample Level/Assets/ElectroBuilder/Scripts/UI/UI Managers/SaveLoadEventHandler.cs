using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ElectroBuilder
{
    namespace SaveLoadSystem
    {
        public class SaveLoadEventHandler : MonoBehaviour
        {
            public event Action OnSaveLevel;
            public event Action OnLoadLevel;
            public event Action OnNewLevel;
            public event Action OnExitSave;

            public void InvokeSaveLevelEvent()
            {
                OnSaveLevel?.Invoke();
            }

            public void InvokeLoadLevelEvent()
            {
                OnLoadLevel?.Invoke();
            }

            public void InvokeNewLevelEvent()
            {
                OnNewLevel?.Invoke();
            }

            public void InvokeExitSaveEvent()
            {
                OnExitSave?.Invoke();
            }
        }
    }
}
