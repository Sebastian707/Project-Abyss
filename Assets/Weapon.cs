using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class Weapon : MonoBehaviour
{
    public enum FireMode { SemiAuto, FullAuto, Burst }

    [Header("General")]
    public string weaponName = "Raycast Weapon";
    public string ammoItemID = "9mm";

    [Header("ADS Settings")]
    public Transform adsTransform;        
    public float adsSpeed = 10f;           
    public float adsSpreadMultiplier = 0.5f; 

    protected bool isAiming = false;
    protected Vector3 defaultLocalPosition;
    private PlayerController player;

    public Camera playerCamera;
    private float defaultFOV;
    public float adsFOVOffset = 15f; 
    public float fovChangeSpeed = 10f; 




    [Header("Ammo / Fire")]
    public FireMode fireMode = FireMode.SemiAuto;
    public int magazineSize = 12;
    public int startingLoaded = 12;
    public int pelletsPerShot = 1;
    public float spreadAngle = 2f;
    public float fireRate = 6f;
    public int burstCount = 3;

    [Header("Weapon Sway Settings")]
    public float swayAmount = 0.02f;           
    public float maxSwayAmount = 0.06f;      
    public float swaySmooth = 6f;           
    private Vector3 swayIdleOffset = Vector3.zero;
    public float idleSwaySpeed = 1f;    
    public float idleSwayAmount = 0.5f;


    [Header("Physics & Raycast")]
    public float maxDistance = 100f;
    public float damage = 25f;
    public LayerMask hitMask = ~0;

    [Header("Transforms & Prefabs")]
    public Transform muzzleTransform;
    public Transform shellEjectPort;
    public GameObject shellPrefab;
    public GameObject bulletHolePrefab;
    public ParticleSystem muzzleFlash;
    public GameObject defaultImpactVfx;

    [Header("Sound (set in inspector)")]
    public AudioClip soundFire;
    public AudioClip soundReload;
    public AudioClip soundEmpty;
    public float audioPitchVariation = 0.02f;

    [Header("Ejection & Visuals")]
    public Vector3 shellEjectVelocity = new Vector3(1.2f, 1f, 0.2f);
    public Vector3 shellAngularVelocityRange = new Vector3(10, 10, 20);

    [Header("Pooling")]
    public int bulletHolePoolSize = 32;
    public int shellPoolSize = 16;

    [Header("UI (optional)")]
    public TextMeshProUGUI ammoUIText;

    [Header("Recoil")]
    public Vector2 recoilKick = new Vector2(1f, 1f);
    public float recoilRecoverySpeed = 5f; 

    [Header("Reload Settings")]
    public float reloadTime = 2f;     
    public bool isShellByShell = false; 

    protected bool isReloading = false; 

    protected int currentMagazine;
    protected float timeSinceLastShot;
    protected bool isTriggerHeld;
    protected int burstLeft;
    protected bool canShoot = true;
    protected Vector3 currentRecoil;
    protected Vector3 targetRecoil;

    protected AudioSource audioSource;

    protected Queue<GameObject> bulletHolePool = new Queue<GameObject>();
    protected Queue<GameObject> shellPool = new Queue<GameObject>();

    protected TetrisSlot playerSlot;

    protected virtual void Awake()
    {
        player = FindObjectOfType<PlayerController>();

        audioSource = GetComponent<AudioSource>();
        playerSlot = FindObjectOfType<TetrisSlot>();
        currentMagazine = Mathf.Clamp(startingLoaded, 0, magazineSize);
        InitPools();
        UpdateUI();
        burstLeft = burstCount;
        timeSinceLastShot = 1f / fireRate;
        defaultLocalPosition = muzzleTransform.parent.localPosition;

        playerCamera = Camera.main;
        if (playerCamera != null)
            defaultFOV = playerCamera.fieldOfView;


    }

    protected virtual void InitPools()
    {
        if (bulletHolePrefab != null)
        {
            for (int i = 0; i < bulletHolePoolSize; i++)
            {
                var go = Instantiate(bulletHolePrefab);
                go.SetActive(false);
                bulletHolePool.Enqueue(go);
            }
        }

        if (shellPrefab != null)
        {
            for (int i = 0; i < shellPoolSize; i++)
            {
                var go = Instantiate(shellPrefab);
                go.SetActive(false);
                shellPool.Enqueue(go);
            }
        }
    }

    protected virtual void Update()
    {

 


        var player = FindObjectOfType<PlayerController>();
       

        if (Time.timeScale == 0) return;
        timeSinceLastShot += Time.deltaTime;

        if (fireMode == FireMode.FullAuto && isTriggerHeld && canShoot)
            TryFire();

        if (Input.GetMouseButtonDown(0))
            OnTriggerDown();

        if (Input.GetMouseButton(0))
            OnTriggerHold(true);

        if (Input.GetMouseButtonUp(0))
            OnTriggerUp();

        if (Input.GetKeyDown(KeyCode.R))
            Reload();

        currentRecoil = Vector3.Lerp(currentRecoil, targetRecoil, Time.deltaTime * recoilRecoverySpeed);

        HandleWeaponSway();

        targetRecoil = Vector3.Lerp(targetRecoil, Vector3.zero, Time.deltaTime * recoilRecoverySpeed);
        if (player != null && !player.isSprinting && Input.GetMouseButton(1))
        {
            isAiming = true;  
        }
        else
        {
            isAiming = false;   
        }

        if (adsTransform != null)
        {
            Vector3 targetPos = isAiming ? adsTransform.localPosition : defaultLocalPosition;
            muzzleTransform.parent.localPosition = Vector3.Lerp(muzzleTransform.parent.localPosition, targetPos, Time.deltaTime * adsSpeed);
        }
        if (player != null && !player.isSprinting && Input.GetMouseButton(1))
        {
            isAiming = true;
        }
        else
        {
            isAiming = false;
        }

        if (playerCamera != null)
        {
            float targetFOV = isAiming ? defaultFOV - adsFOVOffset : defaultFOV;
            playerCamera.fieldOfView = Mathf.Lerp(
                playerCamera.fieldOfView,
                targetFOV,
                Time.deltaTime * fovChangeSpeed
            );
        }

    }

    public void OnTriggerHold(bool hold)
    {
        isTriggerHeld = hold;
    }

    public void OnTriggerDown()
    {
        if (fireMode == FireMode.SemiAuto)
            TryFire();
        else if (fireMode == FireMode.Burst)
        {
            if (burstLeft <= 0) burstLeft = burstCount;
            StartCoroutine(BurstCoroutine());
        }
        else
            TryFire();
    }

    public void OnTriggerUp()
    {
        isTriggerHeld = false;
    }

    protected IEnumerator BurstCoroutine()
    {
        while (burstLeft > 0 && canShoot)
        {
            if (!TryFire()) break;
            burstLeft--;
            yield return new WaitForSeconds(1f / fireRate);
        }
        burstLeft = burstCount;
    }

    protected virtual bool TryFire()
    {

        if (player != null && player.isSprinting)
            return false; 

        if (isReloading) return false;
        if (!canShoot) return false;
        if (timeSinceLastShot < 1f / fireRate) return false;

        if (currentMagazine <= 0)
        {
            PlaySound(soundEmpty);
            return false;
        }

        currentMagazine--;
        float verticalKick = Random.Range(recoilKick.y * 0.7f, recoilKick.y);
        float horizontalKick = Random.Range(-recoilKick.x, recoilKick.x);    
        targetRecoil += new Vector3(-verticalKick, horizontalKick, 0);
        UpdateUI();

        if (muzzleFlash != null) muzzleFlash.Play();
        PlaySound(soundFire);
        SpawnShell();

        for (int i = 0; i < pelletsPerShot; i++)
        {
            Vector3 dir = GetShotDirection(i);
            if (Physics.Raycast(muzzleTransform.position, dir, out RaycastHit hit, maxDistance, hitMask))
                ApplyHit(hit, dir);
        }

        timeSinceLastShot = 0f;
        return true;
    }

    protected Vector3 GetShotDirection(int pelletIndex)
    {
       float spread = pelletsPerShot <= 1 ? 0f : spreadAngle;
    if (isAiming)
    spread *= adsSpreadMultiplier;

    float halfAngle = spread * 0.5f;
    float yaw = Random.Range(-halfAngle, halfAngle);
    float pitch = Random.Range(-halfAngle, halfAngle);
    return Quaternion.Euler(pitch, yaw, 0) * muzzleTransform.forward;
    }

    protected virtual void ApplyHit(RaycastHit hit, Vector3 shotDirection)
    {
        var damageable = hit.collider.GetComponentInParent<IDamageable>();
        if (damageable != null)
            damageable.ApplyDamage(damage);

        SpawnBulletHole(hit);

        if (defaultImpactVfx != null)
        {
            var fx = Instantiate(defaultImpactVfx, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(fx, 5f);
        }
    }

    protected virtual void SpawnBulletHole(RaycastHit hit)
    {
        if (bulletHolePrefab == null) return;

        GameObject go = bulletHolePool.Count > 0 ? bulletHolePool.Dequeue() : Instantiate(bulletHolePrefab);
        go.SetActive(true);
        go.transform.position = hit.point + hit.normal * 0.01f;
        go.transform.rotation = Quaternion.LookRotation(hit.normal);

        if (hit.collider != null)
            go.transform.SetParent(hit.collider.transform);

        StartCoroutine(DespawnBulletHoleAfter(go, 30f));
    }

    protected IEnumerator DespawnBulletHoleAfter(GameObject go, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (go == null) yield break;
        go.SetActive(false);
        go.transform.SetParent(null);
        bulletHolePool.Enqueue(go);
    }

    protected virtual void SpawnShell()
    {
        if (shellPrefab == null || shellEjectPort == null) return;

        GameObject shell = shellPool.Count > 0 ? shellPool.Dequeue() : Instantiate(shellPrefab);
        shell.SetActive(true);

        shell.transform.position = shellEjectPort.position;
        shell.transform.rotation = shellEjectPort.rotation;
        shell.transform.SetParent(null);

        var rb = shell.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 jitter = new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(0.1f, 0.4f), Random.Range(-0.2f, 0.2f));
            Vector3 eject = shellEjectPort.TransformDirection(shellEjectVelocity) + jitter;
            rb.linearVelocity = eject;
            rb.angularVelocity = new Vector3(
                Random.Range(-shellAngularVelocityRange.x, shellAngularVelocityRange.x),
                Random.Range(-shellAngularVelocityRange.y, shellAngularVelocityRange.y),
                Random.Range(-shellAngularVelocityRange.z, shellAngularVelocityRange.z)
            );
        }

        StartCoroutine(DespawnShellAfter(shell, 8f));
    }

    protected IEnumerator DespawnShellAfter(GameObject shell, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (shell == null) yield break;
        shell.SetActive(false);
        shellPool.Enqueue(shell);
    }

    protected void PlaySound(AudioClip clip)
    {
        if (clip == null || audioSource == null) return;
        audioSource.pitch = 1f + Random.Range(-audioPitchVariation, audioPitchVariation);
        audioSource.PlayOneShot(clip);
    }
public virtual void Reload()
{
    if (isReloading) return; 
    if (currentMagazine >= magazineSize)
    {
        Debug.Log("Magazine full.");
        return;
    }

    if (playerSlot == null || playerSlot.itensInBag == null)
    {
        Debug.LogWarning("No player inventory found.");
        return;
    }

    bool hasAmmo = false;
    for (int i = playerSlot.itensInBag.Count - 1; i >= 0; i--)
    {
        TetrisItemSlot slot = playerSlot.itensInBag[i];
        if (slot == null || slot.item == null) continue;

        Ammo ammo = slot.item as Ammo;
        if (ammo != null && ammo.AmmoID == ammoItemID && slot.currentStack > 0)
        {
            hasAmmo = true;
            break;
        }
    }

    if (!hasAmmo)
    {
        PlaySound(soundEmpty);
        Debug.Log("No ammo in inventory to reload!");
        return; 
    }

    StartCoroutine(ReloadCoroutine());
}

    protected IEnumerator ReloadCoroutine()
    {
        isReloading = true;

        int bulletsNeeded = magazineSize - currentMagazine;

        if (isShellByShell)
        {
            while (bulletsNeeded > 0)
            {
                bool bulletLoaded = LoadOneBulletFromInventory();
                if (!bulletLoaded) break;

                bulletsNeeded--;
                UpdateUI();
                PlaySound(soundReload);

                yield return new WaitForSeconds(reloadTime); 
            }
        }
        else
        {
            yield return new WaitForSeconds(reloadTime);

            int bulletsLoaded = 0;
            while (bulletsNeeded > 0)
            {
                bool bulletLoaded = LoadOneBulletFromInventory();
                if (!bulletLoaded) break;

                bulletsNeeded--;
                bulletsLoaded++;
            }

            if (bulletsLoaded > 0)
                PlaySound(soundReload);
            else
                PlaySound(soundEmpty);
        }

        UpdateUI();
        isReloading = false;
    }

    protected void ClearGridCellsForSlot(TetrisItemSlot slot)
    {
        if (playerSlot == null || playerSlot.grid == null || slot == null || slot.item == null)
            return;

        Vector2 start = slot.startPosition;
        Vector2 size = slot.item.itemSize;

        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                int gx = (int)(start.x + x);
                int gy = (int)(start.y + y);
                if (gx >= 0 && gx < playerSlot.maxGridX && gy >= 0 && gy < playerSlot.maxGridY)
                    playerSlot.grid[gx, gy] = 0;
            }
        }
    }

    protected void UpdateUI()
    {
        if (ammoUIText != null)
            ammoUIText.text = $"{currentMagazine}/{magazineSize}";
    }

    public void ForceSetMagazine(int amount)
    {
        currentMagazine = Mathf.Clamp(amount, 0, magazineSize);
        UpdateUI();
    }

    protected bool LoadOneBulletFromInventory()
    {
        if (playerSlot == null || playerSlot.itensInBag == null) return false;

        for (int i = playerSlot.itensInBag.Count - 1; i >= 0; i--)
        {
            TetrisItemSlot slot = playerSlot.itensInBag[i];
            if (slot == null || slot.item == null) continue;

            Ammo ammo = slot.item as Ammo;
            if (ammo != null && ammo.AmmoID == ammoItemID)
            {
                currentMagazine++;
                slot.currentStack--;
                slot.UpdateStackUI();

                if (slot.currentStack <= 0)
                {
                    ClearGridCellsForSlot(slot);
                    playerSlot.itensInBag.RemoveAt(i);
                    Destroy(slot.gameObject);
                }

                return true; 
            }
        }

        return false; 
    }

    private void HandleWeaponSway()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        float swayX = Mathf.Clamp(-mouseX * swayAmount, -maxSwayAmount, maxSwayAmount);
        float swayY = Mathf.Clamp(-mouseY * swayAmount, -maxSwayAmount, maxSwayAmount);

        swayIdleOffset.x = Mathf.PerlinNoise(Time.time * idleSwaySpeed, 0f) - 0.5f;
        swayIdleOffset.y = Mathf.PerlinNoise(0f, Time.time * idleSwaySpeed) - 0.5f;
        swayIdleOffset *= idleSwayAmount;

        Vector3 combinedRotation = currentRecoil + new Vector3(
            -swayY * 50f + swayIdleOffset.y * 50f,
            swayX * 50f + swayIdleOffset.x * 50f,
            0f
        );

        muzzleTransform.localRotation = Quaternion.Slerp(
            muzzleTransform.localRotation,
            Quaternion.Euler(combinedRotation),
            Time.deltaTime * swaySmooth
        );
    }

}

public interface IDamageable
{
    void ApplyDamage(float amount);
}
