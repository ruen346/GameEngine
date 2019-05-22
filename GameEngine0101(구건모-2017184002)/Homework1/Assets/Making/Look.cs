using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Look : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {

        //Debug.Log(Mathf.Atan((Input.mousePosition.y) - 540) / (Input.mousePosition.x - 960));


        transform.LookAt(new Vector3(Input.mousePosition.x - 960, 0, Input.mousePosition.y - 540));
        


    }
}
