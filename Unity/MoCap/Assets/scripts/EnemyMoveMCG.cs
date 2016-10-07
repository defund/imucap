using UnityEngine;
using System.Collections;

public class EnemyMoveMCG : MonoBehaviour {

    Transform player;
    NavMeshAgent nav;

    // Use this for initialization
    void Start () {
        player = GameObject.Find("Robot Kyle").transform;
        nav = GetComponent<NavMeshAgent>();
    }
	
	// Update is called once per frame
	void Update () {
        // If the enemy and the player have health left...
        //if (enemyHealth.currentHealth > 0 && playerHealth.currentHealth > 0)
        //{
            // ... set the destination of the nav mesh agent to the player.
            nav.SetDestination(player.position);
        //}
        // Otherwise...
        //else
        //{
            // ... disable the nav mesh agent.
          //  nav.enabled = false;
        }
    }
