using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimControl : MonoBehaviour {
    private Animator m_Animator;

	// Use this for initialization
	void Start () {
		if(this.GetComponent<Animator>() != null) {
            m_Animator = this.GetComponent<Animator>();
        }
	}
    void update() {
        if (m_Animator.GetBool("Jump")) {
            m_Animator.SetBool("Jump", false);
        }
        if (m_Animator.GetBool("Land")) {
            m_Animator.SetBool("Land", false);
        }
    }
    public void PlayJumpAnim() {
        m_Animator.SetBool("Jump", true);
    }
    public void PlayLandAnim() {
        m_Animator.SetBool("Land", true);
    }
}