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

    public Animator NatiaAnimator;

    public Transform headBone;

    public GameObject Armor;
    public GameObject Underwear;
    public GameObject Boots;

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

    public bool Naked;
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
        MoveToNewPosition();
        DistanceCheck();
        HeadTurn();

        if (CurrentEnemyState == NatiaState.Waiting)
        {
            StandStill();
            return;
        }

        if (EnemyNavMeshAgent.velocity.magnitude < 0.15f || CurrentEnemyState == NatiaState.Waiting || EnemyNavMeshAgent.speed == 0)
        {
            NatiaAnimator.SetBool("Walking", false);
        }
        else
        {
            NatiaAnimator.SetBool("Walking", true);
        }


        if (PlayerControllerScript.Instance.CurrentMovementState == PlayerControllerScript.PlayerMovementState.Running)
        {
            EnemyNavMeshAgent.speed = 16;
        }
        else
        {
            EnemyNavMeshAgent.speed = 16;
        }
        if (PlayerControllerScript.Instance != null)
        {
            Quaternion targetRotation = Quaternion.LookRotation(PlayerControllerScript.Instance.transform.position - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 6 * Time.deltaTime);
            transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
        }

    }

    public void ChangeClothes()
    {
        Naked = !Naked;

        if (!Naked)
        {
            Armor.SetActive(true);
            Underwear.SetActive(false);
            Boots.SetActive(true);
        }
        else
        {
            Armor.SetActive(false);
            Underwear.SetActive(true);
            Boots.SetActive(false);
        }
    }

    public void HeadTurn()
    {
        Vector3 playerDirection = PlayerControllerScript.Instance.transform.position - transform.position;
        Vector3 forwardDirection = transform.forward;

        float dotProduct = Vector3.Dot(playerDirection.normalized, forwardDirection);

        if (PlayerControllerScript.Instance.CinemachineCamera.transform == null || headBone == null)
        {
            Debug.LogWarning("Player transform or head bone is not assigned!");
            return;
        }

        if (dotProduct > 0)
        {

            Vector3 directionToPlayer = (PlayerControllerScript.Instance.CinemachineCamera.transform.position - headBone.position).normalized;

            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

            float yRotation = headBone.rotation.eulerAngles.y;

            headBone.rotation = Quaternion.Slerp(headBone.rotation, targetRotation, 5 * Time.deltaTime);

        }

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

    public void StartDialogue()
    {
        StartCoroutine(TalkAnimation());
    }

    private IEnumerator TalkAnimation()
    {
        NatiaAnimator.SetBool("Talking", true);
        yield return new WaitForSeconds(2f);
        NatiaAnimator.SetBool("Talking", false);
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

        Vector3 playerDirection = PlayerControllerScript.Instance.transform.position - transform.position;
        Vector3 forwardDirection = transform.forward;

        float dotProduct = Vector3.Dot(playerDirection.normalized, forwardDirection);

        if (DistanceToPlayer < 25 && dotProduct > 0)
        {
            Debug.Log("Player in front");
            HeadTurn();
        }
        else
        {
            Debug.Log("Player behind");
        }

        DistanceCheck();
    }

    void HandleNatiaState()
    {
        EnemyNavMeshAgent.enabled = true;
        NatiaAnimator.SetBool("Carrying", false);

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
                NatiaAnimator.SetBool("Carrying", true);
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
