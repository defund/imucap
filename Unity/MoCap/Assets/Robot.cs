using UnityEngine;
using System.Collections;

public class Robot : MonoBehaviour {
    public GameObject cam;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("space"))
        {
            Application.LoadLevel("blossom");
        }
        transform.eulerAngles = new Vector3(0, cam.transform.eulerAngles.y, 0);
	}
}
