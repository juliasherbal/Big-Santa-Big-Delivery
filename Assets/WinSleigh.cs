using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinSleigh : MonoBehaviour
{
    LevelManager lm;
    void Start()
    {
        lm = GameObject.FindObjectOfType<LevelManager>();
    }


    private void OnTriggerEnter(Collider other)
    {
        lm.LevelBeat();
    }
}
