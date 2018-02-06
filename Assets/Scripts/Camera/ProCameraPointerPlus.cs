using System.Collections;

using UnityEngine;

using Com.LuisPedroFonseca.ProCamera2D;
using Rewired;

public class ProCameraPointerPlus : BasePC2D, IPreMover
{
	// Player Input
	Player playerInput;
	// Player ID
	public int iPlayerID = 0;

	// Influence
	Vector2 v2Influence;
	// Prev Frame Influence for Gamepad
	Vector2 v2PrevGamepadInfluence = Vector2.zero;
	// Smoothing Reference Velocity
	Vector2 v2Velocity;

	// Maximum Horizontal Influence
	public float fMaxHorizontalInfluence = 3f;
	// Maximum Vertical Influence
	public float fMaxVerticalInfluence = 1f;
	// Influence Smoothness
	public float fInfluenceSmoothness = 0.2f;
	public float fMouseInfluenceMultiplier = 1f;
	public float fGamepadInfluenceMultiplier = 1f;

	// Max Camera Speed
	public float fMaxSpeed = 1;

	// Camera to Extension Transition Time
	public float fTransitionTime = 0.2f;
	// Transition Reference Velocity
	float fTransitionVelocity = 0f;
	// Influence Multiplier (Used as a smooth transition between camera and extension
	float fInfluenceMultiplier = 0f;

	// Gamepad Aiming flag
	[HideInInspector]
	public bool bGamepadAim = false;

	// Initialization
	protected override void Awake ()
	{
		base.Awake();

		ProCamera2D.AddPreMover(this);

		playerInput = ReInput.players.GetPlayer(iPlayerID);
	}

	// On Destroy
	protected override void OnDestroy()
	{
		base.OnDestroy();

		ProCamera2D.RemovePreMover(this);
	}

	// On Reset (Camera Call)
	public override void OnReset()
	{
		v2Influence = Vector2.zero;
		v2Velocity = Vector2.zero;
	}

	#region IPreMover implementation

	public void PreMove(float deltaTime)
	{
		if (enabled)
			ApplyInfluence();
	}

	public int PrMOrder { get { return _prmOrder; } set { _prmOrder = value; } }

	int _prmOrder = 3000;

	#endregion

	// Sets Camera Focus to center on Player
	public void SetFocusToPlayer()
	{
		StopAllCoroutines();
		StartCoroutine(TransitionBackToCamera());
	}

	// Sets Camera Focus to center on Aim
	public void SetFocusToAim()
	{
		StopAllCoroutines();
		StartCoroutine(TransitionFromCamera());
	}

	// Transitions the influence from the camera to the extension
	private IEnumerator TransitionFromCamera()
	{
		while (fInfluenceMultiplier < 1f)
		{
			fInfluenceMultiplier = Mathf.SmoothDamp(fInfluenceMultiplier, 1f, ref fTransitionVelocity, fTransitionTime);

			yield return null;
		}
	}

	// Transitions the influence back to the camera before fully disabling
	private IEnumerator TransitionBackToCamera()
	{
		while (fInfluenceMultiplier > 0f)
		{
			fInfluenceMultiplier = Mathf.SmoothDamp(fInfluenceMultiplier, 0f, ref fTransitionVelocity, fTransitionTime);
			
			yield return null;
		}
	}

	// Applies the influence to the camera based on control type
	private void ApplyInfluence()
	{
		if (bGamepadAim) 
		{
			GamepadInfluence();
		}
		else
		{
			MouseInfluence();
		}

		ProCamera2D.ApplyInfluence(v2Influence * fInfluenceMultiplier);
	}

	// Calculates camera influence based on mouse position
	private void MouseInfluence()
	{
		Vector3 mousePosViewport = ProCamera2D.GameCamera.ScreenToViewportPoint(Input.mousePosition) * fMouseInfluenceMultiplier;

		float mousePosViewportH = mousePosViewport.x.Remap(0, 1, -1, 1);
		float mousePosViewportV = mousePosViewport.y.Remap(0, 1, -1, 1);

		float hInfluence = mousePosViewportH * fMaxHorizontalInfluence;
		float vInfluence = mousePosViewportV * fMaxVerticalInfluence;

		v2Influence = Vector2.SmoothDamp(v2Influence, new Vector2(hInfluence, vInfluence), ref v2Velocity, fInfluenceSmoothness, fMaxSpeed, Time.deltaTime);
	}

	// Calculates camera influence based on aiming input axes
	private void GamepadInfluence()
	{
		Vector2 gamepadAimPoint = playerInput.GetAxis2D("Aim Horizontal", "Aim Vertical") * fGamepadInfluenceMultiplier;

		if (gamepadAimPoint == Vector2.zero)
		{
			if (!playerInput.GetButton("Aim Mode"))
			{
				v2PrevGamepadInfluence = Vector2.zero;
			}

			v2Influence = v2PrevGamepadInfluence;
			return;
		}

		float hInfluence = gamepadAimPoint.x * fMaxHorizontalInfluence;
		float vInfluence = gamepadAimPoint.y * fMaxVerticalInfluence;

		v2Influence = Vector2.SmoothDamp(v2Influence, new Vector2(hInfluence, vInfluence), ref v2Velocity, fInfluenceSmoothness, fMaxSpeed, Time.deltaTime);

		v2PrevGamepadInfluence = v2Influence;
	}
}
