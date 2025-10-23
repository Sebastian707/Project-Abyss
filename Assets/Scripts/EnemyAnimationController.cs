using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAnimationController : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;

    [Header("Animation Settings")]
    [Tooltip("Animator parameter name for walking state (bool).")]
    public string isWalkingParam = "IsWalking";

    [Tooltip("Movement speed threshold before walking animation triggers.")]
    public float walkThreshold = 0.05f;

    [Header("Rotation Settings")]
    [Tooltip("Degrees per second the enemy can rotate.")]
    public float rotationSpeed = 360f;

    void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        if (agent != null)
            agent.updateRotation = false; // We handle rotation ourselves
    }

    void Update()
    {
        if (agent == null || animator == null)
            return;

        HandleMovementAnimation();
        HandleRotation();
    }

    /// <summary>
    /// Handles walk/idle animation based on agent velocity.
    /// </summary>
    void HandleMovementAnimation()
    {
        float speed = agent.velocity.magnitude;
        bool isWalking = speed > walkThreshold;

        // Instantly switch walking state
        animator.SetBool(isWalkingParam, isWalking);
    }

    /// <summary>
    /// Smoothly rotates enemy toward movement direction.
    /// </summary>
    void HandleRotation()
    {
        Vector3 velocity = agent.velocity;
        velocity.y = 0f; // ignore vertical movement

        if (velocity.sqrMagnitude > 0.001f)
        {
            // Desired facing direction
            Quaternion targetRotation = Quaternion.LookRotation(velocity);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }
}
