using System.Collections;

using UnityEngine;

using Spine;
using Spine.Unity;

public class PlayerAnimation : SpineAnimatorBase
{
	// Player Controller
	[HideInInspector]
	public PlayerController playerController;

	// Current Body State;
	string sBodyState;
	// Current Arm State;
	string sArmState;

	// Player Walk Speed
	public float fWalkSpeed = 1f;
	// Player Walk Animation Speed (Min, Max)
	public Vector2 v2WalkAnimSpeed;
	// Player Run Animation Speed (Min, Max);
	public Vector2 v2RunAnimSpeed;

	// Facing Left flag
	bool bFacingLeft = false;
	// Orientation Based Animation flag
	bool bOrientatedAnimation = false;

	// Weapon Pivot IK Bone
	[HideInInspector]
	public Bone weaponPivotIKBone;
	// Weapon Aim IK Bone
	Bone weaponAimIKBone;
	// Head Pivot IK Bone
	Bone headPivotIKBone;
	// Head Aim IK Bone
	Bone headAimIKBone;

	// Weapon Pivot IK Bone Name
	[SpineBone]
	public string sWeaponPivotIKName;
	// Weapon Aim IK Bone Name
	[SpineBone]
	public string sWeaponAimIKName;
	// Head Pivot IK Bone Name
	[SpineBone]
	public string sHeadPivotIKName;
	// Head Aim IK Bone Name
	[SpineBone]
	public string sHeadAimIKName;

	// Current Weapon Animation Type (Used for determining the arm animations when holding a weapon)
	string sWeaponAnimationType = "";
	// Current Weapon Hold Animation Type (Used for determining the arm holstering/unholstering animations)
	string sWeaponHoldAnimationType = "";
	// Current Arm Animation Weapon Exclusive flag
	bool bArmAnimWeaponExclusive = true;
	// Current Arm Holstering/Unholstering flag
	bool bArmAnimHolstering = false;

	// Time for arm to remain in the Ready state
	public float fArmReadyTime;
	// Timer for the arm ready state
	float fArmReadyTimer = 0f;

	// Arm Busy flag
	bool bArmBusy = false;
	// Previously Arm Busy flag
	bool bPrevArmBusy = false;
	// Firing Weapon flag
	bool bWeaponFire = false;

	// Front Reload Clip Renderer
	public SpriteRenderer frontReloadClipRenderer;
	// Rear Reload Clip Renderer
	public SpriteRenderer rearReloadClipRenderer;
	// Reload Clip In Hand flag
	bool bClipInHand = false;

	[Space(5)]
	[Header("Audio")]

	// Footstep Audio Data
	public SimpleAudioEvent runFootstepAudio;
	// Footstep Audio Data
	public SimpleAudioEvent walkFootstepAudio;
    // Footstep Audio Data
    public SimpleAudioEvent landFootstepAudio;
    // Jump Audio Data
    public SimpleAudioEvent jumpAudio;

	// Jump Thruster Audio Data
	public SimpleAudioEvent jumpThrusterAudio;
    // Dash Thruster Audio Data
    public SimpleAudioEvent dashThrusterAudio;
    // Wall Stick Audio Data
    public SimpleAudioEvent wallStickAudio;
    // Wall Slide Audio Data
    public SimpleAudioEvent wallSlideAudio;
    // Wall Jump Audio Data
    public SimpleAudioEvent wallJumpAudio;
    // Corner Grab Audio Data
    public SimpleAudioEvent cornerGrabAudio;

    [Space(5)]
    [Header("Effects")]

    // Body Thruster Trail Renderer
    public TrailRenderer bodyThrusterTrail;
    // Front LegThruster Trail Renderer
    public TrailRenderer frontLegThrusterTrail;
    // Rear Leg Thruster Trail Renderer
    public TrailRenderer rearLegThrusterTrail;

    // Front Hand Dust Particle System
    public ParticleSystem frontHandDustParticle;
    // Rear Hand Dust Particle System
    public ParticleSystem rearHandDustParticle;
    // Front Foot Dust Particle System
    public ParticleSystem frontFootDustParticle;
    // Rear Foot Dust Particle System
    public ParticleSystem rearFootDustParticle;

    // Are Wallsliding Effects playing flag
    bool bWallslidingEffect = false;

    #region Public Properties

    public string AnimationState
	{
		get { return sBodyState; }
	}

	public bool ArmBusy
	{
		get { return bArmBusy; }
	}

	#endregion

	// Initialization (Earliest)
	protected override void Awake()
	{
		base.Awake();

		playerController = transform.parent.GetComponent<PlayerController>();
		playerController.aOnWeaponEquip += GetCurrentWeaponAnimationType;
	}

	// Initialization
	protected override void Start()
	{
		base.Start();

		skeleton.state.Event += OnSpineEvent;
		skeleton.state.Complete += OnAnimationComplete;
		skeleton.UpdateLocal += SetAimOrientation;

		InitializeAimBones();

        playerController.Motor.onBaseJump += PlayJumpAudio;
		playerController.Motor.onLanded += PlayLandAudio;
		playerController.Motor.onCornerGrab += PlayCornerGrabAudio;
        playerController.Motor.onWallStick += PlayWallStickAudio;
        playerController.Motor.onWallSlide += StartWallSlideEffects;
        playerController.Motor.onWallJump += PlayWallJumpAudio;
        playerController.Motor.onDashEnd += EndDashEffects;

        // TEST - Focused Expression (Update with Game State)
        PlayAnimation("Expression-Focused", 3);

		StartCoroutine(Blink());
	}

	// Update
	void Update()
	{
		HandleTimers();

		HandleBodyAnimation();
		HandleArmAnimation();

        // If player has stopped reloading and the clips are still active, then disable them
		if (!playerController.CurrentWeapon.Reloading)
		{
			if (bClipInHand)
			{
				frontReloadClipRenderer.enabled = false;
				rearReloadClipRenderer.enabled = false;
			}
		}

        // If the wallsliding effect is still active and the player is not wallsliding anymore, deactivate it
        if (bWallslidingEffect)
        {
            if (playerController.Motor.motorState != PlatformerMotor2D.MotorState.WallSliding)
            {
                EndWallSlideEffects();
            }
        }
	}

	#region Spine Events

	// Spine Events
	private void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
	{
		switch (e.Data.Name)
		{
			case "FootstepRun":
				PlayRunFootstepAudio();
			break;

			case "FootstepWalk":
				PlayWalkFootstepAudio();
			break;

			case "Clip-Attach":
				AttachReloadClip();
			break;

			case "Clip-Layer":
				ChangeClipLayer();
			break;

			case "Clip-Detach":
				DetachReloadClip();
			break;

			case "Weapon-Holster":
				playerController.HolsterWeapon();
			break;

			case "Weapon-Unholster":
				playerController.UnholsterWeapon();
			break;

			case "ThrusterDash":
				StartDashEffects();
			break;

			case "ThrusterJump":
				PlayJumpThrusterAudio();
			break;
		}
	}

	// On Animation Complete
	private void OnAnimationComplete(TrackEntry trackEntry)
	{
		// If animating arms
		if (trackEntry.TrackIndex == 1) 
		{
			// If firing weapon
			if (bWeaponFire)
			{
				EndFireWeaponAnimation();
			}

			// If switching weapons
			if (trackEntry.Animation.Name.Contains("Holster"))
			{
				playerController.bHolsteringWeapon = false;
				playerController.bUnholsteringWeapon = true;

				GetCurrentWeaponAnimationType();
			}

			if (trackEntry.Animation.Name.Contains("Unholster"))
			{
				playerController.bUnholsteringWeapon = false;
				bArmBusy = false;
				bPrevArmBusy = true;
			}
		}
	}

	#endregion

	// Finds and Gets Aim IK Bones
	private void InitializeAimBones()
	{
		weaponPivotIKBone = skeleton.skeleton.FindBone(sWeaponPivotIKName);
		weaponAimIKBone = skeleton.skeleton.FindBone(sWeaponAimIKName);
		headPivotIKBone = skeleton.skeleton.FindBone(sHeadPivotIKName);
		headAimIKBone = skeleton.skeleton.FindBone(sHeadAimIKName);
	}

	// Gets the Current Weapon Animation Type
	public void GetCurrentWeaponAnimationType()
	{
		WeaponBehaviour weapon = playerController.CurrentWeapon;

		if (weapon == null)
		{
			sWeaponAnimationType = "";
			sWeaponHoldAnimationType = "";
		}
		else
		{
			sWeaponAnimationType = weapon.PlayerAnimationType + "-";

			if (weapon.HoldType == eWeaponHoldType.Secondary)
			{
				sWeaponHoldAnimationType = "Secondary-";
			}
			else if (weapon.HoldType == eWeaponHoldType.Primary)
			{
				sWeaponHoldAnimationType = "Primary-";
			}
		}
	}

	// Handles animation timers
	private void HandleTimers()
	{
		// If arms are in use but not aiming
		if (bArmBusy && !playerController.bAimingDownSights)
		{
			// Increment arm ready time
			fArmReadyTimer += Time.deltaTime;

			// If time elapsed, reset arms back to idle
			if (fArmReadyTimer >= fArmReadyTime)
			{
				fArmReadyTimer = 0;
				bArmBusy = false;
			}
		}
	}

	#region Main Animation

	// Handle Body Animation
	private void HandleBodyAnimation()
	{
		// Playback Speed
		float playSpeed = 1f;
		// Looping flag
		bool looping = false;
		bool useOrientation = false;
		bool facingLeft = bFacingLeft;
		string orientation = "";

		float playPosition = 0;
		float mix = -1;

        // Check Motor State
        switch (playerController.Motor.motorState)
        {
            // On Ground
            case PlatformerMotor2D.MotorState.OnGround:
                // Horizontal movement
                float xMove = Mathf.Abs(playerController.Motor.normalizedXMovement);
                // Horizontal velocity
                float horVel = Mathf.Abs(playerController.Motor.velocity.x);

                // Set animation based on movement speed
                if (xMove > 0.05f)
                {
                    if (sBodyState == "Crouch" || sBodyState == "Slide")
                    {
                        mix = 0.1f;
                    }

                    if (xMove <= fWalkSpeed)
                    {
                        sBodyState = "Walk";

                        playSpeed = (horVel / (playerController.Motor.groundSpeed * fWalkSpeed)) * (v2WalkAnimSpeed.y - v2WalkAnimSpeed.x) + v2WalkAnimSpeed.x;
                    }
                    else
                    {
                        sBodyState = "Run";

                        playSpeed = (horVel / playerController.Motor.groundSpeed) * (v2RunAnimSpeed.y - v2RunAnimSpeed.x) + v2RunAnimSpeed.x;
                    }

                    if (playerController.AimState || bArmBusy)
                    {
                        if ((bFacingLeft && playerController.Motor.velocity.x > 0.1f) ||
                            (!bFacingLeft && playerController.Motor.velocity.x < -0.1f))
                        {
                            sBodyState += "-Reverse";
                        }
                    }
                }
                else // Otherwise, set animation based on if arms are in use
                {
                    if (playerController.Crouching)
                    {
                        sBodyState = "Crouch";
                        mix = 0.1f;
                    }
                    else
                    {
                        if (playerController.CurrentWeapon != null)
                        {
                            if (sBodyState == "Crouch" || sBodyState == "Slide")
                            {
                                mix = 0.1f;
                            }

                            if (bArmBusy || playerController.CurrentWeapon.Reloading)
                            {
                                sBodyState = "Neutral";
                            }
                            else
                            {
                                sBodyState = "Idle";
                            }
                        }
                    }
                }

                looping = true;
                break;

            // Jumping
            case PlatformerMotor2D.MotorState.Jumping:
                // Set animation based on whether player is double jumping
                if (playerController.AirJumping)
                {
                    sBodyState = "Jump2";
                }
                else
                {
                    // Set animation based on whether player is wall jumping
                    if (playerController.WallJumping)
                    {
                        if ((bFacingLeft && playerController.LastWallCollisionDirLeft) ||
                            (!bFacingLeft && !playerController.LastWallCollisionDirLeft))
                        {
                            sBodyState = "WallJump-Reverse";
                        }
                        else
                        {
                            sBodyState = "WallJump";
                        }

                        skeleton.skeleton.SetBonesToSetupPose();
                        mix = 0;
                    }
                    else
                    {
                        sBodyState = "Jump";
                    }
                }
                break;

            // Falling
            case PlatformerMotor2D.MotorState.Falling:
                sBodyState = "Fall";
                break;

            // Dashing
            case PlatformerMotor2D.MotorState.Dashing:
                if (playerController.Sliding)
                {
                    sBodyState = "Slide";
                }
                else
                {
                    sBodyState = "Dash";
                }

				mix = 0.05f;
			break;

			// Wall Sticking
			case PlatformerMotor2D.MotorState.WallSticking:
				if (playerController.WallJumpReady)
				{
					sBodyState = "WallStickReady";
				}
				else
				{
					sBodyState = "WallStick";
				}

				mix = 0;
				useOrientation = true;
			break;

			// Wall Sliding
			case PlatformerMotor2D.MotorState.WallSliding:
				if (playerController.WallJumpReady)
				{
					if (sBodyState == "WallSlide")
					{
						playPosition = GetAnimationTrack(0).TrackTime;
					}

					sBodyState = "WallSlideReady";
				}
				else
				{
					sBodyState = "WallSlide";
				}

				mix = 0;
				useOrientation = true;
			break;

			// On Corner
			case PlatformerMotor2D.MotorState.OnCorner:
				// Set Animation based on if arms are in use
				if (playerController.AimState || bArmBusy || playerController.WallJumpReady)
				{
					sBodyState = "CornerReady";

					if (bArmBusy && !bPrevArmBusy)
					{
						facingLeft = !facingLeft;
					}
				}
				else
				{
					sBodyState = "CornerGrab";	
				}

				skeleton.skeleton.SetBonesToSetupPose();
				mix = 0;

				useOrientation = true;
			break;

			// Climbing Corner
			case PlatformerMotor2D.MotorState.ClimbingCorner:
				sBodyState = "CornerClimb";

				mix = 0;
				useOrientation = true;
			break;
		}

		// If animation is affected by orientation, add orientation suffix
		if (useOrientation)
		{
			orientation = facingLeft ? "-Left" : "-Right";
			PlayAnimation(sBodyState + orientation, 0, looping, false, playPosition, mix);
		}
		else
		{
			PlayAnimation(sBodyState, 0, looping, false, playPosition, mix);
		}

		skeleton.state.GetCurrent(0).TimeScale = playSpeed;
		bPrevArmBusy = bArmBusy;
	}

	// Handles Arm Animation
	private void HandleArmAnimation()
	{
		// If no weapon animation type or weapon is firing then return
		if (sWeaponAnimationType == "")
		{
			return;
		}

		if (bWeaponFire && playerController.Motor.motorState != PlatformerMotor2D.MotorState.ClimbingCorner)
		{
			return;
		}

		if (playerController.CurrentWeapon.Reloading || playerController.CurrentWeapon.Bolting)
		{
			return;
		}

		// Animation Name
		string animationName = "";
		// Use Orientation
		bool useOrientation = false;
		// Facing Direction
		string orientation = "";
		// Looping flag
		bool loop = false;
		// Play Position
		float playPosition = 0;
		// Mix Duration
		float mix = -1;

		bArmAnimWeaponExclusive = true;
		bArmAnimHolstering = false;

		// Climbing Corner
		if (playerController.Motor.motorState == PlatformerMotor2D.MotorState.ClimbingCorner)
		{
			bArmBusy = false;
		}
		else if (playerController.bAimingDownSights) // Aiming Down Sights
		{
			bArmBusy = true;
		}

		if (playerController.bHolsteringWeapon) // Holstering
		{
			if (playerController.OnWall || playerController.OnCorner)
			{
				if (sArmState == "Holster")
				{
					playPosition = GetAnimationTrack(1).TrackTime;
				}

				sArmState = "HolsterOneHand";
			}
			else
			{
				sArmState = "Holster";
			}

			mix = 0.05f;
			
			useOrientation = true;
			bOrientatedAnimation = true;
			bArmAnimWeaponExclusive = false;
			bArmAnimHolstering = true;
		}
		else if (playerController.bUnholsteringWeapon) // Unholstering
		{
			if (playerController.OnWall || playerController.OnCorner)
			{
				if (sArmState == "Unholster")
				{
					playPosition = GetAnimationTrack(1).TrackTime;
				}

				sArmState = "UnholsterOneHand";
			}
			else
			{
				sArmState = "Unholster";
			}
			mix = 0.05f;

			useOrientation = true;
			bOrientatedAnimation = true;
			bArmAnimWeaponExclusive = false;
			bArmAnimHolstering = true;
		}
		else if (bArmBusy && playerController.Motor.motorState != PlatformerMotor2D.MotorState.Dashing &&
			playerController.Motor.motorState != PlatformerMotor2D.MotorState.ClimbingCorner) // Arm Busy
		{
			// On Wall Aiming
			if (playerController.OnWall || playerController.OnCorner)
			{
				if (playerController.bAimingDownSights)
				{
					sArmState = "SingleArmAim";
				}
				else
				{
					sArmState = "SingleArmReady";
				}

				mix = 0;
				bOrientatedAnimation = true;
				bArmAnimWeaponExclusive = false;
			}
			else // General Aiming
			{
				if (playerController.bAimingDownSights)
				{
					sArmState = "Aim";

					fArmReadyTimer = 0;
				}
				else
				{
					sArmState = "Ready";
				}

				mix = 0;
			}

			useOrientation = true;
			bOrientatedAnimation = false;
		}
		else // Idle
		{
			if (playerController.Motor.motorState == PlatformerMotor2D.MotorState.Dashing)
			{
				mix = 0;
			}

			if (playerController.OnWall || playerController.OnCorner)
			{
				sArmState = "SingleArmIdle";

				bOrientatedAnimation = true;
				useOrientation = true;
				bArmAnimWeaponExclusive = false;

				mix = 0;
			}
			else
			{
				sArmState = "Idle";
				useOrientation = true;
				bOrientatedAnimation = true;

				if (lsPrevAnimations != null)
				{
					// If previously unholstering then mix = 0
					if (lsPrevAnimations[1].Contains("Unholster"))
					{
						mix = 0.075f;
					}
					else
					{
						mix = 0.15f;
					}
				}
			}
		}

		// If current weapon is not one handed, use orientation
		if (playerController.CurrentWeapon.HoldType != eWeaponHoldType.Secondary)
		{
			useOrientation = true;
			bOrientatedAnimation = true;
		}

		// If using orientation, then add left or right to animation name
		if (useOrientation)
		{
			orientation = bFacingLeft ? "-Left" : "-Right";

			if (playerController.OnWall)
			{
				bOrientatedAnimation = false;
			}
		}

		// Set animation based on whether it is exclusive to the current weapon
		if (bArmAnimWeaponExclusive)
		{
			animationName = sWeaponAnimationType + sArmState + orientation;
		}
		else if (bArmAnimHolstering)
		{
			animationName = sWeaponHoldAnimationType + sArmState + orientation;
		}
		else
		{
			animationName = sArmState + orientation;
		}

		PlayAnimation(animationName, 1, loop, false, playPosition, mix);
	}

	// Triggers the start reload animation for the current weapon
	public void TriggerReloadStartAnimation()
	{
		sArmState = "ReloadStart";

		bOrientatedAnimation = true;
		bArmAnimWeaponExclusive = true;
		string orientation = bFacingLeft ? "-Left" : "-Right";

        PlayAnimation(sWeaponAnimationType + sArmState + orientation, 1, false, false, 0, 0.05f);
	}

	// Triggers the reload animation for the current weapon
	public void TriggerReloadAnimation(bool overrideCurrent = false)
	{
		sArmState = "Reload";

		bOrientatedAnimation = true;
		bArmAnimWeaponExclusive = true;
		string orientation = bFacingLeft ? "-Left" : "-Right";

		PlayAnimation(sWeaponAnimationType + sArmState + orientation, 1, false, overrideCurrent, 0, 0); 
	}

	// Triggers the reload bolt animation for the current weapon
	public void TriggerReloadBoltAnimation()
	{
		if (playerController.OnWall || playerController.OnCorner)
		{
			sArmState = "ReloadBoltOneHand";
		}
		else
		{
			if (playerController.CurrentWeapon.weaponData.firingMode == eWeaponFireMode.Single &&
			   playerController.CurrentWeapon.weaponData.bBoltAction &&
			   playerController.bAimingDownSights)
			{
				sArmState = "AimReloadBolt";
			}
			else
			{
				sArmState = "ReloadBolt";
			}
		}

		bOrientatedAnimation = true;
		bArmAnimWeaponExclusive = true;
		string orientation = bFacingLeft ? "-Left" : "-Right";

		PlayAnimation(sWeaponAnimationType + sArmState + orientation, 1, false, false, 0, 0); 
	}

	#endregion

	#region Aim Animation

	// Handles Arm Orientation based on aim
	private void SetAimOrientation(ISkeletonAnimation skeletonAnimator)
	{
		// If arms are not in use then return
		if (!bArmBusy)
		{
			return;
		}

		// If climbing corner then return
		if (playerController.Motor.motorState == PlatformerMotor2D.MotorState.ClimbingCorner)
		{
			return;
		}

		weaponAimIKBone.UpdateWorldTransform();

		// If not aiming, then set aiming for ready state
		if (!playerController.bAimingDownSights)
		{
			ResetAimIK();
			weaponAimIKBone.SetPosition(weaponPivotIKBone.GetSkeletonSpacePosition() + playerController.AimPoint);

		}
		else // Otherwise set aim for both head and weapon
		{
			weaponAimIKBone.SetPosition(weaponPivotIKBone.GetSkeletonSpacePosition() + playerController.AimPoint);
			headAimIKBone.SetPosition(headPivotIKBone.GetSkeletonSpacePosition() + playerController.HeadAimPoint);
		}
	}

	// Resets the position of the IK Transforms to aim forward
	private void ResetAimIK()
	{
		headAimIKBone.SetPosition(headPivotIKBone.GetSkeletonSpacePosition() + Vector2.right);
		weaponAimIKBone.SetPosition(weaponPivotIKBone.GetSkeletonSpacePosition() + Vector2.right);
	}

	// Sets ArmBusy
	public void SetArmBusy(bool busy)
	{
		bArmBusy = busy;
	}

	// Checks the Player State and Current Weapon and plays the appropriate weapon fire animation
	public void FireWeaponAnimation()
	{
		if (playerController.Motor.motorState == PlatformerMotor2D.MotorState.ClimbingCorner)
		{
			return;
		}

		// Animation Name
		string animationName = "";
		// Animation affected by orientation flag
		bool useOrientation = true;
		// Facing Direction
		string orientation = "";

		bArmAnimWeaponExclusive = true;
		bArmBusy = true;
		bWeaponFire = true;
		ResetArmTimer();

		// Wall/Corner Firing
		if (playerController.OnWall || playerController.OnCorner)
		{
			if (playerController.bAimingDownSights)
			{
				animationName = "SingleArmRecoil";
			}
			else
			{
				animationName = "SingleArmReadyRecoil";
			}

            if (playerController.CurrentWeapon.HoldType == eWeaponHoldType.Primary)
            {
                animationName += "Primary";
            }

			bArmAnimWeaponExclusive = false;
		}
		else // General Firing
		{
			if (playerController.bAimingDownSights)
			{
				animationName = "Recoil";
			}
			else
			{
				animationName = "ReadyRecoil";
			}
		}

		if (useOrientation || playerController.CurrentWeapon.HoldType != eWeaponHoldType.Secondary)
		{
			if (playerController.OnCorner && sArmState == "SingleArmIdle")
			{
				orientation = !bFacingLeft ? "-Left" : "-Right";
			}
			else
			{
				orientation = bFacingLeft ? "-Left" : "-Right";
			}
		}

		// If animation is ready to be set
		if (animationName != "")
		{
			if (bArmAnimWeaponExclusive)
			{
				PlayAnimation(sWeaponAnimationType + animationName + orientation, 1, false, true, 0, 0);
			}
			else
			{
				PlayAnimation(animationName + orientation, 1, false, true, 0, 0);
			}

			sArmState = animationName;
		}
	}

	// Resets the arm ready timer
	public void ResetArmTimer()
	{
		fArmReadyTimer = 0;
	}

	// Ends the weapon fire animation. 
	private void EndFireWeaponAnimation()
	{
		bWeaponFire = false;
	}

	#endregion

	#region Misc Animation

	// Checks player for facing direction
	public void FlipCharacter()
	{
		// Change direction 
		bFacingLeft = !bFacingLeft;

		// Flip skeleton and animation
		FlipSkeletonX(bFacingLeft, true);
		// Flip Arm Animation
		FlipArmAnimation();

		// Set facing animation and set clip hand
		if (bFacingLeft)
		{
			PlayAnimation("FaceLeft", 2);

			if (bClipInHand)
			{
				frontReloadClipRenderer.enabled = true;
				rearReloadClipRenderer.enabled = false;
			}
		}
		else
		{
			PlayAnimation("FaceRight", 2);

			if (bClipInHand)
			{
				frontReloadClipRenderer.enabled = false;
				rearReloadClipRenderer.enabled = true;
			}
		}
	}

	// Changes the set animation to its opposite direction animation (Left <-> Right)
	private void FlipArmAnimation()
	{
		// If the animation is not based on orientation, return
		if (!bOrientatedAnimation)
		{
			return;
		}

		// If reloading and on wall, set correct animation
		if (playerController.OnWall && sArmState.Contains("Reload"))
		{
			if (bArmBusy)
			{
				if (playerController.bAimingDownSights)
				{
					sArmState = "SingleArmAim";
				}
				else
				{
					sArmState = "SingleArmReady";
				}
			}
			else
			{
				sArmState = "SingleArmIdle";
			}

			bArmAnimWeaponExclusive = false;
		}	

		// Current arm animation
		TrackEntry armTrack = skeleton.state.GetCurrent(1);

		// If arm animation is null, return
		if (armTrack == null)
		{
			return;
		}

		// Animation Play Position
		float playTime = armTrack.TrackTime;
		// Orientation
		string orientation = bFacingLeft ? "-Left" : "-Right";

		if (bArmAnimWeaponExclusive)
		{
			PlayAnimation(sWeaponAnimationType + sArmState + orientation, 1, armTrack.Loop, false, playTime, 0);
		}
		else if (bArmAnimHolstering)
		{
			PlayAnimation(sWeaponHoldAnimationType + sArmState + orientation, 1, armTrack.Loop, false, playTime, 0, 0);
		}
		else
		{
			PlayAnimation(sArmState + orientation, 1, armTrack.Loop, false, playTime, 0);
		}
	}

	// Blink Coroutine
	private IEnumerator Blink()
	{
		while (true)
		{
			yield return new WaitForSeconds(Random.Range(1f, 3f));
			skeleton.skeleton.SetAttachment("Eyes", "Eye-Closed");
			yield return new WaitForSeconds(0.06f);
			skeleton.skeleton.SetAttachment("Eyes", "Eye-Open");
		}
	}

	// Activates the renderer for the reload clip
	private void AttachReloadClip()
	{
		Sprite clip = playerController.CurrentWeaponData.clipSprite;

		if (bFacingLeft)
		{
			frontReloadClipRenderer.enabled = true;
			frontReloadClipRenderer.sortingOrder = 17;
			rearReloadClipRenderer.enabled = false;
		}
		else
		{
			frontReloadClipRenderer.enabled = false;
			rearReloadClipRenderer.enabled = true;
			rearReloadClipRenderer.sortingOrder = 6;
		}

		frontReloadClipRenderer.sprite = clip;
		rearReloadClipRenderer.sprite = clip;

		bClipInHand = true;
	}

	// Changes the layer of the front clip renderer during a reload animation 
	private void ChangeClipLayer()
	{
		frontReloadClipRenderer.sortingOrder = 6;
	}

	// Deactivates the renderer for the reload clip
	private void DetachReloadClip()
	{
		frontReloadClipRenderer.enabled = false;
		rearReloadClipRenderer.enabled = false;

		bClipInHand = false;
	}

	// Sets the Arm Layer back to Idle when climbing a corner
	public void OnCornerClimb()
	{
		skeleton.state.SetEmptyAnimation(1, 0);
	}

    #endregion

    #region Effects

    // Start Dash Effects
    private void StartDashEffects()
    {
        dashThrusterAudio.Play();

        bodyThrusterTrail.emitting = true;

        if (!playerController.Sliding)
        {
            frontLegThrusterTrail.emitting = true;
            rearLegThrusterTrail.emitting = true;
        }
    }

    // End Dash Effects
    private void EndDashEffects()
    {
        bodyThrusterTrail.emitting = false;
        frontLegThrusterTrail.emitting = false;
        rearLegThrusterTrail.emitting = false;
    }

    // Start Wallslide Effects
    private void StartWallSlideEffects()
    {
        if (bWallslidingEffect)
        {
            return;
        }

        wallSlideAudio.Play();

        Vector3 dustRotation;

        if (bFacingLeft)
        {
            frontHandDustParticle.Play();

            dustRotation = new Vector3(-90, 0, 180);
        }
        else
        {
            rearHandDustParticle.Play();

            dustRotation = new Vector3(-90, 0, 0);
        }

        frontFootDustParticle.Play();
        rearFootDustParticle.Play();

        frontHandDustParticle.transform.rotation = Quaternion.Euler(dustRotation);
        rearHandDustParticle.transform.rotation = Quaternion.Euler(dustRotation);
        frontFootDustParticle.transform.rotation = Quaternion.Euler(dustRotation);
        rearFootDustParticle.transform.rotation = Quaternion.Euler(dustRotation);

        bWallslidingEffect = true;
    }

    // End Wallslide Effects
    private void EndWallSlideEffects()
    {
        if (!bWallslidingEffect)
        {
            return;
        }

        wallSlideAudio.Stop();

        frontHandDustParticle.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
        rearHandDustParticle.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
        frontFootDustParticle.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
        rearFootDustParticle.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);

        bWallslidingEffect = false;
    }

    #endregion

    #region Audio

    // Plays Run Footstep audio event
    private void PlayRunFootstepAudio()
	{
		runFootstepAudio.Play();
	}

	// Plays Walk Footstep audio event
	private void PlayWalkFootstepAudio()
	{
		walkFootstepAudio.Play();
	}

	// Plays Land Footstep audio event
	private void PlayLandAudio()
	{
		landFootstepAudio.Play();
	}

    // Plays Jump audio event
    private void PlayJumpAudio()
    {
        jumpAudio.Play();
    }

	// Plays Jump Thruster audio event
	private void PlayJumpThrusterAudio()
	{
		jumpThrusterAudio.Play();
	}

	// Plays Corner Grab audio event
	private void PlayCornerGrabAudio()
	{
		cornerGrabAudio.Play();
	}

    // Plays Wall Stick audio event
    private void PlayWallStickAudio()
    {
        wallStickAudio.Play();
    }

    // Plays Wall Jump audio event
    private void PlayWallJumpAudio(Vector2 normal)
    {
        wallJumpAudio.Play();
    }

	#endregion
}
