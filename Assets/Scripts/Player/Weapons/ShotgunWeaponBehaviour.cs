using UnityEngine;

using Com.LuisPedroFonseca.ProCamera2D;

public class ShotgunWeaponBehaviour : WeaponBehaviour
{
	// Has the weapon been pumped flag
	bool bPumped = true;
	// Pump in Progress flag
	bool bPumpInProgress = false;

	// On Spine Event
	protected override void OnSpineEvent(Spine.TrackEntry trackEntry, Spine.Event e)
	{
		base.OnSpineEvent(trackEntry, e);

		// Gunshot event
		if (e.Data.Name == "Gunshot")
		{
			CreateShotgunBlast();
		}
	}

    // Creates a bullet with velocity and recoil
    private void CreateShotgunBlast()
    {
        float recoil = fRecoil + weaponData.fShotgunSpreadRadius;

        for (int i = 0; i < weaponData.iShotgunProjectiles; i++)
        {
            Vector3 bulletRotation = tFirePoint.rotation.eulerAngles;

            if (bFacingLeft)
            {
                bulletRotation.z += 180;
            }

            bulletRotation.z += Random.Range(-recoil, recoil);

            float speedMultiplier = Random.Range(0.5f, 1.5f);

            // Create the bullet and add recoil
            int bulletID;
            GameObject bullet = ObjectPoolManager.Instance.GOSpawnObject(sBulletPoolName, tFirePoint.position, Quaternion.Euler(bulletRotation), out bulletID);
            aProjectileBehaviours[bulletID].LaunchProjectile(speedMultiplier);
        }

        if (weaponData.firingMode != eWeaponFireMode.Burst)
        {
            // Increase Recoil
            IncreaseRecoil();
        }

        // Play Audioclip
        weaponData.fireAudio.Play();

        // If fire tail audio exists, play it
        if (weaponData.fireTailAudio != null)
        {
            weaponData.fireTailAudio.Play();
        }

        // Shake Screen
        ProCamera2DShake.Instance.Shake(weaponData.fCameraShakeDuration, weaponData.v2CameraShakeStrength, 2);
        // Fade Laser
        laserSight.StartLaserFade();
        // Vibrate Gamepad
        VibrateGamepadTimed();

        // Decrement ammo and disable firing
        iClipAmmo--;

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
}