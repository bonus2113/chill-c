using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public class Player : MonoBehaviour {

    private const float c_Accel = 100.0f;
    private const float c_Friction = 10.0f;

    private Vector3 m_Vel = Vector3.zero;

    [Header("Animation")]
    public List<Mesh> m_RunFrames = new List<Mesh>();
    public Mesh m_StandMesh = null;

    private MeshFilter m_MeshFilter = null;

    private static Player m_Instance = null;
    public static Player Instance
    {
        get
        {
            return m_Instance;
        }
    }

    public Vector3 Velocity
    {
        get
        {
            return m_Vel;
        }
    }

    public float VelocityNormalised
    {
        get
        {
            return m_Vel.magnitude/(c_Accel/c_Friction);
        }
    }

    private float m_AnimTimer = 0.0f;
    private float m_FramesPerSecond = 12.0f;

    private float m_FrameTime = 0.0f;
    private int m_CurrentFrame = 0;

    private bool b_Running = false;
    public bool isRunning
    {
        get
        {
            return b_Running;
        }
    }

    [Header("RunParticles")]
    public GameObject m_RunParticlePrefab = null;

    private const int c_NumRunParticles = 128;
    private List<GameObject> m_ParticlePool = new List<GameObject>();
    private Transform m_ParticleRoot = null;
    private float m_ParticleTimer = 0.0f;
    private float m_ParticleSpawnInterval = 1.0f / 20.0f;
    private int m_CurrentParticleID = 0;

    private Rigidbody m_RB = null;

    private Vector3 m_LastPos = Vector3.zero;
    private bool b_Colliding = false;

    private float m_FootstepTimer = 0.0f;
    private const float c_FootstepTime = 0.25f;
    private bool b_LeftFoot = false;

    void Awake()
    {
        if (m_Instance != null)
        {
            Debug.LogError("Player already exists.");
        }

        m_Instance = this;

        this.m_MeshFilter = this.GetComponent<MeshFilter>();

        m_FrameTime = 1.0f / m_FramesPerSecond;

        //init particlePool
        m_ParticleRoot = new GameObject().transform;

        for (int i = 0; i < c_NumRunParticles; i++)
        {
            var go = GameObject.Instantiate(m_RunParticlePrefab);
            go.transform.SetParent(m_ParticleRoot);
            go.SetActive(false);
            m_ParticlePool.Add(go);
        }

        this.m_RB = this.GetComponent<Rigidbody>();
    }

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

        //m_Vel = m_RB.velocity;

        //Movement
        float right = (Input.GetKey(KeyCode.A) ? -1.0f : 0.0f) + (Input.GetKey(KeyCode.D) ? 1.0f : 0.0f);
        float forward = (Input.GetKey(KeyCode.S) ? -1.0f : 0.0f) + (Input.GetKey(KeyCode.W) ? 1.0f : 0.0f);

        Vector3 inputDir = new Vector3(right, 0.0f, forward).normalized;

        if (inputDir != Vector3.zero)
        {
            inputDir.Normalize();
        }

        m_Vel -= c_Friction * m_Vel * Time.deltaTime;

        m_Vel += inputDir * c_Accel * Time.deltaTime;

        //m_RB.velocity = m_Vel;

        if (m_Vel.magnitude > 1.0f)
        {
            b_Running = true;
            m_FootstepTimer += Time.deltaTime;

            this.transform.rotation = Quaternion.LookRotation(Quaternion.Euler(Vector3.up * -90.0f)*m_Vel);
        }
        else
        {
            b_Running = false;
            m_FootstepTimer = 0.0f;
            m_MeshFilter.mesh = m_StandMesh;
        }

        //animation

        if (b_Running)
        {
            //anim
            m_AnimTimer += Time.deltaTime;

            if (m_AnimTimer >= m_FrameTime)
            {
                m_AnimTimer -= m_FrameTime;
                m_CurrentFrame++;
                m_CurrentFrame %= m_RunFrames.Count;

                m_MeshFilter.mesh = m_RunFrames[m_CurrentFrame];
            }
            //particle

            m_ParticleTimer += Time.deltaTime;
            if (m_ParticleTimer >= m_ParticleSpawnInterval)
            {
                m_ParticlePool[m_CurrentParticleID].GetComponent<RunParticle>().Activate(m_Vel);
                m_CurrentParticleID++;
                m_CurrentParticleID %= c_NumRunParticles;
                m_ParticleTimer -= m_ParticleSpawnInterval;
            }

            if (GroundShadingManager.Instance != null)
            {
                if (m_FootstepTimer >= c_FootstepTime)
                {
                    m_FootstepTimer -= c_FootstepTime;

                    GameObject go = new GameObject();
                    go.transform.position = Player.Instance.transform.position + this.transform.right + ((b_LeftFoot)?this.transform.forward: -this.transform.forward) * 0.3f;
                    GroundShadingManager.AddEffect(ScalingRing.CreateComponent(go, 1.5f, true, 0.2f, Random.Range(1.1f, 1.55f)));

                    if (Input.GetKey(KeyCode.Space))
                    {

                        go = new GameObject();
                        go.transform.position = Player.Instance.transform.position;

                        GroundShadingManager.AddEffect(ScalingRing.CreateComponent(go, 1.0f + Random.Range(-0.3f, 0.3f), false, 0.1f, Mathf.Lerp(0.1f, 2.5f, this.VelocityNormalised)));
                    }
                    b_LeftFoot = !b_LeftFoot;
                }
            }
        }

        m_RB.velocity = Vector3.zero;
        m_RB.angularVelocity = Vector3.zero;
    }

    void FixedUpdate()
    {
        if (!b_Colliding)
        {
            this.m_LastPos = this.transform.position;
            //this.transform.position += m_Vel * Time.deltaTime;
            this.m_RB.MovePosition(this.transform.position + m_Vel * Time.fixedDeltaTime);
        }
    }

    void OnCollisionExit(Collision colInfo)
    {
        Debug.Log("Not COlliding");

        b_Colliding = false;
    }

    void OnCollisionEnter(Collision colInfo)
    {
        b_Colliding = true;
        Debug.Log("COlliding: " + colInfo.gameObject.name);
        Vector3 colNormal = colInfo.contacts[0].normal * Time.deltaTime;
        colNormal.y = 0.0f;
        //m_LastPos += -colInfo.relativeVelocity * Time.maximumDeltaTime;
        this.transform.position = m_LastPos + colNormal;
        m_Vel = Vector3.zero;

        //Vector3.Reflect(m_Vel, colInfo.contacts[0].normal);
    }
}
