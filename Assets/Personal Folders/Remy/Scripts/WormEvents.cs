using UnityEngine;
using System.Collections;

public class WormEvents{

    public class WormClicked : BaseEvent
    {
        public Vector3 pos;
        public WormClicked(GameObject sender, Vector3 pos)
        {
            this.sender = sender;
            this.pos = pos;
        }
    }

}
