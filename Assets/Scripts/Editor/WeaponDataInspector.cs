using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WeaponData))]
public class WeaponDataInspector : Editor
{
	#region Serialized Properties

	// General Variables

	// Weapon Name - String
	SerializedProperty weaponName;
	// Weapon Prefab - GameObject
	SerializedProperty weaponPrefab;
	// Casing Prefab - GameObject
	SerializedProperty casingPrefab;
    // Casing Pool Size - Int
    SerializedProperty casingPoolSize;
	// Clip Prefab - GameObject
	SerializedProperty clipPrefab;
    // Clip Pool Size - Int
    SerializedProperty clipPoolSize;
    // Clip Sprite - Sprite
    SerializedProperty clipSprite;
	// Weapon Offset - Vector3
	SerializedProperty weaponOffset;
	// Holster Offset - Vector3
	SerializedProperty holsterOffset;
	// Aim IK Offset - Float
	SerializedProperty aimIKOffset;
	// Player Animation Type - String
	SerializedProperty playerAnimationType;

    // UI Variables

    // UI Weapon Icon - Sprite
    SerializedProperty uiWeaponIcon;
    // UI Ammo Icon - Sprite
    SerializedProperty uiAmmoIcon;
    // UI Row Count - Int
    SerializedProperty uiRowCount;

	// Audio Variables

	// Fire - AudioEvent
	SerializedProperty fireAudio;
	// Fire Tail - AudioEvent
	SerializedProperty fireTailAudio;
	// Empty - AudioEvent
	SerializedProperty emptyAudio;
	// Reload Unload - AudioEvent
	SerializedProperty reloadUnloadAudio;
	// Reload Load - AudioEvent
	SerializedProperty reloadLoadAudio;
	// Reload Slide - AudioEvent
	SerializedProperty reloadSlideAudio;
	// Reload Bolt - AudioEvent
	SerializedProperty reloadBoltAudio;

	// Base Weapon Variables

	// Weapon Type - Enum
	SerializedProperty weaponType;
	// Damage - Float
	SerializedProperty damage;
	// Max Ammo - Int
	SerializedProperty maxAmmo;
	// Clip Size - Int
	SerializedProperty clipSize;
	// Firing Mode - Enum
	SerializedProperty firingMode;
	// Bolt Action - Bool
	SerializedProperty boltAction;
	// Shots Per Burst - Int
	SerializedProperty shotsPerBurst;
	// Burst Shot Time - Float
	SerializedProperty burstShotTime;
	// Fire Rate - Float
	SerializedProperty fireRate;
	// Clip Reload - Bool
	SerializedProperty clipReload;
	// Can Full Reload - Bool
	SerializedProperty canFullReload;

	// Hold Type - Enum
	SerializedProperty holdType;
	// Recoil Increase Rate - Float
	SerializedProperty recoilIncreaseRate;
	// Max Recoil - Float
	SerializedProperty maxRecoil;
	// Recoil Decrease Rate - Float
	SerializedProperty recoilDecreaseRate;
	// Recoil Reset Time - Float
	SerializedProperty recoilResetTime;
	// Aim Recoil Increase Rate - Float
	SerializedProperty aimRecoilIncreaseRate;
	// Max Aim Recoil - Float
	SerializedProperty maxAimRecoil;
	// Aim Recoil Decrease Rate - Float
	SerializedProperty aimRecoilDecreaseRate;
	// Aim Recoil Reset Time - Float
	SerializedProperty aimRecoilResetTime;

	// Camera Shake Strength - Vector2
	SerializedProperty cameraShakeStrength;
	// Camera Shake Duration - Float
	SerializedProperty cameraShakeDuration;
	// Vibration Strength - Float
	SerializedProperty vibrationStrength;
	// Vibration Time - Float
	SerializedProperty vibrationTime;

	// Bullet Prefab - GameObject
	SerializedProperty bulletPrefab;
    // Bullet Pool Size - Int
    SerializedProperty bulletPoolSize;

    // Shotgun Projectiles - Int
    SerializedProperty shotgunProjectiles;
	// Shotgun Spread Radius - Float
	SerializedProperty shotgunSpreadRadius;

	// Beam Ammo Rate = Float
	SerializedProperty beamAmmoRate;
	// Beam Collision Audio - Audio Event
	SerializedProperty beamCollisionAudio;

    // Weapon Info Stats Variables

    // Description Info
    SerializedProperty descriptionInfo;
    // Damage Info Stat
    SerializedProperty damageInfoStat;
    // Fire Rate Info Stat
    SerializedProperty fireRateInfoStat;

	#endregion

	// Show Audio Foldout flag
	bool bShowAudio;

	// On Enable
	void OnEnable()
	{
		InitializeProperties();
	}

	// Finds all of the weapon properties
	private void InitializeProperties()
	{
		// General Weapon Properties
		weaponName = serializedObject.FindProperty("sName");
		weaponPrefab = serializedObject.FindProperty("goWeaponPrefab");
		casingPrefab = serializedObject.FindProperty("goCasingPrefab");
        casingPoolSize = serializedObject.FindProperty("iCasingPoolSize");
        clipPrefab = serializedObject.FindProperty("goClipPrefab");
        clipPoolSize = serializedObject.FindProperty("iClipPoolSize");
        clipSprite = serializedObject.FindProperty("clipSprite");
		weaponOffset = serializedObject.FindProperty("v3WeaponOffset");
		holsterOffset = serializedObject.FindProperty("v3HolsterOffset");
		aimIKOffset = serializedObject.FindProperty("fAimIKOffset");
		playerAnimationType = serializedObject.FindProperty("sPlayerAnimationType");

        // UI Properties
        uiWeaponIcon = serializedObject.FindProperty("uiWeaponIcon");
        uiAmmoIcon = serializedObject.FindProperty("uiAmmoIcon");
        uiRowCount = serializedObject.FindProperty("iUIRowCount");

        // Audio Properties
        fireAudio = serializedObject.FindProperty("fireAudio");
		fireTailAudio = serializedObject.FindProperty("fireTailAudio");
		emptyAudio = serializedObject.FindProperty("emptyAudio");
		reloadUnloadAudio = serializedObject.FindProperty("reloadUnloadAudio");
		reloadLoadAudio = serializedObject.FindProperty("reloadLoadAudio");
		reloadSlideAudio = serializedObject.FindProperty("reloadSlideAudio");
		reloadBoltAudio = serializedObject.FindProperty("reloadBoltAudio");

		// Base Weapon Properties
		weaponType = serializedObject.FindProperty("weaponType");
		damage = serializedObject.FindProperty("fDamage");
		maxAmmo = serializedObject.FindProperty("iMaxAmmo");
		clipSize = serializedObject.FindProperty("iClipSize");
		firingMode = serializedObject.FindProperty("firingMode");
		boltAction = serializedObject.FindProperty("bBoltAction");
		shotsPerBurst = serializedObject.FindProperty("iShotsPerBurst");
		burstShotTime = serializedObject.FindProperty("fBurstShotTime");
		fireRate = serializedObject.FindProperty("fFireRate");
		clipReload = serializedObject.FindProperty("bClipReload");
		canFullReload = serializedObject.FindProperty("bCanFullReload");

		holdType = serializedObject.FindProperty("holdType");
		recoilIncreaseRate = serializedObject.FindProperty("fRecoilIncreaseRate");
		maxRecoil = serializedObject.FindProperty("fMaxRecoil");
		recoilDecreaseRate = serializedObject.FindProperty("fRecoilDecreaseRate");
		recoilResetTime = serializedObject.FindProperty("fRecoilResetTime");
		aimRecoilIncreaseRate = serializedObject.FindProperty("fAimRecoilIncreaseRate");
		maxAimRecoil = serializedObject.FindProperty("fMaxAimRecoil");
		aimRecoilDecreaseRate = serializedObject.FindProperty("fAimRecoilDecreaseRate");
		aimRecoilResetTime = serializedObject.FindProperty("fAimRecoilResetTime");

		cameraShakeStrength = serializedObject.FindProperty("v2CameraShakeStrength");
		cameraShakeDuration = serializedObject.FindProperty("fCameraShakeDuration");
		vibrationStrength = serializedObject.FindProperty("fVibrationStrength");
		vibrationTime = serializedObject.FindProperty("fVibrationTime");

		// Bullet Weapon Properties
		bulletPrefab = serializedObject.FindProperty("goBulletPrefab");
        bulletPoolSize = serializedObject.FindProperty("iBulletPoolSize");
	
		// Shotgun Weapon Properties
		shotgunProjectiles = serializedObject.FindProperty("iShotgunProjectiles");
		shotgunSpreadRadius = serializedObject.FindProperty("fShotgunSpreadRadius");

		// Beam Weapon Properties
		beamAmmoRate = serializedObject.FindProperty("fBeamAmmoRate");
		beamCollisionAudio = serializedObject.FindProperty("beamCollisionAudio");

        // Weapon Info Stats Properties
        descriptionInfo = serializedObject.FindProperty("sDescriptionInfo");
        damageInfoStat = serializedObject.FindProperty("iDamageInfoStat");
        fireRateInfoStat = serializedObject.FindProperty("iFireRateInfoStat");
    }

	// Inspector GUI Update
	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		DrawGeneralFields();
		DrawBaseWeaponFields();

		switch (weaponType.enumValueIndex)
		{
			case 0:
				DrawBulletWeaponFields();
			break;

			case 1:
				DrawShotgunWeaponFields();
			break;

			case 2:
				DrawBeamWeaponFields();
			break;
		}

        DrawWeaponInfoFields();

		serializedObject.ApplyModifiedProperties();
	}

	// Draw General Fields 
	private void DrawGeneralFields()
	{
		EditorGUILayout.LabelField(new GUIContent("Weapon Object Settings"), EditorStyles.boldLabel);

		EditorGUILayout.PropertyField(weaponName, new GUIContent("Name"));
		EditorGUILayout.PropertyField(weaponPrefab, new GUIContent("Weapon Prefab"));
		EditorGUILayout.PropertyField(casingPrefab, new GUIContent("Casing Prefab"));
        EditorGUILayout.PropertyField(casingPoolSize, new GUIContent("Casing Object Pool Size"));
        EditorGUILayout.PropertyField(clipPrefab, new GUIContent("Clip Prefab"));
        EditorGUILayout.PropertyField(clipPoolSize, new GUIContent("Clip Object Pool Size"));
        EditorGUILayout.PropertyField(clipSprite, new GUIContent("Clip Sprite"));
		EditorGUILayout.PropertyField(weaponOffset, new GUIContent("Weapon Offset"));
		EditorGUILayout.PropertyField(holsterOffset, new GUIContent("Holster Offset"));
		EditorGUILayout.PropertyField(aimIKOffset, new GUIContent("Aim IK Offset"));
		EditorGUILayout.PropertyField(playerAnimationType, new GUIContent("Animation Type"));

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(uiWeaponIcon, new GUIContent("UI Weapon Icon"));
        EditorGUILayout.PropertyField(uiAmmoIcon, new GUIContent("UI Ammo Icon"));
        EditorGUILayout.PropertyField(uiRowCount, new GUIContent("UI Row Count"));

        EditorGUILayout.Space();

        bShowAudio = EditorGUILayout.Foldout(bShowAudio, new GUIContent("Audio"));

		if (bShowAudio)
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(fireAudio, new GUIContent("Weapon Fire"));
			EditorGUILayout.PropertyField(fireTailAudio, new GUIContent("Weapon Fire Tail"));
			EditorGUILayout.PropertyField(emptyAudio, new GUIContent("Weapon Empty"));
			EditorGUILayout.PropertyField(reloadUnloadAudio, new GUIContent("Weapon Reload Unload"));
			EditorGUILayout.PropertyField(reloadLoadAudio, new GUIContent("Weapon Reload Load"));
			EditorGUILayout.PropertyField(reloadSlideAudio, new GUIContent("Weapon Reload Slide"));
			EditorGUILayout.PropertyField(reloadBoltAudio, new GUIContent("Weapon Reload Bolt"));
			EditorGUI.indentLevel--;
		}

		EditorGUILayout.Space();
	}

	// Draw Base Weapon Fields
	private void DrawBaseWeaponFields()
	{
		EditorGUILayout.LabelField(new GUIContent("Base Weapon Settings"), EditorStyles.boldLabel);

		EditorGUILayout.PropertyField(damage, new GUIContent("Damage"));
		EditorGUILayout.PropertyField(weaponType, new GUIContent("Weapon Type"));
		EditorGUILayout.PropertyField(maxAmmo, new GUIContent("Max Ammo"));
		EditorGUILayout.PropertyField(clipSize, new GUIContent("Clip Size"));
		EditorGUILayout.PropertyField(firingMode, new GUIContent("Fire Mode"));

		if (firingMode.enumValueIndex == 0)
		{
			EditorGUILayout.PropertyField(boltAction, new GUIContent("Bolt Action"));
		}
		else if (firingMode.enumValueIndex == 2)
		{
			EditorGUILayout.PropertyField(shotsPerBurst, new GUIContent("Shots Per Burst"));
			EditorGUILayout.PropertyField(burstShotTime, new GUIContent("Burst Shot Fire Rate"));
		}

		EditorGUILayout.PropertyField(fireRate, new GUIContent("Fire Rate"));
		EditorGUILayout.PropertyField(clipReload, new GUIContent("Clip Reload"));
		EditorGUILayout.PropertyField(canFullReload, new GUIContent("Can Full Reload"));

		EditorGUILayout.Space();

		EditorGUILayout.PropertyField(holdType, new GUIContent("Hold Type"));
		EditorGUILayout.PropertyField(recoilIncreaseRate, new GUIContent("Recoil Increase Rate (Degrees)"));
		EditorGUILayout.PropertyField(maxRecoil, new GUIContent("Max Recoil (Degrees)"));
		EditorGUILayout.PropertyField(recoilDecreaseRate, new GUIContent("Recoil Decrease Rate (Degrees)"));
		EditorGUILayout.PropertyField(recoilResetTime, new GUIContent("Recoil Reset Time"));
		EditorGUILayout.PropertyField(aimRecoilIncreaseRate, new GUIContent("Aim Recoil Increase Rate"));
		EditorGUILayout.PropertyField(maxAimRecoil, new GUIContent("Max Aim Recoil"));
		EditorGUILayout.PropertyField(aimRecoilDecreaseRate, new GUIContent("Aim Recoil Decrease Rate"));
		EditorGUILayout.PropertyField(aimRecoilResetTime, new GUIContent("Aim Recoil Reset Time"));

		EditorGUILayout.Space();

		EditorGUILayout.PropertyField(cameraShakeStrength, new GUIContent("Camera Shake Strength"));
		EditorGUILayout.PropertyField(cameraShakeDuration, new GUIContent("Camera Shake Duration"));
		EditorGUILayout.Slider(vibrationStrength, 0f, 1f, new GUIContent("Vibration Strength"));
		EditorGUILayout.PropertyField(vibrationTime, new GUIContent("Vibration Time"));

		EditorGUILayout.Space();
	}

	// Draw BulletWeaponFields
	private void DrawBulletWeaponFields()
	{
		EditorGUILayout.LabelField(new GUIContent("Bullet Weapon Settings"), EditorStyles.boldLabel);

		EditorGUILayout.PropertyField(bulletPrefab, new GUIContent("Bullet Prefab"));
        EditorGUILayout.PropertyField(bulletPoolSize, new GUIContent("Bullet Object Pool Size"));
	}

	// Draw ShotgunWeaponFields
	private void DrawShotgunWeaponFields()
	{
		EditorGUILayout.LabelField(new GUIContent("Shotgun Weapon Settings"), EditorStyles.boldLabel);

		EditorGUILayout.PropertyField(bulletPrefab, new GUIContent("Bullet Prefab"));
        EditorGUILayout.PropertyField(bulletPoolSize, new GUIContent("Bullet Object Pool Size"));
        EditorGUILayout.PropertyField(shotgunProjectiles, new GUIContent("Shotgun Spread Projectiles"));
		EditorGUILayout.PropertyField(shotgunSpreadRadius, new GUIContent("Shotgun Spread Radius (Degrees)"));
	}

	// Draw Beam Weapon Fields
	private void DrawBeamWeaponFields()
	{
		EditorGUILayout.LabelField(new GUIContent("Beam Weapon Settings"), EditorStyles.boldLabel);

		EditorGUILayout.PropertyField(beamAmmoRate, new GUIContent("Beam Ammo Rate"));
		EditorGUILayout.PropertyField(beamCollisionAudio, new GUIContent("Beam Collision Audio"));
	}

    // Draw Weapon Info Fields
    private void DrawWeaponInfoFields()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField(new GUIContent("Weapon Info Settings"), EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(descriptionInfo, new GUIContent("Description Info"));
        EditorGUILayout.PropertyField(damageInfoStat, new GUIContent("Damage Info Stat"));
        EditorGUILayout.PropertyField(fireRateInfoStat, new GUIContent("Fire Rate Info Stat"));
    }
}