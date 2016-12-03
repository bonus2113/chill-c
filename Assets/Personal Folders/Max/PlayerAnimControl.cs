using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimControl : MonoBehaviour {
    private Animator m_Animator;
    private bool b_Animating = false;

	// Use this for initialization
	void Start () {
        m_Animator = this.GetComponent<Animator>();
	}
    void Update () {
        print("update");
        if (b_Animating) {
            m_Animator.SetBool("Jump", false);
            m_Animator.SetBool("Land", false);
            b_Animating = false;
        }
    }
    public void PlayJumpAnim() {
        m_Animator.SetBool("Jump", true);
        b_Animating = true;
    }
    public void PlayLandAnim() {
        m_Animator.SetBool("Land", true);
        b_Animating = true;
    }
}