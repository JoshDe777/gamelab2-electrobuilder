using UnityEngine;

public class TutorialUIHandler : MonoBehaviour
{
    private void Awake() => Hide();

    public void Hide() => gameObject.SetActive(false);

    public void ShowHideSwitch() => gameObject.SetActive(!gameObject.activeSelf);
}