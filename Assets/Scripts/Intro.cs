using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.UI;

public class Intro : MonoBehaviour {
  public GameCamera cam;
  public PostProcessingBehaviour postP;

  public Transform camStartScreenTransform;

  public DepthOfFieldModel.Settings dofSettings;


  public SwarmIndividual startButterfly;

  public Collider butterflyLandTarget;

  public Image uiFade;

  public MaskableGraphic uiLogo;

  public GameObject UiRoot;

  DepthOfFieldModel.Settings playSettings;

  Vector3 startCamPos;
  Quaternion startCamRot;

  enum State
  {
    FadingIn,
    WaitingOnButterfly,
    FadeInLogo,
    Done,
    TransitionToGame,
    WaitOnButton
  }

  State state = State.FadingIn;

	// Use this for initialization
	void Start () {
    cam.enabled = false;
    startCamPos = cam.transform.position;
    startCamRot = cam.transform.rotation;
    cam.transform.position = camStartScreenTransform.position;
    cam.transform.rotation = camStartScreenTransform.rotation;

    Player.Instance.enabled = false;

    playSettings = postP.profile.depthOfField.settings;
    postP.profile.depthOfField.settings = dofSettings;

    state = State.FadingIn;
    uiFade.color = new Color(0, 0, 0, 1);
    fadeInTimer = fadeInTime;

    uiLogo.color = new Color(0, 0, 0, 0);

    startButterfly.gameObject.SetActive(true);
    startButterfly.Perch(butterflyLandTarget);
    startButterfly.autoStart = false;

    UiRoot.SetActive(true);
    AudioListener.volume = 0;
  }

  // Update is called once per frame
  void Update()
  {
    switch (state)
    {
      case State.FadingIn: FadingIn(); break;
      case State.WaitingOnButterfly: WaitingOnButterfly(); break;
      case State.FadeInLogo: FadeInLogo(); break;
      case State.WaitOnButton: WaitingOnButton(); break;
      case State.TransitionToGame: TransitionToGame(); break;
    }
  }

  float fadeInTimer;
  public float fadeInTime;
  void FadingIn()
  {
    fadeInTimer -= Time.deltaTime;
    uiFade.color = new Color(0, 0, 0, fadeInTimer / fadeInTime);
    AudioListener.volume = 1.0f - fadeInTimer / fadeInTime;
    if(fadeInTimer <= 0)
    {
      state = State.WaitingOnButterfly;
    }
  }

  void WaitingOnButterfly()
  {
    if (startButterfly.isLanded)
    {
      state = State.FadeInLogo;
      fadeInLogoTimer = fadeInLogoTime;
    }
  }


  float fadeInLogoTimer;
  public float fadeInLogoTime;
  void FadeInLogo()
  {
    fadeInLogoTimer -= Time.deltaTime;
    uiLogo.color = new Color(1, 1, 1, 1.0f - fadeInLogoTimer / fadeInLogoTime);

    if (fadeInLogoTimer <= 0)
    {
      state = State.WaitOnButton;
    }
  }

  void WaitingOnButton()
  {
    if(Input.anyKeyDown)
    {
      var dir = Random.onUnitSphere * 1.3f;
      if (dir.y < 0) dir.y *= -1;
      dir = Vector3.ProjectOnPlane(dir, cam.transform.forward);

      startButterfly.Startle(dir, 3.0f);

      state = State.TransitionToGame;
      SoundManager.Instance.StartBackgroundMusic();

      fadeOutLogoTimer = fadeOutLogoTime;
    }
  }

  float fadeOutLogoTimer;
  public float fadeOutLogoTime;
  void TransitionToGame()
  {
    fadeOutLogoTimer -= Time.deltaTime;
    uiLogo.color = new Color(1, 1, 1, fadeOutLogoTimer / fadeOutLogoTime);

    var settings = postP.profile.depthOfField.settings;

    settings.aperture = Mathf.Lerp(dofSettings.aperture, playSettings.aperture, 1.0f - fadeOutLogoTimer / fadeOutLogoTime);
    settings.focusDistance = Mathf.Lerp(dofSettings.focusDistance, playSettings.focusDistance, 1.0f - fadeOutLogoTimer / fadeOutLogoTime);

    postP.profile.depthOfField.settings = settings;

    cam.transform.position = Vector3.Lerp(camStartScreenTransform.position, startCamPos, 1.0f - fadeOutLogoTimer / fadeOutLogoTime);
    cam.transform.rotation = Quaternion.Slerp(camStartScreenTransform.rotation, startCamRot, 1.0f - fadeOutLogoTimer / fadeOutLogoTime);

    if (fadeOutLogoTimer <= 0)
    {
      state = State.Done;
      settings = playSettings;
      Player.Instance.enabled = true;
      cam.enabled = true;
    }
  }
}
