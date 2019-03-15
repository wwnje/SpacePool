using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolTest : MonoBehaviour
{
    public GameObject Obj1;
    public GameObject Obj2;

    public Transform P1;
    public Transform P2;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            Obj1.Spawn(P1);
            Obj2.Spawn(P2, "name");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
