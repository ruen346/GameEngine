using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public GameObject spawn_prefab;
    public float[] spawn_x;
    public float[] spawn_z;
    
    void Start()
    {
        for (int i=0; i<20; i++)
        {
            Instantiate(spawn_prefab, new Vector3(spawn_x[i], 0, spawn_z[i]), transform.rotation);
        }
    }
}
