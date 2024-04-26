using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Cinemachine;
using UnityEngine;
using System;

public class PlayerControllerScript : MonoBehaviour
{
    public static PlayerControllerScript Instance;

    public enum PlayerMovementState
    {
        Walking,
        Running,
        Crouching,
        Exhausted,
        Blocking,
        Idle,
        Carrying
    }

    public enum WeaponState
    {
        Melee,
        Ranged,
        Utility
    }

    public PlayerMovementState CurrentMovementState;
    public WeaponState CurrentWeaponState;

    [Header("Player Variables")]

    [SerializeField] private ParticleSystem DashLines;

    [SerializeField] public Camera CinemachineCamera;

    [SerializeField] private GameObject RangedWeapon;
    [SerializeField] private GameObject MeleeWeapon;
    [SerializeField] private GameObject Utility;

    [SerializeField] public int playerHealth = 100;
    [SerializeField] public int playerMaxHealth = 100;

    [SerializeField] public int playerStamina = 1000;
    [SerializeField] public int playerMaxStamina = 1000;
    [SerializeField] public float staminaDrainMultiplier = 1f;

    private float playerSpeed = 35f;
    private float normalPlayerSpeed = 35f;
    private float playerMaxSpeed = 15f;
    private float playerHeight = 3f;

    public int playerConstitution;

    [Range(0, 100)][SerializeField] public int MutationLevel;

    float TargetFoV = 90;
    float NormalFoV = 80;
    float FoVIncreaseSpeed = 15;
    float FoVDecreaseSpeed = 3;

    private float normalPlayerDrag = 1.0f;
    private float crouchPlayerDrag = 0.5f;

    private float jumpingPenalty = 12;
    private float TimeBetweenDashes = 0.33f;
    private int DashCost = 20;
    private int JumpCost = 20;
    public float walkSpeed = 7f;
    public float runSpeed = 13f;
    private float crouchSpeed = 3f;
    private float exhaustedSpeed = 4f;

    [SerializeField] private float playerMovementMultiplier = 15f;
    [SerializeField] private float playerAirMultiplier = 1f;

    [SerializeField] private float playerGroundDrag = 8f;
    [SerializeField] private float playerAirDrag = 2f;

    [SerializeField] public float playerJumpHeight = 4f;
    [SerializeField] private float horizontalMovement;
    [SerializeField] private float verticalMovement;

    [SerializeField] private bool soundPlaying = false;
    [SerializeField] public bool casting = false;

    [SerializeField] private float fallHeight;
    [SerializeField] private float landHeight;

    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float normalHeight = 3f;

    KeyCode ButtonPressed;
    KeyCode previousInput = KeyCode.None;

    [Header("Conditions")]

    private float doubleTapTimeThreshold = 0.3f;
    private float lastTapTime = 0f;

    public float DashCooldown = 0.25f;
    private float fallDamageThreshhold = -0.7f;
    public bool QuickDashAvailable;
    public bool crouching = false;
    public bool canJump = true;
    public bool forceCrouch = false;
    public bool Dead = false;
    public bool dashingBackwards = false;
    public bool recovering = false;
    public bool InWater;

    public bool switchingWeapon;
    public bool paralyzed = false;
    public bool Incapacitated;
    public bool sprinting;
    public bool OnGround = false;
    private bool playerFalling = false;
    private bool canDoubleJump = false;
    private bool playerWalking = false;
    private bool hasDoubleJumped = false;
    private bool normalHit = false;
    private bool fallDeath = false;
    public bool AdaptabilityDialogue;

    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private CapsuleCollider playerCollider;

    public GameObject _cameraFollowObject;
    public GameObject _crouchFollowObject;

    public GameObject playerHealthObject;
    public HealthBar playerHealthBar;

    [Header("Animator")]

    public Animator playerAnimator;
    public Animator CameraAnimator;

    [Header("Audio")]

    public AudioSource PlayerAudioSource;

    public AudioClip[] QuickDashSounds;
    public AudioClip[] JumpSounds;
    public AudioClip[] FootstepSounds;
    public AudioClip[] HitSounds;

    public AudioClip readyKnives;
    public AudioClip fallDamageSound;
    public AudioClip Panting;

    [Header("Movement Logic")]

    [SerializeField] private LayerMask layerMask;
    public Transform Orientation;
    public Vector3 direction;
    public Transform playerDirection;
    private Vector3 jumpDirection = new Vector3(0, 5, 0);
    private Vector3 fallDirection = new Vector3(0, -1, 0);
    private float slopeRaycastDistance = 1;

    [Header("Slope Handling")]
    public float MaxSlopeAngle;
    private RaycastHit slopeHit;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        CurrentWeaponState = WeaponState.Melee;
        MeleeWeapon.SetActive(true);
        RangedWeapon.SetActive(false);
        GetComponent<MainAttackScript>().enabled = false;
        GetComponent<MeleeAttack>().enabled = true;

        PlayerAudioSource = gameObject.GetComponent<AudioSource>();
        playerRigidbody = GetComponentInChildren<Rigidbody>();
        playerCollider = gameObject.GetComponent<CapsuleCollider>();

        playerRigidbody.freezeRotation = true;
        playerRigidbody.drag = playerGroundDrag;

        playerHealthBar.SetMaxHealth(playerMaxHealth);
        playerHealthBar.SetMaxStamina(playerMaxStamina);

        normalPlayerSpeed = playerSpeed;
    }

    void Update()
    {
        if (paralyzed || Dead)
        {
            StopAnimation();
            return;
        }
        Crouch();
        SwitchWeapons();
        Interact();
        Animation();
        Controls();
        QuickDashing();
        UIElements();
        Sprinting();
        Jump();
        Conditions();
    }

    private void FixedUpdate()
    {
        if (paralyzed)
        {
            StopAnimation();
            playerRigidbody.AddForce(fallDirection * playerJumpHeight / 6f, ForceMode.Impulse);
            return;
        }
        PhysicsElements();
        Movement();
    }

    private void SwitchWeapons()
    {
        if (!switchingWeapon)
        {

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                CurrentWeaponState = WeaponState.Melee;
                MeleeWeapon.SetActive(true);
                RangedWeapon.SetActive(false);
                Utility.SetActive(false);
                GetComponent<MainAttackScript>().enabled = false;
                GetComponent<MeleeAttack>().enabled = true;
            }

            //if (Input.GetKeyDown(KeyCode.Alpha2))
            //{
            //    CurrentWeaponState = WeaponState.Ranged;
            //    MeleeWeapon.SetActive(false);
            //    RangedWeapon.SetActive(true);
            //    Utility.SetActive(false);
            //    GetComponent<MainAttackScript>().enabled = true;
            //    GetComponent<MeleeAttack>().enabled = false;
            //}

            //if (Input.GetKeyDown(KeyCode.Alpha3))
            //{
            //    CurrentWeaponState = WeaponState.Utility;
            //    MeleeWeapon.SetActive(false);
            //    RangedWeapon.SetActive(false);
            //    Utility.SetActive(true);
            //    GetComponent<MainAttackScript>().enabled = false;
            //    GetComponent<MeleeAttack>().enabled = false;
            //    GetComponent<Flashbang>().enabled = true;
            //}
        }
    }
    private void Interact()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Natia.Instance.CurrentEnemyState != Natia.NatiaState.PickedUp && Natia.Instance != null && Natia.Instance.InConversation)
            {
                Ray ray = new Ray(Orientation.position, Orientation.forward);
                float raycastDistance = 15;
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, raycastDistance))
                {
                    if (hit.collider.CompareTag("Natia") && !DialogueManagerScript.Instance.InProgress)
                    {
                        DialogueManagerScript.Instance.AdaptabilityDialogue();
                    }
                    else if (hit.collider.CompareTag("Natia") && DialogueManagerScript.Instance.InProgress && AdaptabilityDialogue)
                    {
                        DialogueManagerScript.Instance.EndOfDialogue();
                        DialogueManagerScript.Instance.CloseDialogue();
                        Natia.Instance.CurrentEnemyState = Natia.NatiaState.PickedUp;
                        DialogueManagerScript.Instance.NatiaPickedUp();
                    }

                    if (hit.collider.CompareTag("SaveStation"))
                    {
                        AudioManager.Instance.PlaySound(AudioManager.Instance.SaveGame, 1.0f);
                        hit.collider.gameObject.GetComponent<SaveStation>().PlayParticles();
                        SaveStation saveStation = hit.collider.gameObject.GetComponent<SaveStation>();
                        saveStation.ActivateShrine();
                        GameDataHandler.Instance.SaveEssentialsData();
                    }

                    if (hit.collider.CompareTag("Lever"))
                    {
                        if (hit.collider.GetComponent<Levers>())
                        {
                            hit.collider.GetComponent<Levers>().PullLever();
                        }
                        else if (hit.collider.GetComponent<DisableTraps>())
                        {
                            hit.collider.GetComponent<DisableTraps>().PullLever();
                        }

                    }

                    if (hit.collider.CompareTag("Zekrael"))
                    {
                        DialogueManagerScript.Instance.ZekraelFirstDialogue();
                    }

                    if (hit.collider.CompareTag("Door"))
                    {
                        hit.collider.GetComponent<Door>().ChangeScene();
                    }

                    if (hit.collider.CompareTag("RandomDoor"))
                    {
                        hit.collider.GetComponent<RandomDoor>().ChangeScene();
                    }

                    if (hit.collider.CompareTag("Chest"))
                    {
                        hit.collider.GetComponent<Chest>().OpenChest();
                    }
                }
            }
            else if (Natia.Instance.CurrentEnemyState == Natia.NatiaState.PickedUp && Natia.Instance != null)
            {
                Natia.Instance.CurrentEnemyState = Natia.NatiaState.Following;
                Natia.Instance.NatiaCollider.enabled = true;
                Natia.Instance.gameObject.transform.position = gameObject.transform.position + new Vector3(-5, 0, 0);
                DialogueManagerScript.Instance.NatiaDropped();
            }
        }
    }

    private void Conditions()
    {
        OnGround = Physics.Raycast(transform.position, Vector3.down, playerHeight / 2.1f, layerMask);

        if (Natia.Instance != null && Natia.Instance.CurrentEnemyState == Natia.NatiaState.PickedUp)
        {
            CurrentMovementState = PlayerMovementState.Carrying;
        }

        switch (CurrentMovementState)
        {
            case PlayerMovementState.Walking:
                playerCollider.height = normalHeight;
                playerAirDrag = normalPlayerDrag;
                playerSpeed = walkSpeed;
                break;
            case PlayerMovementState.Running:
                playerCollider.height = normalHeight;
                playerAirDrag = normalPlayerDrag;
                playerSpeed = runSpeed;
                break;
            case PlayerMovementState.Crouching:
                playerCollider.height = crouchHeight;
                playerAirDrag = crouchPlayerDrag;
                playerSpeed = crouchSpeed;
                break;
            case PlayerMovementState.Exhausted:
                playerCollider.height = normalHeight;
                playerAirDrag = normalPlayerDrag;
                playerSpeed = exhaustedSpeed;
                break;
            case PlayerMovementState.Blocking:
                playerCollider.height = normalHeight;
                playerAirDrag = normalPlayerDrag;
                playerSpeed = walkSpeed;
                break;
            case PlayerMovementState.Idle:
                break;
            case PlayerMovementState.Carrying:
                playerCollider.height = normalHeight;
                playerAirDrag = normalPlayerDrag;
                playerSpeed = exhaustedSpeed * 2;
                break;

        }

        if (Natia.Instance != null)
        {
            if (Natia.Instance.CurrentEnemyState == Natia.NatiaState.PickedUp)
            {
                Incapacitated = true;
            }
            else
            {
                Incapacitated = false;
            }
        }


        if (horizontalMovement != 0 && OnGround || verticalMovement != 0 && OnGround)
        {
            if (!Input.GetButton("Sprint"))
            {
                CameraAnimator.SetBool("Walking", true);
                sprinting = false;
            }
            else
            {
                if (CurrentMovementState == PlayerMovementState.Running)
                {
                    CameraAnimator.SetBool("Walking", false);
                }

                sprinting = true;
            }

            playerWalking = true;
            gameObject.GetComponent<CameraShakeController>().active = true;

            if (!Input.GetButton("Crouch"))
            {
                gameObject.GetComponent<CameraShakeController>().running = true;
            }
            else
            {
                gameObject.GetComponent<CameraShakeController>().running = false;
            }
        }
        else
        {
            CameraAnimator.SetBool("Walking", false);
        }

    }
    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < MaxSlopeAngle && angle != 0;
        }
        return false;
    }
    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    public void UIElements()
    {
        playerHealthBar.SetStamina(playerStamina);
        playerHealthBar.SetHealth(playerHealth);
    }

    public void PhysicsElements()
    {
        if (OnGround == true)
        {
            playerRigidbody.drag = playerGroundDrag;
        }
        else
        {
            playerRigidbody.drag = playerAirDrag;
        }

        if (playerRigidbody.velocity.y < 0 && !OnGround)
        {
            playerFalling = true;
            fallHeight = transform.position.y;
        }

        if (OnGround == true && playerFalling && !Dead)
        {
            landHeight = transform.position.y - fallHeight;

            StartCoroutine(FallDamage());

            if (landHeight < fallDamageThreshhold)
            {
                PlayerAudioSource.PlayOneShot(fallDamageSound);
                TakeDamage((int)(landHeight * -10));
                playerFalling = false;
            }
            else
            {
                playerFalling = false;
            }
        }
    }

    private void Controls()
    {
        // GET PLAYER DIRECTION

        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        // IF THE PLAYER ISN'T RUNNING, WE WILL ALLOW MOVEMENT ON THE VERTICAL PLANE. IN A SPRINT, PLAYERS CAN ONLY MOVE FORWARD.

        if (CurrentMovementState != PlayerMovementState.Running)
        {
            direction = Orientation.forward * verticalMovement + Orientation.right * horizontalMovement;
        }
        else
        {
            // IF THE PLAYER TRIES MOVING BACKWARDS, STOP THEM

            if (verticalMovement <= 0)
            {
                verticalMovement = 0;
            }

            direction = Orientation.forward * verticalMovement + Orientation.right * 0;
        }
    }

    private void Crouch()
    {
        if (Input.GetButton("Crouch"))
        {
            crouching = true;

            CurrentMovementState = PlayerMovementState.Crouching;

            if (OnGround)
            {
                playerRigidbody.AddForce(fallDirection * playerJumpHeight / 4f, ForceMode.Impulse);
            }
        }
        else
        {
            crouching = false;
        }
    }

    public void QuickDashing()
    {
        if (!dashingBackwards)
        {
            if (DashCooldown > 0 || CurrentMovementState == PlayerMovementState.Running)
            {
                DashCooldown -= Time.deltaTime;
                CinemachineCamera.fieldOfView = Mathf.Clamp(Mathf.Lerp(CinemachineCamera.fieldOfView, NormalFoV, FoVIncreaseSpeed * Time.deltaTime), NormalFoV, TargetFoV);
            }
            else
            {
                CinemachineCamera.fieldOfView = Mathf.Clamp(Mathf.Lerp(CinemachineCamera.fieldOfView, TargetFoV, FoVDecreaseSpeed * Time.deltaTime), NormalFoV, TargetFoV);
            }
        }
        else
        {
            if (DashCooldown > 0)
            {
                DashCooldown -= Time.deltaTime;
                CinemachineCamera.fieldOfView = Mathf.Clamp(Mathf.Lerp(CinemachineCamera.fieldOfView, TargetFoV, FoVIncreaseSpeed * Time.deltaTime), NormalFoV, TargetFoV);
            }
            else
            {
                CinemachineCamera.fieldOfView = Mathf.Clamp(Mathf.Lerp(CinemachineCamera.fieldOfView, NormalFoV, FoVDecreaseSpeed * Time.deltaTime), NormalFoV, TargetFoV);
            }
        }

        if (!Input.GetKeyDown(KeyCode.W) && !Input.GetKeyDown(KeyCode.A) && !Input.GetKeyDown(KeyCode.S) && !Input.GetKeyDown(KeyCode.D))
        {
            if (CurrentMovementState != PlayerMovementState.Crouching)
            {
                CurrentMovementState = PlayerMovementState.Idle;
            }
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            if (previousInput == KeyCode.W)
            {
                float timeSinceLastTap = Time.time - lastTapTime;

                if (timeSinceLastTap <= doubleTapTimeThreshold && DashCooldown <= 0)
                {
                    PerformDash();
                    dashingBackwards = false;
                }
            }

            lastTapTime = Time.time;
            previousInput = KeyCode.W;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (previousInput == KeyCode.S)
            {
                float timeSinceLastTap = Time.time - lastTapTime;

                if (timeSinceLastTap <= doubleTapTimeThreshold && DashCooldown <= 0)
                {
                    PerformDash();
                    dashingBackwards = true;
                }
            }

            lastTapTime = Time.time;
            previousInput = KeyCode.S;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            if (previousInput == KeyCode.A)
            {
                float timeSinceLastTap = Time.time - lastTapTime;

                if (timeSinceLastTap <= doubleTapTimeThreshold && DashCooldown <= 0)
                {
                    PerformDash();
                    dashingBackwards = false;
                }
            }

            lastTapTime = Time.time;
            previousInput = KeyCode.A;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            if (previousInput == KeyCode.D)
            {
                float timeSinceLastTap = Time.time - lastTapTime;

                if (timeSinceLastTap <= doubleTapTimeThreshold && DashCooldown <= 0)
                {
                    PerformDash();
                    dashingBackwards = false;
                }
            }

            lastTapTime = Time.time;
            previousInput = KeyCode.D;
        }
    }

    public void Sprinting()
    {
        // IF THE PLAYER IS IN A NORMAL STATE AND HAS THE REQUIRED AMOUNT OF STAMINA, BURST INTO A SPRINT

        if (Input.GetButton("Sprint") && CurrentMovementState != PlayerMovementState.Crouching && Input.GetKey(KeyCode.W) && playerStamina > 0 && CurrentMovementState != PlayerMovementState.Exhausted && !casting)
        {
            CurrentMovementState = PlayerMovementState.Running;
            sprinting = true;

            if (OnGround)
            {
                CameraAnimator.SetBool("CameraRunning", true);
            }
            else
            {
                CameraAnimator.SetBool("CameraRunning", false);
            }
        }
        else
        {
            // AUTO EXIT SPRINTING IF THE CONDITIONS ARE NOT MET

            sprinting = false;

            if (playerStamina > 0 && !casting)
            {
                if (!crouching)
                {
                    CurrentMovementState = PlayerMovementState.Walking;
                }
            }
            else
            {
                if (!recovering)
                {
                    StartCoroutine(Recover());
                }

                CurrentMovementState = PlayerMovementState.Exhausted;
            }

            CameraAnimator.SetBool("CameraRunning", false);
            CameraAnimator.SetBool("Walking", true);
        }
    }


    private void Movement()
    {
        // CALLED WHEN THE PLAYER MOVES - CHECKS IF THE PLAYER IS ON GROUND OR NOT AND APPLIES THE CORRECT FORCE.
        if (OnGround == true)
        {
            hasDoubleJumped = false;
            canDoubleJump = false;

            if (OnSlope() == true)
            {
                playerRigidbody.useGravity = false;
                playerRigidbody.AddForce(GetSlopeMoveDirection() * playerSpeed * playerMovementMultiplier, ForceMode.Acceleration);
            }
            else
            {
                playerRigidbody.useGravity = true;
                playerRigidbody.AddForce(direction.normalized * playerSpeed * playerMovementMultiplier, ForceMode.Acceleration);
            }
        }
        else
        {
            // MAKE THE PLAYER FALL TOWARDS THE GROUND, THE PLAYERAIRMULTIPLIER IS USED TO LIMIT MOMENTUM IN MIDAIR.
            canDoubleJump = true;

            playerRigidbody.AddForce(direction.normalized * (playerSpeed * 0.7f) * playerMovementMultiplier * playerAirMultiplier, ForceMode.Acceleration);
            playerRigidbody.AddForce(fallDirection * playerJumpHeight / 6f, ForceMode.Impulse);
        }

        // IF THE PLAYER IS NOT RUNNING, STAMINA RECOVERS.
        if (CurrentMovementState != PlayerMovementState.Running)
        {
            if (playerStamina < playerMaxStamina && CurrentMovementState != PlayerMovementState.Exhausted)
            {
                playerStamina++;
            }
        }
        else
        {
            if (playerStamina > 0)
            {
                playerStamina -= 1;
            }
            else
            {
                CurrentMovementState = PlayerMovementState.Exhausted;
            }
        }
    }

    private IEnumerator Recover()
    {
        recovering = true;
        playerStamina = 0;
        yield return new WaitForSeconds(3);
        playerStamina = 1;
        recovering = false;
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && OnGround && !paralyzed && canJump && playerStamina > JumpCost && CurrentMovementState != PlayerMovementState.Crouching)
        {
            StartCoroutine(AnimatorCoroutine());

            PlayerAudioSource.PlayOneShot(JumpSounds[UnityEngine.Random.Range(0, JumpSounds.Length)]);
            playerStamina -= JumpCost;

            playerRigidbody.velocity = Vector3.zero;
            playerRigidbody.AddForce(jumpDirection * playerJumpHeight, ForceMode.Impulse);
            playerRigidbody.AddForce(direction.normalized * (playerSpeed / jumpingPenalty) * playerMovementMultiplier, ForceMode.Impulse);
        }
    }

    private void PerformDash()
    {
        if (OnGround)
        {
            DashLines.Play();
            PlayerAudioSource.PlayOneShot(QuickDashSounds[UnityEngine.Random.Range(0, 2)]);

            previousInput = KeyCode.None;
            playerStamina -= DashCost;
            DashCooldown = TimeBetweenDashes;
            direction = Orientation.forward * verticalMovement + Orientation.right * horizontalMovement;
            playerRigidbody.AddForce(direction.normalized * (playerSpeed * 2f) * playerMovementMultiplier * playerAirMultiplier, ForceMode.Impulse);

            QuickDashAvailable = false;
        }
    }

    public void TakeDamage(int damage)
    {
        if (!Dead)
        {
            PlayerAudioSource.PlayOneShot(HitSounds[UnityEngine.Random.Range(0, HitSounds.Length)], 0.7f);
            playerHealth -= damage;
            playerHealthBar.SetHealth(playerHealth);
            normalHit = true;
        }
        if (playerHealth <= 0)
        {
            StartCoroutine(DeathCoroutine());
        }
    }

    public void Animation()
    {
        switch (CurrentMovementState)
        {
            case PlayerMovementState.Walking:
                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
                {
                    playerAnimator.SetBool("Running", false);
                    playerAnimator.SetBool("Walking", true);
                }
                else
                {
                    playerAnimator.SetBool("Running", false);
                    playerAnimator.SetBool("Walking", false);
                }
                break;
            case PlayerMovementState.Running:
                playerAnimator.SetBool("Walking", false);
                playerAnimator.SetBool("Running", true);
                break;
        }
    }

    public void StopAnimation()
    {
        CameraAnimator.SetBool("CameraRunning", false);
        CameraAnimator.SetBool("Walking", false);
        CameraAnimator.SetBool("HitGround", false);
    }

    private IEnumerator AnimatorCoroutine()
    {
        CameraAnimator.SetBool("CameraJump", true);
        yield return new WaitForSeconds(0.1f);
        CameraAnimator.SetBool("CameraJump", false);
    }

    private IEnumerator QuickDash()
    {
        QuickDashAvailable = true;
        yield return new WaitForSeconds(0.25f);
        QuickDashAvailable = false;
    }

    private IEnumerator FallDamage()
    {
        CameraAnimator.SetBool("HitGround", true);
        yield return new WaitForSeconds(0.1f);
        CameraAnimator.SetBool("HitGround", false);
    }

    private IEnumerator DeathCoroutine()
    {
        if (!Dead)
        {
            Dead = true;
            CameraAnimator.SetBool("Death", true);

            playerRigidbody.velocity = Vector3.zero;
            playerRigidbody.isKinematic = true;

            UIManager.Instance.FadeInScreen();
            AudioManager.Instance.BGMAudioSource.Stop();
            AudioManager.Instance.PlaySound(AudioManager.Instance.HaliconDeath, 1.0f);

            yield return new WaitForSeconds(3f);

            AudioManager.Instance.PlaySound(AudioManager.Instance.MissionFailed, 1.0f);
            UIManager.Instance.GameOverSceen("You died.");
            UIManager.Instance.FadeOutScreen();
            UIManager.Instance.ForceCall = true;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            PlayerAudioSource.PlayOneShot(AudioManager.Instance.BigWaterSplash, 0.5f);
            InWater = true;
        }

    }

    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            // splash splosh
            InWater = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            // splash splosh
            InWater = false;
        }
    }

    public bool IsInWater()
    {
        return InWater;
    }
}
