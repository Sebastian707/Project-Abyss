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
    public Image healthBar; // Assign a UI Image (fill type) in inspector

    [Header("References")]
    public Transform player;
    private NavMeshAgent agent;

    [Header("Detection Settings")]
    public float detectionRange = 15f; // Distance at which enemy starts chasing
    public float loseSightRange = 20f; // Distance to stop chasing if player too far

    [Header("Roaming Settings")]
    public float roamRadius = 10f;      // How far from the start point enemy can roam
    public float roamWaitTime = 3f;     // Wait time between roaming destinations

    [Header("Movement Settings")]
    public float updateRate = 0.1f;     // How fast to update destination

    private Vector3 startPosition;
    private Coroutine roamRoutine;

    void Awake()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        startPosition = transform.position;
        SetState(EnemyState.Roaming);

        InvokeRepeating(nameof(CheckPlayerDistance), 0f, updateRate);
    }

    void SetState(EnemyState newState)
    {
        if (currentState == newState) return;

        // Clean up old state
        if (roamRoutine != null)
            StopCoroutine(roamRoutine);

        currentState = newState;

        switch (currentState)
        {
            case EnemyState.Roaming:
                roamRoutine = StartCoroutine(RoamRoutine());
                break;
            case EnemyState.Seeking:
                StartCoroutine(SeekPlayer());
                break;
        }
    }

    IEnumerator RoamRoutine()
    {
        while (currentState == EnemyState.Roaming)
        {
            Vector3 randomPoint = GetRandomPoint(startPosition, roamRadius);
            agent.SetDestination(randomPoint);

            // Wait until destination reached or timeout
            yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance < 0.5f);
            yield return new WaitForSeconds(roamWaitTime);
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
        if (player == null) return;

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

    Vector3 GetRandomPoint(Vector3 center, float range)
    {
        Vector3 randomPos = center + new Vector3(
            Random.Range(-range, range),
            0,
            Random.Range(-range, range)
        );

        NavMeshHit hit;
        NavMesh.SamplePosition(randomPos, out hit, range, NavMesh.AllAreas);
        return hit.position;
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
}
