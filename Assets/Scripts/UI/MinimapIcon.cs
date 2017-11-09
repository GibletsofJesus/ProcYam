using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapIcon : MonoBehaviour
{
    [SerializeField]
    Transform followMe;
    [SerializeField]
    float yPos = 0;
    [SerializeField]
    SpriteRenderer m_spriteRenderer;

    void Update()
    {
        m_spriteRenderer.enabled = GameStateManager.instance.m_currentState == GameStateManager.GameSate.Gameplay
        || GameStateManager.instance.m_currentState == GameStateManager.GameSate.Paused;
        
        transform.rotation = Quaternion.Euler(90, 0, 0);
        Vector3 newPos = followMe.position;
        newPos.y = yPos;
        transform.position = newPos;
    }
}
