using UnityEngine;
using System.Collections;

public class BigAssDoor : MonoBehaviour {

    public float m_Duration = 5.0f;

    public Transform m_EndPosition;
    private Vector3 m_StartPos;

    private float m_Timer = 0.0f;

    private bool b_Triggered = false;

    private Vector3 m_Direction;

	// Use this for initialization
	void Start () {

        m_Direction = m_EndPosition.position - this.transform.position;
        //m_Direction.Normalize();

        m_StartPos = this.transform.position;
    }
	
	// Update is called once per frame
	void Update () {

        if (b_Triggered)
        {
            m_Timer += Time.deltaTime;

            this.transform.position = m_StartPos + m_Direction * (m_Timer / m_Duration);

            if (m_Timer >= m_Duration)
            {
                UnityEngine.Camera.main.GetComponent<Camera>().StopShake();
                Destroy(this);
            }
        }
	}

    void OnTriggerEnter(Collider col)
    {
        b_Triggered = true;
        UnityEngine.Camera.main.GetComponent<Camera>().StartShake(10.0f, 0.1f);
    }
}
