using UnityEngine;
using UnityEngine.AI;

public static class NavMeshAgentExtensions
{
    public static bool SetDestinationImmediate(this NavMeshAgent agent, Vector3 targetLocation)
    {
        if (agent == null) return false;

        NavMeshPath path = new NavMeshPath();

        NavMeshQueryFilter queryFilter = new NavMeshQueryFilter
        {
            agentTypeID = agent.agentTypeID,
            areaMask = agent.areaMask
        };

        bool canSetPath = NavMesh.CalculatePath(
            agent.transform.position,
            targetLocation,
            queryFilter,
            path
        );

        if (canSetPath && path.status == NavMeshPathStatus.PathComplete)
        {
            agent.SetPath(path);
        }

        return canSetPath;
    }
}
