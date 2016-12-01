using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;

using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour {

    private const float c_Accel = 100.0f;
    private const float c_Friction = 10.0f;

    private Vector3 m_Vel = Vector3.zero;

    [Header("Animation")]
    public List<Mesh> m_RunFrames = new List<Mesh>();
    public Mesh m_StandMesh = null;

    private MeshFilter m_MeshFilter = null;

    [SerializeField]
    private Texture2D m_FloorTexture = null;

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

    private CharacterController m_CharacterController = null;

    private float m_FootstepTimer = 0.0f;
    private const float c_FootstepTime = 0.25f;

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
        m_ParticleRoot.name = "RunParticleRoot";

        for (int i = 0; i < c_NumRunParticles; i++)
        {
            var go = GameObject.Instantiate(m_RunParticlePrefab);
            go.transform.SetParent(m_ParticleRoot);
            go.SetActive(false);
            m_ParticlePool.Add(go);
        }

        this.m_CharacterController = this.GetComponent<CharacterController>();
    }

	// Use this for initialization
	void Start () {

        RxInputs.Instance.MoveInputs.Subscribe(rxinput => ProcessInput(rxinput));

	}

    private void ProcessInput(RxInputs.MovementInputs input)
    {
        //Movement
        Vector3 inputDir = new Vector3(input.Direction2D.x, 0.0f, input.Direction2D.y).normalized;

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

            this.transform.rotation = Quaternion.LookRotation(Quaternion.Euler(Vector3.up * -90.0f) * m_Vel);
        }
        else
        {
            b_Running = false;
            m_FootstepTimer = 0.0f;
            m_MeshFilter.mesh = m_StandMesh;
        }

        m_CharacterController.Move((m_Vel + Physics.gravity) * Time.fixedDeltaTime);

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

            //get color at position
            Color? floorColor = null;
            if (m_FloorTexture != null)
            {
                RaycastHit hitInfo;
                if (Physics.Raycast(this.transform.position + Vector3.up, Vector3.down, out hitInfo, 5.0f, (1 << LayerMask.NameToLayer("GroundRaycast"))))
                {
                    floorColor = m_FloorTexture.GetPixelBilinear(hitInfo.textureCoord.x, hitInfo.textureCoord.y);
                }
            }
                
            m_ParticleTimer += Time.deltaTime;
            if (m_ParticleTimer >= m_ParticleSpawnInterval)
            {
                m_ParticlePool[m_CurrentParticleID].GetComponent<RunParticle>().Activate(m_Vel, floorColor);
                m_CurrentParticleID++;
                m_CurrentParticleID %= c_NumRunParticles;
                m_ParticleTimer -= m_ParticleSpawnInterval;
            }
        }
    }
	
	// Update is called once per frame
	void Update () {


    }
}
 