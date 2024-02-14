using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Animator enemyAnimator;
    [SerializeField] private NavMeshAgent EnemyNavMeshAgent;

    [SerializeField] private AudioSource EnemyAudioSource;
    [SerializeField] private AudioClip Footsteps;
    [SerializeField] private AudioClip AlertScream;
    [SerializeField] private AudioClip DamageScream;
    [SerializeField] private AudioClip AttackSound;
    [SerializeField] private AudioClip Swing;

    [SerializeField] private GameObject playerCharacter;
    [SerializeField] private GameObject destinationGameObject;
    [SerializeField] private GameObject FireTeleportSpell;
    [SerializeField] private GameObject GoreExplosion;
    [SerializeField] private GameObject enemyBody;


    [SerializeField] private ParticleSystem BloodParticles;

    [SerializeField]
    private enum EnemyState
    {
        Waiting,
        Chasing,
        Attacking,
        Stunned
    }

    private EnemyState CurrentEnemyState;

    [Range(0, 360)]
    public float viewAngle;

    public float coneRadius = 2.0f;
    public float coneLength = 5.0f;

    private LayerMask hitMask;
    private LayerMask blockedMask;

    private bool dead;
    private bool InRange = false;
    private bool InLineOfSight;
    private bool CanMove = false;
    private Transform _targetPosition;

    private float MeleeRange = 5f;

    private bool canBeHarmed;

    private int EnemyDamage = 10;
    public float Health = 10f;
    private float stunDuration = 3f;
    private float refreshRate = 0.5f;
    public float NormalMovementSpeed = 9f;
    private float MovementSpeed = 9f;
    private bool isAttacking = false;

    public bool Initialized { get; private set; }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        EnemyNavMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        EnemyAudioSource = gameObject.GetComponent<AudioSource>();
        enemyAnimator = gameObject.GetComponentInChildren<Animator>();

        InvokeRepeating("CheckLoS", 1.0f, 1.0f);
    }

    void Update()
    {
        if (CurrentEnemyState == EnemyState.Stunned)
        {
            EnemyNavMeshAgent.speed = 0;
            InRange = false;
            StopCoroutine(AttackCoroutine());
            return;
        }
        else
        {
            EnemyNavMeshAgent.speed = NormalMovementSpeed;
        }

        if (Natia.Instance.Dead || PlayerControllerScript.Instance.Dead)
        {
            StopAllCoroutines();
            return;
        }

        DetermineTarget();

        MoveToNewPosition();

        float distanceToTarget = Vector3.Distance(EnemyNavMeshAgent.transform.position, _targetPosition.position);

        if (distanceToTarget <= MeleeRange)
        {
            InRange = true;
        }
        else
        {
            InRange = false;
            CurrentEnemyState = EnemyState.Chasing;
        }

        if (CurrentEnemyState == EnemyState.Chasing)
        {
            Attack();
        }
    }

    void MoveToNewPosition()
    {
        if (CanMove)
        {
            DetermineTarget();
            EnemyNavMeshAgent.destination = _targetPosition.transform.position;
            EnemyNavMeshAgent.stoppingDistance = 2;
        }
    }

    public void Attack()
    {
        if (InRange && CurrentEnemyState != EnemyState.Attacking && !isAttacking && CanMove)
        {
            StartCoroutine(AttackCoroutine());
        }
    }

    public void StunEnemy()
    {
        StartCoroutine(StunnedCoroutine());
    }

    private IEnumerator StunnedCoroutine()
    {
        CurrentEnemyState = EnemyState.Stunned;
        enemyAnimator.SetBool("Flashed", true);
        yield return new WaitForSeconds(stunDuration);
        enemyAnimator.SetBool("Flashed", false);
        CurrentEnemyState = EnemyState.Chasing;
    }

    public void DetermineTarget()
    {
        if (Vector3.Distance(transform.position, PlayerControllerScript.Instance.transform.position) > Vector3.Distance(transform.position, Natia.Instance.transform.position))
        {
            _targetPosition = Natia.Instance.transform;
        }
        else
        {
            _targetPosition = PlayerControllerScript.Instance.transform;
        }
    }

    public IEnumerator AttackCoroutine()
    {
        isAttacking = true;
        CurrentEnemyState = EnemyState.Attacking;

        while (InRange)
        {
            EnemyAudioSource.PlayOneShot(AttackSound, 0.7f);
            enemyAnimator.SetBool("Attack", true);

            MovementSpeed = 0;
            EnemyNavMeshAgent.speed = MovementSpeed;

            Vector3 coneDirection = transform.forward;

            yield return new WaitForSeconds(0.5f);

            RaycastHit[] hits = Physics.SphereCastAll(transform.position, coneRadius, coneDirection, coneLength);
            List<GameObject> hitObjects = new List<GameObject>();

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject != gameObject)
                {
                    hitObjects.Add(hit.collider.gameObject);
                }
            }

            foreach (GameObject hitObject in hitObjects)
            {
                if (hitObject.CompareTag("Player") && InRange)
                {
                    hitObject.GetComponent<PlayerControllerScript>().TakeDamage(EnemyDamage);
                }
                else if (hitObject.CompareTag("Natia") && InRange)
                {
                    hitObject.GetComponent<Natia>().TakeDamage(EnemyDamage);
                }
            }

            enemyAnimator.SetBool("Attack", false);

            MovementSpeed = NormalMovementSpeed;
            EnemyNavMeshAgent.speed = MovementSpeed;

            hitObjects.Clear();

            yield return new WaitForSeconds(0.7f);
        }

        isAttacking = false;
    }

    public void CheckLoS()
    {
        Collider[] cone = Physics.OverlapSphere(transform.position, coneRadius);

        if (cone.Length != 0 && !CanMove)
        {
            foreach (var hitCollider in cone)
            {
                var target = hitCollider.GetComponent<PlayerControllerScript>();
                var Natia = hitCollider.GetComponent<Natia>();
                Transform targetTransform = hitCollider.GetComponent<Transform>();
                Vector3 targetDirection = (targetTransform.position - transform.position).normalized;

                if (Vector3.Angle(transform.forward, targetDirection) < viewAngle / 2 && target || Vector3.Angle(transform.forward, targetDirection) < viewAngle / 2 && Natia)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, targetTransform.position);

                    if (!Physics.Raycast(transform.position, targetDirection, distanceToTarget, blockedMask))
                    {
                        InLineOfSight = true;
                        EnemyAudioSource.PlayOneShot(AlertScream, 0.5f);
                        enemyAnimator.SetBool("Awaken", true);

                        StartCoroutine(AwakeCoroutine());
                    }
                    else
                    {
                        InLineOfSight = false;
                    }

                }
            }
        }
    }

    public void TakeDamage(float Damage, bool ShouldExplode)
    {
        if (canBeHarmed)
        {
            Health -= (int)Damage;
            BloodParticles.Play();

            if (Health <= 0 && !dead)
            {
                StartCoroutine(DeathCoroutine());

                if (ShouldExplode)
                {
                    Explode();
                }
            }
        }
    }

    private IEnumerator AwakeCoroutine()
    {
        yield return new WaitForSeconds(1);
        canBeHarmed = true;
        CanMove = true;
        CurrentEnemyState = EnemyState.Chasing;
    }

    private IEnumerator DeathCoroutine()
    {
        dead = true;
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }

    public void Explode()
    {
        GameObject GoreExplosionEffect = Instantiate(GoreExplosion, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
