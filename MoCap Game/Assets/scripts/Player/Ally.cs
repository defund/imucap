//This script controls the allies. An ally is a simple entity which, once spawned, simply moves to a designated position

using UnityEngine;

public class Ally : MonoBehaviour
{
	public float Duration;							//How long the ally stays spawned

	[SerializeField] NavMeshAgent navMeshAgent;		//A reference to the ally's navmesh agent

	void Reset()
	{
		//Get a reference to the navmesh agent
		navMeshAgent = GetComponent<NavMeshAgent>();
	}

	public void Move(Vector3 point)
	{
		//Tell the navmesh agent to move to the designated point
		navMeshAgent.SetDestination(point);
	}
}
