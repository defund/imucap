using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System;

public class Accelerate : MonoBehaviour {
    SerialPort sp = new SerialPort("COM1", 9600);
    float[] cal = { 0, 0, 0 };
    float[] pf = { 0, 0, 0 };
    int calibrationFrames = 1 * 5;
    int cf;
	// Use this for initialization
	void Start () {
        cf = calibrationFrames;
        sp.Open();
        sp.ReadTimeout = 0;
	}
	
	// Update is called once per frame
	void Update () {
        if (sp.IsOpen)
        {
            try
            {
                string st = sp.ReadLine();
                string[] stp = st.Split(' ');
                float[] xyz = { (float)Int32.Parse(stp[0])/1000, (float)-Int32.Parse(stp[2])/1000, (float)Int32.Parse(stp[1])/1000 };
                Debug.Log(st);
                if (cf>0)
                {
                    cal[0] += xyz[0]/calibrationFrames;
                    cal[1] += xyz[1]/calibrationFrames;
                    cal[2] += xyz[2]/calibrationFrames;
                    cf--;
                }
                else
                {
                    //GetComponent<Rigidbody>().AddForce(new Vector3(-(pf[0]-cal[0]), -(pf[1]-cal[1]), -(pf[2]-cal[2])), ForceMode.Acceleration);
                    GetComponent<Rigidbody>().AddForce(new Vector3((int)xyz[0]-cal[0], (int)xyz[1]-cal[1], (int)xyz[2]-cal[2]),ForceMode.Acceleration);
                    //pf = xyz;
                }
            }
            catch (System.Exception)
            {
            }
        }
	
	}
}
