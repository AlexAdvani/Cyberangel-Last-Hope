using System;
using System.Collections;

using UnityEngine;

using Rewired;
using Spine.Unity;

[SelectionBase]
public class WeaponBehaviour : SpineAnimatorBase
{
    // Player Controller
    protected PlayerController playerController;
    // Player Input
    protected Player playerInput;
    // Mesh Renderer
    protected MeshRenderer meshRenderer;
    // Facing Left flag
    protected bool bFacingLeft = false;

    // Weapon Data
    public WeaponData weaponData;

    // Ammo in clip
    protected int iClipAmmo;
    // Ammo held outside of clip
    protected int iHeldAmmo;

    // Recoil (Degrees)
    protected float fRecoil = 0f;
    // Aim Recoil (Degrees)
    protected float fAimRecoil = 0f;
    // Recoil Multiplier
    protected float fRecoilMultiplier = 1f;

    // Is Reloading flag
    protected bool bReloading = false;
    // Is Start Reload flag (In Pre Reload player animation for weapons with per bullet reloading)
    protected bool bStartReload = false;
    // Is Full Reload flag
    protected bool bFullReload = false;
    // Reload Coroutine
    protected Coroutine reloadCoroutine;
    // Can Fire flag
    protected bool bCanFire = true;
    // Firing flag
    protected bool bFiring = false;
    // Can Reduce Recoil flag
    protected bool bReduceRecoil = false;
    // Can Reduce Aim Recoil flag
    protected bool bReduceAimRecoil = false;

    // Point to Fire Bullet From
    public Transform tFirePoint;
    // Weapon Visual Offset
    protected Vector3 v3VisualOffset = Vector3.zero;
    // Weapon Visual Object
    protected Transform tVisualObject;
    // Muzzle Flash Particle System
    public ParticleSystem muzzleFlash;

    // Is Weapon Disabled flag
    protected bool bWeaponDisabled;
    // Is Reload Disabled flag
    protected bool bReloadDisabled;
    // Reload Timer
    protected float fReloadTimer = 0;
    // Last Time That the Weapon Was Active
    protected float fLastActiveTime;
    // Weapon Active flag
    [HideInInspector]
    public bool bActive;
    // Weapon Holstered flag
    [HideInInspector]
    public bool bHolstered;
    // Weapon Holstered on Previous Frame flag
    protected bool bPrevHolstered = true;

    // Recoil Coroutine
    protected Coroutine recoilCoroutine;
    // Aim Recoil Coroutine
    protected Coroutine aimRecoilCoroutine;

    // Shell Casing Eject Point
    public Transform tCasingPoint;
    // Clip Eject Point
    public Transform tClipPoint;
    // Laser Sight Behaviour
    public LaserSightBehaviour laserSight;

    // Camera Distance from Player when Aiming
    protected float fAimDistance;

    // Number of Shots fired in a burst
    protected int iBurstShots = 0;

    // Bolted flag
    protected bool bBolted = true;
    // Bolting flag
    protected bool bBolting = false;
    // Bolt Timer
    protected float fBoltTimer = 0;
    // Bolt Coroutine
    protected Coroutine boltCoroutine;

    // TEMP - Remove Public once I have stopped testing Infinite Ammo during game modes ------------------------------------------------------------------
    // Infinite Ammo flag
    public bool bInfiniteAmmo = false;

    // Bullet Object Pool Name
    protected string sBulletPoolName = "";
    // Casing Object Pool Name
    protected string sCasingPoolName = "";
    // Clip Object Pool Name
    protected string sClipPoolName = "";

    // Projectile Behaviour Array
    protected ProjectileBehaviour[] aProjectileBehaviours;

    // On Weapon Fire
    public Action<int> onWeaponFire;
    // On Weapon Empty
    public Action onWeaponEmpty;
    // On Weapon Refill
    public Action onWeaponRefill;
    // On Weapon Reload
    public Action<int> onWeaponReload;

    #region Public Attributes 

    // Name
    public string Name
    {
        get { return weaponData.sName; }
    }

    // Player Animation Type
    public string PlayerAnimationType
    {
        get { return weaponData.sPlayerAnimationType; }
    }

    // Clip Ammo
    public int ClipAmmo
    {
        get { return iClipAmmo; }
    }

    // Held Ammo
    public int HeldAmmo
    {
        get { return iHeldAmmo; }
    }

    // Max Ammo
    public int MaxAmmo
    {
        get { return weaponData.iMaxAmmo; }
    }

    // Total Ammo
    public int TotalAmmo
    {
        get { return iHeldAmmo + iClipAmmo; }
    }

    // Fire Rate
    public float FireRate
    {
        get { return weaponData.fFireRate; }
    }

    // Damage
    public float Damage
    {
        get { return weaponData.fDamage; }
    }

    // Reloading flag
    public bool Reloading
    {
        get { return bReloading; }
    }

    // Full Reload flag
    public bool FullReload
    {
        get { return bFullReload; }
    }

    // IK Offset
    public float IKOffset
    {
        get { return weaponData.fAimIKOffset; }
    }

    // Recoil
    public float Recoil
    {
        get { return fRecoil; }
    }

    // Aim Recoil
    public float AimRecoil
    {
        get { return fAimRecoil; }
    }

    // Recoil Multiplier
    public float RecoilMultiplier
    {
        get { return fRecoilMultiplier; }
    }

    // Can Fire
    public bool CanFire
    {
        get { return bCanFire; }
    }

    // Firing
    public bool Firing
    {
        get { return bFiring; }
    }

    // Two-Handed flag
    public eWeaponHoldType HoldType
    {
        get { return weaponData.holdType; }
    }

    // Aim Distance
    public float AimDistance
    {
        get { return fAimDistance; }
    }

    // Firing Mode
    public eWeaponFireMode FiringMode
    {
        get { return weaponData.firingMode; }
    }

    // Weapon Disabled flag
    public bool WeaponDisabled
    {
        get { return bWeaponDisabled; }
    }

    // Reload Disabled flag
    public bool ReloadDisabled
    {
        get { return bReloadDisabled; }
    }

    // Bolting flag
    public bool Bolting
    {
        get { return bBolting; }
    }

    // Infinite Ammo flag
    public bool InfiniteAmmo
    {
        get { return bInfiniteAmmo; }
    }

    #endregion

    // On Create Initialization
    protected override void Awake()
    {
        base.Awake();

        CreateObjectPools();
        GetBulletBehaviours();
    }

    // Initialization
    protected override void Start()
    {
        base.Start();

        playerInput = ReInput.players.GetPlayer(0);
        meshRenderer = skeleton.GetComponent<MeshRenderer>();
        tVisualObject = transform.GetChild(0);

        if (muzzleFlash != null)
        {
            muzzleFlash.gameObject.SetActive(false);
        }

        if (bHolstered)
        {
            tVisualObject.localPosition = weaponData.v3HolsterOffset;
        }
        else
        {
            tVisualObject.localPosition = weaponData.v3WeaponOffset;
        }

        skeleton.state.Event += OnSpineEvent;
        skeleton.state.End += OnAnimationEnd;

        RefillAllAmmo();
    }

    // Update
    protected virtual void Update()
    {
        // Handle Animation
        HandleAnimation();
        // Handle Facing
        HandleFacing();
        // Handle Visual Offset
        HandleVisualOffset();

        bPrevHolstered = bHolstered;

        // If game paused, return
        if (GameManager.GamePaused)
        {
            return;
        }

        // Fire Input
        HandleFireInput();
        // Handle Reload Input
        HandleReloadInput();
        // Handle Bolting
        HandleBolting();
        // Handle Laser Sight
        HandleLaserSight();

        ///// TEST ///////////////// Use for finding firepoint ik offset
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (playerController.CurrentWeapon == this)
            {
                print(tFirePoint.position.y - playerController.animator.weaponPivotIKBone.GetWorldPosition(playerController.animator.transform).y);
            }
        }
    }

    // On Spine Event
    protected virtual void OnSpineEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "EmptyGunshot":
                EmptyGunshot();
                break;

            case "CasingEject":
                EjectCasing();
                break;

            case "ClipEjected":
                EjectClip();
                break;

            case "ReloadUnload":
                PlayReloadUnloadAudio();
                break;

            case "ReloadLoad":
                PlayReloadLoadAudio();
                break;

            case "ReloadSlide":
                PlayReloadSlideAudio();
                break;

            case "ReloadBolt":
                PlayReloadBoltAudio();
                break;
        }
    }

    // On Spine Completed
    protected virtual void OnAnimationEnd(Spine.TrackEntry trackEntry)
    {
        switch (trackEntry.Animation.Name)
        {
            case "Fire":
                if (weaponData.firingMode == eWeaponFireMode.Single && weaponData.bBoltAction)
                {
                    bBolted = false;
                }
                break;
        }
    }

    // Creates Object Pools for the Bullets, Casings and Clip
    private void CreateObjectPools()
    {
        // Bullets
        if (weaponData.goBulletPrefab != null)
        {
            sBulletPoolName = weaponData.sName + " Bullets";

            ObjectPoolManager.Instance.CreatePool(sBulletPoolName, weaponData.goBulletPrefab, weaponData.iBulletPoolSize);
        }

        // Casings
        if (weaponData.goCasingPrefab != null)
        {
            sCasingPoolName = weaponData.sName + " Casings";

            ObjectPoolManager.Instance.CreatePool(sCasingPoolName, weaponData.goCasingPrefab, weaponData.iCasingPoolSize);
        }

        // Clips
        if (weaponData.goClipPrefab != null)
        {
            sClipPoolName = weaponData.sName + " Clips";

            ObjectPoolManager.Instance.CreatePool(sClipPoolName, weaponData.goClipPrefab, weaponData.iClipPoolSize);
        }
    }

    // Gets all of the bullets in the bullet object pool and finds their projectile behaviours
    private void GetBulletBehaviours()
    {
        if (sBulletPoolName == "")
        {
            return;
        }

        aProjectileBehaviours = new ProjectileBehaviour[weaponData.iBulletPoolSize];
        GameObject[] bulletObjects = ObjectPoolManager.Instance.GetObjectPool(sBulletPoolName).PooledObjects.ToArray();

        for (int i = 0; i < aProjectileBehaviours.Length; i++)
        {
            aProjectileBehaviours[i] = bulletObjects[i].GetComponent<ProjectileBehaviour>();
            aProjectileBehaviours[i].SetDamage(weaponData.fDamage);
        }
    }

    // Destroys the object pools associated with this weapon
    private void DestroyObjectPools()
    {
        if (sBulletPoolName != "")
        {
            ObjectPoolManager.Instance.RemovePool(sBulletPoolName);
            sBulletPoolName = "";
        }

        if (sCasingPoolName != "")
        {
            ObjectPoolManager.Instance.RemovePool(sCasingPoolName);
            sCasingPoolName = "";
        }

        if (sClipPoolName != "")
        {
            ObjectPoolManager.Instance.RemovePool(sClipPoolName);
            sClipPoolName = "";
        }
    }

    // Remove Weapon 
    public void RemoveWeapon()
    {
        DestroyObjectPools();
    }

    #region Update

    // Handles Animation
    protected virtual void HandleAnimation()
    {
        if (bActive)
        {
            if (bFiring || bReloading || bBolting)
            {
                return;
            }
        }

        if (iClipAmmo <= 0)
        {
            PlayAnimation("Empty");
        }
        else
        {
            PlayAnimation("Idle");
        }
    }

    // Checks player facing and plays animation to match
    public void HandleFacing()
    {
        if (bFacingLeft != playerController.FacingLeft)
        {
            bFacingLeft = !bFacingLeft;

            if (bFacingLeft)
            {
                PlayAnimation("FaceLeft", 1);
            }
            else
            {
                PlayAnimation("FaceRight", 1);
            }
        }

        if (weaponData.holdType == eWeaponHoldType.Primary)
        {
            if (playerController.bHolsteringWeapon || playerController.bUnholsteringWeapon)
            {
                if (playerController.HoldingWeapon && playerController.CurrentWeapon == this)
                {
                    if (bFacingLeft)
                    {
                        meshRenderer.sortingOrder = 7;
                    }
                    else
                    {
                        if (playerController.OnWall || playerController.OnCorner)
                        {
                            meshRenderer.sortingOrder = 22;
                        }
                        else
                        {
                            meshRenderer.sortingOrder = 12;
                        }
                    }
                }
                else
                {
                    meshRenderer.sortingOrder = 7;
                }
            }
            else
            {
                if (playerController.CurrentWeapon == this)
                {
                    if (!bFacingLeft && (playerController.OnWall || playerController.OnCorner))
                    {
                        meshRenderer.sortingOrder = 22;
                    }
                    else
                    {
                        if (bFacingLeft)
                        {
                            meshRenderer.sortingOrder = 7;
                        }
                        else
                        {
                            meshRenderer.sortingOrder = 12;
                        }
                    }
                }
                else
                {
                    meshRenderer.sortingOrder = 7;
                }
            }
        }
        else if (weaponData.holdType == eWeaponHoldType.Secondary)
        {
            if (playerController.bHolsteringWeapon || playerController.bUnholsteringWeapon)
            {
                if (bFacingLeft)
                {
                    meshRenderer.sortingOrder = -5;
                }
                else
                {
                    meshRenderer.sortingOrder = 17;
                }
            }
            else if (bHolstered)
            {
                if (bFacingLeft)
                {
                    meshRenderer.sortingOrder = -5;
                }
                else
                {
                    if (playerController.OnWall || playerController.OnCorner)
                    {
                        meshRenderer.sortingOrder = 17;
                    }
                    else
                    {
                        meshRenderer.sortingOrder = 22;
                    }
                }
            }
            else
            {
                if (!bFacingLeft && (playerController.OnWall || playerController.OnCorner))
                {
                    meshRenderer.sortingOrder = 22;
                }
                else
                {
                    if (bFacingLeft)
                    {
                        meshRenderer.sortingOrder = 7;
                    }
                    else
                    {
                        meshRenderer.sortingOrder = 12;
                    }
                }
            }
        }
    }

    // Handles visual object transform offset when weapon activates and deactivates
    public void HandleVisualOffset(bool force = false)
    {
        if (!force && bHolstered == bPrevHolstered)
        {
            return;
        }

        if (bHolstered)
        {
            tVisualObject.localPosition = weaponData.v3HolsterOffset;
        }
        else
        {
            tVisualObject.localPosition = weaponData.v3WeaponOffset;
        }
    }

    // Handles Input for Reloading
    protected virtual void HandleReloadInput()
    {
        if (!bActive || bWeaponDisabled)
        {
            return;
        }

        // If reloading and reload is disabled or reload animation not triggered, cancel reload
        if (bReloading)
        {
            // Reload Disabled
            if (bReloadDisabled)
            {
                CancelReload();
            }

            // Not using Reload animation
            if (!lsCurrentAnimations[0].Contains("Reload") && !bStartReload)
            {
                CancelReload();
            }

            // Firing to cancel per bullet reload
            if (weaponData.firingMode == eWeaponFireMode.Single && weaponData.bBoltAction)
            {
                if (iClipAmmo > 0)
                {
                    if (playerInput.GetButtonDown("Fire"))
                    {
                        CancelReload();
                    }
                }
            }
        }

        // Reload if ammo left and button pressed or no bullets in clip
        if (!bReloadDisabled)
        {
            if (iHeldAmmo > 0)
            {
                if (playerInput.GetButtonDown("Reload") || iClipAmmo <= 0)
                {
                    if (!bReloading)
                    {
                        StartReload();
                    }
                }
            }
        }
    }

    // Handles input for firing
    protected virtual void HandleFireInput()
    {
        if (!bActive)
        {
            return;
        }

        bool fire = false;

        // If can fire and not reloading, then fire weapon based on fire mode type
        if (bCanFire && !bReloading && !bWeaponDisabled)
        {
            switch (weaponData.firingMode)
            {
                case eWeaponFireMode.Automatic:
                    if (playerInput.GetButton("Fire"))
                    {
                        fire = true;
                    }
                    break;

                case eWeaponFireMode.Burst:
                    if (playerInput.GetButtonDown("Fire"))
                    {
                        fire = true;
                    }
                    break;

                case eWeaponFireMode.Single:
                    if (playerInput.GetButtonDown("Fire"))
                    {
                        fire = true;
                    }
                    break;
            }

            if (fire)
            {
                if (iClipAmmo > 0)
                {
                    if (weaponData.firingMode == eWeaponFireMode.Burst)
                    {
                        StartCoroutine(FireBurst());
                    }
                    else
                    {
                        Fire();
                    }
                }
                else
                {
                    if (weaponData.firingMode == eWeaponFireMode.Burst)
                    {
                        StartCoroutine(FireBurstEmpty());
                    }
                    else
                    {
                        FireEmpty();
                    }
                }

                playerController.animator.SetArmBusy(true);
                bFiring = true;
            }
        }
    }

    // Handles Bolting
    private void HandleBolting()
    {
        // If not a bolt action weapon or single shot, return
        if (!weaponData.bBoltAction || weaponData.firingMode != eWeaponFireMode.Single)
        {
            return;
        }

        if (bWeaponDisabled)
        {
            CancelBolt();
            return;
        }

        if (bFiring || bBolted || bReloading)
        {
            return;
        }

        if (!bBolting)
        {
            StartBolting();
        }
    }

    // Handle Laser Sight
    private void HandleLaserSight()
    {
        if (laserSight == null)
        {
            return;
        }

        if (playerController.CurrentWeapon == this)
        {
            if (!playerController.animator.ArmBusy || bReloading || bBolting || bWeaponDisabled || iClipAmmo == 0)
            {
                laserSight.SetLaserSightActive(false);
            }
            else
            {
                laserSight.SetLaserSightActive(true);
            }
        }
        else
        {
            laserSight.SetLaserSightActive(false);
        }
    }

    #endregion

    // Sets the link to the player instance
    public void SetPlayerController(PlayerController player)
    {
        playerController = player;
    }

    // Set Weapon Disabled
    public virtual void SetWeaponDisabled(bool disabled)
    {
        bWeaponDisabled = disabled;
    }

    // Set Reload Disabled
    public virtual void SetReloadDisabled(bool disabled)
    {
        bReloadDisabled = disabled;
    }

    // Start Vibrating the gamepad continuously
    protected void StartVibrateGamepad()
    {
        if (!GlobalSettings.bVibration)
        {
            playerInput.SetVibration(0, 0);
            playerInput.SetVibration(1, 0);

            return;
        }

        playerInput.SetVibration(0, weaponData.fVibrationStrength);
        playerInput.SetVibration(1, weaponData.fVibrationStrength);
    }

    // Stop vibrating the gamepad continuously
    protected void StopVibrateGamepad()
    {
        if (!GlobalSettings.bVibration)
        {
            playerInput.SetVibration(0, 0);
            playerInput.SetVibration(1, 0);

            return;
        }

        playerInput.SetVibration(0, 0);
        playerInput.SetVibration(1, 0);
    }

    // Vibrates the gamepad by the amount set in the Weapon Data
    protected void VibrateGamepadTimed()
    {
        if (!GlobalSettings.bVibration)
        {
            playerInput.SetVibration(0, 0);
            playerInput.SetVibration(1, 0);

            return;
        }

        playerInput.SetVibration(0, weaponData.fVibrationStrength, weaponData.fVibrationTime);
        playerInput.SetVibration(1, weaponData.fVibrationStrength, weaponData.fVibrationTime);
    }

    #region Firing

    // Fire Weapon
    protected virtual void Fire()
    {
        if (iClipAmmo <= 0)
        {
            return;
        }

		// Play Player Animation
		playerController.animator.FireWeaponAnimation();
        // Play Muzzle Flash
        PlayMuzzleFlash();
        // Play Animation
        PlayAnimation("Fire", 0, false, true);
		bCanFire = false;

		if (weaponData.firingMode != eWeaponFireMode.Single || !weaponData.bBoltAction)
		{
			StartCoroutine(ReenableFiring());
		}
	}

	// Fire Weapon Burst
	protected virtual IEnumerator FireBurst()
	{
        if (iClipAmmo <= 0)
        {
            yield return null;
        }

        // Play Player Animation
        playerController.animator.FireWeaponAnimation();
        // Play Muzzle Flash
        PlayMuzzleFlash();
        // Play Animation
        PlayAnimation("Fire", 0, false, true, 0, 0);

		bReloadDisabled = true;
		iBurstShots++;

		// If there are shots to be fired then continue firing
		if (iBurstShots < weaponData.iShotsPerBurst)
		{
			if (iClipAmmo > 0)
			{
				yield return new WaitForSeconds(weaponData.fBurstShotTime);
				StartCoroutine(FireBurst());
			}
			else
			{
				StartCoroutine(FireBurstEmpty());
			}
		}
		else // Otherwise, stop firing for burst
		{
			StartCoroutine(ReenableFiring());

			iBurstShots = 0;

			// Increase recoil
			IncreaseRecoil();
		}
	}

	// Fire Empty
	protected virtual void FireEmpty()
	{
        playerController.animator.ResetArmTimer();
		StartCoroutine(ReenableFiring());

        // Loop empty animation if the weapon is not a beam weapon and if the weapon is automatic
        bool animLoop = weaponData.weaponType != eWeaponType.Beam && 
            weaponData.firingMode == eWeaponFireMode.Automatic;

        PlayAnimation("EmptyFire", 0, animLoop, !animLoop);

		// Stop firing
		if (weaponData.firingMode == eWeaponFireMode.Burst && iBurstShots < weaponData.iShotsPerBurst)
		{
			bFiring = true;
		}
		else
		{
			bFiring = false;
		}
	}

	// Fire Empty Burst
	protected virtual IEnumerator FireBurstEmpty()
	{
		if (weaponData.emptyAudio != null)
		{
			weaponData.emptyAudio.Play();
		}
		iBurstShots++;

		bReloadDisabled = true;

		if (iBurstShots < weaponData.iShotsPerBurst)
		{
			yield return new WaitForSeconds(weaponData.fBurstShotTime);

			StartCoroutine(FireBurstEmpty());
		}
		else
		{
			StartCoroutine(ReenableFiring());

			iBurstShots = 0;
		}

		playerController.animator.ResetArmTimer();
		bFiring = false;

		yield return null;
	}

    // Plays empty gunshot sound
    protected void EmptyGunshot()
    {
        if (weaponData.emptyAudio != null)
        {
            weaponData.emptyAudio.Play();
        }
    }

    // Reenables the ability to fire
    protected IEnumerator ReenableFiring()
	{
		yield return new WaitForSeconds(weaponData.fFireRate);

		bCanFire = true;
	}

	// Ejects a Shell Casing from the Casing Point
	protected void EjectCasing()
	{
        GameObject casing = ObjectPoolManager.Instance.GOSpawnObject(sCasingPoolName, tCasingPoint.position, tCasingPoint.rotation);

        if (casing != null)
        {
            if (playerController.FacingLeft)
            {
                casing.transform.localPosition += new Vector3(0, 0, 0.1f);
            }
            else
            {
                casing.transform.localPosition += new Vector3(0, 0, -0.1f);
            }
        }
	}

	// Ejects a Clip from the Clip Point
	protected void EjectClip()
	{
		GameObject clip = ObjectPoolManager.Instance.GOSpawnObject(sClipPoolName, tClipPoint.position, tClipPoint.rotation);

        if (playerController.FacingLeft)
        {
            clip.transform.localScale = new Vector2(1, -1);
        }
        else
        {
            clip.transform.localScale = new Vector2(1, 1);
        }
	}

    // Plays the Muzzle Flash Effect
    protected void PlayMuzzleFlash()
    {
        if (muzzleFlash != null)
        {
            if (muzzleFlash.gameObject.activeSelf)
            {
                muzzleFlash.gameObject.SetActive(false);
            }

            muzzleFlash.gameObject.SetActive(true);
        }
    }

    // Stops the Muzzle Flash Effect
    public void StopMuzzleFlash()
    {
        if (muzzleFlash != null)
        {
            muzzleFlash.gameObject.SetActive(false);
        }
    }

    #endregion

    #region Recoil

    // Enables Recoil Reduction
    protected IEnumerator BeginRecoilCooldown()
	{
		yield return new WaitForSeconds(weaponData.fRecoilResetTime);

		bReduceRecoil = true;
		StartCoroutine(DecreaseRecoil());
	}

	// Increases recoil
	protected void IncreaseRecoil()
	{
		fRecoil += weaponData.fRecoilIncreaseRate;

		if (fRecoil > (weaponData.fMaxRecoil))
		{
			fRecoil = Mathf.Sign(fRecoil) * weaponData.fMaxRecoil;
		}

		fAimRecoil += weaponData.fAimRecoilIncreaseRate * fRecoilMultiplier * Mathf.Sign(UnityEngine.Random.Range(-1,1));

		if (Mathf.Abs(fAimRecoil) > weaponData.fMaxAimRecoil * fRecoilMultiplier)
		{
			fAimRecoil = weaponData.fMaxAimRecoil * Mathf.Sign(fAimRecoil);
		} 

		// Reset the reduce recoil timers
		bReduceRecoil = false;
		bReduceAimRecoil = false;

		if (recoilCoroutine != null)
		{
			StopCoroutine(recoilCoroutine);
			recoilCoroutine = null;
		}

		if (aimRecoilCoroutine != null)
		{
			StopCoroutine(aimRecoilCoroutine);
			aimRecoilCoroutine = null;
		}

		recoilCoroutine = StartCoroutine(BeginRecoilCooldown());
		aimRecoilCoroutine = StartCoroutine(BeginAimRecoilCooldown());
	}

	// Decreases recoil back to 0
	protected IEnumerator DecreaseRecoil()
	{
		while (bReduceRecoil)
		{
			fRecoil -= weaponData.fRecoilDecreaseRate * Time.deltaTime;

			if (fRecoil <= 0)
			{
				fRecoil = 0;
				bReduceRecoil = false;
			}

			yield return null;
		}
	}

	// Sets the Recoil Multiplier
	public void SetRecoilMultiplier(float multiplier)
	{
		if (weaponData.holdType == eWeaponHoldType.Secondary)
		{
			multiplier = 1f;
		}

		fRecoilMultiplier = multiplier;
	}

	// Starts aim recoil cooldown after a set time
	protected IEnumerator BeginAimRecoilCooldown()
	{
		yield return new WaitForSeconds(weaponData.fAimRecoilResetTime);

		bReduceAimRecoil = true;
		StartCoroutine(DecreaseAimRecoil());
	}

	// Decreases aim recoil over time while active
	protected IEnumerator DecreaseAimRecoil()
	{
		while (bReduceAimRecoil)
		{
			fAimRecoil -= weaponData.fAimRecoilDecreaseRate * Time.deltaTime;

			if (fAimRecoil <= 0)
			{
				fAimRecoil = 0;
				bReduceAimRecoil = false;
			}

			yield return null;
		}
	}

	// Starts the cooldown timers
	public void StartCooldownTimers()
	{
		fLastActiveTime = Time.time;
	}

	#endregion

	#region Ammo Management

	// Begins to reload the weapon
	protected virtual void StartReload()
	{
		// If reload disabled, return
		if (bReloadDisabled)
		{
			return;
		}

		// If there is no ammo remaining, return
		if (TotalAmmo == 0)
		{
			return;
		}

		// If already reloading, return
		if (bReloading)
		{
			return;
		}

		if (iClipAmmo >= weaponData.iClipSize)
		{
			return;
		}

        StopMuzzleFlash();
		bReloading = true;

		if (weaponData.bCanFullReload && iClipAmmo == 0)
		{
			bFullReload = true;
		}
			
		reloadCoroutine = StartCoroutine(Reload());
	}

	// Triggers a reload after reload time has elapsed
	protected virtual IEnumerator Reload()
	{
		if (!weaponData.bClipReload)
		{
			playerController.animator.TriggerReloadStartAnimation();
			float reloadStartTime = playerController.animator.skeleton.AnimationState.GetCurrent(1).Animation.Duration;
			bStartReload = true;

			yield return new WaitForSeconds(reloadStartTime);
		}

		float reloadTime = skeleton.Skeleton.Data.FindAnimation("Reload").Duration;

		playerController.animator.TriggerReloadAnimation();
		PlayAnimation("Reload");

		bStartReload = false;

		while (bReloading)
		{
			fReloadTimer += Time.deltaTime;

			if (fReloadTimer >= reloadTime)
			{
				if (weaponData.bClipReload)
				{
					EndReload();
				}
				else
				{
					PerBulletReload();
				}
			}

			yield return null;
		}

		fReloadTimer = 0;
	}

	// Reloads a single bullet and then repeats reload if clip is not full and ammo remaining
	protected virtual void PerBulletReload()
	{
		// Reload bullet
		iClipAmmo++;

		// If not infinite ammo, decrease ammo
		if (!bInfiniteAmmo)
		{
			iHeldAmmo--;
		}
			
		// Reset reload time
		fReloadTimer = 0;

		// If clip full, stop reloading
		if (iClipAmmo == weaponData.iClipSize)
		{
			bReloading = false;
			bFullReload = false;
		}
		else // Otherwise, replay reload animation
		{
			playerController.animator.TriggerReloadAnimation(true);
			PlayAnimation("Reload", 0, false, true, 0, 0);
		}

        // Reload Event
        if (onWeaponReload != null)
        {
            onWeaponReload(iClipAmmo);
        }
    }

	// Ends reload
	protected virtual void EndReload()
	{
		bool chamberReload = false;

		// If can full reload 
		if (weaponData.bCanFullReload)
		{
			if (bFullReload)
			{
				bCanFire = false;
				StartBolting();
			}
			else
			{
				chamberReload = true;
			}
		}

		// If infinite ammo, then refill clip 
		if (bInfiniteAmmo)
		{
			iClipAmmo = weaponData.iClipSize;

			if (chamberReload)
			{
				iClipAmmo++;
			}
		}
		else // Otherwise, find the number of bullets to reload, and add them if available
		{
			int difference = weaponData.iClipSize - iClipAmmo;

			if (iHeldAmmo >= difference)
			{
				iClipAmmo = weaponData.iClipSize;
				iHeldAmmo -= difference;

				if (chamberReload)
				{
					iClipAmmo++;
					iHeldAmmo--;
				}
			}
			else
			{
				iClipAmmo += iHeldAmmo;
				iHeldAmmo = 0;
			}
		}

        // Reload Event
        if (onWeaponReload != null)
        {
            onWeaponReload(iClipAmmo);
        }

		bReloading = false;
		bFullReload = false;
	}

	// Cancels a reload
	public virtual void CancelReload()
	{
		PlayAnimation("Empty");

		if (reloadCoroutine != null)
		{
			StopCoroutine(reloadCoroutine);
			reloadCoroutine = null;
		}

		if (bReloading)
		{
			if (AudioManager.Instance.BGetIsPlaying(weaponData.fireAudio.sAudioSlot))
			{
				weaponData.fireAudio.Stop();
			}

			if (AudioManager.Instance.BGetIsPlaying(weaponData.reloadLoadAudio.sAudioSlot))
			{
				weaponData.reloadLoadAudio.Stop();
			}
		}

		fReloadTimer = 0;
		fRecoil = 0;
		bReloading = false;
		bFullReload = false;
    }

	// Gives ammo to the weapon
	public void RefillAmmo(int ammo)
	{
		iHeldAmmo += ammo;

		if (TotalAmmo > weaponData.iMaxAmmo)
		{
			iHeldAmmo = weaponData.iMaxAmmo - iClipAmmo;
		}

        if (onWeaponRefill != null)
        {
            onWeaponRefill();
        }
    }

	// Sets ammo to maximum
	public void RefillAllAmmo()
	{
		iClipAmmo = weaponData.iClipSize;
		iHeldAmmo = weaponData.iMaxAmmo;

        if (onWeaponRefill != null)
        {
            onWeaponRefill();
        }
    }

	// Takes ammo from the weapon
	public void TakeAmmo(int ammo)
	{
		iHeldAmmo -= ammo;

		if (iHeldAmmo < 0)
		{
			iHeldAmmo = 0;
		}
	}

	// Takes all ammo from the weapon (excluding clip)
	public void TakeAllAmmo()
	{
		iHeldAmmo = 0;
	}

	#endregion

	#region Bolting

	private void StartBolting()
	{
		bBolted = false;
		bBolting = true;

		string animationName;

		if (playerController.OnWall || playerController.OnCorner)
		{
			animationName = "ReloadBoltOneHand";
		}
		else
		{
			animationName = "ReloadBolt";
		}

		PlayAnimation(animationName, 0, false, false, 0, 0);
		playerController.animator.TriggerReloadBoltAnimation();

		boltCoroutine = StartCoroutine(BoltWeapon());
	}

	// Bolts the weapon
	private IEnumerator BoltWeapon()
	{
		float boltTime = skeleton.AnimationState.GetCurrent(0).Animation.Duration;

		while (bBolting)
		{
			fBoltTimer += Time.deltaTime;

			if (fBoltTimer >= boltTime)
			{
				EndBolting();
			}

			yield return null;
		}

		fBoltTimer = 0;
	}

	// Handles weapon bolting state
	private void EndBolting()
	{
		bBolted = true;
		bBolting = false;

		StartCoroutine(ReenableFiring());
	}

	public void CancelBolt()
	{
		if (boltCoroutine == null)
		{
			return;
		}
			
		StopCoroutine(boltCoroutine);
		fBoltTimer = 0;
		bBolting = false;
		bBolted = true;

		bCanFire = true;
	}

	#endregion

	#region Audio

	// Plays the Unload Audio during a reload
	private void PlayReloadUnloadAudio()
	{
		if (weaponData.reloadUnloadAudio == null)
		{
			return;
		}

		weaponData.reloadUnloadAudio.Play();
	}

	// Plays the Load Audio during a reload
	private void PlayReloadLoadAudio()
	{
		if (weaponData.reloadLoadAudio == null)
		{
			return;
		}

		weaponData.reloadLoadAudio.Play();
	}

	// Plays the Slide Audio during a reload
	private void PlayReloadSlideAudio()
	{
		if (weaponData.reloadSlideAudio == null)
		{
			return;
		}

		weaponData.reloadSlideAudio.Play();
	}

	// Plays the Unload Audio during a reload
	private void PlayReloadBoltAudio()
	{
		if (weaponData.reloadBoltAudio == null)
		{
			return;
		}

		weaponData.reloadBoltAudio.Play();
	}

	#endregion

	// Applies cooldowns based on time counted
	public virtual void EndCooldownTimers()
	{
		if (bReduceRecoil)
		{
			float difference = Time.time - fLastActiveTime;

			fRecoil -= weaponData.fRecoilDecreaseRate * difference;

			if (fRecoil <= 0)
			{
				fRecoil = 0;
				bReduceRecoil = false;
			}
		}
	}
}

// Weapon Hold Types
public enum eWeaponHoldType
{
	Primary,
	Secondary,
	Heavy, 
    Temporary
}

// Weapon Fire Modes
public enum eWeaponFireMode
{
	Single,
	Automatic,
	Burst
}