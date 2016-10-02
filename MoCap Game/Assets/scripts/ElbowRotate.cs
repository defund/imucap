using UnityEngine;
using System.Collections;

public class ElbowRotate : MonoBehaviour {

    private const float minZ = 0;   // TODO: change the values and actually use these
    private const float maxZ = 180;


    // Use this for initialization
    void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void LateUpdate() {
        //Transform transform = GetComponent<Transform>();
        //Plane playerPlane = new Plane(Vector3.forward, transform.position);
        //Vector3 mp = Input.mousePosition;
        //print(mp);
        //Ray ray = Camera.main.ScreenPointToRay(mp);
        //float hitdist = 0.0F;
        //if (playerPlane.Raycast(ray, out hitdist))
        //{
        //  var targetPoint = ray.GetPoint(hitdist);
        //}
        //Vector3 mouseVec = new Vector3(targetPoint.x - transform.position.x, 0, targetPoint.z - transform.position.z);
        //float zAngle = Vector3.Angle(mouseVec, new Vector3(0, -1, 0));
        //if (zAngle > minZ && zAngle < maxZ)
        //{
        //print(zAngle);
        //transform.localEulerAngles = new Vector3(0, 0, zAngle + 270);
        //}
        transform.localEulerAngles = new Vector3(90, 0, 0);    
    }
}
