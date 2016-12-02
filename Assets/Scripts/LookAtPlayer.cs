using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : ReactOnPlayerNear
{
    private Quaternion m_TargetRotation = Quaternion.identity;
    [SerializeField]
    private ParticleSystem m_EmissionRateControl = null;

    [SerializeField]
    private float m_MaxEmissionRate = 30.0f;
    [SerializeField]
    private float m_LerpRate = 0.5f; //in 2Pi radians per second, maybe

    protected override void Start()
    {
        base.Start();

        m_TargetRotation = this.transform.rotation;
    }

	// Update is called once per frame
	public override void UpdateMe () {

        base.UpdateMe();

        if (b_PlayerNear)
        {
            Debug.Log("Player near: " + this.gameObject.name);
            //update target rot
            LookTowardsPlayer();

            if (m_EmissionRateControl != null)
            {
                //updateEmissionRate
                Vector3 targetNoY = m_TargetRotation * Vector3.forward;
                targetNoY.y = 0.0f;
                float dotResult = Mathf.Abs(Vector3.Dot(this.transform.forward, targetNoY));
                var emission = m_EmissionRateControl.emission;
                emission.rateOverTimeMultiplier = ((1.0f-dotResult) * m_MaxEmissionRate);
                Debug.Log("Dot rate: " + dotResult);
            }
        }
        else
        {
            if (m_EmissionRateControl != null)
            {
                var emission = m_EmissionRateControl.emission;
                emission.rateOverTimeMultiplier = 0.0f;
            }
        }
    }

    private void Update()
    {
        UpdateLookDir();
    }

    private void UpdateLookDir()
    {
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, m_TargetRotation, (m_LerpRate * Mathf.PI * 2.0f) * Time.deltaTime);
    }

    private void LookTowardsPlayer()
    {
        var playerTransform = Player.Instance.transform;

        Vector3 dirToPlayer = (playerTransform.position - this.transform.position).normalized;

        dirToPlayer.y = 0.0f; //horizontal only

        m_TargetRotation = Quaternion.LookRotation(dirToPlayer);
    }
}
