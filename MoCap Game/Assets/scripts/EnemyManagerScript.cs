using UnityEngine;
using System.Collections;

public class EnemyManagerScript : MonoBehaviour {
    // Constants
    private const long timePerSpawn = 9000;
    private const int numSpawnPoints = 10;

    private System.Random rand;

    public GameObject enemy;
    public Transform[] spawnPoints;

	// Use this for initialization
	void Start () {
        rand = new System.Random();
		InvokeRepeating("SpawnEnemy", 1, 2);
	}
	
	void SpawnEnemy() {
        int i = rand.Next(spawnPoints.Length);

        Instantiate(enemy, spawnPoints[i].position, spawnPoints[i].rotation);
	}
}
