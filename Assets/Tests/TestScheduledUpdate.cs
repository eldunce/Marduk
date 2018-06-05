using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Marduk;

public class TestScheduledUpdate : MonoBehaviour, IUpdatable {

    public Transform start;
    public Transform end;

    public float min, max;

    float time;

	// Use this for initialization
	void OnEnable()
    {
        UpdateManager.RegisterUpdatable(this, min, max, min, max);	
	}
	
    void OnDisable()
    {
        UpdateManager.DeregisterUpdatable(this);
    }
	
    public void ManagedUpdate()
    {
        time += Time.deltaTime;
        transform.position = Vector3.Lerp(start.position, end.position, Mathf.Sin(time));
    }
}
