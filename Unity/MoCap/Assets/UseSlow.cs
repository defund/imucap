using UnityEngine;
using System.Collections;
using System;
using System.IO.Ports;

public class UseSlow : MonoBehaviour {
    SerialPort sp = new SerialPort("COM1", 4800);
    Vector3 cal = Vector3.zero;
    int c = 0;
    float t;
    void Start()
    {
        t = Time.time;
        sp.Open();
        sp.ReadTimeout = 0;
    }

    void Update()
    {
        if (sp.IsOpen)
        {
            try
            {
                string st = sp.ReadLine();
                Debug.Log(st);
                string[] stp = st.Split('\t');
                Vector3 cur = new Vector3(-float.Parse(stp[2]), float.Parse(stp[0]), -float.Parse(stp[1]));
                float g = Time.time;
                Debug.Log(g - t);
                t = g;
                if (c == 2)
                {
                    transform.eulerAngles = cur - new Vector3(cal.x, cal.y, cal.z);// - transform.parent.transform.eulerAngles;
                }
                if (c == 1)
                {
                    c = 2;
                    cal = cur;
                }
                if (c==0)
                {
                    c = 1;
                }
                
                
            }
            catch
            {

            }
        }
    }
}
