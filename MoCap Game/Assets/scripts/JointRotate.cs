using UnityEngine;
using System.Collections;

public class JointRotate : MonoBehaviour {

    private const float minZ = 0;   // TODO: change the values and actually use these
    private const float maxZ = 180;

    public Vector3 heading;

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void LateUpdate() {
        transform.localEulerAngles = heading;    
    }

    public void setHeading(Vector3 head)
    {
        heading = head;
    }

    public void setHeading(float x, float y, float z)
    {
        heading = new Vector3(x, y, z);
    }
}
