using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamControl : MonoBehaviour {
    private static CamControl instance;
    protected CamControl() { }
    public static CamControl Instance {
        get {
            if(instance == null) {
                instance = new CamControl();
            }
            return instance;
        }
    }
    void Awake() {
        if(instance != null && instance != this) {
            Destroy(this);
        }
        if (instance == null) {
            instance = this;
        }
    }

    public enum CamState {
        TopDown = 0,
        StaticShot = 1,
        Follow = 2,
        FollowRail = 3
    };
    public CamState m_CamState = CamState.TopDown;
    private Vector3 m_DefualtOffset = new Vector3(0, 7, -10);

    private float m_CamSpeed = 2.0f;
    private Vector3 m_Offset = Vector3.zero;
    private Transform m_StaticShot;
    private float m_StaticDrift = 0.0f;

    private bool b_Shaking = false;
    private float m_ShakeTimer = 0.0f;
    private float m_ShakeMaxMagnitude = 1.0f;
    private float m_ShakeInterval = 0.0f;

    // Use this for initialization
    void Start () {
        m_Offset = m_DefualtOffset;
    }
	// Update is called once per frame
	void Update () {
        if (b_Shaking) {
            m_ShakeTimer += Time.deltaTime;
            if (m_ShakeTimer >= m_ShakeInterval) {
                m_ShakeTimer -= m_ShakeInterval;

                this.transform.position += Random.onUnitSphere * m_ShakeMaxMagnitude;
            }
        }

        switch (m_CamState) {
            case CamState.TopDown:
                TopDown();
                break;
            case CamState.StaticShot:
                StaticShot();
                break;
            case CamState.Follow:
                break;
            case CamState.FollowRail:
                break;
        }
	}

    void TopDown() {
        Vector3 targetPos = Player.Instance.transform.position + m_Offset;
        this.transform.position += (targetPos - this.transform.position) * m_CamSpeed * Time.deltaTime;
    }
    void StaticShot() {
        Vector3 targetLookRotation = Player.Instance.transform.position - this.transform.position;
        targetLookRotation.Normalize();
        this.transform.forward = Vector3.Slerp(m_StaticShot.forward, targetLookRotation, m_StaticDrift);
    }

    public void SetCamTopDown(Vector3 offset) {
        m_CamState = CamState.TopDown;
        if(offset != Vector3.zero) {
            m_Offset = offset;
        } else {
            m_Offset = m_DefualtOffset;
        }
    }
    public void SetCamStaticShot (Transform camPosAndRot, float drift) {
        m_CamState = CamState.StaticShot;
        m_StaticDrift = drift;
        m_StaticShot = camPosAndRot;
        this.transform.position = m_StaticShot.position;
        this.transform.rotation = m_StaticShot.rotation;
    }
    public void SetCamFollow() {

    }
    public void SetCamRail() {

    }

    public void StartShake(float shakesPerSecond, float magnitude) {
        b_Shaking = true;
        m_ShakeTimer = 0.0f;

        m_ShakeInterval = 1.0f / shakesPerSecond;
        m_ShakeMaxMagnitude = magnitude;
    }
    public void StopShake() {
        b_Shaking = false;
    }
}