using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneTrigger : MonoBehaviour {

    static int runesActive = 0; 

    public bool triggered;
    public RuneTrigger triggers;
    public GameObject triggerEffect;

    public GameObject m_SecondaryEffect = null;
	void Update () 
    {

         // update visuals
        if(!triggered && triggers != null)
        {
            GetComponent<MeshRenderer>().material.SetFloat("_Emission", 2.9f + Mathf.Abs(Mathf.Sin(Time.time * 2f)) * 0.35f);
        }
        else if(triggered)
        {
            GetComponent<MeshRenderer>().material.SetFloat("_Emission", 3.25f + Mathf.Abs(Mathf.Sin(Time.time)) * 0.05f);
        }
        else
        {
            GetComponent<MeshRenderer>().material.SetFloat("_Emission", 1.5f);
        }

        
        if (!triggered && Vector3.Distance(Player.Instance.transform.position, transform.position) < 1f )
        {
            Trigger();
        }
	}

    void Trigger()
    {
        if (triggers.m_SecondaryEffect != null)
        {
            triggers.m_SecondaryEffect.SetActive(true);
        }
        runesActive++;
        triggers.triggered = triggered = true;
        Events.Instance.Raise(new WormEvents.WormClicked(Camera.main.gameObject, this.gameObject.transform.position));  

        if(runesActive == 2)
        {
            Player.Instance.b_CanOpenDoor = true;
        }

        if(triggerEffect != null)
        {
            var obj = (GameObject)GameObject.Instantiate(triggerEffect, transform);
            obj.transform.localPosition = Vector3.zero;
        }

        SoundManager.Instance.PlaySoundNotSpatial(SoundManager.Sounds.RUNE, 0.65f);
    }
}
