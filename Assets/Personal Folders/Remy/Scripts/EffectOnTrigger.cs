using UnityEngine;
using System.Collections;

public class EffectOnTrigger : MonoBehaviour {


    void OnTriggerEnter(Collider col)
    {
        var go = new GameObject();
        go.transform.position = this.transform.position;
        GroundShadingManager.AddEffect(ScalingRing.CreateComponent(go, 0.5f, false, 0.1f, 3.0f));
        go = new GameObject();
        go.transform.position = this.transform.position;
        GroundShadingManager.AddEffect(ScalingRing.CreateComponent(go, 0.5f, true, 0.1f, 3.2f));
    }
}
