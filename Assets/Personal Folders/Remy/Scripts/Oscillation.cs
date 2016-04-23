using UnityEngine;
using System.Collections;

public class Oscillation : MonoBehaviour {

    private float m_Rate = 1.0f;

    public float m_Magnitude = 1.0f;

    public Vector3 m_Axis = Vector3.up;

    private float m_Timer = 0.0f;

    private float m_CurrentOffset = 0.0f;

	// Use this for initialization
	void Start () {

        m_Timer = Random.Range(0.0f, Mathf.PI * 2.0f);
        m_Rate = Random.Range(0.5f, 1.0f);

	}
	
	// Update is called once per frame
	void LateUpdate () {

        this.transform.position -= m_Axis * m_CurrentOffset;

        m_Timer += Time.deltaTime * m_Rate * Mathf.PI * 2.0f;

        if (m_Timer >= Mathf.PI * 2.0f)
        {
            m_Timer -= Mathf.PI * 2.0f;
        }

        m_CurrentOffset = Mathf.Sin(m_Timer) * m_Magnitude;

        this.transform.position += m_Axis * m_CurrentOffset;

	}
}
