using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Common : MonoBehaviour {
    public Transform[] commonWP;
    public GameObject SpecialPoint;

    private void Awake()
    {
        commonWP = new Transform[4];
        for(int i=0;i<4;i++)
        {
            Vector3 pos = UnityEngine.Random.insideUnitSphere * 20;
            pos.y = 0;
            GameObject w = Instantiate(SpecialPoint, pos, Quaternion.identity);
            commonWP[i] = w.transform;
        }

    

      
    }

    // Use this for initialization
    void Start () {
        
		


	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
