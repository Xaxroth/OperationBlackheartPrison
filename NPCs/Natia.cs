using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Natia : MonoBehaviour
{
    public static Natia Instance;

    [Header("Emotions")]

    [Range(0, 100)]
    public int Affection;
    [Range(0, 100)]
    public int Intimidation;
    [Range(0, 200)]
    public int Health = 200;

    public int MaxHealth = 200;
    public int MaxAffection = 100;
    public int MaxIntimidation = 100;

    [Header("UI Elements")]
    public Slider HealthSlider;
    public Slider AffectionSlider;
    public Slider IntimidationSlider;

    [Header("Logistics")]

    public Transform PlayerTransform;
    public AudioSource EnemyAudioSource;
    public NavMeshAgent NatiaNavMeshAgent;
    public Collider NatiaCollider;

    public Animator NatiaAnimator;

    public Transform headBone;

    [Header("Armor")]
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
        PickedUp,
        Dead
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

    public float DistanceToPlayer = 0;
    float refreshRate = 0.5f;
    float MovementSpeed = 9f;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        NatiaNavMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        EnemyAudioSource = gameObject.GetComponent<AudioSource>();

        SetUIElements(MaxHealth, MaxAffection, MaxIntimidation);
    }

    void Update()
    {
        if (!Event1Trigger.EventFinished)
        {
            return;
        }

        HandleNatiaState();
        CheckNatiaAffection();
        IncreaseNatiaAffection();
        MoveToNewPosition();
        DistanceCheck();
        HeadTurn();
        UpdateUIValues();

        if (CurrentEnemyState == NatiaState.Waiting || InConversation)
        {
            StandStill();
            return;
        }

        HandleMovement();
        Rotation();

    }

    public void PlayDialogueSound(AudioClip clip)
    {
        EnemyAudioSource.PlayOneShot(clip);
    }

    public void SetUIElements(int maxHealth, int maxAffection, int maxIntimidation)
    {
        HealthSlider.maxValue = maxHealth;
        HealthSlider.value = maxHealth;

        AffectionSlider.maxValue = maxAffection;
        AffectionSlider.value = maxAffection;

        IntimidationSlider.maxValue = maxIntimidation;
        IntimidationSlider.value = maxIntimidation;
    }

    public void UpdateUIValues()
    {
        HealthSlider.value = Health;
        AffectionSlider.value = Affection;
        IntimidationSlider.value = Intimidation;
    }

    void HandleNatiaState()
    {
        NatiaNavMeshAgent.enabled = true;
        NatiaAnimator.SetBool("Carrying", false);

        switch (CurrentEnemyState)
        {
            case NatiaState.Waiting:
                NatiaNavMeshAgent.stoppingDistance = 10;
                NatiaAnimator.SetBool("Walking", false);
                break;
            case NatiaState.Following:
                NatiaNavMeshAgent.stoppingDistance = 8;
                NatiaNavMeshAgent.destination = PlayerControllerScript.Instance.gameObject.transform.position;
                break;
            case NatiaState.Relaxed:
                NatiaNavMeshAgent.stoppingDistance = 15;
                NatiaNavMeshAgent.destination = PlayerControllerScript.Instance.gameObject.transform.position;
                break;
            case NatiaState.Cautious:
                NatiaNavMeshAgent.stoppingDistance = 8;
                NatiaNavMeshAgent.destination = PlayerControllerScript.Instance.gameObject.transform.position;
                break;
            case NatiaState.Lockpicking:
                NatiaNavMeshAgent.stoppingDistance = 3;
                break;
            case NatiaState.PickedUp:
                NatiaCollider.enabled = false;
                NatiaNavMeshAgent.stoppingDistance = 0;
                NatiaNavMeshAgent.enabled = false;
                gameObject.transform.position = PlayerControllerScript.Instance.gameObject.transform.position;
                NatiaAnimator.SetBool("Carrying", true);
                break;
            case NatiaState.Dead:
                NatiaCollider.enabled = false;
                NatiaNavMeshAgent.stoppingDistance = 0;
                NatiaNavMeshAgent.speed = 0;
                NatiaNavMeshAgent.enabled = false;
                NatiaAnimator.SetBool("Dead", true);
                break;
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

    public void HandleMovement()
    {
        if (NatiaNavMeshAgent.velocity.magnitude < 0.15f || CurrentEnemyState == NatiaState.Waiting || NatiaNavMeshAgent.speed == 0)
        {
            NatiaAnimator.SetBool("Walking", false);
        }
        else
        {
            NatiaAnimator.SetBool("Walking", true);
        }
    }

    public void HeadTurn()
    {
        Vector3 playerDirection = PlayerControllerScript.Instance.transform.position - transform.position;
        Vector3 forwardDirection = transform.forward;

        float dotProduct = Vector3.Dot(playerDirection.normalized, forwardDirection);

        if (dotProduct > 0)
        {

            Vector3 directionToPlayer = (PlayerControllerScript.Instance.CinemachineCamera.transform.position - headBone.position).normalized;

            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

            float yRotation = headBone.rotation.eulerAngles.y;

            headBone.rotation = Quaternion.Slerp(headBone.rotation, targetRotation, 5 * Time.deltaTime);

        }

    }

    public void Rotation()
    {
        if (PlayerControllerScript.Instance != null)
        {
            Quaternion targetRotation = Quaternion.LookRotation(PlayerControllerScript.Instance.transform.position - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 6 * Time.deltaTime);
            transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
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
        NatiaAnimator.SetBool("Walking", false);
        NatiaAnimator.SetBool("Talking", true);
        InConversation = true;
        yield return new WaitForSeconds(0.5f);
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
        NatiaNavMeshAgent.destination = gameObject.transform.position;
        CanMove = false;

        Vector3 playerDirection = PlayerControllerScript.Instance.transform.position - transform.position;
        Vector3 forwardDirection = transform.forward;

        float dotProduct = Vector3.Dot(playerDirection.normalized, forwardDirection);

        if (DistanceToPlayer < 25 && dotProduct > 0)
        {
            HeadTurn();
        }

        DistanceCheck();
    }


    void MoveToNewPosition()
    {
        if (CanMove && !OpeningDoor && CurrentEnemyState != NatiaState.PickedUp)
        {
            NatiaNavMeshAgent.destination = PlayerTransform.transform.position;
        }

        if (OpeningDoor && NatiaNavMeshAgent.remainingDistance < 3f && DoorToOpen != null && !StartedLockpicking)
        {
            StartCoroutine(PickLock());
        }

        if (OpeningDoor && NatiaNavMeshAgent.remainingDistance < 3f && ChestToOpen != null && !StartedLockpicking)
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
            ChestToOpen.gameObject.GetComponent<Animator>().SetBool("Opened", true);
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
            NatiaNavMeshAgent.stoppingDistance = 1;
            NatiaNavMeshAgent.destination = Door.transform.position;
            DoorToOpen = Door.GetComponent<Door>();
        }
    }

    public void OpenChest(GameObject Chest)
    {
        if (CanMove)
        {
            OpeningDoor = true;
            NatiaNavMeshAgent.stoppingDistance = 1;
            NatiaNavMeshAgent.destination = Chest.transform.position;
            ChestToOpen = Chest.GetComponent<Chest>();
        }
    }

    public void TakeDamage(float Damage)
    {
        Health -= (int)Damage;

        if (Health <= 0 && !Dead)
        {
            StartCoroutine(DeathCoroutine());
        }

        if (!DialogueManagerScript.Instance.InProgress)
        {
            if (Health < 0 && !Dead)
            {
                DialogueManagerScript.Instance.NatiaDied();
                StartCoroutine(DeathCoroutine());
            }
        }
    }

    private IEnumerator DeathCoroutine()
    {
        Dead = true;
        CurrentEnemyState = NatiaState.Dead;
        EnemyAudioSource.PlayOneShot(AudioManager.Instance.NatiaDeath, 1.0f);

        UIManager.Instance.FadeInScreen();
        DialogueManagerScript.Instance.EndOfDialogue();
        DialogueManagerScript.Instance.CloseDialogue();
        AudioManager.Instance.BGMAudioSource.Stop();

        yield return new WaitForSeconds(4);

        AudioManager.Instance.PlaySound(AudioManager.Instance.MissionFailed, 1.0f);
        UIManager.Instance.FadeOutScreen();
        UIManager.Instance.GameOverSceen("Natia is dead.");
        UIManager.Instance.ForceCall = true;
    }
}
