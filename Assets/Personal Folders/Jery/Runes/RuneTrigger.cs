using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneTrigger : MonoBehaviour {

    static int runesActive = 0; 

    public bool triggered;
    public RuneTrigger triggers;
    public GameObject triggerEffect;


	void Update () 
    {

         // update visuals
        if(!triggered && triggers != null)
        {
            GetComponent<MeshRenderer>().material.SetFloat("_Emission", 1f + Mathf.Abs(Mathf.Sin(Time.time * 2f)) * 0.7f);
        }
        else if(triggered)
        {
            GetComponent<MeshRenderer>().material.SetFloat("_Emission", 1.4f + Mathf.Abs(Mathf.Sin(Time.time)) * 0.3f);
        }
        else
        {
            GetComponent<MeshRenderer>().material.SetFloat("_Emission", 1.0f);
        }

        
        if (!triggered && Vector3.Distance(Player.Instance.transform.position, transform.position) < 1f )
        {
            Trigger();
        }
	}

    void Trigger()
    {
        runesActive++;
        triggers.triggered = triggered = true;

        if(runesActive == 2)
        {
            Player.Instance.b_CanOpenDoor = true;
        }

        if(triggerEffect != null)
        {
            var obj = (GameObject)GameObject.Instantiate(triggerEffect, transform);
            obj.transform.localPosition = Vector3.zero;
        }
    }
}
