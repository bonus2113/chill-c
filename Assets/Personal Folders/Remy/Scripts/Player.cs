using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    private const float c_Accel = 100.0f;
    private const float c_Friction = 10.0f;

    private Vector3 m_Vel = Vector3.zero;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        float right = (Input.GetKey(KeyCode.A) ? -1.0f : 0.0f) + (Input.GetKey(KeyCode.D) ? 1.0f : 0.0f);
        float forward = (Input.GetKey(KeyCode.S) ? -1.0f : 0.0f) + (Input.GetKey(KeyCode.W) ? 1.0f : 0.0f);

        Vector3 inputDir = new Vector3(right, 0.0f, forward).normalized;

        if (inputDir != Vector3.zero)
        {
            inputDir.Normalize();
        }

        m_Vel -= c_Friction * m_Vel * Time.deltaTime;

        m_Vel += inputDir * c_Accel * Time.deltaTime;

        Debug.Log("InputDir: " + inputDir);
        Debug.Log("Vel: " + m_Vel);

        this.transform.position += m_Vel * Time.deltaTime;

    }
}
