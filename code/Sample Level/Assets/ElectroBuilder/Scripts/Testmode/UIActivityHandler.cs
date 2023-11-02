using UnityEngine;
using ElectroBuilder.Testmode;
using ElectroBuilder.SaveLoadSystem;

public class UIActivityHandler : MonoBehaviour
{
    private TestModeEventHandler testModeEventHandler;

    // Start is called before the first frame update
    void Start()
    {
        //Event when Testmode Switch Button is pressed
        testModeEventHandler = FindObjectOfType<TestModeEventHandler>();
        testModeEventHandler.OnTestmodeSwitch += ChangeTestModeBool;

        SaveLoadEventHandler saveLoadEventHandler = FindObjectOfType<SaveLoadEventHandler>();
        saveLoadEventHandler.OnNewLevel += AddToTestMode;
        saveLoadEventHandler.OnLoadLevel += AddToTestMode;

        if (gameObject.name.Equals("EditModeButton"))
        {
            gameObject.SetActive(false);
        }
    }

    private void ShowHideSwitch() => gameObject.SetActive(!gameObject.activeSelf);

    private void ChangeTestModeBool()
    {
        ShowHideSwitch();
    }

    void AddToTestMode()
    {
        testModeEventHandler.OnTestmodeSwitch += ChangeTestModeBool;
    }

}
