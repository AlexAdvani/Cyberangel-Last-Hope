using Com.LuisPedroFonseca.ProCamera2D;
using Rewired;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class PlayerController : MonoBehaviour
{
	// Platformer Motor
	PlatformerMotor2D motor;
	// Animator
	public PlayerAnimation animator;
    // Health Manager
    HealthManager healthManager;

	// Rigidbody 2D
	Rigidbody2D rigidbody2d;
	// Collider 2D
	BoxCollider2D boxCollider;
	// Environment Collision Filter
	ContactFilter2D collisionFilter;

    // Collision Shape
    ePlayerCollisionShape collisionShape = ePlayerCollisionShape.Standard;
    // Previous Frame Collision Frame
    ePlayerCollisionShape prevCollisionShape;
    // Motor Dash Easing Function
    PC2D.EasingFunctions.Functions dashEasing;
    // Motor Dash Time
    float fDashTime;
    // Motor Dash Distance
    float fDashDistance;
    // Dash Action
    public Action onDash;

    // Standing Bounding Box Rect
    public Rect rStandBounds;
    // Crouch Bounding Box Rect
    public Rect rCrouchBounds;
    // Sliding Bounding Box Rect
    public Rect rSlideBounds;

	// Player Input
	Player playerInput;

	// Scene Game Camera
	Camera gameCamera;
	// Pro Camera Window Extension
	ProCamera2DCameraWindow cameraWindow;
	// Pro Camera Forward Focus
	ProCamera2DForwardFocus cameraForward;
	// Pro Camera Pointer Extension
	ProCameraPointerPlus cameraPointer;

	// Facing Left flag
	bool bFacingLeft = false;
	// Facing Left on Previous Frame flag
	bool bPrevFacingLeft = false;

	// Player Control Disabled flag
	bool bPlayerControlDisabled = false;
	// Movement Disabled flag
	bool bMovementDisabled = false;

    // Is Crouching flag
    bool bCrouching = false;
    // Is Sliding flag
    bool bSliding = false;
	// Force Walking Speed flag
	bool bForceWalk = false;
	// Double Jumping flag
	bool bAirJumping = false;
	// On Wall flag
	bool bOnWall = false;
	// On Corner flag
	bool bOnCorner = false;
	// Wall Jumping flag
	bool bWallJumping = false;
	// Is last wall collision direction on the left flag
	bool bLastWallCollisionDirLeft = false;
	// Wall Jump Ready flag (Holding away from wall and jump is possible)
	bool bWallJumpReady = false;
	// Wall Jump Timing Coroutine
	Coroutine wallJumpCoroutine;

	[Header("Weapons")]
	// Weapon Data
	public List<WeaponData> lWeaponData;
	// List of Held Weapons
	List<WeaponBehaviour> lWeapons;
	// Max number of weapons that can be held at once
	public int iMaxWeaponCount = 2;
	// Current Weapon Index
	int iCurrentWeaponIndex = 0;
	// On Wall Weapon Recoil for non Secondary weapons
	public float fOnWallWeaponRecoil = 2f;
    // On Weapon Swap Action
    public Action<int, bool> aOnWeaponSwitch;

	// Front Aiming Hand Bone
	[SpineBone]
	public string sWeaponFrontHandBone;
	// Rear Aiming Hand Bone
	[SpineBone]
	public string sWeaponRearHandBone;
	// Weapon Equip Action
	public Action aOnWeaponEquip;

	// Primary Weapon Holster Bone Name
	[SpineBone]
	public string sPrimaryWeaponHolsterBone;
	// Secondary Weapon Holster Bone Name
	[SpineBone]
	public string sSecondaryWeaponHolsterBone;
	// Holstering Weapon flag
	[HideInInspector]
	public bool bHolsteringWeapon;
	// Unholstering Weapon flag
	[HideInInspector]
	public bool bUnholsteringWeapon;
	// Holding Weapon flag
	bool bHoldingWeapon = true;

	// Aim State flag
	bool bAimState = false;
	// Aiming On Previous Frame flag
	bool bPrevAimState = false;
	// Aiming Down Sights (Disable to stop animation aiming, keep AimState active to keep aim calculation for facing directions)
	[HideInInspector]
	public bool bAimingDownSights = false;
	// Prev Aiming Diff (Degrees)
	Vector2 v2PrevAimDiff = Vector2.zero;
	// Weapon Aim Point
	Vector2 v2AimPoint;
	// Head Aim Point
	Vector2 v2HeadAimPoint;

	// Locked Aim Direction flag
	bool bLockedDirection = false;
	// Reload Disabled flag
	bool bReloadDisabled = false;
	// Weapon Disabled flag
	bool bWeaponDisabled = false;

	#region Public Properties

	// Motor
	public PlatformerMotor2D Motor
	{
		get { return motor; }
	}

	// Facing Left flag
	public bool FacingLeft
	{
		get { return bFacingLeft; }
	}

	// Air Jumping flag
	public bool AirJumping
	{
		get { return bAirJumping; }
	}

    // Crouching flag
    public bool Crouching
    {
        get { return bCrouching; }
    }

    // Sliding flag
    public bool Sliding
    {
        get { return bSliding; }
    }

	// On Wall flag
	public bool OnWall
	{
		get { return bOnWall; }
	}

	// On Corner flag
	public bool OnCorner
	{
		get { return bOnCorner; }
	}

	// Wall Jumping flag
	public bool WallJumping
	{
		get { return bWallJumping; }
	}

	// Last Wall Collision Direction Left flag
	public bool LastWallCollisionDirLeft
	{
		get { return bLastWallCollisionDirLeft; }
	}

	// Wall Jump Ready flag
	public bool WallJumpReady
	{
		get { return bWallJumpReady; }
	}

    // Movement Disabled flag
    public bool MovementDisabled
    {
        get { return bMovementDisabled; }
    }

    // Player Control Disabled flag
    public bool PlayerControlDisabled
    {
        get { return bPlayerControlDisabled; }
    }

	// Weapon List
	public List<WeaponBehaviour> Weapons
    {
        get { return lWeapons; }
    }

    // Current Weapon Index
    public int CurrentWeaponIndex
    {
        get { return iCurrentWeaponIndex; }
    }

	// Current Weapon Data
	public WeaponData CurrentWeaponData
	{
		get
		{
			if (lWeaponData.Count == 0)
			{
				return null;
			}

			return lWeaponData[iCurrentWeaponIndex];
		}
	}

	// Current Weapon
	public WeaponBehaviour CurrentWeapon
	{
		get
		{
			if (lWeapons.Count == 0)
			{
				return null;
			}

			return lWeapons[iCurrentWeaponIndex];
		}
	}

	// Holding Weapon flag
	public bool HoldingWeapon
	{
		get { return bHoldingWeapon; }
	}

	// Aim State flag
	public bool AimState
	{
		get { return bAimState; }
	}
		
	public Vector2 AimPoint
	{
		get{ return v2AimPoint; }
	}

	public Vector2 HeadAimPoint
	{
		get{ return v2HeadAimPoint; }
	}

	#endregion

	// Initialization
	void Awake()
	{
		motor = GetComponent<PlatformerMotor2D>();
        motor.onAirJump += OnAirJump;
		motor.onWallJump += ActivateWallJump;
		motor.onCornerJump += ActivateWallJump;
		motor.onCornerClimb += animator.OnCornerClimb;
        motor.onDashEnd += OnDashEnd;

		rigidbody2d = GetComponent<Rigidbody2D>();
		boxCollider = GetComponent<BoxCollider2D>();
		collisionFilter = new ContactFilter2D();
		collisionFilter.SetLayerMask(motor.staticEnvLayerMask);

		playerInput = ReInput.players.GetPlayer(0);

		gameCamera = Camera.main;
		cameraWindow = gameCamera.GetComponent<ProCamera2DCameraWindow>();
		cameraForward = gameCamera.GetComponent<ProCamera2DForwardFocus>();
		cameraPointer = gameCamera.GetComponent<ProCameraPointerPlus>();

		lWeapons = new List<WeaponBehaviour>();
		
        dashEasing = motor.dashEasingFunction;
        fDashTime = motor.dashDuration;
        fDashDistance = motor.dashDistance;
        bFacingLeft = motor.facingLeft;

        healthManager = GetComponent<HealthManager>();

        InitializeWeapons();
	}

    // Update
    void Update()
	{
		// If game paused or player control is disabled, return
		if (GameManager.GamePaused)
		{
			return;
		}

		// Motor State and Movement Input
		HandleMotorState();
		HandleMovementInput();

		// If any weapons are equipped, update weapon state
		if (lWeapons.Count > 0)
		{
			HandleWeaponState();
			HandleWeaponInput();
			HandleWeaponAimInput();
		}

        // If collision shape has changed, change collider bounds
        if (collisionShape != prevCollisionShape)
        {
            switch (collisionShape)
            {
                case ePlayerCollisionShape.Standard: // Standard bounds
                    // Previous Bounds
                    Bounds prevBounds = boxCollider.bounds;
                    // Raycast for ceiling checking
                    RaycastHit2D hit = Physics2D.Raycast(transform.position,Vector2.up, rStandBounds.size.y, 
                        motor.staticEnvLayerMask | motor.movingPlatformLayerMask, 0);

                    // If clipping could occur, push away by distance
                    if (hit.collider != null)
                    {
                        StartCoroutine(TweenColliderSize(rStandBounds.size, rStandBounds.position, 0.25f));
                    }
                    else
                    {
                        boxCollider.size = rStandBounds.size;
                        boxCollider.offset = rStandBounds.position;
                    }
                break;

                case ePlayerCollisionShape.Crouch: // Crouch Bounds
                    boxCollider.size = rCrouchBounds.size;
                    boxCollider.offset = rCrouchBounds.position;
                break;

                case ePlayerCollisionShape.Slide: // Slide Bounds
                    boxCollider.size = rSlideBounds.size;
                    boxCollider.offset = rSlideBounds.position;
                break;
            }
        }

        // If facing direction has changed by end of update, change weapon hand
        if (bFacingLeft != bPrevFacingLeft)
		{
			animator.FlipCharacter();
			StartCoroutine(SwitchWeaponHand());
		}

        // TEMP - Display current aim angle difference to target
		if (Input.GetKeyDown(KeyCode.B))
		{
			print(animator.weaponPivotIKBone.GetWorldPosition(animator.transform).x);
		}

        if (Input.GetKeyDown(KeyCode.M))
        {
            GetComponent<HealthManager>().TakeHealth(10);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            GetComponent<HealthManager>().GiveHealth(10);
        }

        bPrevFacingLeft = bFacingLeft;
        prevCollisionShape = collisionShape;
	}

	#region Movement

	// Checks the Motor for any changes
	private void HandleMotorState()
	{
		// If not facing in the same direction as the motor, then change facing
		if (bFacingLeft != motor.facingLeft)
		{
			bFacingLeft = !bFacingLeft;
		}

		// If double jump is active but not in jumping state, then deactivate double jump
		if (bAirJumping)
		{
			if (motor.motorState != PlatformerMotor2D.MotorState.Jumping)
			{
				bAirJumping = false;
			}
		}

		// If walljumping is true but no longer jumping, then disable walljumping
		if (bWallJumping)
		{
			if (motor.motorState != PlatformerMotor2D.MotorState.Jumping)
			{
				bWallJumping = false;
			}
		}
	}

	// Checks for Movement Input and applies it to the motor
	private void HandleMovementInput()
	{
		if (bPlayerControlDisabled)
		{
			return;
		}

		// If movement is disabled, return
		if (bMovementDisabled)
		{
			motor.normalizedXMovement = 0;
			motor.normalizedYMovement = 0;

			return;
		}

		if (!bWallJumping)
		{
            // Horizontal Axis
            motor.normalizedXMovement = playerInput.GetAxis("Movement Horizontal");
            // Vertical Axis
            motor.normalizedYMovement = playerInput.GetAxis("Movement Vertical");
        }

        // Crouching
        if (playerInput.GetAxis("Movement Vertical") < -0.8f && Mathf.Abs(motor.normalizedXMovement) < 0.4f)
        {
            if (motor.motorState == PlatformerMotor2D.MotorState.OnGround)
            {
                motor.normalizedXMovement = 0;
                collisionShape = ePlayerCollisionShape.Crouch;
                bCrouching = true;
            }
            else
            {
                if (!bSliding)
                {
                    collisionShape = ePlayerCollisionShape.Standard;
                }

                bCrouching = false;
            }
        }
        else
        {
            if (!bSliding)
            {
                collisionShape = ePlayerCollisionShape.Standard;
            }

            bCrouching = false;
        }

		// If on wall or corner and conditions for walljumping is met then enable walljumpready
		if (bOnWall || bOnCorner)
		{
			if ((bLastWallCollisionDirLeft && motor.normalizedXMovement >= 0.1f) ||
				(!bLastWallCollisionDirLeft && motor.normalizedXMovement <= -0.1f))
			{
				bWallJumpReady = true;
			}
			else
			{
				bWallJumpReady = false;
			}
		}
		else
		{
			bWallJumpReady = false;
		}

		// Jumping
		if (playerInput.GetButtonDown("Jump"))
		{
            if (playerInput.GetAxis("Movement Vertical") < -0.25f)
            {
                if (motor.motorState == PlatformerMotor2D.MotorState.OnGround)
                {
                    bool ghostJump = GhostJump();

                    if (!ghostJump)
                    {
                        motor.Jump();
                    }
                }
                else
                {
                    motor.Jump();
                }
            }
            else
            {
                motor.Jump();
            }
		}

		// Variable Jump Height
		motor.jumpingHeld = playerInput.GetButton("Jump");

        // Dashing
        if (playerInput.GetButtonDown("Dash"))
        {
            if (motor.canDash)
            {
                if (motor.motorState != PlatformerMotor2D.MotorState.ClimbingCorner)
                {
                    // If On Ground, Jumping or Falling
                    if (motor.motorState == PlatformerMotor2D.MotorState.OnGround ||
                        motor.motorState == PlatformerMotor2D.MotorState.Jumping ||
                        motor.motorState == PlatformerMotor2D.MotorState.Falling)
                    {
                        // Check velocity and aim direction 
                        if (Mathf.Abs(motor.velocity.x) < 0.1f && motor.canDash)
                        {
                            // If aiming then set motor direction to aiming direction
                            if (bAimState)
                            {
                                motor.facingLeft = v2PrevAimDiff.x >= 0 ? true : false;
                            }
                        }
                    }

                    // If Slide Method is Down + Dash
                    if (GlobalSettings.iSlideInputMethod == 0)
                    {
                        if (playerInput.GetAxis("Movement Vertical") < -0.4f)
                        {
                            if (motor.motorState == PlatformerMotor2D.MotorState.OnGround)
                            {
                                collisionShape = ePlayerCollisionShape.Slide;
                                bSliding = true;
                            }
                        }
                    }

                    motor.Dash();

                    if (onDash != null)
                    {
                        onDash();
                    }
                }
            }
		}

        // If Slide Input Method is set to Own Input
        if (GlobalSettings.iSlideInputMethod == 1)
        {
            if (playerInput.GetButtonDown("Slide"))
            {
                if (motor.canDash && motor.motorState == PlatformerMotor2D.MotorState.OnGround)
                {
                    collisionShape = ePlayerCollisionShape.Slide;
                    bSliding = true;

                    motor.Dash();

                    if (onDash != null)
                    {
                        onDash();
                    }
                }
            }
        }

		// Walk
		if (!bOnWall && !bOnCorner)
		{
			if (playerInput.GetButton("Walk") || bForceWalk)
			{
				motor.normalizedXMovement = Mathf.Clamp(motor.normalizedXMovement, -0.25f, 0.25f);
			}
		}
	}

	// Sets Force Walk
	public void ForceWalking(bool force)
	{
		bForceWalk = force;
	}

	// Checks the platform underneath the player if it is a one-way platform and 
	// if it is then drop through it
	private bool GhostJump()
	{
		RaycastHit2D[] hits = new RaycastHit2D[2];
		rigidbody2d.Cast(Vector2.down, collisionFilter, hits);

		for (int i = 0; i < hits.Length; i++)
		{
			if (hits[i].collider != null)
			{
				if (hits[i].collider.GetComponent<PlatformEffector2D>() != null)
				{
					StartCoroutine(GhostJumpCoroutine(hits[i].collider));
                    return true;
				}
			}
		}

        return false;
	}

	// Sets the collision to be ignored briefly to fall through a oneway platform
	private IEnumerator GhostJumpCoroutine(Collider2D platformCollider)
	{
		boxCollider.enabled = false;
		Physics2D.IgnoreCollision(boxCollider, platformCollider, true);
		yield return new WaitForSeconds(0.05f);
		boxCollider.enabled = true;
		Physics2D.IgnoreCollision(boxCollider, platformCollider, false);
	}

	// Activates double jump (Motor Action)
	private void OnAirJump()
	{
		bAirJumping = true;
	}

	// Activates wall jump (Motor Action)
	private void ActivateWallJump(Vector2 velocity)
	{
		bWallJumping = true;

		if (wallJumpCoroutine != null)
		{
			StopCoroutine(wallJumpCoroutine);
		}

		wallJumpCoroutine = StartCoroutine(DeactivateWallJump());
	}

	// Deactivates wall jump after motor ignore movement time
	private IEnumerator DeactivateWallJump()
	{
		yield return new WaitForSeconds(motor.ignoreMovementAfterJump);

		if (bWallJumping)
		{
			bWallJumping = false;
		}
	}

	// Sets Player Control to be enabled or disabled
	public void SetPlayerControlDisabled(bool disabled)
	{
		bPlayerControlDisabled = disabled;
	}

	// Sets Player Movement to be enabled or disabled
	public void SetDisabledMovement(bool enabled)
	{
		bMovementDisabled = enabled;

		motor.normalizedXMovement = 0;
		motor.normalizedYMovement = 0;
	}

	// Set Player Facing
	public void SetFacing(bool facingLeft)
	{
		bool oldFacing = bFacingLeft;

		bFacingLeft = facingLeft;
		bPrevFacingLeft = facingLeft;
		motor.facingLeft = facingLeft;

		if (facingLeft != oldFacing)
		{
			animator.FlipCharacter();
			StartCoroutine(SwitchWeaponHand());
		}
	}

    // On Dash End
    private void OnDashEnd()
    {
        // Ray to check if we are still sliding under something
        RaycastHit2D ceilingRay = Physics2D.Raycast(new Vector2(boxCollider.bounds.center.x, boxCollider.bounds.min.y),
            Vector2.up, rStandBounds.height, motor.staticEnvLayerMask | motor.movingPlatformLayerMask);
        RaycastHit2D floorRay = Physics2D.Raycast(transform.position, Vector2.down, 0.05f, motor.staticEnvLayerMask | motor.movingPlatformLayerMask);

        // If ray hits environment, slide until we are out of the tunnel
        if (ceilingRay.collider != null && floorRay.collider != null)
        {
            motor.dashDuration = 0.05f;
            motor.dashDistance = 1;
            motor.dashEasingFunction = PC2D.EasingFunctions.Functions.Linear;
            motor.ResetDashCooldown();
            motor.Dash();
            motor.ReupdateVelocity();

            bSliding = true;

            return;
        }

        // Reset dash variables
        motor.dashEasingFunction = dashEasing;
        motor.dashDuration = fDashTime;
        motor.dashDistance = fDashDistance;
        
        // Change collision shape to match state
        if (bCrouching)
        {
            collisionShape = ePlayerCollisionShape.Crouch;
        }
        else
        { 
            collisionShape = ePlayerCollisionShape.Standard;
        }
        
        bSliding = false;
    }

	#endregion

	#region Weapons

	#region Input

	// Handles Weapon State Based on Current Motor State
	private void HandleWeaponState()
	{
		// Facing Direction and Reload Usage on Walls and Corners
		if (motor.motorState == PlatformerMotor2D.MotorState.WallSticking ||
			motor.motorState == PlatformerMotor2D.MotorState.WallSliding)
		{
			if (motor.collidingAgainst == PlatformerMotor2D.CollidedSurface.LeftWall)
			{
				bLastWallCollisionDirLeft = true;
			}
			else if (motor.collidingAgainst == PlatformerMotor2D.CollidedSurface.RightWall)
			{
				bLastWallCollisionDirLeft = false;
			}

			motor.facingLeft = !bLastWallCollisionDirLeft;

			bOnWall = true;
			bOnCorner = false;
			bLockedDirection = true;
			bReloadDisabled = true;

			if (CurrentWeapon.Reloading)
			{
				CurrentWeapon.CancelReload();
			}
		}
		else if (motor.motorState == PlatformerMotor2D.MotorState.OnCorner ||
			motor.motorState == PlatformerMotor2D.MotorState.ClimbingCorner)
		{
			if (motor.collidingAgainst == PlatformerMotor2D.CollidedSurface.LeftWall)
			{
				bLastWallCollisionDirLeft = true;
			}
			else if (motor.collidingAgainst == PlatformerMotor2D.CollidedSurface.RightWall)
			{
				bLastWallCollisionDirLeft = false;
			}

			motor.facingLeft = bLastWallCollisionDirLeft;

			if (motor.motorState == PlatformerMotor2D.MotorState.OnCorner)
			{
				if (bAimState || animator.ArmBusy || bWallJumpReady)
				{
					motor.facingLeft = !motor.facingLeft;
				}
			}
			else
			{
				if (CurrentWeapon.Bolting)
				{
					CurrentWeapon.CancelBolt();
				}
			}
	
			bFacingLeft = motor.facingLeft;

			bOnWall = false;
			bOnCorner = true;
			bLockedDirection = true;
			bReloadDisabled = true;

			if (CurrentWeapon.Reloading)
			{
				CurrentWeapon.CancelReload();
			}
		}
		else
		{
			bOnWall = false;
			bOnCorner = false;
			bLockedDirection = false;
			bReloadDisabled = false;
		}

		// Weapon Usage while Dashing or Climbing Corners
		if (motor.motorState == PlatformerMotor2D.MotorState.Dashing)
		{
			bWeaponDisabled = true;
			bAimState = false;

			if (motor.velocity.x > 0.1f)
			{
				bFacingLeft = false;
			}
			else if (motor.velocity.x < -0.1f)
			{
				bFacingLeft = true;
			}

			bHolsteringWeapon = false;
			bUnholsteringWeapon = false;
			bReloadDisabled = true;

			if (CurrentWeapon.Reloading)
			{
				CurrentWeapon.CancelReload();
			}

			if (CurrentWeapon.Bolting)
			{
				CurrentWeapon.CancelBolt();
			}

			motor.facingLeft = bFacingLeft;
		}
		else if (motor.motorState == PlatformerMotor2D.MotorState.ClimbingCorner)
		{
			bAimState = false;
			bWeaponDisabled = true;
			bHolsteringWeapon = false;
			bUnholsteringWeapon = false;
		}
		else
		{
			// If not switching weapon and not holding weapon
			if (!bHolsteringWeapon && !bUnholsteringWeapon)
			{
				if (bHoldingWeapon)
				{
					CurrentWeapon.bActive = true;
				}
				else
				{
					bUnholsteringWeapon = true;
				}
			}

			bWeaponDisabled = false;
		}

		// If not holding weapon, disable weapon
		if (!bHoldingWeapon)
		{
			bWeaponDisabled = true;
		}

		// If on wall or corner, set increased recoil multiplier of non secondary weapons
		if (CurrentWeapon.HoldType != eWeaponHoldType.Secondary)
		{
			if (bOnWall || bOnCorner)
			{
				CurrentWeapon.SetRecoilMultiplier(fOnWallWeaponRecoil);
			}
			else
			{
				CurrentWeapon.SetRecoilMultiplier(1);
			}
		}
		else
		{
			CurrentWeapon.SetRecoilMultiplier(1);
		}
			
		if (!bAimState)
		{
			bAimingDownSights = false;
		}

		CurrentWeapon.SetReloadDisabled(bReloadDisabled);
	}

	// Handles Weapon Based Input
	private void HandleWeaponInput()
	{
		// If weapon disabled, return
		if (bWeaponDisabled || bPlayerControlDisabled)
		{
			return;
		}

		// Switch Weapons
		if (playerInput.GetButtonDown("Switch Weapon") || playerInput.GetNegativeButtonDown("Switch Weapon"))
		{
			StartCoroutine(StartSwitchWeapon());
		}

		// Aim Mode
		if (playerInput.GetButton("Aim Mode"))
		{
			if (motor.motorState != PlatformerMotor2D.MotorState.ClimbingCorner)
			{
				if (lWeapons.Count > 0)
				{
					bAimState = true;
				}
				else
				{
					bAimState = false;
				}
			}
			else
			{
				bAimState = false;
			}
		}
		else
		{
			bAimState = false;
		}

		// Camera Extensions - If aiming, move camera to focus on it
		if (cameraPointer != null && cameraWindow != null)
		{
			if (bAimState != bPrevAimState)
			{
				if (bAimState)
				{
					cameraPointer.SetFocusToAim();
				}
				else
				{
					cameraPointer.SetFocusToPlayer();
				}

				cameraForward.enabled = !bAimState;
				cameraWindow.enabled = !bAimState;
			}
		}
	}

	// Checks aim input based on last control type
	private void HandleWeaponAimInput()
	{
		// If Weapon Disabled then stop aiming and return
		if (bWeaponDisabled)
		{
			CurrentWeapon.SetWeaponDisabled(true);
		}
		else
		{
			CurrentWeapon.SetWeaponDisabled(false);
		}

		if (motor.motorState == PlatformerMotor2D.MotorState.Dashing ||
			motor.motorState == PlatformerMotor2D.MotorState.ClimbingCorner)
		{
			return;
		}

		// If aiming
		if (bAimState)
		{
			// Last used control type
			Controller lastController = playerInput.controllers.GetLastActiveController();

			// Aim Position Difference (From Aim Pivot)
			Vector2 diff = Vector2.zero;
			// Aim IK Offset Distance (Used to calculate aim difference from aim pivot to weapon fire point)
			Vector2 ikOffset = Vector2.zero;

			// If using Keyboard and Mouse then aim with mouse position
			if (lastController.type == ControllerType.Keyboard ||
			    lastController.type == ControllerType.Mouse)
			{
				diff = animator.weaponPivotIKBone.GetWorldPosition(animator.transform) - gameCamera.ScreenToWorldPoint(Input.mousePosition);
				ikOffset = new Vector2(diff.y, -diff.x).normalized * CurrentWeapon.IKOffset;

				if (diff.x >= 0)
				{
					ikOffset *= -1;
				}

				diff += ikOffset;

				cameraPointer.bGamepadAim = false;
			}
			else if (lastController.type == ControllerType.Joystick) // Otherwise, use aim input axes from gamepad
			{
				diff = -playerInput.GetAxis2D("Aim Horizontal", "Aim Vertical").normalized;

				// If aim input is zero
				if (diff == Vector2.zero)
				{
					// If previously was aiming then maintain aim
					if (v2PrevAimDiff != Vector2.zero)
					{
						diff = v2PrevAimDiff;	
					}
					else // Otherwise, aim forward based on facing direction
					{
						diff = bFacingLeft ? Vector2.right : Vector2.left;

						// If on corner and arm is not busy, reverse aim to flip character to face corner again
						if (bOnCorner && !animator.ArmBusy)
						{
							diff.x *= -1;
						}
					}
				}

				cameraPointer.bGamepadAim = true;
			}

			v2PrevAimDiff = diff;

			// If direction is not locked then set facing based on direction
			if (!bLockedDirection)
			{
				if (diff.x >= 0)
				{
					bFacingLeft = true;
					diff.x *= -1;

					if (!cameraPointer.bGamepadAim)
					{
						diff.x += 0.035f;
					}
				}
				else
				{
					bFacingLeft = false;
				}
			}
			else
			{
				if (bFacingLeft)
				{
					diff.x *= -1;

					if (!cameraPointer.bGamepadAim)
					{
						diff.x += 0.035f;
					}
				}
			}
				
			CalculateWeaponAim(diff);
			CalculateHeadAim(diff);

			bAimingDownSights = true;
		}
		else // Otherwise
		{
			// If was previously aiming
			if (bPrevAimState)
			{
				// If direction is not locked then set motor facing based on previous aim direction
				if (!bLockedDirection)
				{
					if (v2PrevAimDiff.x >= 0)
					{
						bFacingLeft = true;
					}
					else
					{
						bFacingLeft = false;
					}
				}

				motor.facingLeft = bFacingLeft;
			}
			else // Otherwise
			{
				// If not reloading then reset aiming
				if (!CurrentWeapon.Reloading && !bHolsteringWeapon && !bUnholsteringWeapon)
				{
					v2PrevAimDiff = Vector2.zero;
				}

				// Ready stance aiming
				CalculateWeaponAim(-Vector2.right);
			}
		}

		bPrevAimState = bAimState;

		// If switching weapons, return
		if (bHolsteringWeapon || bUnholsteringWeapon || !bHoldingWeapon)
		{
			animator.SetArmBusy(false);
			bAimingDownSights = false;

			if (playerInput.GetButton("Aim Mode"))
			{
				bAimState = true;
			}
		}
	}

	// Calculates weapon aim point
	private void CalculateWeaponAim(Vector2 diff)
	{
		Vector2 weaponDiff = diff;
		float angle = Mathf.Atan2(weaponDiff.y, weaponDiff.x) * Mathf.Rad2Deg;
		bool angleChanged = false;

		// Restrict angle regardless of facing direction
		if (!bLockedDirection)
		{
			if (angle > 60 && angle < 90)
			{
				angle = 60;
				angleChanged = true;
			}
			else if (angle >= 90 && angle < 120)
			{
				angle = 120;
				angleChanged = true;
			}
		}
		else // Otherwise, restrict angle based on facing direction
		{
			if (angle > 0 && angle < 90)
			{
				angle = 90;
				angleChanged = true;
			}
			else if (angle > -90 && angle < 0)
			{
				angle = -90;
				angleChanged = true;
			}
			else if (angle == 0)
			{
				angle = 180;
				angleChanged = true;
			}
		}

		if (angleChanged)
		{
			angle *= Mathf.Deg2Rad;
			weaponDiff = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
		}

		// Apply weapon recoil
		weaponDiff += new Vector2 (weaponDiff.normalized.y, -weaponDiff.normalized.x) * CurrentWeapon.AimRecoil * weaponDiff.magnitude;
		v2AimPoint = -weaponDiff;
	}

	// Calculates head aim point
	private void CalculateHeadAim(Vector2 diff)
	{
		Vector2 headDiff = diff;
		float angle = (float)Math.Atan2(headDiff.y, headDiff.x) * Mathf.Rad2Deg;
		bool angleChanged = false;

		if (bLockedDirection && angle == 0)
		{
			angle = 180;
			angleChanged = true;
		}

		if ((motor.motorState == PlatformerMotor2D.MotorState.OnGround && 
			(motor.velocity.x > animator.fWalkSpeed && !bFacingLeft) ||
			(motor.velocity.x < -animator.fWalkSpeed && bFacingLeft)) ||
			bWallJumping || bCrouching)
		{
			if (bFacingLeft)
			{
				if (angle < 0 && angle > -165)
				{
					angle = -165;
					angleChanged = true;
				}
				else if (angle > 0 && angle < 165)
				{
					angle = 165;
					angleChanged = true;
				}
			}
			else
			{
				if (angle < 0 && angle > -165)
				{
					angle = -165;
					angleChanged = true;
				}
				else if (angle > 0 && angle < 155)
				{
					angle = 155;
					angleChanged = true;
				}
			}
		}
		else
		{
			if (angle < 0 && angle > -155)
			{
				angle = -155;
				angleChanged = true;
			}
			else if (angle > 0 && angle < 160)
			{
				angle = 160;
				angleChanged = true;
			}
		}

		if (angleChanged)
		{
			angle *= Mathf.Deg2Rad;
			headDiff = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * 2.5f;
		}

		v2HeadAimPoint = -headDiff;
	}

	// Sets aiming weapon
	public void SetAiming(bool aiming)
	{
		bAimState = aiming;
		bPrevAimState = aiming;
		bAimingDownSights = aiming;
		v2AimPoint = Vector2.zero;

		if (!aiming)
		{
			v2PrevAimDiff = Vector2.zero;

			if (cameraPointer != null && cameraWindow != null)
			{
				cameraPointer.SetFocusToPlayer();
				cameraForward.enabled = true;
				cameraWindow.enabled = true;
			}
		}
	}

	// Switches to the next weapon in the weapon list
	private IEnumerator StartSwitchWeapon()
	{
		if (bHolsteringWeapon || bUnholsteringWeapon)
		{
			yield return null;
		}

		// If there are less than 2 weapons in the list, return
		if (lWeapons.Count < 2)
		{
			yield return null;
		}

		// Currently Held Weapon
		WeaponBehaviour weapon = CurrentWeapon;

        if (weapon.Bolting)
        {
            yield return new WaitForSpineAnimationComplete(animator.GetAnimationTrack(1));
        }

		weapon.CancelReload();
		weapon.StartCooldownTimers();
        weapon.StopMuzzleFlash();
		weapon.bActive = false;

		bHolsteringWeapon = true;
	}

	// Holsters Weapon
	public void HolsterWeapon(int index = -1, bool instantSwitch = false)
	{
        if (!bHoldingWeapon)
        {
            return;
        }

		BoneFollower follower = CurrentWeapon.GetComponent<BoneFollower>();
		WeaponBehaviour weapon = CurrentWeapon;
		string holsterBone = weapon.HoldType == eWeaponHoldType.Secondary 
			? sSecondaryWeaponHolsterBone : sPrimaryWeaponHolsterBone;
		weapon.bHolstered = true;
		follower.boneName = holsterBone;
		follower.HandleRebuildRenderer(follower.SkeletonRenderer);
        weapon.HandleVisualOffset();

		if (index == -1)
		{
			// Move to the next weapon index
			if (iCurrentWeaponIndex == lWeapons.Count - 1)
			{
				iCurrentWeaponIndex = 0;
			}
			else
			{
				iCurrentWeaponIndex++;
			}
		}
		else
		{
			index = Mathf.Clamp(index, 0, lWeapons.Count - 1);
			iCurrentWeaponIndex = index;
		}

        if (aOnWeaponSwitch != null)
        {
            aOnWeaponSwitch(iCurrentWeaponIndex, instantSwitch);
        }

		// Setup new current weapon
		weapon = CurrentWeapon;
		weapon.EndCooldownTimers();

		bHoldingWeapon = false;
	}

	// Unholsters Weapon
	public void UnholsterWeapon()
	{
		BoneFollower follower = CurrentWeapon.GetComponent<BoneFollower>();
		WeaponBehaviour weapon = CurrentWeapon;
		string handBone = bFacingLeft ? sWeaponRearHandBone: sWeaponFrontHandBone;
		weapon.bHolstered = false;
		follower.boneName = handBone;
		follower.HandleRebuildRenderer(follower.SkeletonRenderer);

		bHoldingWeapon = true;
	}

	// Switches the weapon hand transform for the currently held weapon
	public IEnumerator SwitchWeaponHand()
	{
		// If no weapons are in the weapon list, return
		if (lWeapons.Count <= 0)
		{
			yield break;
		}

		if (!Application.isEditor)
		{
			yield return new WaitForEndOfFrame();
		}

		// Bone Follower
		BoneFollower follower = CurrentWeapon.GetComponent<BoneFollower>();
		// Weapon Hand Transform
		string handBone = bFacingLeft ? sWeaponRearHandBone : sWeaponFrontHandBone;

		follower.SetBone(handBone);
	}

	#endregion

	#region Management

	// Adds the WeaponData to Held Weapons
	private void InitializeWeapons()
	{
        lWeaponData = WeaponManager.Instance.PlayerLoadout;

		for (int i = 0; i < lWeaponData.Count; i++)
		{
			AddWeapon(lWeaponData[i]);
		}
	}

    // Equips a currently held weapon
    public void EquipWeapon(int index, bool instantSwitch = false)
    {
        StartCoroutine(StartSwitchWeapon());

        if (instantSwitch)
        {
            HolsterWeapon(index, true);
            UnholsterWeapon();
            animator.GetCurrentWeaponAnimationType();
            CurrentWeapon.HandleVisualOffset(true);

            bHolsteringWeapon = false;
            bUnholsteringWeapon = false;
            bHoldingWeapon = true;
        }
    }

	// Adds a new weapon to the weapon list via Weapon ID
	public void AddWeapon(WeaponData weaponData)
	{
		// If no room for new weapon, return
		if (lWeapons.Count >= iMaxWeaponCount)
		{
			return;
		}

		// Weapon Game Object
		GameObject weaponObject = weaponData.GOInstantiateWeapon(transform);

		// Weapon Behaviour to be added to the list
		WeaponBehaviour weaponBehaviour = weaponObject.GetComponent<WeaponBehaviour>();
		weaponBehaviour.SetPlayerController(this);
		lWeapons.Add(weaponBehaviour);

		// Weapon Parent Bone
		string parentBone;

        if (iCurrentWeaponIndex != lWeapons.Count - 1)
        {
            // Holster Transform
            parentBone = weaponBehaviour.HoldType == eWeaponHoldType.Secondary ?
                sSecondaryWeaponHolsterBone : sPrimaryWeaponHolsterBone;
            weaponBehaviour.bHolstered = true;
        }
        else
        {
            // Aiming Hand Transform
            parentBone = bFacingLeft ? sWeaponRearHandBone : sWeaponFrontHandBone;
            weaponBehaviour.bHolstered = false;

            if (aOnWeaponEquip != null) // Otherwise trigger Equip Action
            {
                aOnWeaponEquip();
            }
        }

        // Set Parent, Position and Rotation
        BoneFollower follower = weaponObject.GetComponent<BoneFollower>();
		follower.skeletonRenderer = animator.skeleton;
		follower.boneName = parentBone;
	}

	// Removes a weapon from the weapon list via Weapon ID
	public void RemoveWeapon(int weaponIndex)
	{
		// If no weapons in the weapon list, return
		if (lWeapons.Count == 0)
		{
			return;
		}

		// If weapon index out of range, return
		if (weaponIndex < 0 || weaponIndex >= lWeapons.Count)
		{
			return;
		}

		// Remove Weapon
		lWeapons[weaponIndex].RemoveWeapon();
		Destroy(lWeapons[weaponIndex].gameObject);
		lWeapons.RemoveAt(weaponIndex);

		if (aOnWeaponEquip != null) // Otherwise trigger Equip Action
		{
			aOnWeaponEquip();
		}
	}

	// Removes all weapons from the weapon list
	public void RemoveAllWeapons()
	{
		// If no weapons in the list, return
		if (lWeapons.Count == 0)
		{
			return;
		}

		// Loop through each weapon in the weapon list and remove them
		for (int i = lWeapons.Count - 1; i >= 0; i--)
		{
			lWeapons[i].RemoveWeapon();
			Destroy(lWeapons[i].gameObject);
			lWeapons.Remove(lWeapons[i]);
		}
	}

    // Swaps a weapon with a new weapon
    public void SwapWeapon(int weaponID, WeaponData newWeaponData)
    {
        if (weaponID < 0 || weaponID >= lWeapons.Count)
        {
            return;
        }

        lWeapons[weaponID].RemoveWeapon();
        Destroy(lWeapons[weaponID].gameObject);

        // Weapon Game Object
        GameObject weaponObject = newWeaponData.GOInstantiateWeapon(transform);

        // Weapon Behaviour to be added to the list
        WeaponBehaviour weaponBehaviour = weaponObject.GetComponent<WeaponBehaviour>();
        weaponBehaviour.SetPlayerController(this);
        lWeapons[weaponID] = weaponBehaviour;

        // Weapon Parent Bone
        string parentBone;

        if (iCurrentWeaponIndex != weaponID)
        {
            // Holster Transform
            parentBone = weaponBehaviour.HoldType == eWeaponHoldType.Secondary ?
                sSecondaryWeaponHolsterBone : sPrimaryWeaponHolsterBone;
            weaponBehaviour.bHolstered = true;
        }
        else
        {
            // Aiming Hand Transform
            parentBone = bFacingLeft ? sWeaponRearHandBone : sWeaponFrontHandBone;
            weaponBehaviour.bHolstered = false;

            if (aOnWeaponEquip != null) // Otherwise trigger Equip Action
            {
                aOnWeaponEquip();
            }
        }

        // Set Parent, Position and Rotation
        BoneFollower follower = weaponObject.GetComponent<BoneFollower>();
        follower.skeletonRenderer = animator.skeleton;
        follower.boneName = parentBone;
    }

    #endregion

    #endregion

    // Tween Collider Size over time
    private IEnumerator TweenColliderSize(Vector3 newSize, Vector3 newOffset, float time)
    {
        float timer = 0f;
        float timePercentage;
        Vector3 startSize = boxCollider.size;
        Vector2 prevSize = startSize;
        Vector2 startOffset = boxCollider.offset;

        while (timer < time)
        {
            timer += Time.deltaTime;
            timePercentage = timer / time;
            
            boxCollider.size = Vector3.Lerp(startSize, newSize, timePercentage);
            boxCollider.offset = Vector3.Lerp(startOffset, newOffset, timePercentage);

            transform.Translate(-(boxCollider.size - prevSize));

            prevSize = boxCollider.size;

            yield return null;
        }
    }

	// Resets the player to a base state (For mission restarts and checkpoints)
	public void ResetPlayer(Vector3 resetPos, bool facing = false)
	{
		// Position
		transform.position = resetPos;

		// Movement
		motor.normalizedXMovement = 0;
		motor.normalizedYMovement = 0;
		SetDisabledMovement(false);

		// Facing
		SetFacing(facing);

		// Aiming
		SetAiming(false);
		animator.SetArmBusy(false);

        // Health
        healthManager.GiveAllHealth();

		// Weapons
		EquipWeapon(0, true);

        for (int i = 0; i < lWeapons.Count; i++)
        {
            lWeapons[i].RefillAllAmmo();
            lWeapons[i].laserSight.SetLaserSightActive(false);
        }
	}
}

// Player Collider Sgape 
public enum ePlayerCollisionShape
{
    Standard,
    Crouch,
    Slide
}