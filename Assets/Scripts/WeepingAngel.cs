using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour, IDamageable
{
    public enum EnemyState { Roaming, Seeking }
    private EnemyState currentState;

    [Header("Health")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("UI")]
    public Image healthBar;

    [Header("References")]
    public Transform player;
    private NavMeshAgent agent;

    [Header("Detection Settings")]
    public float detectionRange = 15f;
    public float loseSightRange = 20f;
    [Tooltip("Optional delay (seconds) before the AI starts checking for the player after spawn.")]
    public float initialDetectionDelay = 0.2f;

    [Header("Roaming Settings")]
    public float roamRadius = 10f;
    public float roamWaitTime = 3f;
    [Tooltip("Max attempts to find a valid roam point")]
    public int roamPointAttempts = 6;

    [Header("Movement Settings")]
    public float updateRate = 0.1f;
    [Tooltip("Timeout (seconds) waiting to reach a roam destination before trying a new one")]
    public float roamReachTimeout = 8f;

    private Vector3 startPosition;
    private Coroutine roamRoutine;
    private Coroutine seekRoutine;

    void Awake()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("[EnemyAI] No NavMeshAgent found on " + gameObject.name);
            enabled = false;
            return;
        }

        if (player == null)
        {
            var pgo = GameObject.FindGameObjectWithTag("Player");
            if (pgo != null) player = pgo.transform;
        }

        startPosition = transform.position;
        SetState(EnemyState.Roaming);

        // small initial delay before checks, prevents immediate detection glitches
        InvokeRepeating(nameof(CheckPlayerDistance), initialDetectionDelay, updateRate);
    }

    void SetState(EnemyState newState)
    {
        if (currentState == newState) return;

        // stop any state coroutines
        if (roamRoutine != null)
        {
            StopCoroutine(roamRoutine);
            roamRoutine = null;
        }
        if (seekRoutine != null)
        {
            StopCoroutine(seekRoutine);
            seekRoutine = null;
        }

        currentState = newState;

        switch (currentState)
        {
            case EnemyState.Roaming:
                // Make sure agent is allowed to move
                if (agent != null) agent.isStopped = false;
                roamRoutine = StartCoroutine(RoamRoutine());
                break;
            case EnemyState.Seeking:
                if (agent != null) agent.isStopped = false;
                seekRoutine = StartCoroutine(SeekPlayer());
                break;
        }
    }

    IEnumerator RoamRoutine()
    {
        // small delay so agent can settle after spawn
        yield return null;

        while (currentState == EnemyState.Roaming)
        {
            Vector3 randomPoint;
            bool found = TryGetRandomNavmeshPoint(startPosition, roamRadius, out randomPoint, roamPointAttempts);

            if (!found)
            {
                // fallback to staying near startPosition so we don't spam invalid points
                randomPoint = startPosition;
            }

            // set destination and wait until reached or timeout
            agent.SetDestination(randomPoint);

            float startTime = Time.time;
            // Wait while path is pending OR agent hasn't reached within stopping distance
            while (currentState == EnemyState.Roaming && (agent.pathPending || agent.remainingDistance > agent.stoppingDistance))
            {
                // if path status is invalid, break
                if (agent.pathStatus == NavMeshPathStatus.PathInvalid)
                    break;

                if (Time.time - startTime > roamReachTimeout)
                {
                    // timed out reaching this point -> break to choose new one
                    Debug.Log("[EnemyAI] Roam destination timeout, picking new point.");
                    break;
                }

                yield return null;
            }

            // Wait a bit at that point before moving to the next roam point
            float timer = 0f;
            while (currentState == EnemyState.Roaming && timer < roamWaitTime)
            {
                timer += Time.deltaTime;
                yield return null;
            }
        }
    }

    IEnumerator SeekPlayer()
    {
        while (currentState == EnemyState.Seeking)
        {
            if (player != null)
                agent.SetDestination(player.position);

            yield return new WaitForSeconds(updateRate);
        }
    }

    void CheckPlayerDistance()
    {
        if (player == null || agent == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (currentState == EnemyState.Roaming && distance <= detectionRange)
        {
            SetState(EnemyState.Seeking);
        }
        else if (currentState == EnemyState.Seeking && distance > loseSightRange)
        {
            SetState(EnemyState.Roaming);
        }
    }

    bool TryGetRandomNavmeshPoint(Vector3 center, float range, out Vector3 result, int attempts = 4)
    {
        NavMeshHit hit;
        for (int i = 0; i < attempts; i++)
        {
            Vector3 randomPos = center + new Vector3(
                Random.Range(-range, range),
                0f,
                Random.Range(-range, range)
            );

            if (NavMesh.SamplePosition(randomPos, out hit, range, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }

        // fallback: try sample at center
        if (NavMesh.SamplePosition(center, out hit, 1f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }

    public void ApplyDamage(float amount)
    {
        currentHealth -= amount;
        UpdateHealthUI();

        if (currentHealth <= 0)
            Die();
    }

    void UpdateHealthUI()
    {
        if (healthBar != null)
            healthBar.fillAmount = Mathf.Clamp01(currentHealth / maxHealth);
    }

    void Die()
    {
        Destroy(gameObject);
    }

    // Editor gizmos to help tune ranges
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, roamRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, loseSightRange);
    }
}
