using System;

using UnityEngine;

public class HealthManager : MonoBehaviour
{
	// Health
	float fHealth;
	// Max Health
	public float fMaxHealth;

	// Dead flag
	bool bDead = false;

    // On Health Changed
    public Action<float> onHealthChanged;
	// On Kill Action
	public Action onKill;
    // On Ray Collision Action
    public Action onRayCollision;

	#region Public Attributes

	// Health
	public float Health
	{
		get	{ return fHealth; }
		set
		{
			fHealth = value;

			fHealth = Mathf.Clamp(fHealth, 0, fMaxHealth);
		}
	}

	// Dead flag
	public bool Dead
	{
		get { return bDead; }
	}

	#endregion

	// Initialization
	void Start()
	{
		fHealth = fMaxHealth;
	}

	// Update
	void Update()
	{
		// If Health is less than 0, kill object
		if (fHealth <= 0)
		{
			Kill();
		}
	}

	// Gives Health by value
	public void GiveHealth(float health)
	{
		fHealth += Mathf.Abs(health);

		if (fHealth > fMaxHealth)
		{
			fHealth = fMaxHealth;
		}

        if (onHealthChanged != null)
        {
            onHealthChanged(fHealth);
        }
	}

	// Restores all health
	public void GiveAllHealth()
	{
		fHealth = fMaxHealth;

        if (onHealthChanged != null)
        {
            onHealthChanged(fHealth);
        }
    }

	// Takes health by value
	public void TakeHealth(float health)
	{
		fHealth -= Mathf.Abs(health);

		if (fHealth < 0)
		{
			fHealth = 0;
		}

        if (onHealthChanged != null)
        {
            onHealthChanged(fHealth);
        }
    }

	// Takes all health
	public void TakeAllHealth()
	{
		fHealth = 0;

        if (onHealthChanged != null)
        {
            onHealthChanged(fHealth);
        }
    }

	// Kills Object
	public void Kill()
	{
		if (onKill != null)
		{
			onKill();
		}

		bDead = true;
	}

	// Revives 
	public void Revive(float health = -1)
	{
		bDead = false;

		if (health == -1)
		{
			GiveAllHealth();
		}
		else
		{
			GiveHealth(health);
		}
	}
}
