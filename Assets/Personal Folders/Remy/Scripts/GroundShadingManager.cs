using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GroundShadingManager : MonoBehaviour {

    private static GroundShadingManager m_Instance = null;
    public static GroundShadingManager Instance
    {
        get
        {
            return m_Instance;
        }
    }

    public static void RegisterSelf(GroundShaderController ctrlr)
    {
        m_GroundShaders.Add(ctrlr);
    }

    private static List<GroundShaderController> m_GroundShaders = new List<GroundShaderController>();
    private static List<GroundFX> m_Effects = new List<GroundFX>();

    public static Vector2 WorldToUVSpace(Vector3 wordlPos)
    {
        return new Vector2(-wordlPos.x, -wordlPos.z);
    }

    public Texture2D m_RadialGraphic = null;
    public Texture2D m_RíngGraphic = null;

    void Awake()
    {
        if (m_Instance != null)
        {
            Debug.LogWarning("Multiple GroundShadingManager deleted.");
            Destroy(this);
        }

        m_Instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("KEYDOWN");
            GameObject go = new GameObject();
            go.transform.position = Player.Instance.transform.position;
            AddEffect(ScalingRing.CreateComponent(go, 1.0f, true, 0.0f, 1.0f));
            //this.BlitAtPosition(GroundShadingManager.WorldToUVSpace(go.transform.position), 1.0f, false);
        }

        //update effects
        foreach (var effect in m_Effects)
        {
            if (effect == null)
            {
                continue;
            }
            effect.UpdateEffect(Time.deltaTime);
        }

        //foreach (var ctrl in m_GroundShaders)
        //{
        //    //blit player
        //    ctrl.BlitGraphicToUVPosition(m_RadialGraphic, playerPos, Mathf.Max(Player.Instance.VelocityNormalised + 0.5f, 0.75f), !Input.GetKey(KeyCode.Space));
        //    //ctrl.BlitAtPosition(playerPos, 0.25f, false);

        //    //blit effects
        //}
    }

    public static void AddEffect(GroundFX effect)
    {
        m_Effects.Add(effect);
    }

    public Texture2D GetTextureForEffect(GroundFX.EffectTypes type)
    {
        switch (type)
        {
            case GroundFX.EffectTypes.SPOT:
                return m_RadialGraphic;
            default:
                return m_RadialGraphic;
        }
    }

    public void BlitEffectAtPosition(GroundFX.EffectTypes type, Vector2 pos, float blobScale, bool fade)
    {
        foreach (var ctrl in m_GroundShaders)
        {
            //blit
            switch (type)
            {
                case GroundFX.EffectTypes.SPOT:
                    ctrl.BlitAtPosition(m_RadialGraphic, pos, blobScale, fade);
                    break;
                case GroundFX.EffectTypes.GROWING_RING:
                    ctrl.BlitAtPosition(m_RíngGraphic, pos, blobScale, fade);
                    break;
                default:
                    ctrl.BlitAtPosition(m_RadialGraphic, pos, blobScale, fade);
                    break;
            }
        }
    }
}
