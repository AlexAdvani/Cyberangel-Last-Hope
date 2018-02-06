using UnityEngine;

[CreateAssetMenu(menuName = "This Game Data/Weapon Data")]
public class WeaponData : ScriptableObject
{
	// General Weapon Variables

	// Weapon Name
	public string sName;
	// Weapon Prefab
	public GameObject goWeaponPrefab;
	// Casing Prefab
	public GameObject goCasingPrefab;
    // Casing Object Pool Size
    public int iCasingPoolSize;
	// Clip Prefab
	public GameObject goClipPrefab;
    // Clip Object Pool Size
    public int iClipPoolSize;
	// Clip Sprite
	public Sprite clipSprite;
	// Weapon-Hand Offset Position
	public Vector3 v3WeaponOffset;
	// Weapon Holster Offset Position
	public Vector3 v3HolsterOffset;
	// Position Offset Distance for Aim IK Points
	public float fAimIKOffset;
	// Player Arm Animation Type
	public string sPlayerAnimationType;

	// Fire AudioEvent
	public AudioEvent fireAudio;
	// Fire Tail AudioEvent
	public AudioEvent fireTailAudio;
	// Empty AudioEvent
	public AudioEvent emptyAudio;
	// Reload Unload AudioEvent
	public AudioEvent reloadUnloadAudio;
	// Reload Load AudioEvent
	public AudioEvent reloadLoadAudio;
	// Reload Slide AudioEvent
	public AudioEvent reloadSlideAudio;
	// Reload Bolt AudioEvent
	public AudioEvent reloadBoltAudio;

	// Base Weapon Variables

	// Weapon Type
	public eWeaponType weaponType;
	// Damage
	public float fDamage;
	// Max Ammo
	public int iMaxAmmo;
	// Clip Size
	public int iClipSize;
	// Firing Mode - Single / Automatic / Burst
	public eWeaponFireMode firingMode;
	// Bolt Action flag - Single Only
	public bool bBoltAction;
	// Shots Per Burst - Burst Only
	public int iShotsPerBurst;
	// Burst Shot Time - Burst Only
	public float fBurstShotTime;
	// Fire Rate
	public float fFireRate;
	// Clip Reload flag
	public bool bClipReload = true;
	// Can Full Reload flag
	public bool bCanFullReload;

	// Two Handed Weapon flag
	public eWeaponHoldType holdType;
	// Recoil Increase Rate
	public float fRecoilIncreaseRate;
	// Max Recoil
	public float fMaxRecoil;
	// Recoil Decrease Rate
	public float fRecoilDecreaseRate;
	// Time Before Recoil Decreases
	public float fRecoilResetTime;
	// Aim Recoil Increase Rate
	public float fAimRecoilIncreaseRate;
	// Max Aim Recoil
	public float fMaxAimRecoil;
	// Aim Recoil Decrease Rate
	public float fAimRecoilDecreaseRate;
	// Time before Aim Recoil Decreases
	public float fAimRecoilResetTime;

	// Gunshot Camera Shake Strength
	public Vector2 v2CameraShakeStrength;
	// Gunshot Camera Shake Duration
	public float fCameraShakeDuration;
	// Vibration Motor Strength
	public float fVibrationStrength;
	// Vibration Time
	public float fVibrationTime;

	// Bullet Prefab - Bullet/Shotgun Variable
	public GameObject goBulletPrefab;
    // Bullet Object Pool Size - Bullet/Shotgun Variable
    public int iBulletPoolSize;

	// Shotgun Variables

	// Number of Bullet Projectiles in Shotgun Spread - Shotgun Variable
	public int iShotgunProjectiles;
	// Shotgun Bullet Spread Radius - Used to set inital spread at zero recoil
	public float fShotgunSpreadRadius;

	// Beam Weapon Variables

	// Beam Ammo Consumption Rate
	public float fBeamAmmoRate;
	// Beam Collision Audio Event
	public AudioEvent beamCollisionAudio;

	// Instantiates the weapon under a given parent transform and returns it
	public GameObject GOInstantiateWeapon(Transform parent)
	{
		GameObject weapon = Instantiate(goWeaponPrefab, parent);

		return weapon;
	}
}

// Weapon Type
public enum eWeaponType
{
	Bullet,
	Shotgun,
	Beam
}