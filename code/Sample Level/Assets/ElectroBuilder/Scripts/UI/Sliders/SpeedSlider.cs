using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ElectroBuilder.ObjectActions;
using ElectroBuilder.SupportedObjects;

namespace ElectroBuilder
{
    namespace UI
    {
        public class SpeedSlider : MonoBehaviour
        {
            SelectionManager selectionManager;

            Slider slider;

            [SerializeField]
            TMP_Text sliderValueText;

            private void Start()
            {
                selectionManager = FindObjectOfType<SelectionManager>();
                slider = GetComponent<Slider>();
            }

            public void ChangeSpeed()
            {
                MovingObject movingObject = selectionManager.GetSelectedObject().GetComponent<MovingObject>();
                if (movingObject != null)
                {
                    movingObject.speed = slider.value;
                    sliderValueText.text = "x" + slider.value.ToString();
                }
            }

            public void AssignSliderValueOnSelection()
            {
                MovingObject movingObject = selectionManager?.GetSelectedObject().GetComponent<MovingObject>();
                if (movingObject != null)
                {
                    slider.value = movingObject.speed;
                    sliderValueText.text = "x" + slider.value.ToString();
                }
            }

            public void SetupSlider()
            {
                MovingObject movingObject = selectionManager.GetSelectedObject().GetComponent<MovingObject>();
                if (movingObject != null)
                {
                    slider.value = movingObject.speed;
                    sliderValueText.text = "x" + slider.value.ToString();
                }
            }
        }
    }
}

