using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Natia : MonoBehaviour
{
    public static Natia Instance;

    public Transform player;
    public Animator enemyAnimator;
    public GameObject playerCharacter;
    public AudioSource EnemyAudioSource;
    public AudioClip BellRinging;
    public AudioClip EnemyAlert;
    public GameObject destinationGameObject;
    public GameObject FireTeleportSpell;
    public NavMeshAgent EnemyNavMeshAgent;
    public GameObject enemyBody;
    public Collider NatiaCollider; 

    public enum EnemyState
    {
        Waiting,
        Following,
        Cautious,
        Relaxed,
        Lockpicking,
        PickedUp
    }

    public EnemyState CurrentEnemyState;

    public Door DoorToOpen;
    public Chest ChestToOpen;

    public bool CanMove = false;
    public bool Dead = false;
    public bool OpeningDoor = false;
    public bool InConversation;
    public bool MessagePlayed;
    public bool StartedLockpicking = false;

    float DistanceToPlayer = 0;
    float Health = 10f;
    float refreshRate = 0.5f;
    float MovementSpeed = 9f;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        EnemyNavMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        EnemyAudioSource = gameObject.GetComponent<AudioSource>();
    }

    void Update()
    {
        NatiaState();

        if (CurrentEnemyState == EnemyState.Waiting)
        {
            StandStill();
            return;
        }

        MoveToNewPosition();

        if (PlayerControllerScript.Instance.CurrentMovementState == PlayerControllerScript.PlayerMovementState.Running)
        {
            EnemyNavMeshAgent.speed = 25;
        }
        else
        {
            EnemyNavMeshAgent.speed = 10;
        }
        if (PlayerControllerScript.Instance != null)
        {
            Quaternion targetRotation = Quaternion.LookRotation(PlayerControllerScript.Instance.transform.position - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 6 * Time.deltaTime);
            transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
        }

        DistanceCheck();
    }

    void DistanceCheck()
    {
        if (PlayerControllerScript.Instance != null && Natia.Instance != null)
        {
            DistanceToPlayer = Vector3.Distance(PlayerControllerScript.Instance.transform.position, Natia.Instance.transform.position);

            if (DistanceToPlayer > 75 && !MessagePlayed && !DialogueManagerScript.Instance.InProgress)
            {
                DialogueManagerScript.Instance.PlayerTooFarAway();
                MessagePlayed = true;
            }

            if (DistanceToPlayer < 25 && MessagePlayed && !DialogueManagerScript.Instance.InProgress)
            {
                DialogueManagerScript.Instance.PlayerComesBack();
                MessagePlayed = false;
            }
        }
    }

    void StandStill()
    {
        EnemyNavMeshAgent.destination = gameObject.transform.position;
        CanMove = false;
        DistanceCheck();
    }

    void NatiaState()
    {
        EnemyNavMeshAgent.enabled = true;

        switch (CurrentEnemyState)
        {
            case EnemyState.Waiting:
                EnemyNavMeshAgent.stoppingDistance = 10;
                break;
            case EnemyState.Following:
                EnemyNavMeshAgent.stoppingDistance = 8;
                EnemyNavMeshAgent.destination = PlayerControllerScript.Instance.gameObject.transform.position;
                break;
            case EnemyState.Relaxed:
                EnemyNavMeshAgent.stoppingDistance = 15;
                EnemyNavMeshAgent.destination = PlayerControllerScript.Instance.gameObject.transform.position;
                break;
            case EnemyState.Cautious:
                EnemyNavMeshAgent.stoppingDistance = 8;
                EnemyNavMeshAgent.destination = PlayerControllerScript.Instance.gameObject.transform.position;
                break;
            case EnemyState.Lockpicking:
                EnemyNavMeshAgent.stoppingDistance = 3;
                break;
            case EnemyState.PickedUp:
                NatiaCollider.enabled = false;
                EnemyNavMeshAgent.stoppingDistance = 0;
                EnemyNavMeshAgent.enabled = false;
                gameObject.transform.position = PlayerControllerScript.Instance.gameObject.transform.position;
                break;
        }
    }

    void MoveToNewPosition()
    {
        if (CanMove && !OpeningDoor && CurrentEnemyState != EnemyState.PickedUp)
        {
            EnemyNavMeshAgent.destination = player.transform.position;
        }

        if (OpeningDoor && EnemyNavMeshAgent.remainingDistance < 3f && DoorToOpen != null && !StartedLockpicking)
        {
            StartCoroutine(PickLock());
        }

        if (OpeningDoor && EnemyNavMeshAgent.remainingDistance < 3f && ChestToOpen != null && !StartedLockpicking)
        {
            StartCoroutine(PickLock());
        }
    }

    private IEnumerator PickLock()
    {
        StartedLockpicking = true;
        AudioManager.Instance.PlaySound(AudioManager.Instance.PickLock, 1.0f);
        yield return new WaitForSeconds(1.5f);
        if (ChestToOpen != null)
        {
            ChestToOpen.locked = false;
            ChestToOpen = null;
        }

        if (DoorToOpen != null)
        {
            DoorToOpen.locked = false;
            DoorToOpen = null;
        }

        CurrentEnemyState = EnemyState.Following;

        StartedLockpicking = false;
        OpeningDoor = false;
    }

    public void OpenDoor(GameObject Door)
    {
        if (CanMove)
        {
            OpeningDoor = true;
            EnemyNavMeshAgent.stoppingDistance = 1;
            EnemyNavMeshAgent.destination = Door.transform.position;
            DoorToOpen = Door.GetComponent<Door>();
        }
    }

    public void OpenChest(GameObject Chest)
    {
        if (CanMove)
        {
            OpeningDoor = true;
            EnemyNavMeshAgent.stoppingDistance = 1;
            EnemyNavMeshAgent.destination = Chest.transform.position;
            ChestToOpen = Chest.GetComponent<Chest>();
        }
    }

    public void TakeDamage(float Damage)
    {
        Health -= (int)Damage;

        if (Health <= 0)
        {
            StartCoroutine(DeathCoroutine());
        }

        if (!DialogueManagerScript.Instance.InProgress)
        {
            if (Health < 0)
            {
                DialogueManagerScript.Instance.NatiaDied();
                StartCoroutine(DeathCoroutine());
            }
            else
            {
                DialogueManagerScript.Instance.NatiaFriendlyFireEvent();
            }
        }
    }

    private IEnumerator DeathCoroutine()
    {
        UIManager.Instance.FadeInScreen();
        yield return new WaitForSeconds(2);
        UIManager.Instance.ForceCall = true;
        Destroy(gameObject);
    }
}
