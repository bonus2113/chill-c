using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButterflyToRune : MonoBehaviour {

    public GameObject swarm;
    public DecalAnimation rune;
    int childIndex = 0;

    List<GameObject> buttflies = new List<GameObject>();
    List<Vector3> starts = new List<Vector3>();
    List<Vector3> targets = new List<Vector3>();
    List<float> times = new List<float>();



    float time = -20;
    int arrived = 0;
    void Update()
    {

        // get buttflies
        time += Time.deltaTime * 4;
        if(time > childIndex && childIndex < 16)
        {
            GameObject butt = swarm.transform.GetChild(childIndex).gameObject;
            buttflies.Add(butt);
            starts.Add(butt.transform.position);
            targets.Add(rune.GetStartPosition(childIndex));
            times.Add(0f);
            butt.GetComponent<SwarmIndividual>().enabled = false;
            butt.GetComponent<Rigidbody>().velocity = Vector3.zero;

            childIndex++;
        }


        for(int i = buttflies.Count - 1; i >= 0; i--)
        {
            GameObject butt = buttflies[i];
            times[i] += Time.deltaTime * 0.2f;
            butt.transform.position = Vector3.Lerp(starts[i], targets[i], times[i]);
            if (times[i] >= 1f)
            {
                buttflies.RemoveAt(i);
                starts.RemoveAt(i);
                targets.RemoveAt(i);
                times.RemoveAt(i);
                rune.AnimateRunepart(arrived);
                butt.active = false;
                arrived++;
            }
        }
	}
}
