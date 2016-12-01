using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SporeTrigger : MonoBehaviour {

    [SerializeField]
    private ParticleSystem m_SporeParticles = null;

    void OnTriggerEnter(Collider col)
    {
        m_SporeParticles.Play();
    }
}
