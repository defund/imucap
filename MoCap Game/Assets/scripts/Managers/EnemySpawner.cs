//This script controls the spawning of enemies. The enemy spawner is a "pooled" spawner. A "pool" is effectively a collection of objects
//that are generally disabled. Then when they are needed, an object is enabled, used, and then disabled again when it is done. This is in
//contrast to a system where we intantiate and destroy objects when we need them and need to get rid of them. Instantiating and destroying 
//can cause lag as well as memory spikes which are both bad things. This script also has a maximum number of enemies it can spawn to prevent
//the scene from being flooded with enemies which makes the game very difficult and can cause lag / crashes.

using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
	[Header("Spawner Properties")]
	[SerializeField] GameObject enemyPrefab;	//The enemy prefab to spawn
	[SerializeField] float spawnRate = 5f;		//Rate, in seconds, to spawn enemies
	[SerializeField] int maxEnemies = 10;		//Maximum number of enemies that this spawner can have at a time

	[Header("Debugging Properties")]
	[SerializeField] bool canSpawn = true;		//Can this spawner spawn enemies? This is useful for testing when you want to turn a spawner off

	EnemyHealth[] enemies;						//An array of the pooled enemies
	WaitForSeconds spawnDelay;					//The delay between attempts to spawn an enemy

	void Awake()
	{
		//Create an array to store the pool of enemies
		enemies = new EnemyHealth[maxEnemies];
		//Loop through the array and...
		for (int i = 0; i < maxEnemies; i++)
		{
			//...instantiate an enemy game object from a prefab...
			GameObject obj = (GameObject)Instantiate(enemyPrefab);
			//...get a reference to its EnemyHealth script...
			EnemyHealth enemy = obj.GetComponent<EnemyHealth>();
			//...parent it to this gamn object...
			obj.transform.parent = transform;
			//...disable it...
			obj.SetActive(false);
			//...finally, store the enemy's health script in the pool
			enemies[i] = enemy;
		}
		//Create the WaitForSeconds variable that will be used to delay spawning
		spawnDelay = new WaitForSeconds(spawnRate);
	}

	//This is an interesting use of Start() where the Start() method itself is
	//used as a coroutine. You could have the Start() method run a different coroutine to
	//achieve the same effect, but it's useful to know that using the Start() method like
	//this is possible
	IEnumerator Start()
	{
		//While the spawner can spawn...
		while (canSpawn)
		{
			//...wait the specified delay...
			yield return spawnDelay;
			//...and then spawn an enemy before looping again
			SpawnEnemy();

			//In case you are wondering, you could swap lines 55 and 57. Doing
			//so would cause an enemy to be immediately spawned when the game starts.
			//The way it is now, however, the spawner waits first which gives the 
			//player a chance to orient themselves
		}
	}

	//This method spawns an enemy into the scene by searching the pool for an available enemy
	//and enabling it. It's worth nothing that it would be more efficient to create a system
	//where we didn't have to search the pool for an available enemy and instead pulled any
	//enemies that weren't available out of the pool. It is constructed this way, however, in 
	//an attempt to keep the code as simple and clean as possible. Also, the size of the pools are
	//very small, so the difference in efficiency between the two ways of doing this is negligable
	void SpawnEnemy()
	{
		//Loop through the pool of enemies
		for (int i = 0; i < enemies.Length; i++)
		{
			//If the current enemy is available (not active)...
			if (!enemies[i].gameObject.activeSelf)
			{
				//...orient it with the spawner...
				enemies[i].transform.position = transform.position;
				enemies[i].transform.rotation = transform.rotation;
				//...enable it...
				enemies[i].gameObject.SetActive(true);
				//...and leave this method so it doesn't accidently spawn more enemies
				return;
			}
		}
	}
}
