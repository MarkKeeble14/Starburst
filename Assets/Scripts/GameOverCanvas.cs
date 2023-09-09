using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverCanvas : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI restartText;
    [SerializeField] private TextMeshProUGUI skipWaitText;
    [SerializeField] private float waitDuration;
    [SerializeField] private GameObject overlay;
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private HighScores highScores;
    private bool triggered;

    public void GameOver()
    {
        if (triggered) return;
        triggered = true;
        inGameUI.SetActive(false);
        overlay.SetActive(true);
        highScores.SetScores();
        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        skipWaitText.gameObject.SetActive(true);
        for (float f = 0; f < waitDuration; f += Time.deltaTime)
        {
            restartText.text = "Restarting In: " + Mathf.RoundToInt((waitDuration - f)).ToString();
            skipWaitText.text = "Press Space to Restart Immediately";
            if (Input.GetKeyDown(KeyCode.Space))
                break;
            yield return null;
        }
        skipWaitText.gameObject.SetActive(false);

        restartText.text = "Restarting";

        TransitionManager._Instance.FadeOut(() => SceneManager.LoadScene(SceneManager.GetActiveScene().name));
    }
}
