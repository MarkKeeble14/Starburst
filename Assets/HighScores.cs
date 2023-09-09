using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HighScores : MonoBehaviour
{
    [SerializeField] private MainBall ball;
    [SerializeField] private string secondsSurvivedKey = "SecondsSurvived";
    [SerializeField] private string starsCollapsedKey = "StarsCollapsed";

    [SerializeField] private TextMeshProUGUI currentSecondsSurvivedText;
    [SerializeField] private TextMeshProUGUI hsSecondsSurvivedText;
    [SerializeField] private TextMeshProUGUI currentStarsCollapsedText;
    [SerializeField] private TextMeshProUGUI hsStarsCollapsedText;


    public void SetScores()
    {
        currentSecondsSurvivedText.text = "Seconds Survived: " + Mathf.RoundToInt(ball.SurvivalTimer).ToString() + " Seconds";
        currentStarsCollapsedText.text = "Stars Collapsed: " + ball.PickedUp.ToString() + " Stars";

        // Seconds Survived
        if (PlayerPrefs.HasKey(secondsSurvivedKey))
        {
            float hsSecondsSurvived = PlayerPrefs.GetFloat(secondsSurvivedKey);
            if (ball.SurvivalTimer > hsSecondsSurvived)
            {
                PlayerPrefs.SetFloat(secondsSurvivedKey, ball.SurvivalTimer);
                hsSecondsSurvivedText.text = "New High Score!: " + Mathf.RoundToInt(ball.SurvivalTimer).ToString() + " Seconds";
            }
            else
            {
                hsSecondsSurvivedText.text = "High Score: " + Mathf.RoundToInt(hsSecondsSurvived).ToString() + " Seconds";
            }
        }
        else
        {
            PlayerPrefs.SetFloat(secondsSurvivedKey, ball.SurvivalTimer);
            hsSecondsSurvivedText.text = "New High Score!: " + Mathf.RoundToInt(ball.SurvivalTimer).ToString() + " Seconds";
        }

        // Stars Collapsed
        if (PlayerPrefs.HasKey(starsCollapsedKey))
        {
            int hsStarsCollapsed = PlayerPrefs.GetInt(starsCollapsedKey);
            if (ball.PickedUp > hsStarsCollapsed)
            {
                PlayerPrefs.SetInt(starsCollapsedKey, ball.PickedUp);
                hsStarsCollapsedText.text = "New High Score!: " + ball.PickedUp.ToString() + " Stars";
            }
            else
            {
                hsStarsCollapsedText.text = "High Score: " + hsStarsCollapsed.ToString() + " Stars";
            }
        }
        else
        {
            PlayerPrefs.SetInt(starsCollapsedKey, ball.PickedUp);
            hsStarsCollapsedText.text = "New High Score!: " + ball.PickedUp.ToString() + " Stars";
        }
    }
}
