using UnityEngine;
using System.Collections;

public class Camera : MonoBehaviour {

    private const float c_CameraSpeed = 2.0f;

    public GameObject m_PlayerObject;

    public Vector3 m_Offset = Vector3.zero;

	// Use this for initialization
	void Start () {

        m_Offset += this.transform.position;

	}
	
	// Update is called once per frame
	void Update () {
        Vector3 targetPos = m_PlayerObject.transform.position + m_Offset;

        this.transform.position += (targetPos - this.transform.position) * c_CameraSpeed * Time.deltaTime;


	}
}
