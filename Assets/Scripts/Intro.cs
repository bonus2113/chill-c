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
    public MaskableGraphic uiLogo1;
    public MaskableGraphic uiLogo2;

    public GameObject UiRoot;

    private Color logoStartColor = Color.white;
    private Color logoStartColor1 = Color.white;
    private Color logoStartColor2 = Color.white;
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

    void Awake()
    {
        logoStartColor = uiLogo.color;
        logoStartColor1 = uiLogo1.color;
        logoStartColor2 = uiLogo2.color;
    }

	void Start () {
    cam.enabled = false;
    startCamPos = cam.transform.position;
    startCamRot = cam.transform.rotation;
    cam.transform.position = camStartScreenTransform.position;
    cam.transform.rotation = camStartScreenTransform.rotation;

    Player.Instance.AllowInput = false;

    playSettings = postP.profile.depthOfField.settings;
    postP.profile.depthOfField.settings = dofSettings;

    state = State.FadingIn;
    uiFade.color = new Color(0, 0, 0, 1);
    fadeInTimer = fadeInTime;

    uiLogo.color = new Color(0, 0, 0, 0);
        uiLogo1.color = new Color(0, 0, 0, 0);
        uiLogo2.color = new Color(0, 0, 0, 0);

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

        Color logoColor = logoStartColor;
        float fadeVal = (1.0f - fadeInLogoTimer / fadeInLogoTime);
        logoColor.a = fadeVal;
        uiLogo.color = logoColor;

        Color logoColor1 = logoStartColor1;
        logoColor1.a = fadeVal;
        uiLogo1.color = logoColor1;

        Color logoColor2 = logoStartColor2;
        logoColor2.a = fadeVal;
        uiLogo2.color = logoColor2;

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

        Color logoColor = logoStartColor;
        float fadeVal = fadeOutLogoTimer / fadeOutLogoTime;
        logoColor.a = fadeVal;
        uiLogo.color = logoColor;

        Color logoColor1 = logoStartColor1;
        logoColor1.a = fadeVal;
        uiLogo1.color = logoColor1;

        Color logoColor2 = logoStartColor2;
        logoColor2.a = fadeVal;
        uiLogo2.color = logoColor2;


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
      Player.Instance.AllowInput = true;
      cam.enabled = true;
    }
  }
}
