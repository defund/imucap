using UnityEngine;
using System.Collections;

public class Hide : MonoBehaviour {
    public GameObject[] g = new GameObject[3];
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnTriggerEnter(Collider other)
    {
        for(int i = 0; i < g.Length; i++)
        {
            if (g[i].active == false)
            {
                g[i].SetActive(!g[i].active);
            }
        }
        gameObject.SetActive(!gameObject.active);
    }
}
