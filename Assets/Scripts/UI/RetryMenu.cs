using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RetryMenu : MonoBehaviour
{
    [SerializeField]
    KjuAnimation kjuAnimation = null;

    [SerializeField]
    public Button retryButton = null;
    [SerializeField]
    public Button menuButton = null;

    public void NewGameOver(bool playerWins)
    {
        kjuAnimation.Reset();

        if (playerWins)
        {
            kjuAnimation.SetColor(Color.HSVToRGB(143.0f / 360.0f, 1.0f, 1.0f));
            retryButton.GetComponentInChildren<Text>().text = "Next Level";
        }
        else
        {
            kjuAnimation.SetColor(Color.white);
            retryButton.GetComponentInChildren<Text>().text = "Retry";
        }
    }
}
