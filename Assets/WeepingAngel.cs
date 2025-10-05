using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;

    [Header("Movement Settings")]
    public float updateRate = 0.1f;   // How fast to update destination (lower = more responsive)

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        InvokeRepeating(nameof(UpdateDestination), 0f, updateRate);
    }

    void UpdateDestination()
    {
        if (player != null)
            agent.SetDestination(player.position);
    }
}
