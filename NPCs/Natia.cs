using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Natia : MonoBehaviour
{
    public static Natia Instance;

    [Header("Emotions")]

    [Range(0, 100)]
    public int Affection;
    [Range(0, 100)]
    public int Sanity;

    [Header("Logistics")]

    public Transform PlayerTransform;
    public AudioSource EnemyAudioSource;
    public NavMeshAgent EnemyNavMeshAgent;
    public Collider NatiaCollider;

    public enum NatiaState
    {
        Waiting,
        Following,
        Cautious,
        Relaxed,
        Lockpicking,
        PickedUp
    }

    public enum AffectionLevel
    {
        Enemy,
        Rival,
        Stranger,
        Acquaintance,
        Friend,
        Partner,
        Lover
    }

    public event Action<AffectionLevel> OnStateChanged;

    public NatiaState CurrentEnemyState;
    public AffectionLevel CurrentAffectionLevel;

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
        HandleNatiaState();
        CheckNatiaAffection();
        IncreaseNatiaAffection();

        if (CurrentEnemyState == NatiaState.Waiting)
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

    void CheckNatiaAffection()
    {
        if (Affection <= 20)
        {
            SetAffectionLevel(AffectionLevel.Enemy);
        }
        else if (Affection <= 35)
        {
            SetAffectionLevel(AffectionLevel.Rival);
        }
        else if (Affection <= 50)
        {
            SetAffectionLevel(AffectionLevel.Stranger);
        }
        else if (Affection <= 65)
        {
            SetAffectionLevel(AffectionLevel.Acquaintance);
        }
        else if (Affection <= 80)
        {
            SetAffectionLevel(AffectionLevel.Friend);
        }
        else if (Affection < 100)
        {
            SetAffectionLevel(AffectionLevel.Partner);
        }
        else if (Affection >= 100)
        {
            SetAffectionLevel(AffectionLevel.Lover);
        }
        else
        {
            Debug.LogError("AffectionLevel not valid.");
        }
    }

    void SetAffectionLevel(AffectionLevel NewAffectionLevel)
    {
        if (NewAffectionLevel != CurrentAffectionLevel)
        {
            CurrentAffectionLevel = NewAffectionLevel;
            CheckStateChange(CurrentAffectionLevel);
        }
    }

    void IncreaseNatiaAffection()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Affection++;
            CheckNatiaAffection();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Affection--;
            CheckNatiaAffection();
        }
    }
    
    void CheckStateChange(AffectionLevel NewAffectionLevel)
    {
        if (DialogueManagerScript.Instance != null)
        {
            DialogueManagerScript.Instance.NatiaChangedAffectionDialogue(NewAffectionLevel);
        }
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

    void HandleNatiaState()
    {
        EnemyNavMeshAgent.enabled = true;

        switch (CurrentEnemyState)
        {
            case NatiaState.Waiting:
                EnemyNavMeshAgent.stoppingDistance = 10;
                break;
            case NatiaState.Following:
                EnemyNavMeshAgent.stoppingDistance = 8;
                EnemyNavMeshAgent.destination = PlayerControllerScript.Instance.gameObject.transform.position;
                break;
            case NatiaState.Relaxed:
                EnemyNavMeshAgent.stoppingDistance = 15;
                EnemyNavMeshAgent.destination = PlayerControllerScript.Instance.gameObject.transform.position;
                break;
            case NatiaState.Cautious:
                EnemyNavMeshAgent.stoppingDistance = 8;
                EnemyNavMeshAgent.destination = PlayerControllerScript.Instance.gameObject.transform.position;
                break;
            case NatiaState.Lockpicking:
                EnemyNavMeshAgent.stoppingDistance = 3;
                break;
            case NatiaState.PickedUp:
                NatiaCollider.enabled = false;
                EnemyNavMeshAgent.stoppingDistance = 0;
                EnemyNavMeshAgent.enabled = false;
                gameObject.transform.position = PlayerControllerScript.Instance.gameObject.transform.position;
                break;
        }
    }

    void MoveToNewPosition()
    {
        if (CanMove && !OpeningDoor && CurrentEnemyState != NatiaState.PickedUp)
        {
            EnemyNavMeshAgent.destination = PlayerTransform.transform.position;
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

        CurrentEnemyState = NatiaState.Following;

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
