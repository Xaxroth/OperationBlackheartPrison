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

    private int numberOfBulletsPerShot = 1;

    [SerializeField] private float rateOfFire = 0.6f;
    [SerializeField] private int playerAmmo = 12;
    [SerializeField] private float reloadSpeed = 2f;
    [SerializeField] private int playerMaxAmmo = 12;
    [SerializeField] private int projectileForce = 25;

    public List<GameObject> projectilePool { get; private set; }

    void Start()
    {
        InitializeProjectilePool();
        ammoDisplay.text = playerAmmo.ToString();
        KnifeAudioSource = gameObject.GetComponent<AudioSource>();
        PlayerInstance = PlayerControllerScript.Instance;
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            firing = true;
        }
        else
        {
            firing = false;
        }

        ShootPosition.transform.rotation = Orientation.transform.rotation;

        PrimaryFire();

        CheckReload();
    }

    private void InitializeProjectilePool()
    {
        projectilePool = new List<GameObject>();

        for (int i = 0; i < numberOfBulletsPerShot * 4; i++)
        {
            GameObject projectile = Instantiate(UnstableProjectilePrefab, gameObject.transform.position, gameObject.transform.rotation);
            DontDestroyOnLoad(projectile);
            projectile.SetActive(false);
            projectilePool.Add(projectile);
        }
    }

    private void PrimaryFire()
    {
        if (Input.GetMouseButtonDown(0) && canFire)
        {
            PlayerControllerScript.Instance.casting = true;
            StartCoroutine(FireShotgun());
        }
    }

    private IEnumerator FireShotgun()
    {
        while (firing)
        {
            canFire = false;
            AudioManager.Instance.PlaySound(AudioManager.Instance.ReleaseEnergy, 1.0f);

            playerAmmo--;

            for (int i = 0; i < amountOfProjectilesPerShot; i++)
            {
                GameObject currentProjectile = GetPooledProjectile();

                currentProjectile.GetComponent<MainAttackProjectile>().Activate();
                currentProjectile.transform.position = ShootPosition.transform.position + new Vector3(0, 2f, 0);
                currentProjectile.transform.rotation = ShootPosition.transform.rotation;

                currentProjectile.GetComponent<MainAttackProjectile>().power = projectileForce;
                currentProjectile.GetComponent<MainAttackProjectile>().SetRayCast();
                Physics.IgnoreCollision(currentProjectile.GetComponent<Collider>(), GetComponent<Collider>());
                PlayerControllerScript.Instance.casting = false;
            }

            yield return new WaitForSeconds(rateOfFire * 3);
            canFire = true;
        }
    }

    private GameObject GetPooledProjectile()
    {
        // WHY IS THIS CAUSING PROBLEMS

        //for (int i = 0; i < projectilePool.Count; i++)
        //{
        //    if (!projectilePool[i].activeInHierarchy)
        //    {
        //        return projectilePool[i];
        //    }
        //}

        GameObject newProjectile = Instantiate(UnstableProjectilePrefab);
        newProjectile.SetActive(false);
        projectilePool.Add(newProjectile);

        return newProjectile;
    }

    private void CheckReload()
    {
        if (playerAmmo <= 0 && reloading == false && !secondaryFire || Input.GetKeyDown(KeyCode.R) && reloading == false && playerAmmo < playerMaxAmmo && !secondaryFire)
        {
            reloading = true;
            canFire = false;
            StartCoroutine(ReloadWeapon());
            return;
        }
    }

    private IEnumerator ReloadWeapon()
    {
        canFire = false;
        yield return new WaitForSeconds(0.4f);
        KnifeAudioSource.PlayOneShot(CrossbowReload, 0.6f);
        yield return new WaitForSeconds(reloadSpeed - 0.1f);
        playerAmmo = playerMaxAmmo;
        ammoDisplay.text = playerAmmo.ToString();
        canFire = true;
        reloading = false;
    }
}
