using System.Collections;

using UnityEngine;

public class MovingPlatformBehaviour : MonoBehaviour
{
    // Platform Motor
    MovingPlatformMotor2D motor;

    // Local Waypoints (Points relative to intial position)
    public PlatformWaypointNode[] aLocalWaypoints;
    // Global Waypointsa
    Vector2[] av2GlobalWaypoints;

    // Previous Waypoint Index
    int iFromWaypointIndex = 0;
    // Next Waypoint Index
    int iNextWaypointIndex;
    // Radius to check for the next node before recalculating velocity
    public float fNodeCheckRadius = 0.01f;

    // Path Loop Type
    public ePlatformPathLoopType loopType;
    // Initiate Velocity Coroutine
    Coroutine velocityCoroutine;
    // Is platform moving from waypoint flag
    bool bStartingAtWaypoint = true;
    // Active flag
    bool bActive = true;

    #region Public Properties

    // Active flag
    public bool Active
    {
        get { return bActive; }
    }

    #endregion

    // Initialization
    void Start()
    {
        motor = GetComponent<MovingPlatformMotor2D>();

        av2GlobalWaypoints = new Vector2[aLocalWaypoints.Length];

        if (aLocalWaypoints.Length > 1)
        {
            for (int i = 0; i < av2GlobalWaypoints.Length; i++)
            {
                av2GlobalWaypoints[i] = (Vector2)transform.position + aLocalWaypoints[i].v2Position;
            }

            iNextWaypointIndex = 1;
            transform.position = av2GlobalWaypoints[0];

            CalculateNewVelocity();
        }
        else
        {
            Debug.LogError("Cannot activate moving platform as there are not enough waypoints.");
            Deactivate();
        }
	}
	
	// Update
	void Update()
    {
        if (!bActive)
        {
            return;
        }

        if (Vector2.Distance(transform.position, av2GlobalWaypoints[iNextWaypointIndex]) <= fNodeCheckRadius)
        {
            iFromWaypointIndex++;
            iNextWaypointIndex = iFromWaypointIndex + 1;

            if (iFromWaypointIndex == av2GlobalWaypoints.Length - 1)
            {
                if (loopType == ePlatformPathLoopType.None)
                {
                    transform.position = av2GlobalWaypoints[iFromWaypointIndex];
                    Deactivate();
                    return;
                }
                else if (loopType == ePlatformPathLoopType.Loop)
                {
                    iNextWaypointIndex = 0;
                }
                else
                {
                    System.Array.Reverse(av2GlobalWaypoints);
                    iFromWaypointIndex = 0;
                    iNextWaypointIndex = 1;
                }
            }
            else if (iFromWaypointIndex == av2GlobalWaypoints.Length)
            {
                iFromWaypointIndex = 0;
                iNextWaypointIndex = 1;
            }

            motor.velocity = Vector2.zero;
            transform.position = av2GlobalWaypoints[iFromWaypointIndex];
            bStartingAtWaypoint = true;
            CalculateNewVelocity();
        }
	}

    // Editor Draw Gizmos
    private void OnDrawGizmos()
    {
        if (aLocalWaypoints == null)
        {
            return;
        }

        Gizmos.color = Color.red;
        float size = 0.3f;

        for (int i = 0; i < aLocalWaypoints.Length; i++)
        {
            Vector2 globalWaypointPos;
            Vector2 nextGlobalWaypointPos = Vector2.zero;

            if (Application.isPlaying)
            {
                globalWaypointPos = av2GlobalWaypoints[i];

                if (i == aLocalWaypoints.Length - 1)
                {
                    if (loopType == ePlatformPathLoopType.Loop)
                    {
                        nextGlobalWaypointPos = av2GlobalWaypoints[0];
                    }
                }
                else
                {
                    nextGlobalWaypointPos = av2GlobalWaypoints[i + 1];
                }
            }
            else
            {
                globalWaypointPos = (Vector2)transform.position + aLocalWaypoints[i].v2Position;
                nextGlobalWaypointPos = (Vector2)transform.position + aLocalWaypoints[(i + 1) % aLocalWaypoints.Length].v2Position;
            }

            if (loopType == ePlatformPathLoopType.ReverseLoop)
            {
                Gizmos.color = Color.yellow;
            }

            if (i < aLocalWaypoints.Length - 1 || loopType == ePlatformPathLoopType.Loop)
            {
                Gizmos.DrawLine(globalWaypointPos, nextGlobalWaypointPos);
            }

            Gizmos.DrawCube(globalWaypointPos, Vector2.one * size);
        }
    }

    // Activates the Platform and sets the velocity
    public void Activate()
    {
        if (av2GlobalWaypoints.Length <= 1)
        {
            Debug.LogError("Cannot activate moving platform as there are not enough waypoints.");
            return;
        }

        bActive = true;
        CalculateNewVelocity();
    }

    // Stops the platform and deactivates it
    public void Deactivate()
    {
        bActive = false;
        motor.velocity = Vector2.zero;

        if (velocityCoroutine != null)
        {
            StopCoroutine(velocityCoroutine);
            bStartingAtWaypoint = true;
        }
    }

    // Calculate new velocity based on waypoints
    private void CalculateNewVelocity()
    {
        Vector2 currentWaypoint = av2GlobalWaypoints[iFromWaypointIndex];
        Vector2 nextWaypoint = av2GlobalWaypoints[iNextWaypointIndex];
        Vector2 direction = (nextWaypoint - currentWaypoint).normalized;

        velocityCoroutine = StartCoroutine(InitiateVelocity(aLocalWaypoints[iFromWaypointIndex].fWaitTime, direction * aLocalWaypoints[iFromWaypointIndex].fSpeed));
    }

    // Start Moving Platform
    private IEnumerator InitiateVelocity(float waitTime, Vector2 velocity)
    {
        if (!bStartingAtWaypoint)
        {
            yield return null;
        }
        else
        {
            yield return new WaitForSeconds(waitTime);
        }

        motor.velocity = velocity;
    }
}

[System.Serializable]
public struct PlatformWaypointNode
{
    // Position
    public Vector2 v2Position;
    // Speed to get to the next node
    public float fSpeed;
    // Wait Time before moving to the next node
    public float fWaitTime;
}

public enum ePlatformPathLoopType
{
    None, 
    Loop,
    ReverseLoop
}
