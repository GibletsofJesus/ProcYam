using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUI : UIFunction
{
    [Header("References")]
    [SerializeField]
    Text[] m_creditTexts;
    [SerializeField]
    Image m_creditsBackground;

    public override void CallFunction(int _index, UiScroller _ref)
    {
        switch (_index)
        {
            case 2:
                Application.Quit();
                break;
            case 0:
                //start game, generate a map
                SceneManager.LoadScene(1);
                break;  
            case 1:
                _ref.ToggleTextElements(false);
                StartCoroutine(FadeCredits(!creditsDisplayed, _ref));
                break;
        }
    }

    bool fading = false;
    bool creditsDisplayed = false;

    IEnumerator FadeCredits(bool b, UiScroller _ref)
    {
        _ref.m_allowMove = false;
        _ref.m_allowClick = false;

        fading = true;
        float lerpo = 0;
        while (lerpo < 1)
        {
            lerpo += Time.deltaTime;

            foreach (Text t in m_creditTexts)
            {
                t.color = Color.Lerp(Color.white, new Color(1, 1, 1, 0), !b ? lerpo : 1 - lerpo);
            }
            m_creditsBackground.color = Color.Lerp(new Color(0, 0, 0, 0.5f), new Color(0, 0, 0, 0), !b ? lerpo : 1 - lerpo);
            yield return new WaitForEndOfFrame();
        }
        fading = false;

        _ref.m_allowMove = !b;
        _ref.m_allowClick = true;
        creditsDisplayed = b;
        _ref.ToggleTextElements(!b);
    }
}
