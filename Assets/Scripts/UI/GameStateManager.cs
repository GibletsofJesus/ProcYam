using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager instance;

    public enum GameSate
    {
        Paused,
        Gameplay,
        GameOver,
        TrackSetup
    }

    public GameSate m_currentState;

    [Header("References")]
    [SerializeField]
    GameObject m_pauseMenu, m_gameplayUI;
    [SerializeField]
    UiScroller PauseUI;
    [SerializeField]
    TrackCamera m_cameraTracker;

    // Use this for initialization
    void Start()
    {
        instance = this;
    }

    void Update()
    {
        if (m_currentState == GameSate.TrackSetup)
        {
            Camera.main.transform.position = new Vector3(
                (Mathf.Cos(Time.time * .25f) * 500f),
                150f,
                (Mathf.Sin(Time.time * .25f) * 500f) - 0);
            Camera.main.transform.LookAt(TrackMaker.instance.transform.position);
        }
    }

    public void ChangeStategames(GameSate newState)
    {
        //Do things for exiting states
        switch (m_currentState)
        {
            case GameSate.GameOver:
                break;
            case GameSate.Paused:
                m_pauseMenu.SetActive(false);
                PauseUI.enabled = false;    
                break;
            case GameSate.Gameplay:
                break;
            case GameSate.TrackSetup:
                break;
        }

        m_currentState = newState;

        //Do things for entering states
        switch (m_currentState)
        {
            case GameSate.GameOver:
                m_gameplayUI.SetActive(false);
                m_cameraTracker.enabled = true;
                break;
            case GameSate.Paused:
                m_pauseMenu.SetActive(true);
                PauseUI.enabled = true;    
                break;
            case GameSate.Gameplay:
                break;
            case GameSate.TrackSetup:
                break;
        }
    }
}
