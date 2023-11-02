using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerScript : MonoBehaviour
{

    private AudioSource buttonClick;

    private void Start()
    {
        buttonClick = GetComponent<AudioSource>();
    }

    public void playButtonSound()
    {
        buttonClick.Play();
    }
}
