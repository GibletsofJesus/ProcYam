using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapIcon : MonoBehaviour
{
    [SerializeField]
    Transform followMe;
    [SerializeField]
    float yPos = 0;
    // Update is called once per frame

    void Update()
    {
        transform.rotation = Quaternion.Euler(90, 0, 0);
        Vector3 newPos = followMe.position;
        newPos.y = yPos;
        transform.position = newPos;
    }
}
