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
    [SerializeField] private GameObject BulletPrefab;
    [SerializeField] private GameObject ExplosionPrefab;
    [SerializeField] private GameObject ShootEffect;
    [SerializeField] private Transform ShootPosition;
    [SerializeField] public Light ProjectilePointLight;
    [SerializeField] private Text ammoDisplay;
    [SerializeField] private Transform Orientation;
    [SerializeField] private TextMeshProUGUI AmmoText;
    [SerializeField] private LayerMask layerMask;
    public float Ammo = 10;
    public float MaxAmmo = 20;
    public ParticleSystem MuzzleFlash;
    public Transform Drum;

    private bool reloading = false;
    private bool canFire = true;

    [SerializeField] private bool firing = false;
    [SerializeField] private float Damage = 50f;
    [SerializeField] private int amountOfProjectilesPerShot = 10;
    [SerializeField] private int numberOfBulletsPerShot = 1;
    [SerializeField] private float rateOfFire = 0.6f;
    [SerializeField] private int playerAmmo = 1;
    [SerializeField] private float reloadSpeed = 2f;
    [SerializeField] private int playerMaxAmmo = 12;
    [SerializeField] private int projectileForce = 25;

    private int currentClipAmmo = 6;
    private int clipSize = 6;

    public Animator PlayerArms;
    public Animator pistolAnimator;

    void Start()
    {
        ammoDisplay.text = playerAmmo.ToString();
        AudioSource = gameObject.GetComponent<AudioSource>();
        PlayerInstance = PlayerControllerScript.Instance;
    }

    public void OnEnable()
    {
        StartCoroutine(DrawAnimation());
        AudioManager.Instance.PlaySound(AudioManager.Instance.MoveItem, 0.5f);
    }

    public IEnumerator DrawAnimation()
    {
        PlayerArms.SetBool("PullOut", true);
        yield return new WaitForSeconds(0.5f);
        PlayerControllerScript.Instance.switchingWeapon = false;
        PlayerArms.SetBool("PullOut", false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !reloading)
        {
            PrimaryFire();
        }

        ShootPosition.transform.rotation = Orientation.transform.rotation;
        Debug.Log(currentClipAmmo + "clipammo");
        Debug.Log(clipSize + "clipsize");
        CheckReload();
    }

    private void PrimaryFire()
    {
        if (canFire)
        {
            if (currentClipAmmo > 0)
            {
                StartCoroutine(FireShotgun());
                currentClipAmmo--;
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
        canFire = false; // Prevents continuous firing
        GameObject bullet = Instantiate(BulletPrefab, Drum.position, Drum.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(transform.up * 2, ForceMode.Impulse);
        bullet.GetComponent<Rigidbody>().AddForce(transform.right * 8, ForceMode.Impulse);
        StartCoroutine(ShotgunVFX());
        AudioManager.Instance.PlaySound(AudioManager.Instance.ReleaseEnergy, 1.0f);

        for (int i = 0; i < amountOfProjectilesPerShot; i++)
        {
            FireRaycast();
        }

        RemoveItem();
        playerAmmo = SetAmmo();

        yield return new WaitForSeconds(rateOfFire);
        canFire = true;
    }

    private IEnumerator ShotgunVFX()
    {
        ShootEffect.SetActive(true);
        MuzzleFlash.Play();
        StartCoroutine(FadeOut());
        PlayerControllerScript.Instance.playerAnimator.SetBool("Fire", true);
        PlayerControllerScript.Instance.CameraAnimator.SetBool("ShotgunRecoil", true);
        pistolAnimator.SetBool("Shoot", true);
        yield return new WaitForSeconds(0.05f);
        MuzzleFlash.Stop();
        PlayerControllerScript.Instance.playerAnimator.SetBool("Fire", false);
        PlayerControllerScript.Instance.CameraAnimator.SetBool("ShotgunRecoil", false);
        pistolAnimator.SetBool("Shoot", false);
        ShootEffect.SetActive(false);
    }

    private void FireRaycast()
    {
        RaycastHit hit;
        Vector3 direction = Camera.main.transform.forward;

        int layerMask = LayerMask.GetMask("Default"); // Update with your layer mask if needed

        if (Physics.Raycast(Camera.main.transform.position, direction, out hit, Mathf.Infinity, layerMask))
        {
            Debug.Log("hit " + hit);

            GameObject impact = Instantiate(ExplosionPrefab, hit.point, Quaternion.LookRotation(hit.normal));
            if (hit.collider.gameObject.CompareTag("Enemy"))
            {
                Enemy damageableEnemy = hit.collider.gameObject.GetComponent<Enemy>();

                if (damageableEnemy != null)
                {
                    damageableEnemy.TakeDamage(Damage, true);
                }
            }

            if (hit.collider.gameObject.CompareTag("Natia"))
            {
                Natia natia = hit.collider.gameObject.GetComponent<Natia>();

                if (natia != null)
                {
                    natia.TakeDamage(Damage);
                }
            }
        }

        //Debug.DrawRay(ShootPosition.position, direction * (hit.distance > 10f ? hit.distance : 80), Color.red, 2f);
    }

    private void CheckReload()
    {
        if ((currentClipAmmo <= 0 && !reloading || Input.GetKeyDown(KeyCode.R)) && !reloading && playerAmmo > 0)
        {
            StartCoroutine(ReloadWeapon());
        }
    }

    private IEnumerator ReloadWeapon()
    {
        reloading = true;
        AudioSource.PlayOneShot(Reload, 0.6f);
        yield return new WaitForSeconds(reloadSpeed);

        int ammoNeeded = clipSize - currentClipAmmo;
        int ammoToReload = Mathf.Min(ammoNeeded, playerAmmo);
        currentClipAmmo += ammoToReload;
        playerAmmo -= ammoToReload;

        ammoDisplay.text = currentClipAmmo.ToString(); // Update ammo display
        reloading = false;
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
    }
}
