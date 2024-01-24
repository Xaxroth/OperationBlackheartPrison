using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public Transform player;
    public Animator enemyAnimator;

    public AudioSource EnemyAudioSource;
    public AudioClip Footsteps;
    public AudioClip AlertScream;
    public AudioClip DamageScream;
    public AudioClip Swing;

    public GameObject playerCharacter;
    public GameObject destinationGameObject;
    public GameObject FireTeleportSpell;
    public GameObject GoreExplosion;
    public GameObject enemyBody;

    public NavMeshAgent EnemyNavMeshAgent;

    public ParticleSystem BloodParticles;

    public enum EnemyState
    {
        Waiting,
        Chasing,
        Attacking
    }

    public EnemyState CurrentEnemyState;

    [Range(0, 360)]
    public float viewAngle;

    public float coneRadius = 2.0f;
    public float coneLength = 5.0f;
    public LayerMask hitMask;
    public LayerMask blockedMask;

    public bool dead;
    public bool InRange = false;
    public bool InLineOfSight;
    public bool CanMove = false;

    public int EnemyDamage = 10;
    public float Health = 10f;
    float refreshRate = 0.5f;
    float MovementSpeed = 9f;
    private bool isAttacking = false;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        EnemyNavMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        EnemyAudioSource = gameObject.GetComponent<AudioSource>();
    }

    void Update()
    {
        MoveToNewPosition();

        if (CurrentEnemyState == EnemyState.Waiting)
        {
            CheckLoS();
        }

        if (EnemyNavMeshAgent.remainingDistance < 4.0f)
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
            EnemyNavMeshAgent.destination = player.transform.position;
            EnemyNavMeshAgent.stoppingDistance = 4;
        }
    }

    public void Attack()
    {
        if (InRange && CurrentEnemyState != EnemyState.Attacking && !isAttacking)
        {
            StartCoroutine(AttackCoroutine());
        }
    }

    public IEnumerator AttackCoroutine()
    {
        isAttacking = true;
        CurrentEnemyState = EnemyState.Attacking;

        while (InRange)
        {
            MovementSpeed = 0;
            EnemyNavMeshAgent.speed = MovementSpeed;
            Vector3 coneDirection = transform.forward;
            yield return new WaitForSeconds(0.5f);
            // Perform a sphere cast in the cone direction
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
            }

            MovementSpeed = 15;
            EnemyNavMeshAgent.speed = MovementSpeed;

            yield return new WaitForSeconds(1.0f);
        }

        isAttacking = false;
    }

    public void CheckLoS()
    {
        Collider[] cone = Physics.OverlapSphere(transform.position, coneRadius);

        if (cone.Length != 0)
        {
            foreach (var hitCollider in cone)
            {
                var target = hitCollider.GetComponent<PlayerControllerScript>();
                Transform targetTransform = hitCollider.GetComponent<Transform>();
                Vector3 targetDirection = (targetTransform.position - transform.position).normalized;

                if (Vector3.Angle(transform.forward, targetDirection) < viewAngle / 2 && target)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, targetTransform.position);

                    if (!Physics.Raycast(transform.position, targetDirection, distanceToTarget, blockedMask))
                    {
                        InLineOfSight = true;
                        CanMove = true;
                        EnemyAudioSource.PlayOneShot(AlertScream);
                        CurrentEnemyState = EnemyState.Chasing;
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
        Health -= (int)Damage;
        BloodParticles.Play();

        if (Health <= 0 && !dead)
        {

            StartCoroutine(DeathCoroutine());

            if (ShouldExplode)
            {
                Explode();
                Natia.Instance.Affection += 20;
            }
        }
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
