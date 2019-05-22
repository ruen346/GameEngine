using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public GameObject main_tank;

    public GameObject spawn_prefab;
    public float[] spawn_x;
    public float[] spawn_z;

    // Start is called before the first frame update
    void Start()
    {
        //Instantiate(main_tank, new Vector3(0, 0, 0), transform.rotation);

        for (int i=0; i<20; i++)
        {
            Instantiate(spawn_prefab, new Vector3(spawn_x[i], 0, spawn_z[i]), transform.rotation);
        }
    }
}
