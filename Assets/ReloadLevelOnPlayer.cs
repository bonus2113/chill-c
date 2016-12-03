using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReloadLevelOnPlayer : MonoBehaviour
{

  public MaskableGraphic uiFade;
  public float FadeTime;

  bool triggered = false;

  float fadeTimer;

  // Use this for initialization
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    if (triggered)
    {
      fadeTimer += Time.deltaTime;
      uiFade.color = new Color(0, 0, 0, fadeTimer / FadeTime);
      AudioListener.volume = 1.0f - fadeTimer / FadeTime;
      if (fadeTimer >= FadeTime)
      {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
      }
    }
  }


  void OnTriggerEnter(Collider col)
  {
    if (col.gameObject == Player.Instance.gameObject)
    {
      triggered = true;
      fadeTimer = 0;
    }
  }
}
