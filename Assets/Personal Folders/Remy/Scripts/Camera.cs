using UnityEngine;
using System.Collections;

public class Camera : MonoBehaviour {

    private const float c_CameraSpeed = 2.0f;

    public Vector3 m_Offset = Vector3.zero;

    private const float c_MaxZoomOut = 0.25f;
    private const float c_VelAtMaxZoom = 10.0f;

    private float m_CurrentZoom = 1.0f;
    private float m_TargetZoom = 1.0f;
    private const float m_ZoomRate = 1.0f;

    private bool b_Shaking = false;
    private float m_ShakeIntensity = 1.0f;
    private float m_ShakeTimer = 0.0f;
    private float m_ShakeMaxMagnitude = 1.0f;
    private float m_ShakeInterval = 0.0f;

	// Use this for initialization
	void Start () {

        m_Offset = -(Player.Instance.transform.position - this.transform.position);

	}
	
	// Update is called once per frame
	void Update () {

        m_TargetZoom = 1.0f + (Mathf.Clamp01(Player.Instance.Velocity.magnitude / c_VelAtMaxZoom) * c_MaxZoomOut);

        m_CurrentZoom += (m_TargetZoom - m_CurrentZoom) * Time.deltaTime * m_ZoomRate;


        Vector3 targetPos = Player.Instance.transform.position + m_Offset * m_CurrentZoom;

        if (b_Shaking)
        {
            m_ShakeTimer += Time.deltaTime;
            if (m_ShakeTimer >= m_ShakeInterval)
            {
                m_ShakeTimer -= m_ShakeInterval;

                this.transform.position += Random.onUnitSphere * m_ShakeMaxMagnitude;
            }
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            this.StartShake(10.0f, 0.1f);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            this.StopShake();
        }

        this.transform.position += (targetPos - this.transform.position) * c_CameraSpeed * Time.deltaTime;
	}

    public void StartShake(float shakesPerSecond, float magnitude)
    {
        b_Shaking = true;
        m_ShakeTimer = 0.0f;

        m_ShakeInterval = 1.0f / shakesPerSecond;
        m_ShakeMaxMagnitude = magnitude;
    }

    public void StopShake()
    {
        b_Shaking = false;
    }
}
