using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimControl : MonoBehaviour {
    private Animator m_Animator;

	// Use this for initialization
	void Start () {
        m_Animator = this.GetComponent<Animator>();
	}

    public void PlayJumpAnim() {
        m_Animator.SetTrigger("Jump 0");
    }
    public void PlayLandAnim() {
        m_Animator.SetTrigger("Land 0");
    }
}