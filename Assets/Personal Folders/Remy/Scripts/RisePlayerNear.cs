using UnityEngine;
using System.Collections;

public class RisePlayerNear : ReactOnPlayerNear {

    public float m_RiseMagnitude = 1.0f;

    private Vector3 m_StartPos = Vector3.zero;

    private float m_TargetOffset = 0.0f;
    private float m_CurrentOffset = 0.0f;

    public float m_NearLerpRate = 1.0f;
    public float m_FarLerpRate = 5.0f;

    // Use this for initialization
    void Start () {

        m_StartPos = this.transform.position;
	}
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();

        if (b_PlayerNear)
        {
            m_CurrentOffset += (m_TargetOffset - m_CurrentOffset) * m_NearLerpRate * Time.deltaTime;
        }
        else
        {
            m_CurrentOffset += (m_TargetOffset - m_CurrentOffset) * m_FarLerpRate * Time.deltaTime;

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
    }
}
