using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameResultUI : MonoBehaviour
{
    public TextMeshProUGUI scoreText, outcomeText;
    public bool showScore;
    
    public void Show(string outcome, int score)
    {
        gameObject.SetActive(true);
        scoreText.text =   $"Score : {score}";
        scoreText.gameObject.SetActive(showScore);
        outcomeText.text = outcome;
    }
    public void Retry()
    {
        Scene scene = SceneManager.GetActiveScene();
        Debug.Log("Name: " + scene.name);
        SceneManager.LoadScene(scene.name);
    }
}
