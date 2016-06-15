using UnityEngine;
using System.Collections;

public class GroundFX : MonoBehaviour {

    public enum EffectTypes
    {
        UNKNOWN,
        SPOT,
        GROWING_RING
    };

    protected EffectTypes m_Type = EffectTypes.UNKNOWN;
    protected float m_Time = 0;
    protected float m_LifeTime = 1.0f;
    protected bool b_Fade = true;

    protected GroundFX()
    {
        m_Time = 0.0f;
        m_LifeTime = 1.0f;
        b_Fade = true;
    }

    protected GroundFX(float life, bool fade)
    {
        m_Time = 0.0f;
        m_LifeTime = life;
        b_Fade = fade;
    }

	// Update is called once per frame
	public virtual void UpdateEffect (float deltaTime)
    {
        m_Time += deltaTime;
	}
}
