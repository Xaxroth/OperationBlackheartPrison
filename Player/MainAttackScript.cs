using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainAttackScript : MonoBehaviour
{
    private PlayerControllerScript PlayerInstance;
    public List<GameObject> projectilePool = new List<GameObject>();
    public List<GameObject> explosivePool = new List<GameObject>();
    private List<GameObject> Projectiles = new List<GameObject>();
    [SerializeField] private int ProjectilePoolAmount = 30;

    [SerializeField] private AudioSource AudioSource;
    [SerializeField] private AudioClip Reload;
    [SerializeField] private GameObject ProjectilePrefab;
    [SerializeField] private GameObject ExplosionPrefab;
    [SerializeField] private GameObject ShootEffect;
    [SerializeField] private Transform ShootPosition;
    [SerializeField] public Light ProjectilePointLight;
    [SerializeField] private Text ammoDisplay;
    [SerializeField] private Transform Orientation;
    [SerializeField] private TextMeshProUGUI AmmoText;
    public float Ammo = 10;
    public float MaxAmmo = 20;
    public ParticleSystem MuzzleFlash;

    private bool reloading = false;
    private bool canFire = true;

    [SerializeField] private bool firing = false;
    [SerializeField] private int amountOfProjectilesPerShot = 10;
    [SerializeField] private int numberOfBulletsPerShot = 1;
    [SerializeField] private float rateOfFire = 0.6f;
    [SerializeField] private int playerAmmo = 1;
    [SerializeField] private float reloadSpeed = 2f;
    [SerializeField] private int playerMaxAmmo = 12;
    [SerializeField] private int projectileForce = 25;

    public Animator PlayerArms;

    void Start()
    {
        InitializeProjectilePool();
        ammoDisplay.text = playerAmmo.ToString();
        AudioSource = gameObject.GetComponent<AudioSource>();
        PlayerInstance = PlayerControllerScript.Instance;
    }

    public void OnEnable()
    {
        StartCoroutine(DrawAnimation());
        AudioManager.Instance.PlaySound(AudioManager.Instance.MoveItem, 0.5f);
        Debug.Log("RAAWRGH");
    }

    public IEnumerator DrawAnimation()
    {
        PlayerControllerScript.Instance.switchingWeapon = true;
        PlayerArms.SetBool("PullOut", true);
        yield return new WaitForSeconds(0.5f);
        PlayerControllerScript.Instance.switchingWeapon = false;
        PlayerArms.SetBool("PullOut", false);
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
        for (int i = 0; i < numberOfBulletsPerShot * 30; i++)
        {
            GameObject projectile = Instantiate(ProjectilePrefab, gameObject.transform.position, gameObject.transform.rotation);
            DontDestroyOnLoad(projectile);
            projectile.SetActive(false);
            projectilePool.Add(projectile);
        }

        for (int i = 0; i < numberOfBulletsPerShot * 30; i++)
        {
            GameObject projectile = Instantiate(ExplosionPrefab, gameObject.transform.position, gameObject.transform.rotation);
            DontDestroyOnLoad(projectile);
            projectile.SetActive(false);
            explosivePool.Add(projectile);
        }
    }

    private void PrimaryFire()
    {
        if (Input.GetMouseButtonDown(0) && canFire)
        {
            if (SetAmmo() > 0)
            {
                StartCoroutine(FireShotgun());
            }
            else
            {
                AudioManager.Instance.PlaySound(AudioManager.Instance.NoAmmo, 0.25f);
            }
        }
    }

    IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        float startIntensity = 35;

        while (elapsedTime < 0.35f)
        {
            float currentIntensity = Mathf.Lerp(startIntensity, 0f, elapsedTime / 0.35f);

            ProjectilePointLight.intensity = currentIntensity;

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        ProjectilePointLight.intensity = 0f;
    }

    private IEnumerator FireShotgun()
    {
        while (firing && SetAmmo() > 0)
        {
            canFire = false;
            StartCoroutine(ShotgunVFX());
            AudioManager.Instance.PlaySound(AudioManager.Instance.ReleaseEnergy, 1.0f);

            for (int i = 0; i < amountOfProjectilesPerShot; i++)
            {
                GameObject currentProjectile = GetPooledProjectile();

                currentProjectile.GetComponent<MainAttackProjectile>().Activate();
                currentProjectile.transform.position = ShootPosition.transform.position + new Vector3(0, 3f, 0);
                currentProjectile.transform.rotation = ShootPosition.transform.rotation;

                currentProjectile.GetComponent<MainAttackProjectile>().power = projectileForce;
                currentProjectile.GetComponent<MainAttackProjectile>().SetRayCast();
                Physics.IgnoreCollision(currentProjectile.GetComponent<Collider>(), GetComponent<Collider>());
            }

            RemoveItem();

            yield return new WaitForSeconds(rateOfFire);
            canFire = true;
        }
    }

    private IEnumerator ShotgunVFX()
    {
        ShootEffect.SetActive(true);
        MuzzleFlash.Play();
        StartCoroutine(FadeOut());
        PlayerControllerScript.Instance.playerAnimator.SetBool("Fire", true);
        PlayerControllerScript.Instance.CameraAnimator.SetBool("ShotgunRecoil", true);
        yield return new WaitForSeconds(0.2f);
        MuzzleFlash.Stop();
        PlayerControllerScript.Instance.playerAnimator.SetBool("Fire", false);
        PlayerControllerScript.Instance.CameraAnimator.SetBool("ShotgunRecoil", false);
        ShootEffect.SetActive(false);
    }

    private GameObject GetPooledProjectile()
    {
        for (int i = 0; i < projectilePool.Count; i++)
        {
            if (!projectilePool[i].activeInHierarchy)
            {
                return projectilePool[i];
            }
        }

        GameObject newProjectile = Instantiate(ProjectilePrefab);
        newProjectile.SetActive(false);
        projectilePool.Add(newProjectile);

        return newProjectile;
    }

    public GameObject GetPooledExplosion()
    {
        for (int i = 0; i < explosivePool.Count; i++)
        {
            if (!explosivePool[i].activeInHierarchy)
            {
                return explosivePool[i];
            }
        }

        GameObject newProjectile = Instantiate(ProjectilePrefab);
        explosivePool.Add(newProjectile);

        return newProjectile;
    }

    private void CheckReload()
    {
        if (playerAmmo <= 0 && reloading == false || Input.GetKeyDown(KeyCode.R) && reloading == false && playerAmmo < playerMaxAmmo)
        {
            StartCoroutine(ReloadWeapon());
            return;
        }
    }

    public int SetAmmo()
    {
        int AmountOfAmmo = 0;

        foreach (GameObject item in InventoryManager.Instance.Inventory)
        {
            if (item.CompareTag("FilledSlot"))
            {
                ItemData itemData = item.GetComponent<ItemData>();

                if (itemData.ItemName.Equals("Ammo"))
                {
                    AmountOfAmmo = itemData.Quantity;
                }
            }
        }

        return AmountOfAmmo;
    }

    public void RemoveItem()
    {
        for (int i = 0; i < InventoryManager.Instance.Inventory.Count; i++)
        {
            if (InventoryManager.Instance.Inventory[i].CompareTag("FilledSlot") && InventoryManager.Instance.Inventory[i].gameObject.GetComponent<ItemData>().ItemName.Equals("Ammo"))
            {
                InventoryManager.Instance.Inventory[i].gameObject.GetComponent<ItemData>().Quantity--;

                if (InventoryManager.Instance.Inventory[i].gameObject.GetComponent<ItemData>().Quantity <= 0)
                {
                    InventoryManager.Instance.Inventory[i].gameObject.GetComponent<ItemData>().ClearItemSlot();
                }
                break;
            }
        }

        StartCoroutine(ReloadWeapon());
    }

    private IEnumerator ReloadWeapon()
    {
        AudioSource.PlayOneShot(Reload, 0.6f);
        canFire = false;
        reloading = true;
        firing = false;
        yield return new WaitForSeconds(reloadSpeed);
        playerAmmo = playerMaxAmmo;
        ammoDisplay.text = Ammo.ToString();
        canFire = true;
        reloading = false;
    }
}
