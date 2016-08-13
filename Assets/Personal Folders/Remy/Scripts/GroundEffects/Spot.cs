using UnityEngine;
using System.Collections;

public class Spot : GroundFX {

    public static Spot CreateComponent(GameObject where, float life, bool fade)
    {
        Spot myC = where.AddComponent<Spot>();

        myC.m_LifeTime = life;
        myC.b_Fade = fade;

        return myC;
    }

    // Use this for initialization
    void Start () {
        m_Type = EffectTypes.SPOT;
        GroundShadingManager.Instance.BlitEffectAtPosition(GroundFX.EffectTypes.SPOT, GroundShadingManager.WorldToUVSpace(this.transform.position), 2.0f, b_Fade);
        Destroy(this.gameObject, m_LifeTime);
	}
	// Update is called once per frame
	public override void UpdateEffect(float deltaTime) {
        base.UpdateEffect(deltaTime);
	}

    public Spot(float life, bool fade): base(life, fade)
    {
    }
}
