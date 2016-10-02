/*
using UnityEngine;
using System.Collections;

public class JointRotate : MonoBehaviour {

	// Use this for initialization
	void Start () {	
	}
	
	// Update is called once per frame
	void Update () {
        var startRotation = transform.localRotation;
        var joint = GetComponent<ConfigurableJoint>();

        // Calculate the rotation expressed by the joint's axis and secondary axis
        var right = joint.axis;
        var forward = Vector3.Cross(joint.axis, joint.secondaryAxis).normalized;
        var up = Vector3.Cross(forward, right).normalized;
        Quaternion worldToJointSpace = Quaternion.LookRotation(forward, up);

        // Transform into world space
        Quaternion resultRotation = Quaternion.Inverse(worldToJointSpace);

        resultRotation *= Quaternion.Inverse(Quaternion.Euler(30, 30, 0) * startRotation);
        resultRotation *= worldToJointSpace;
        joint.targetRotation = resultRotation;
    }
}
*/