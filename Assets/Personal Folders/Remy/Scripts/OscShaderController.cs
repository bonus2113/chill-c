using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OscShaderController : MonoBehaviour {

    [SerializeField]
    private float m_RateMin = 0.0f;
    [SerializeField]
    private float m_RateMax = 1.0f;

    [SerializeField]
    private float m_OscMagnitude = 0.2f;

    void Awake()
    {
        Material mat = this.GetComponent<Renderer>().material;
        if (mat == null)
        {
            Destroy(this);
            return;
        }

        mat.SetFloat("_OscOffset", Random.Range(0.0f, Mathf.PI * 2.0f));
        mat.SetFloat("_OscMagnitude", m_OscMagnitude);
        mat.SetFloat("_OscRate", Random.Range(m_RateMin, m_RateMax));
    }
}
