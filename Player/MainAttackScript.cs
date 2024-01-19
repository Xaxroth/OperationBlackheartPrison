using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainAttackScript : MonoBehaviour
{
    [SerializeField] private AudioSource KnifeAudioSource;
    [SerializeField] private AudioClip CrossbowReload;
    [SerializeField] private AudioClip ChargeMagic;
    [SerializeField] private AudioClip FullyCharged;
    [SerializeField] private AudioClip ReleaseMagic;
    [SerializeField] private AudioClip ChannelMagicSound;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Animator cameraAnimator;

    [SerializeField] private GameObject DamageProjectilePrefab;
    [SerializeField] private GameObject UnstableProjectilePrefab;

    [SerializeField] private GameObject DamageProjectileCosmetics;

    [SerializeField] public Transform ShootPosition;
    [SerializeField] private Transform PlayerRotation;

    [SerializeField] private Text ammoDisplay;
    [SerializeField] private Transform Orientation;

    private PlayerControllerScript PlayerInstance;

    private List<GameObject> Projectiles = new List<GameObject>();

    [SerializeField] private int ProjectilePoolAmount = 30;
    private bool UnstableMode = false;
    private bool reloading = false;
    private bool canFire = true;
    private bool toggleJumpMode = false;
    private bool cooldownRoutineStarted = false;
    private bool secondaryFire = false;

    private bool firing = false;

    private bool fullyCharged = false;
    public float chargeAmount = 0.5f;
    public int amountOfProjectilesPerShot = 10;

    private int amountOfKnivesThrown = 1;

    [SerializeField] private float rateOfFire = 0.6f;
    [SerializeField] private int playerAmmo = 12;
    [SerializeField] private float reloadSpeed = 2f;
    [SerializeField] private int playerMaxAmmo = 12;

    void Start()
    {
        ammoDisplay.text = playerAmmo.ToString();
        KnifeAudioSource = gameObject.GetComponent<AudioSource>();
        PlayerInstance = PlayerControllerScript.Instance;
        UnstableMode = true;

        //for (int i = 0; i < ProjectilePoolAmount; i++)
        //{
        //    GameObject currentDamageProjectile = Instantiate(UnstableProjectilePrefab, Orientation.position + new Vector3(0, 1.5f, 0), ShootPosition.rotation);
        //    currentDamageProjectile.GetComponent<MainAttackProjectile>().power = chargeAmount;
        //    Physics.IgnoreCollision(currentDamageProjectile.GetComponent<Collider>(), GetComponent<Collider>());
        //    Projectiles.Add(currentDamageProjectile);
        //    currentDamageProjectile.SetActive(false);
        //    DontDestroyOnLoad(currentDamageProjectile);
        //}
    }

    void Update()
    {
        //if (Input.GetMouseButtonDown(1) && !UnstableMode)
        //{
        //    UnstableMode = true;
        //}
        //else if (Input.GetMouseButtonDown(1) && UnstableMode)
        //{
        //    UnstableMode = false;
        //}

        ShootPosition.transform.rotation = Orientation.transform.rotation;

        PrimaryFire();

        //CheckReload();
    }

    private void PrimaryFire()
    {
        if (Input.GetKeyDown(KeyCode.F) && !firing)
        {
            firing = true;
            PlayerControllerScript.Instance.casting = true;
            StartCoroutine(ChannelMagic());
            //cameraAnimator.SetBool("ExpelMagic", true);
        }

        //if (Input.GetButtonUp("Fire2"))
        //{
        //    firing = false;
        //    StopCoroutine(ChannelMagic());
        //    //cameraAnimator.SetBool("ExpelMagic", false);
        //}
    }

    //private void SecondaryFire()
    //{
    //    if (Input.GetButton("Fire1") && canFire && playerAmmo > 0 && !reloading)
    //    {
    //        if (!secondaryFire)
    //        {
    //            fullyCharged = false;
    //            PlayerControllerScript.Instance.casting = true;
    //            playerAnimator.SetBool("ChargingKnives", true);
    //            //KnifeAudioSource.PlayOneShot(ChargeMagic, 0.3f);
    //            chargeAmount = 0.5f;
    //            secondaryFire = true;
    //        }

    //        if (chargeAmount < 2)
    //        {
    //            //PlayerControllerScript.Instance.playerStamina--;
    //            chargeAmount += 0.005f;
    //        }
    //        else
    //        {
    //            if (fullyCharged == false)
    //            {
    //                KnifeAudioSource.PlayOneShot(FullyCharged);
    //                fullyCharged = true;
    //            }
    //        }
    //    }

    //    if (Input.GetButtonUp("Fire1") && secondaryFire)
    //    {
    //        playerAnimator.SetBool("ChargingKnives", false);
    //        KnifeAudioSource.Stop();
    //        secondaryFire = false;
    //        if (playerAmmo >= 0)
    //        {
    //            ammoDisplay.text = playerAmmo.ToString();
    //        }
    //        playerAnimator.SetBool("ChargingKnives", false);
    //        playerAnimator.SetBool("ReleasingKnives", true);

    //        StartCoroutine(DelayedLoopCoroutine());
    //        PlayerControllerScript.Instance.casting = false;

    //        if (!cooldownRoutineStarted)
    //        {
    //            StartCoroutine(GrenadeCoolDown());
    //            cooldownRoutineStarted = true;
    //        }
    //    }
    //}

    private IEnumerator DelayedLoopCoroutine()
    {
        yield return new WaitForSeconds(1.0f);

        for (int i = 0; i < amountOfKnivesThrown; i++)
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.ReleaseEnergy, 1.0f);
            GameObject currentDamageProjectile = Instantiate(UnstableProjectilePrefab, Orientation.position + new Vector3(0, 1.5f, 0), ShootPosition.rotation);
            currentDamageProjectile.GetComponent<MainAttackProjectile>().power = 1;
            currentDamageProjectile.GetComponent<MainAttackProjectile>().SetRayCast();
            Physics.IgnoreCollision(currentDamageProjectile.GetComponent<Collider>(), GetComponent<Collider>());
            PlayerControllerScript.Instance.casting = false;
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.5f);
        firing = false;
        playerAnimator.SetBool("ReleasingKnives", false);
    }

    private IEnumerator ChannelMagic()
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.ChargeEnergy, 1.0f);
        chargeAmount = 1.0f;
        StartCoroutine(DelayedLoopCoroutine());

        yield return new WaitForSeconds(0.5f);
    }

    //private void CheckReload()
    //{
    //    if (playerAmmo <= 0 && reloading == false && !secondaryFire || Input.GetKeyDown(KeyCode.R) && reloading == false && playerAmmo < playerMaxAmmo && !secondaryFire)
    //    {
    //        reloading = true;
    //        canFire = false;
    //        StartCoroutine(ReloadWeapon());
    //        return;
    //    }
    //}

    //private IEnumerator ReloadWeapon()
    //{
    //    playerAnimator.SetBool("ThrowingKnives", false);
    //    canFire = false;
    //    yield return new WaitForSeconds(0.4f);
    //    KnifeAudioSource.PlayOneShot(CrossbowReload, 0.6f);
    //    yield return new WaitForSeconds(reloadSpeed - 0.1f);
    //    playerAmmo = playerMaxAmmo;
    //    ammoDisplay.text = playerAmmo.ToString();
    //    canFire = true;
    //    reloading = false;
    //}

    //private IEnumerator GrenadeCoolDown()
    //{
    //    canFire = false;
    //    chargeAmount = 0.1f;
    //    yield return new WaitForSeconds(rateOfFire);
    //    cooldownRoutineStarted = false;
    //    canFire = true;
    //}
}
