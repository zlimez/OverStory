using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FamilyDisappear : MonoBehaviour
{
    public GameObject dissapearParticles;
    public GameObject dissapearAudio;
    void OnDisable()
    {
        dissapearParticles.SetActive(true);
        dissapearAudio.SetActive(true);
    }
}
