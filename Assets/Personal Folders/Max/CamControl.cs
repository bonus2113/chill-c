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
    public AnimationCurve m_CamHeightCurve = new AnimationCurve(new Keyframe(0, 3), new Keyframe(5, 0));
    private Vector3 m_DefualtTopDownOffset = new Vector3(0, 7, -10);
    private Quaternion m_DefualtTopDownRotation = new Quaternion(0.3f,0f,0f,1f);

    private const float c_CamSpeed = 2.0f;
    private Vector3 m_Offset = Vector3.zero;
    private Transform m_StaticShot;
    private float m_CameraDrift = 0.0f;

    private List<Vector3> m_FollowPoints = new List<Vector3>();
    private bool b_TrackPlayer = false;

    private bool b_Shaking = false;
    private float m_ShakeTimer = 0.0f;
    private float m_ShakeMaxMagnitude = 1.0f;
    private float m_ShakeInterval = 0.0f;

    // Use this for initialization
    void Start () {
        m_Offset = m_DefualtTopDownOffset;
        transform.rotation = m_DefualtTopDownRotation;
    }
    // Update is called once per frame
    void Update() {
        if (b_Shaking) {
            m_ShakeTimer += Time.deltaTime;
            if (m_ShakeTimer >= m_ShakeInterval) {
                m_ShakeTimer -= m_ShakeInterval;

                this.transform.position += Random.onUnitSphere * m_ShakeMaxMagnitude;
            }
        }
        Vector3 CamTargetPos = Vector3.zero;
        Quaternion CamTargetRot = Quaternion.identity;
        switch (m_CamState) {
            case CamState.TopDown:
                CamTargetPos = TopDown();
                CamTargetRot = m_DefualtTopDownRotation;
                break;
            case CamState.StaticShot:
                StaticShot();
                break;
            case CamState.Follow:
                CamTargetPos = FollowPlayer();
                CamTargetRot = LookAtPlayer();
                break;
            case CamState.FollowRail:
                CamTargetPos = FollowRail();
                if (b_TrackPlayer)
                    CamTargetRot = LookAtPlayer();
                break;
        }
        if (CamTargetPos != Vector3.zero)
            this.transform.position += (CamTargetPos - this.transform.position) * c_CamSpeed * Time.deltaTime;
        if(CamTargetRot != Quaternion.identity)
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, CamTargetRot, (0.3f * Mathf.PI * 2.0f) * Time.deltaTime);
    }

    Vector3 TopDown() {
        Vector3 targetPos = Player.Instance.transform.position + m_Offset;
        return targetPos;
    }
    void StaticShot() {
        Vector3 targetLookRotation = Player.Instance.transform.position - this.transform.position;
        targetLookRotation.Normalize();
        this.transform.forward = Vector3.Slerp(m_StaticShot.forward, targetLookRotation, m_CameraDrift);
    }
    Vector3 FollowPlayer() {
        Vector3 playerPos = Player.Instance.transform.position;
        if (Vector3.Distance(m_FollowPoints[0], playerPos) > 1) {
            m_FollowPoints.Insert(0, playerPos);
            if(m_FollowPoints.Count > 7) {
                m_FollowPoints.RemoveAt(7);
            }
        }
        if (m_FollowPoints.Count > 2) {
            Vector3 targetPos = m_FollowPoints[m_FollowPoints.Count - 1] + m_Offset;
            playerPos.y = transform.position.y;
            targetPos.y += m_CamHeightCurve.Evaluate(Vector3.Distance(transform.position, playerPos));
            return targetPos;
        } else {
            return Vector3.zero;
        }
    }
    Vector3 FollowRail() {
        Vector3[] temp = new Vector3[m_FollowPoints.Count - 1];
        Vector3 p = Player.Instance.transform.position + m_Offset;
        for (int i = 0; i < temp.Length; i++) {
            Vector3 a = m_FollowPoints[i];
            Vector3 b = m_FollowPoints[i + 1];
            temp[i] = a + (Mathf.Clamp(Vector3.Dot((p-a),(b-a).normalized),0,Vector3.Magnitude(b-a)) * (b-a).normalized);
        }
        float min = Mathf.Infinity;
        Vector3 targetPos = Vector3.zero;
        for(int i = 0; i < temp.Length; i++) {
            if (Vector3.Distance(temp[i], p) < min) {
                min = Vector3.Distance(temp[i], p);
                targetPos = temp[i];
            }
        }
        return targetPos;
    }
    Quaternion LookAtPlayer() {
        Vector3 dirToPlayer = (Player.Instance.transform.position - this.transform.position).normalized;
        Quaternion targetRot = Quaternion.LookRotation(dirToPlayer);
        return targetRot;
    }

    public void SetCamTopDown(Vector3 offset) {
        print("switch to: <color=yellow>Top Down</color>");
        m_CamState = CamState.TopDown;
        if(offset != Vector3.zero) {
            m_Offset = offset;
        } else {
            m_Offset = m_DefualtTopDownOffset;
        }
        transform.rotation = m_DefualtTopDownRotation;
    }
    public void SetCamStaticShot (Transform camPosAndRot, float drift) {
        print("switch to: <color=yellow>Static Shot</color>");
        m_CamState = CamState.StaticShot;
        m_CameraDrift = drift;
        m_StaticShot = camPosAndRot;
        this.transform.position = m_StaticShot.position;
        this.transform.rotation = m_StaticShot.rotation;
    }
    public void SetCamFollow(Vector3 offset) {
        print("switch to: <color=yellow>Follow Player</color>");
        m_CamState = CamState.Follow;
        m_Offset = offset;
        m_FollowPoints.Insert(0, Player.Instance.transform.position);
    }
    public void SetCamRail(CameraRail rail, bool trackPlayer) {
        print("switch to: <color=yellow>Follow Rail</color>");
        m_CamState = CamState.FollowRail;
        for (int i = 0; i < rail.m_PointsOnRail.Length; i++) {
            m_FollowPoints.Insert(i, rail.m_PointsOnRail[i]);
        }
        b_TrackPlayer = trackPlayer;
        print("Tracking on rail: "+b_TrackPlayer);
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