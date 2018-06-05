using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestHarness : MonoBehaviour {

    List<TestScript1> test1 = new List<TestScript1>();
    List<TestScript2> test2 = new List<TestScript2>();
    float countdown = 0f;

    public int testSize = 1000;
    public bool doPrint;
    public bool addRemove;
    private void Awake()
    {
        for(int i = 0; i < testSize; i++)
        {
            GameObject go1 = new GameObject("Non-managed");            
            test1.Add(CreateTest1(go1));

            GameObject go2 = new GameObject("Managed");
            test2.Add(CreateTest2(go2));
        }
    }

    TestScript1 CreateTest1(GameObject go)
    {
        TestScript1 t1 = go.AddComponent<TestScript1>();
        t1.doPrint = doPrint;
        return t1;
    }

    TestScript2 CreateTest2(GameObject go)
    {
        TestScript2 t1 = go.AddComponent<TestScript2>();
        t1.doPrint = doPrint;
        return t1;
    }

    private void LateUpdate()
    {
        if (addRemove)
        {
            countdown -= Time.deltaTime;
            if (countdown <= 0f)
            {
                countdown = Random.Range(.2f, 3f);
                int idx = Random.Range(0, test1.Count);
                GameObject go1 = test1[idx].gameObject;
                test1.RemoveAt(idx);
                Destroy(go1);

                GameObject go2 = test2[idx].gameObject;
                test2.RemoveAt(idx);
                Destroy(go2);

                go1 = new GameObject("Non-managed");
                test1.Add(CreateTest1(go1));

                go2 = new GameObject("Managed");
                test2.Add(CreateTest2(go2));
            }
        }
    }
}
