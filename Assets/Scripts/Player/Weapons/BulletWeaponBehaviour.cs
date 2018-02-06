using UnityEngine;

using Com.LuisPedroFonseca.ProCamera2D;

public class BulletWeaponBehaviour : WeaponBehaviour
{
    // On Spine Event
    protected override void OnSpineEvent(Spine.TrackEntry trackEntry, Spine.Event e)
	{
		base.OnSpineEvent(trackEntry, e);

		// Gunshot event
		if (e.Data.Name == "Gunshot")
		{
			CreateBullet();
		}
	}

	// Creates a bullet with velocity and recoil
	private void CreateBullet()
	{
		// Add recoil to rotation
		Vector3 bulletRotation = tFirePoint.rotation.eulerAngles;

		if (bFacingLeft)
		{
			bulletRotation.z += 180;
		}

		bulletRotation.z += Random.Range(-fRecoil, fRecoil);

        // Create the bullet and add recoil
        int bulletID;
        GameObject bullet = ObjectPoolManager.Instance.GOSpawnObject(sBulletPoolName, tFirePoint.position, Quaternion.Euler(bulletRotation), out bulletID);
		aProjectileBehaviours[bulletID].LaunchProjectile();

		if (weaponData.firingMode != eWeaponFireMode.Burst)
		{
			// Increase Recoil
			IncreaseRecoil();
		}

		// Play Audioclip
		if (weaponData.fireAudio != null)
		{
			weaponData.fireAudio.Play();
		}

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