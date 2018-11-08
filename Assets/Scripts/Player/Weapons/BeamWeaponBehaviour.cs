using System.Collections;

using UnityEngine;

using Com.LuisPedroFonseca.ProCamera2D;

public class BeamWeaponBehaviour : WeaponBehaviour
{
    // Line Renderer
    public LineRenderer lineRenderer;

    // Continuous Clip Ammo
    float fBeamClipAmmo;

    // Beam Recoil Multiplier - Increases with sustained fire
    float fBeamRecoilMultiplier = 0;

    // Beam Width
    public float fBeamWidth = 0.05f;
    // Beam Length
    public float fBeamLength = 100;
    // Mask of layers that can block the beam
    public LayerMask beamCollisionMask;

    // Beam Impact Prefab
    public GameObject goBeamImpactPrefab;
    // Beam Impact Instance
    GameObject goBeamImpactInstance;
    // Beam Particle Systems
    ParticleSystem[] aBeamParticleSystems;
    // Beam Impact Effect Disable Time
    public float fBeamImpactDisableTime = 5;
    // Disable Beam Effect Coroutine
    Coroutine disableBeamEffectCoroutine;

    // Beam Firing flag
    bool bBeamFiring = false;
    bool bBeamParticlesLooping = true;

    // Initialization
    protected override void Start()
    {
        base.Start();

        goBeamImpactInstance = Instantiate(goBeamImpactPrefab);
        aBeamParticleSystems = goBeamImpactInstance.GetComponentsInChildren<ParticleSystem>();
        goBeamImpactInstance.SetActive(false);

        weaponData.firingMode = eWeaponFireMode.Automatic;
        fBeamClipAmmo = iClipAmmo;
    }

    // On Spine Event
    protected override void OnSpineEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        base.OnSpineEvent(trackEntry, e);

        // Gunshot Event
        if (e.Data.Name == "Gunshot")
        {
            StartBeam();
        }
    }

    // Update
    protected override void Update()
    {
        base.Update();

        // If timescale is 0, return
        if (GameManager.GamePaused)
        {
            return;
        }

        if (bBeamFiring)
        {
            HandleBeamFiring();
        }
        else
        {
            if (AudioManager.Instance.BGetIsPlaying(weaponData.beamCollisionAudio.sAudioSlot))
            {
                weaponData.beamCollisionAudio.Stop();
            }
        }
    }

    // Handles input for firing
    protected override void HandleFireInput()
    {
        if (!bActive)
        {
            return;
        }

        // If can fire and not reloading, then fire weapon
        if (bCanFire && !bReloading)
        {
            if (fBeamClipAmmo > 0)
            {
                if (playerInput.GetButton("Fire"))
                {
                    Fire();

                    playerController.animator.SetArmBusy(true);
                    bFiring = true;
                }
            }
            else
            {
                if (playerInput.GetButtonDown("Fire"))
                {
                    FireEmpty();

                    playerController.animator.SetArmBusy(true);
                    bFiring = true;
                }
            }
        }

        if (bBeamFiring)
        {
            if (!playerInput.GetButton("Fire") || fBeamClipAmmo <= 0 || bReloading)
            {
                StopBeam();
            }
        }
    }

    // Fire Weapon
    protected override void Fire()
    {
        // Play Player Animation
        playerController.animator.FireWeaponAnimation();
        // Play Animation
        PlayAnimation("Fire", 0, false, false, 0, 0);

        if (weaponData.firingMode != eWeaponFireMode.Automatic)
        {
            bCanFire = false;
        }

        if (weaponData.firingMode != eWeaponFireMode.Single || !weaponData.bBoltAction)
        {
            StartCoroutine(ReenableFiring());
        }
    }

    // Starts firing the beam
    private void StartBeam()
    {
        // Activate line renderer and set beam width
        lineRenderer.enabled = true;
        lineRenderer.startWidth = fBeamWidth;
        lineRenderer.endWidth = fBeamWidth;
        laserSight.EnableLaser(false);

        // Fire direction
        Vector2 direction = tFirePoint.rotation * Vector2.up;

        // If facing left then reverse direction vector
        if (playerController.FacingLeft)
        {
            direction *= -1;
        }

        // Raycast Hit
        RaycastHit2D rayHit = Physics2D.Raycast(tFirePoint.position, direction, fBeamLength, beamCollisionMask);
        // Beam End Position
        Vector3 beamEndPoint;

        // If the ray hit, set the end point to the collision point
        if (rayHit)
        {
            beamEndPoint = rayHit.point;
        }
        else // Otherwise, set the end point to the length of the beam
        {
            beamEndPoint = tFirePoint.position + (Vector3)(direction.normalized * fBeamLength);
        }

        // Set the line renderer positions
        lineRenderer.SetPosition(0, tFirePoint.position);
        lineRenderer.SetPosition(1, beamEndPoint);

        SetBeamParticlesLoopActive(true);

        // Vibrate Gamepad
        StartVibrateGamepad();

        bBeamFiring = true;
        bBeamParticlesLooping = true;
        bFiring = true;
    }

    // Projects a ray and fires a beam
    private void HandleBeamFiring()
    {
        // If overheated, stop the beam and return
        if (!bFiring)
        {
            StopBeam();
            return;
        }

        // Fire direction
        Vector2 direction = tFirePoint.rotation * Vector2.up;

        // If facing left then reverse direction vector
        if (playerController.FacingLeft)
        {
            direction *= -1;
        }

        // Raycast Hit
        RaycastHit2D rayHit = Physics2D.Raycast(tFirePoint.position, direction, fBeamLength, beamCollisionMask);
        // Beam End Position
        Vector3 beamEndPoint;

        // If the ray hit, set the end point to the collision point
        if (rayHit)
        {
            beamEndPoint = rayHit.point;

            if (rayHit.collider.gameObject.layer == LayerMask.NameToLayer("Environment") ||
                rayHit.collider.gameObject.layer == LayerMask.NameToLayer("MovingPlatform"))
            {
                if (!goBeamImpactInstance.activeInHierarchy)
                {
                    goBeamImpactInstance.SetActive(true);
                }
                else
                {
                    SetBeamParticlesLoopActive(true);
                }

                goBeamImpactInstance.transform.position = beamEndPoint;
                goBeamImpactInstance.transform.rotation = Quaternion.Euler(rayHit.normal);
            }
            else
            {
                SetBeamParticlesLoopActive(false);
            }
        }
        else // Otherwise, set the end point to the length of the beam
        {
            SetBeamParticlesLoopActive(false);

            beamEndPoint = tFirePoint.position + (Vector3)(direction.normalized * fBeamLength);
        }

        // Set the line renderer positions
        lineRenderer.SetPosition(0, tFirePoint.position);
        lineRenderer.SetPosition(1, beamEndPoint);

        if (weaponData.fireAudio != null)
        {
            weaponData.fireAudio.Play(false);
        }

        // Increase Beam Recoil
        IncreaseBeamRecoil();

        // Beam Collision
        BeamDamageCollision(direction);

        // Shake Screen
        ProCamera2DShake.Instance.Shake(weaponData.fCameraShakeDuration, weaponData.v2CameraShakeStrength * fBeamRecoilMultiplier, 2);

        fBeamClipAmmo -= weaponData.fBeamAmmoRate * Time.deltaTime;
        // Set discrete clip ammo (for UI and reload management)
        iClipAmmo = Mathf.CeilToInt(fBeamClipAmmo);

        // If discrete clip ammo is 0, set continuous clip ammo to 0
        if (iClipAmmo <= 0)
        {
            iClipAmmo = 0;
            fBeamClipAmmo = 0;
        }

        if (onWeaponFire != null)
        {
            onWeaponFire(iClipAmmo);
        }

        if (TotalAmmo == 0)
        {
            if (onWeaponEmpty != null)
            {
                onWeaponEmpty();
            }
        }
    }

    // Stops the beam
    private void StopBeam()
    {
        lineRenderer.enabled = false;
        laserSight.EnableLaser(true);

        if (bBeamFiring)
        {
            if (weaponData.fireAudio != null)
            {
                weaponData.fireAudio.Stop();
            }

            SetBeamParticlesLoopActive(false);

            if (disableBeamEffectCoroutine != null)
            {
                StopCoroutine(disableBeamEffectCoroutine);
            }

            disableBeamEffectCoroutine = StartCoroutine(DisableBeamEffects());
        }

        if (!bReloading)
        {
            PlayAnimation("Idle");
        }

        recoilCoroutine = StartCoroutine(DecreaseBeamRecoil());

        StopVibrateGamepad();

        bBeamFiring = false;
        bFiring = false;
    }

    // Set the loop property on the beam impact particles systems
    private void SetBeamParticlesLoopActive(bool active)
    {
        if (goBeamImpactInstance.activeInHierarchy)
        {
            if (bBeamParticlesLooping != active)
            {
                for (int i = 0; i < aBeamParticleSystems.Length; i++)
                {
                    ParticleSystem.MainModule main = aBeamParticleSystems[i].main;
                    main.loop = active;

                    if (active && !aBeamParticleSystems[i].isPlaying)
                    {
                        aBeamParticleSystems[i].Play();
                    }
                }

                bBeamParticlesLooping = active;
            }
        }
    }

    // Disables the beam impact effect gameobject after a set interval
    private IEnumerator DisableBeamEffects()
    {
        yield return new WaitForSeconds(fBeamImpactDisableTime);

        for (int i = 0; i < aBeamParticleSystems.Length; i++)
        {
            aBeamParticleSystems[i].Stop();
        }

        goBeamImpactInstance.SetActive(false);
    }

	// Checks for damageable objects in the path of the beam and applies damage to those objects
	private void BeamDamageCollision(Vector2 direction)
	{
		RaycastHit2D hit = Physics2D.CircleCast(tFirePoint.position,fBeamWidth, direction, fBeamLength, beamCollisionMask);
		float damage = weaponData.fDamage;

		if (weaponData.firingMode == eWeaponFireMode.Automatic)
		{
			damage *= Time.deltaTime;
		}
			
		if (hit.collider != null)
		{
			HealthManager healthManager = hit.collider.GetComponent<HealthManager>();
		
			if (healthManager != null)
			{
				healthManager.TakeHealth(damage);

                if (healthManager.onRayCollision != null)
                {
                    healthManager.onRayCollision();    
                }
			}

			// Collision Audio
			if (weaponData.beamCollisionAudio != null)
			{
				weaponData.beamCollisionAudio.Play(false);
			}
		}
		else
		{
			if (AudioManager.Instance.BGetIsPlaying(weaponData.beamCollisionAudio.sAudioSlot))
			{
				weaponData.beamCollisionAudio.Stop();
			}
		}
	}

	// Increase Beam Recoil
	private void IncreaseBeamRecoil()
	{
		fAimRecoil += weaponData.fAimRecoilIncreaseRate * fRecoilMultiplier * Mathf.Sign(Random.Range(-1, 1)) * Time.deltaTime;

		if (Mathf.Abs(fAimRecoil) > weaponData.fMaxAimRecoil * fRecoilMultiplier)
		{
			fAimRecoil = weaponData.fMaxAimRecoil * Mathf.Sign(fAimRecoil);
		}
	
		bReduceRecoil = false;

		if (recoilCoroutine != null)
		{
			StopCoroutine(recoilCoroutine);
			recoilCoroutine = null;
		}

		fBeamRecoilMultiplier += weaponData.fRecoilIncreaseRate * Time.deltaTime;
		fBeamRecoilMultiplier = Mathf.Clamp01(fBeamRecoilMultiplier);

		fAimRecoil *= fBeamRecoilMultiplier;
	}

	// Decrease Beam Recoil
	private IEnumerator DecreaseBeamRecoil()
	{
		bReduceRecoil = true;
		yield return new WaitForSeconds(weaponData.fRecoilResetTime);

		while (bReduceRecoil)
		{
			fAimRecoil -= weaponData.fAimRecoilDecreaseRate * Time.deltaTime;
			fAimRecoil = Mathf.Clamp(fAimRecoil, 0, weaponData.fMaxAimRecoil);

			fBeamRecoilMultiplier -= weaponData.fRecoilDecreaseRate * Time.deltaTime;
			fBeamRecoilMultiplier = Mathf.Clamp01(fBeamRecoilMultiplier);

			if (fAimRecoil <= 0 && fBeamRecoilMultiplier <= 0)
			{
				bReduceRecoil = false;
			}

			yield return null;
		}
	}

	// Start Reload
	protected override void StartReload()
	{
		base.StartReload();

		StopBeam();
	}

	// End Reload
	protected override void EndReload()
	{
		base.EndReload();

		// Set continuous clip ammo
		fBeamClipAmmo = iClipAmmo;
	}

	// Cancel Reload
	public override void CancelReload()
	{
		base.CancelReload();

		StopBeam();
	}

	// Set Weapon Disabled
	public override void SetWeaponDisabled(bool disabled)
	{
		base.SetWeaponDisabled(disabled);

		if (disabled)
		{
			if (bBeamFiring)
			{
				StopBeam();
			}
		}
	}
}