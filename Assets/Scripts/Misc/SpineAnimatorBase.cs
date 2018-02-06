using System.Collections.Generic;

using UnityEngine;

using Spine;
using Spine.Unity;

public class SpineAnimatorBase : MonoBehaviour
{
	// Skeleton Animation
	public SkeletonAnimation skeleton;

	// Current Animation List
	protected List<string> lsCurrentAnimations;
	// Previous Animation List
	protected List<string> lsPrevAnimations;
	// Animation to Play at the Start
	[SpineAnimation]
	public string sStartingAnimation;
	// Loop Starting Animation flag
	public bool bLoopOnStart = true;

	// Early Initialization
	protected virtual void Awake()
	{
		if (skeleton == null)
		{
			skeleton = GetComponent<SkeletonAnimation>();
		}
	}

	// Initialization
	protected virtual void Start ()
	{
		lsCurrentAnimations = new List<string>();

		PlayAnimation(sStartingAnimation, 0, bLoopOnStart, false, 0, 0);
	}

	// Late Update
	protected virtual void LateUpdate()
	{
		lsPrevAnimations = lsCurrentAnimations;
	}

	// Get current animation for a track
	public string SCurrentAnimation(int track = 0)
	{
		if (track >= lsCurrentAnimations.Count)
		{
			return "";
		}

		return lsCurrentAnimations[track];
	}

	// Plays an animation to a track
	public void PlayAnimation(string animationName, int track = 0, bool loop = false,
		bool overrideCurrent = false, float playPosition = 0f, float mixDuration = -1, float eventThreshold = 1)
	{
		if (track >= lsCurrentAnimations.Count)
		{
			while (track >= lsCurrentAnimations.Count)
			{
				lsCurrentAnimations.Add("");
			}
		}

		if (!overrideCurrent)
		{
			if (animationName == lsCurrentAnimations[track])
			{
				return;
			}
		}

		TrackEntry entry = skeleton.state.SetAnimation(track, animationName, loop);

		if (playPosition > 0f)
		{
			entry.TrackTime = playPosition;
		}

		if (mixDuration >= 0f)
		{
			entry.MixDuration = mixDuration;
		}

        entry.EventThreshold = eventThreshold;

		lsCurrentAnimations[track] = animationName;
	}

	// Adds an animation to a track
	public void AddAnimation(string animationName, int track = 0, bool loop = false, float delay = 0f, 
		bool overrideCurrent = false, float playPosition = 0f, float mixDuration = 0f)
	{
		if (lsCurrentAnimations.Count >= track)
		{
			if (track - lsCurrentAnimations.Count > 1)
			{
				for (int i = 0; i < track - lsCurrentAnimations.Count; i++)
				{
					lsCurrentAnimations.Add("");
				}
			}
			else
			{
				lsCurrentAnimations.Add("");
			}
		}

		if (animationName == lsCurrentAnimations[track] && !overrideCurrent)
		{
			return;
		}

		TrackEntry entry = skeleton.state.AddAnimation(track, animationName, loop, delay);

		if (playPosition > 0f)
		{
			entry.TrackTime = playPosition;
		}

		if (mixDuration >= 0f)
		{
			entry.MixDuration = mixDuration;
		}
	}

	// Get Track
	public TrackEntry GetAnimationTrack(int track)
	{
		return skeleton.state.GetCurrent(track);
	}

	// Empty Track
	public void ClearTrack(int track)
	{
		if (lsCurrentAnimations.Count <= track)
		{
			return;
		}

		skeleton.state.ClearTrack(track);
		lsCurrentAnimations[track] = "";
	}

	// Flips the skeleton horizontally
	public void FlipSkeletonX(bool flipX, bool instant = false)
	{
		skeleton.skeleton.FlipX = flipX;

		if (instant)
		{
			skeleton.skeleton.UpdateWorldTransform();
		}
	}

	// Flips the skeleton vertically
	public void FlipSkeletonY(bool flipY, bool instant = false)
	{
		skeleton.skeleton.FlipY = flipY;

		if (instant)
		{
			skeleton.skeleton.UpdateWorldTransform();
		}
	}

	// TO-DO -------------------------- Think whether or not audio section is required for basic management of audio data
}
