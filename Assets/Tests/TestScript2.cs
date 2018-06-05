using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Marduk;

public class TestScript2 : MonoBehaviour, IUpdatable {

    public bool doPrint = false;    
	// Use this for initialization
	void OnEnable() {
        Marduk.UpdateManager.RegisterUpdatable(this);
	}

    void OnDisable()
    {
        Marduk.UpdateManager.DeregisterUpdatable(this);
    }
	
	public void ManagedUpdate()
    {
        if(doPrint)
        {
            Debug.Log("Managed");
        }
    }
}
