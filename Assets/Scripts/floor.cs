using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class floor : MonoBehaviour
{
    [SerializeField]
    MeshRenderer m_renderer;

    public static float val = 0;
	
    // Update is called once per frame
    void Update()
    {
        m_renderer.material.mainTextureOffset = (Vector2.right * (val - .5f)) + (Vector2.right * (Mathf.Sin(Time.time) / 10));
    }
}
