using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmIndividual : MonoBehaviour
{
  public Swarm swarm;
  public float watchDistance;
  public float separationDistance;
  public float perchSearchRadius;

  public float followMultiplier = 1.0f;
  public float avoidanceLookahead = 1.0f;

  public float maxSpeed = 0.8f;

  private Rigidbody body;
  private int neighbourhoodCount;
  private Collider[] neighbourhood;

  private Vector3 startledDir;
  private float startledTime;
  private float totalStartledTime;

  private Vector3 landingPos;

  enum State
  {
    Stationary,
    Startled,
    Flying,
    Landing,
    Confused
  }

  State state = State.Stationary;

  void Start()
  {
    body = GetComponent<Rigidbody>();
    neighbourhood = new Collider[64];
    followMultiplier = Random.Range(0.5f, 1.1f);
  }

  public void Startle(Vector3 dir, float time)
  {
    state = State.Startled;
    startledDir = dir;
    startledTime = totalStartledTime = time;
  }

  public void Perch()
  {
    var colliders = Physics.OverlapSphere(transform.position, perchSearchRadius, 1 << LayerMask.NameToLayer("BoidLand"));

    if(colliders.Length == 0)
    {
      state = State.Confused;
      return;
    }


    var closest = colliders[Random.Range(0, colliders.Length)];


    landingPos = closest.bounds.center + closest.bounds.extents.y * closest.transform.up;
    landingPos += closest.bounds.extents.x * Random.Range(-1.0f, 1.0f) * closest.transform.right + closest.bounds.extents.z * Random.Range(-1.0f, 1.0f) * closest.transform.forward;

    state = State.Landing;
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

  Vector3 cohesion()
  {
    Vector3 res = Vector3.zero;
    if (neighbourhoodCount == 0) return res;
    for (int i = 0; i < neighbourhoodCount; i++)
    {
      var b = neighbourhood[i];
      if (!checkVis(b)) continue;
      res += b.transform.position;
    }

    res *= 1.0f / neighbourhoodCount;

    return (res - transform.position) / 100;
  }

  Vector3 separation()
  {
    Vector3 res = Vector3.zero;
    if (neighbourhoodCount == 0) return res;

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
    if (neighbourhoodCount == 0) return res;
    for (int i = 0; i < neighbourhoodCount; i++)
    {
      var b = neighbourhood[i];
      if (!checkVis(b)) continue;
      res += b.GetComponent<Rigidbody>().velocity;
    }

    res *= 1.0f / neighbourhoodCount;
    return (res - body.velocity) / 8;
  }

  Vector3 avoidance()
  {
    Vector3 res = Vector3.zero;

    RaycastHit hit;
    Vector3 dir = transform.forward;
    if(Physics.Raycast(transform.position, Vector3.RotateTowards(transform.forward, dir, 0.2f, 0.0f), out hit, avoidanceLookahead, 1 << LayerMask.NameToLayer("BoidAvoid")))
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

        var dir = Random.onUnitSphere * 0.1f;
        if (dir.y < 0) dir.y *= -1;
        Startle(dir, 3.0f);
      }
      else
      {
        Perch();
      }
    }
  }

  // Update is called once per frame
  void FixedUpdate()
  {

    if (state == State.Stationary)
    {
      body.velocity = Vector3.zero;
      return;
    }


    neighbourhoodCount = Physics.OverlapSphereNonAlloc(transform.position, watchDistance, neighbourhood, 1 << gameObject.layer);

    Vector3 sep = separation() * 0.1f;
    Vector3 align = alignment();
    Vector3 cohe = cohesion();
    Vector3 avoid = avoidance();

    float seperationFactor = 1.0f;
    float alignmentFactor  = 1.0f;
    float coherenceFactor  = 1.0f;
    float avoidanceFactor  = 1.0f;

    Vector3 impulse = Vector3.zero;

    confusedTimer -= Time.fixedDeltaTime;
    if (confusedTimer <= 0)
    {
      confusedTimer = Random.Range(0.2f, 1.0f);
      confusedTargetDir = Random.onUnitSphere * Random.Range(0.05f, 0.1f);
    }

    confusedCurrentDir = Vector3.SmoothDamp(confusedCurrentDir, confusedTargetDir, ref confusedCurrentVel, 0.5f);

    if(state == State.Landing)
    {
      coherenceFactor = 0;
      seperationFactor = 0;
      alignmentFactor = 0.1f;
      avoidanceFactor = 0.5f;
      Vector3 landingDir = landingPos - transform.position;
      impulse += Vector3.ClampMagnitude(landingDir, 1.0f) * 0.05f;
      if (landingDir.magnitude < 2) avoidanceFactor = 0;
      if (landingDir.magnitude < 0.1f) state = State.Stationary;
    }
    else
    {
      impulse += confusion();
      impulse += swarm.Wind();
      impulse += swarm.SteeringVelocity(transform.position) * followMultiplier;
      impulse += Vector3.up * 0.01f;
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

    impulse += sep * seperationFactor + align * alignmentFactor + cohe * coherenceFactor + avoid * avoidanceFactor;

    body.AddForce(impulse, ForceMode.VelocityChange);
    transform.forward = body.velocity;

    body.velocity = Vector3.ClampMagnitude(body.velocity, maxSpeed);
  }
}