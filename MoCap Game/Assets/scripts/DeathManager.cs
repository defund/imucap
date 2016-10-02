using UnityEngine;
using System.Collections;

public class DeathManager : MonoBehaviour {

    public Texture2D fadeOutTexture;
    public float fadeSpeed = 0.8f;

    private int drawDepth = -1000;
    private float alpha = 1.0f;
    private int fadeDir = -1;

    private bool dead = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {
        if (dead)
        {
            alpha += fadeDir * fadeSpeed * Time.deltaTime;
            alpha = Mathf.Clamp01(alpha);

            GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
            GUI.depth = drawDepth;
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeOutTexture);
            Application.LoadLevel(1);
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.name == "Katana") {
            Destroy(gameObject);
            print("Killed");
        } else if (other.gameObject.name == "Robot Kyle") {
            dead = true;
        }
    }
}
