using ElectroBuilder.SaveLoadSystem;
using ElectroBuilder.VirtualCamera;
using System;
using UnityEngine;

namespace ElectroBuilder
{
    namespace Testmode
    {
        public class TestModeEventHandler : MonoBehaviour
        {
            public event Action OnTestmodeSwitch;

            public bool TestmodeToggle { get; private set; }

            public void OnTestModeButtonPressBoolSwitch()
            {
                TestmodeToggle = !TestmodeToggle;
                OnTestmodeSwitch?.Invoke();
            }

            private void ClearTestModeEvent()
            {
                OnTestmodeSwitch = null;
                VirtualCameraManager vcm = FindObjectOfType<VirtualCameraManager>();
                OnTestmodeSwitch += vcm.changeTestModeBool;
            }

            private void Start()
            {
                SaveLoadEventHandler saveLoadEventHandler =  FindObjectOfType<SaveLoadEventHandler>();
                saveLoadEventHandler.OnNewLevel += ClearTestModeEvent;
                saveLoadEventHandler.OnLoadLevel += ClearTestModeEvent;
            }
        }
    }
}
