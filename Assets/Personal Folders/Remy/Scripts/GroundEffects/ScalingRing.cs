using UnityEngine;
using System.Collections;

public class ScalingRing : GroundFX
{
    protected float m_StartScale = 1.0f;
    protected float m_EndScale = 1.0f;

    public static ScalingRing CreateComponent(GameObject where, float life, bool fade, float startScale, float endScale)
    {
        ScalingRing myC = where.AddComponent<ScalingRing>();

        myC.m_LifeTime = life;
        myC.b_Fade = fade;
        myC.m_StartScale = startScale;
        myC.m_EndScale = endScale;

        return myC;
    }

    // Use this for initialization
    void Start()
    {
        m_Type = EffectTypes.SPOT;
        Destroy(this.gameObject, m_LifeTime);
    }
    // Update is called once per frame
    public override void UpdateEffect(float deltaTime)
    {
        base.UpdateEffect(deltaTime);

        float currentScale = m_StartScale + (m_EndScale - m_StartScale) * (m_Time / m_LifeTime);
        GroundShadingManager.Instance.BlitEffectAtPosition(EffectTypes.GROWING_RING, GroundShadingManager.WorldToUVSpace(this.transform.position), currentScale, b_Fade);

    }

    public ScalingRing(float life, bool fade) : base(life, fade)
    {
    }
}
