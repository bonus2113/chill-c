using UnityEngine;
using System.Collections;

public class RisePlayerNear : ReactOnPlayerNear {

    public float m_RiseMagnitude = 1.0f;

    private Vector3 m_StartPos = Vector3.zero;

    private float m_TargetOffset = 0.0f;
    private float m_CurrentOffset = 0.0f;

    public float m_NearLerpRate = 1.0f;
    public float m_FarLerpRate = 5.0f;

    private bool b_TriggerClickBehaviour = false;
    private bool b_ClickBehaviourStarted = false;
    private float m_ClickTimer = 0.0f;
    private float m_ClickStartTime = 0.0f;
    private const float c_DistanceTimeFactor = 0.09f;

    private const float c_MaximumClickRange = 5.0f;

    // Use this for initialization
    void Start () {

        m_StartPos = this.transform.position;
        Events.Instance.AddListener<WormEvents.WormClicked>(OnWormClicked);
	}
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();

        if (!b_TriggerClickBehaviour)
        {
            if (b_PlayerNear)
            {
                m_CurrentOffset += (m_TargetOffset - m_CurrentOffset) * m_NearLerpRate * Time.deltaTime;
            }
            else
            {
                m_CurrentOffset += (m_TargetOffset - m_CurrentOffset) * m_FarLerpRate * Time.deltaTime;
            }
        }
        else
        {
            if (!b_ClickBehaviourStarted)
            {
                m_ClickTimer += Time.deltaTime;

                if (m_ClickTimer >= m_ClickStartTime)
                {
                    b_ClickBehaviourStarted = true;
                    SoundManager.Instance.PlaySound(SoundManager.Sounds.PLOP, 0.2f);
                }
            }
            else
            {
                float dif = (m_TargetOffset - m_CurrentOffset);
                m_CurrentOffset += dif * m_NearLerpRate * Time.deltaTime;

                if (Mathf.Abs(dif) < 0.01f)
                {
                    b_TriggerClickBehaviour = false;
                    m_TargetOffset = (b_PlayerNear)?m_RiseMagnitude:0.0f;
                    SoundManager.Instance.PlaySound(SoundManager.Sounds.PLOP, 0.2f);
                }
            }
        }

        this.transform.position = m_StartPos + new Vector3(0.0f, m_CurrentOffset, 0.0f);
	}

    protected override void OnPlayerNear()
    {
        base.OnPlayerNear();

        m_TargetOffset = m_RiseMagnitude;
    }

    protected override void OnPlayerFar()
    {
        base.OnPlayerFar();

        m_TargetOffset = 0.0f;

        SoundManager.Instance.PlaySound(SoundManager.Sounds.PLOP, 0.2f);
    }

    protected void OnWormClicked(WormEvents.WormClicked e)
    {
        float posMag = (e.pos - this.transform.position).magnitude;

        if (posMag >= c_MaximumClickRange)
        {
            return;
        }

        m_TargetOffset = m_RiseMagnitude;
        b_TriggerClickBehaviour = true;
        m_ClickTimer = 0.0f;
        m_ClickStartTime = posMag * c_DistanceTimeFactor;
        b_ClickBehaviourStarted = false;
    }

    protected void OnMouseDown()
    {
        Events.Instance.Raise(new WormEvents.WormClicked(this.gameObject, this.gameObject.transform.position));  
    }
}
