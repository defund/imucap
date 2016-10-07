using UnityEngine;
using System.Collections;

public class SpawnPrimitive : MonoBehaviour {
    GameObject[] g = new GameObject[3];
    System.Random rand;
	// Use this for initialization
	void Start () {
        g[0] = GameObject.Find("Spawn 1");
        g[1] = GameObject.Find("Spawn 2");
        g[2] = GameObject.Find("Spawn 3");
        rand = new System.Random();
        

    }
	
	// Update is called once per frame
	void Update () {
	    
	}
}
