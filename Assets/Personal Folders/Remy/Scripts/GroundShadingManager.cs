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

        Vector2 playerPos = new Vector2(-Player.Instance.transform.position.x, -Player.Instance.transform.position.z);

        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("KEYDOWN");
            GameObject go = new GameObject();
            go.transform.position = Player.Instance.transform.position;
            AddEffect(Spot.CreateComponent(go, 1.0f, true));
            //this.BlitAtPosition(GroundShadingManager.WorldToUVSpace(go.transform.position), 1.0f, false);
        }

        //update effects
        foreach (var effect in m_Effects)
        {
            effect.UpdateEffect(Time.deltaTime);
        }

        foreach (var ctrl in m_GroundShaders)
        {
            //blit player
            ctrl.BlitAtPosition(playerPos, Mathf.Max(Player.Instance.VelocityNormalised + 0.5f, 0.75f), !Input.GetKey(KeyCode.Space));
            //ctrl.BlitAtPosition(playerPos, 0.25f, false);

            //blit effects
        }
    }

    public static void AddEffect(GroundFX effect)
    {
        m_Effects.Add(effect);
    }

    public void BlitAtPosition(Vector2 pos, float blobScale, bool fade)
    {
        foreach (var ctrl in m_GroundShaders)
        {
            //blit
            ctrl.BlitAtPosition(pos, blobScale, fade);
        }
    }
}
