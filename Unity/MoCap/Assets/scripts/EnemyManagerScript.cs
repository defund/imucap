using UnityEngine;
using System.Collections;

public class EnemyManagerScript : MonoBehaviour {
    // Constants
    private const long timePerSpawn = 3;
    private const int minR = 5;
    private const int maxR = 25;

    private System.Random rand;

    public GameObject enemy;
    public GameObject spawnPoint;

	// Use this for initialization
	void Start () {
        rand = new System.Random();
		InvokeRepeating("SpawnEnemy", 1, timePerSpawn);
	}
	
	void SpawnEnemy() {
        float x = (float)rand.NextDouble() * maxR;
        float z;
        if (x > minR)
        {
            z = (float) rand.NextDouble() * Mathf.Sqrt(Mathf.Pow(maxR, 2) - Mathf.Pow(x, 2));
        }
        else
        {
            z = (float)rand.NextDouble() * (Mathf.Sqrt(Mathf.Pow(maxR, 2) - Mathf.Pow(x, 2)) - Mathf.Sqrt(Mathf.Pow(minR, 2) - Mathf.Pow(maxR, 2))) + Mathf.Sqrt(Mathf.Pow(minR, 2) - Mathf.Pow(maxR, 2));
        }
        if (rand.NextDouble() < .5)
        {
            x = -x;
        }



        spawnPoint.transform.position = new Vector3(x,0,z);
        Instantiate(enemy, spawnPoint.transform.position, spawnPoint.transform.rotation);
	}
}
