﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmIndividual : MonoBehaviour
{
  public Texture2D[] Textures;

  public Swarm swarm;
  public float watchDistance;
  public float separationDistance;
  public float perchSearchRadius;

  public float followMultiplier = 1.0f;
  public float avoidanceLookahead = 1.0f;

  public Transform WingTransformLeft;
  public Transform WingTransformRight;

  public float maxSpeed = 0.8f;
  public bool autoStart = true;

  private Rigidbody body;
  private int neighbourhoodCount;
  private Collider[] neighbourhood;

  private Vector3 startledDir;
  private float startledTime;
  private float totalStartledTime;

  private float stateTimer;

  public bool isLanded { get { return state == State.Stationary; } }

  private Vector3 landingPos;

    private HingeJoint m_Hinge = null;

  enum State
  {
    Stationary,
    Startled,
    Flying,
    Landing,
    Confused
  }

  State state = State.Stationary;
  float scale;

  void Start()
  {
    body = GetComponent<Rigidbody>();
    neighbourhood = new Collider[64];
    followMultiplier = Random.Range(0.5f, 1.1f);
    stateTimer = Random.Range(4.0f, 12.0f);
    int index = Random.Range(0, Textures.Length);
    WingTransformLeft.GetComponent<MeshRenderer>().material.mainTexture = Textures[index];
    WingTransformRight.GetComponent<MeshRenderer>().material.mainTexture = Textures[index];

    scale = Random.Range(3.0f, 6.0f);
    transform.localScale = new Vector3(scale, scale, scale);
  }

  public void Startle(Vector3 dir, float time)
  {
    state = State.Startled;
    startledDir = dir;
    startledTime = totalStartledTime = time;

    //body.isKinematic = false;
    GetComponent<Collider>().enabled = true;

        if (m_Hinge != null)
        {
            m_Hinge.connectedBody.AddForceAtPosition(-dir.normalized * 1.5f, this.transform.position, ForceMode.Impulse);
            Destroy(m_Hinge);
            m_Hinge = null;
        }
  }

  Collider collToLandOn;

  public void Perch(Collider coll)
  {
    var closest = coll;

    landingPos = closest.bounds.center + closest.bounds.extents.y * closest.transform.up;
    landingPos += closest.bounds.extents.x * Random.Range(-1.0f, 1.0f) * closest.transform.right + closest.bounds.extents.z * Random.Range(-1.0f, 1.0f) * closest.transform.forward;

    state = State.Landing;
    collToLandOn = closest;
    landingPos = collToLandOn.transform.InverseTransformPoint(landingPos);
  }

  public void Perch()
  {
    var colliders = Physics.OverlapSphere(transform.position, perchSearchRadius, 1 << LayerMask.NameToLayer("BoidLand"));

    if (colliders.Length == 0)
    {
      state = State.Confused;
      return;
    }


    var closest = colliders[Random.Range(0, colliders.Length)];

    Perch(closest);
  }

  bool checkVis(Collider coll)
  {
    SwarmIndividual other = coll.GetComponent<SwarmIndividual>();
    if (coll.gameObject == this.gameObject || !other || other.swarm != this.swarm) return false;

    if (other.state == State.Stationary) return false;

    Vector3 dir = (coll.transform.position - this.transform.position).normalized;

    if (Vector3.Dot(dir, transform.forward) < 0 && Vector3.Angle(dir, transform.forward) < 60) return false;

    return true;
  }

  public void OnTriggerEnter(Collider other)
  {
    if (state != State.Landing) return;

    if (other != collToLandOn) return;

    stateTimer = Random.Range(3.0f, 12.0f);
    state = State.Stationary;

        //body.isKinematic = true;

        var rig = other.transform.GetComponent<Rigidbody>();
        if (rig)
        {
          rig.AddForceAtPosition(body.velocity * 0.75f, transform.position, ForceMode.Impulse);
            //add joint 420 boii
            m_Hinge = this.gameObject.AddComponent<HingeJoint>();
            m_Hinge.useLimits = true;
            var limits = m_Hinge.limits;
            limits.max = 0;
            limits.min = 0;
            m_Hinge.limits = limits;
            m_Hinge.autoConfigureConnectedAnchor = true;
            m_Hinge.connectedBody = rig;
        }
        GetComponent<Collider>().enabled = false;

        body.velocity = Vector3.zero;
    }

  public void OnCollisionEnter(Collision collision)
  {
    OnTriggerEnter(collision.collider);
  }

  Vector3 cohesion()
  {
    Vector3 res = Vector3.zero;
    if (neighbourhoodCount <= 1) return res;
    for (int i = 0; i < neighbourhoodCount; i++)
    {
      var b = neighbourhood[i];
      if (!checkVis(b)) continue;
      res += b.transform.position;
    }

    res *= 1.0f / (neighbourhoodCount - 1);

    return (res - transform.position) / 100;
  }

  Vector3 separation()
  {
    Vector3 res = Vector3.zero;
    if (neighbourhoodCount <= 1) return res;

    for (int i = 0; i < neighbourhoodCount; i++)
    {
      var b = neighbourhood[i];
      if (!checkVis(b)) continue;
      var dist = b.transform.position - transform.position;
      if (dist.magnitude < separationDistance)
      {
        res -= dist;
      }
    }

    return res;
  }

  Vector3 alignment()
  {
    Vector3 res = Vector3.zero;
    if (neighbourhoodCount <= 1) return res;
    for (int i = 0; i < neighbourhoodCount; i++)
    {
      var b = neighbourhood[i];
      if (!checkVis(b)) continue;
      res += b.GetComponent<Rigidbody>().velocity;
    }

    res *= 1.0f / (neighbourhoodCount - 1);
    return (res - body.velocity) / 8;
  }

  Vector3 avoidance()
  {
    Vector3 res = Vector3.zero;

    RaycastHit hit;
    Vector3 dir = transform.forward;
    if (Physics.Raycast(transform.position, Vector3.RotateTowards(transform.forward, dir, 0.2f, 0.0f), out hit, avoidanceLookahead, 1 << LayerMask.NameToLayer("BoidAvoid")))
    {
      float dist = hit.distance / avoidanceLookahead;
      res += Vector3.Reflect(transform.forward, hit.normal) * dist;
    }

    dir = transform.right;
    if (Physics.Raycast(transform.position, Vector3.RotateTowards(transform.forward, dir, 0.2f, 0.0f), out hit, avoidanceLookahead, 1 << LayerMask.NameToLayer("BoidAvoid")))
    {
      float dist = hit.distance / avoidanceLookahead;
      res -= dir * (1.0f - dist);
    }

    dir = -transform.right;
    if (Physics.Raycast(transform.position, Vector3.RotateTowards(transform.forward, dir, 0.2f, 0.0f), out hit, avoidanceLookahead, 1 << LayerMask.NameToLayer("BoidAvoid")))
    {
      float dist = hit.distance / avoidanceLookahead;
      res -= dir * (1.0f - dist);
    }

    dir = transform.up;
    if (Physics.Raycast(transform.position, Vector3.RotateTowards(transform.forward, dir, 0.2f, 0.0f), out hit, avoidanceLookahead, 1 << LayerMask.NameToLayer("BoidAvoid")))
    {
      float dist = hit.distance / avoidanceLookahead;
      res -= dir * (1.0f - dist);
    }
    dir = -transform.up;

    return res;
  }

  Vector3 attractors()
  {
    Vector3 res = Vector3.zero;
    var colls = Physics.OverlapSphere(transform.position, 30, 1 << LayerMask.NameToLayer("BoidTractor"));
    foreach (var coll in colls)
    {
      BoidAttractor attr = coll.GetComponent<BoidAttractor>();
      if (attr)
      {
        res -= attr.strength * Vector3.ClampMagnitude(transform.position - attr.transform.position, 0.002f);
      }
    }
    return res;
  }

  float confusedTimer = 0;
  Vector3 confusedTargetDir = Vector3.zero;
  Vector3 confusedCurrentDir = Vector3.zero;
  Vector3 confusedCurrentVel = Vector3.zero;

  Vector3 confusion()
  {
    return confusedCurrentDir;
  }

  void Update()
  {
    if (Input.GetKeyDown(KeyCode.Space))
    {
      if (state == State.Stationary)
      {

        var dir = Random.onUnitSphere * 0.7f;
        if (dir.y < 0) dir.y *= -1;
        Startle(dir, 3.0f);
      }
      else
      {
        Perch();
      }
    }
  }

  float wingState = 0.0f;

  // Update is called once per frame
  void FixedUpdate()
  {

    if (state == State.Stationary)
    {
      //body.velocity = Vector3.zero;
      wingState += Time.fixedDeltaTime * 1.7f;

      var wingLeftRot = WingTransformRight.localEulerAngles;
      wingLeftRot.x = Mathf.LerpAngle(100.0f, 170.0f, Mathf.Sin(wingState) * 0.35f + 0.35f);
      WingTransformLeft.localEulerAngles = wingLeftRot;

      var wingRightRot = WingTransformRight.localEulerAngles;
      wingRightRot.x = Mathf.LerpAngle(80.0f, 10.0f, Mathf.Sin(wingState) * 0.35f + 0.65f);
      WingTransformRight.localEulerAngles = wingRightRot;

      if (autoStart)
      {

        stateTimer -= Time.fixedDeltaTime;
        if (stateTimer <= 0)
        {
          stateTimer = Random.Range(15.0f, 25.0f);
          var dir = Random.onUnitSphere * 0.7f;
          if (dir.y < 0) dir.y *= -1;
          Startle(dir, 3.0f);
        }

        if (Player.Instance)
        {

          var playerPos = Player.Instance.transform.position;
          var playerMoving = Player.Instance.isRunning;

          if (Vector3.Distance(playerPos, transform.position) < 4 && playerMoving)
          {
            stateTimer = Random.Range(15.0f, 25.0f);
            var dir = Random.onUnitSphere * 1.2f;
            if (dir.y < 0) dir.y *= -1;
            Startle(dir, 3.0f);
          }
        }
      }
      return;
    }

    {
      var wingLeftRot = WingTransformRight.localEulerAngles;
      wingLeftRot.x = Mathf.LerpAngle(100.0f, 170.0f, Mathf.Sin(wingState) * 0.5f + 0.5f);
      if (wingLeftRot.x < 125.0f) wingLeftRot.x = 100.0f; else if (wingLeftRot.x < 145.0f) wingLeftRot.x = 125.0f; else wingLeftRot.x = 170.0f;
      WingTransformLeft.localEulerAngles = wingLeftRot;

      var wingRightRot = WingTransformRight.localEulerAngles;
      wingRightRot.x = Mathf.LerpAngle(80.0f, 10.0f, Mathf.Sin(wingState) * 0.5f + 0.5f);
      if (wingRightRot.x < 35.0f) wingRightRot.x = 10.0f; else if (wingRightRot.x < 55.0f) wingRightRot.x = 35.0f; else wingRightRot.x = 80.0f;
      WingTransformRight.localEulerAngles = wingRightRot;
    }

    neighbourhoodCount = Physics.OverlapSphereNonAlloc(transform.position, watchDistance, neighbourhood, 1 << gameObject.layer);

    Vector3 sep = separation() * 0.3f;
    Vector3 align = alignment();
    Vector3 cohe = cohesion();
    Vector3 avoid = avoidance();
    Vector3 attract = attractors();

    float seperationFactor = 1.0f;
    float alignmentFactor = 1.0f;
    float coherenceFactor = 1.0f;
    float avoidanceFactor = 1.0f;
    float attractorFactor = 1.0f;
    float straightFactor = 0.4f;
    float confusionFactor = 1.0f;

    Vector3 impulse = Vector3.zero;

    confusedTimer -= Time.fixedDeltaTime;
    if (confusedTimer <= 0)
    {
      confusedTimer = Random.Range(0.2f, 0.5f);
      confusedTargetDir = Random.onUnitSphere * Random.Range(0.15f, 0.3f);
    }



    confusedCurrentDir = Vector3.SmoothDamp(confusedCurrentDir, confusedTargetDir, ref confusedCurrentVel, 0.2f);

    if (state == State.Landing)
    {
      coherenceFactor = 0;
      seperationFactor = 0;
      alignmentFactor = 0.1f;
      avoidanceFactor = 0.8f;
      attractorFactor = 0.0f;
      confusionFactor = 0.4f;
      Vector3 landingDir = collToLandOn.transform.TransformPoint(landingPos) - transform.position;
      if(landingDir.magnitude > perchSearchRadius)
      {
        state = State.Flying;
      }
      else
      {
      impulse += Vector3.ClampMagnitude(landingDir, 1.0f) * 0.05f;
      if (landingDir.magnitude < 2) avoidanceFactor = 0;
      if (landingDir.magnitude < 4) confusionFactor = 0;
      if (landingDir.magnitude < 3.4f) straightFactor = 1.0f - landingDir.magnitude / 3.4f;
      }
    }
    else
    {
      impulse += swarm.Wind();
      impulse += swarm.SteeringVelocity(transform.position) * followMultiplier;
      if (transform.position.y < 4)
      {
        impulse += Vector3.up * 0.01f;
      }
      else if (transform.position.y > 7)
      {
        impulse -= Vector3.up * 0.1f;
      }

      stateTimer -= Time.fixedDeltaTime;
      if (stateTimer <= 0)
      {
        Perch();
      }
    }

    if (state == State.Startled)
    {
      impulse += startledDir * (startledTime / totalStartledTime);
      startledTime -= Time.fixedDeltaTime;
      if (startledTime <= 0)
      {
        startledTime = 0;
        state = State.Flying;
      }
    }

    impulse += confusion() * confusionFactor;
    impulse += sep * seperationFactor + align * alignmentFactor + cohe * coherenceFactor + avoid * avoidanceFactor + attractorFactor * attract;

    body.AddForce(impulse, ForceMode.VelocityChange);

    body.velocity = Vector3.ClampMagnitude(body.velocity, maxSpeed);

    wingState += (Mathf.Max(0, body.velocity.y + impulse.y) * 15.0f + 25.0f) * Time.fixedDeltaTime ;

    if (body.velocity.magnitude > 0.05f)
    {
      var straightVel = Vector3.ProjectOnPlane(body.velocity, Vector3.up);
      transform.LookAt(transform.position + Vector3.Slerp(transform.forward, (body.velocity.normalized * (1.0f - straightFactor) + straightVel * straightFactor), 10.0f * Time.fixedDeltaTime), Vector3.up);
    }
  }
}