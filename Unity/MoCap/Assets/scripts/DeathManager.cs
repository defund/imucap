using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class DeathManager : MonoBehaviour {

    public GameObject target;

    public Texture2D fadeOutTexture;
    public float fadeSpeed = 0.8f;

    private int drawDepth = -1000;
    private float alpha = 1.0f;
    private int fadeDir = -1;

    private double threshold = 1.0;

    private bool dead = false;

	// Use this for initialization
	void Start () {
        target = GameObject.Find("Robot Kyle");
	}
	
	// Update is called once per frame
	void Update () {
        double distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance < threshold)
        {
            //UnityEngine.Debug.Log("You died");
            //Application.LoadLevel("start");
            Destroy(gameObject);
       }
    }

    void OnGUI()
    {
        if (dead) {
            alpha += fadeDir * fadeSpeed * Time.deltaTime;
            alpha = Mathf.Clamp01(alpha);

            GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
            GUI.depth = drawDepth;
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeOutTexture);

        }
    }

    void OnTriggerEnter(Collider other) {
       // Destroy(other);
        //UnityEngine.Debug.Log("Killed a Zombunny");

        //if (other.gameObject.name == "Right_Shoulder_Joint_01") {
          //  Destroy(gameObject);
            //print("Killed");
        //} else if (other.gameObject.name == "Robot Kyle") {
           // UnityEngine.Debug.Log("You die");
            //Application.LoadLevel("start");
        //}
    }
}
