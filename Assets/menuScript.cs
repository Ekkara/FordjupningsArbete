using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class menuScript : MonoBehaviour
{
    public static int score;
    static int topScore;

    [SerializeField] Text scoreText, topScoreText;

    public void Start()
    {
        if(topScore <= score)
        {
            topScore = score;
        }
        scoreText.text = "Last score: " + score;
        topScoreText.text = "Top score: " + topScore;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    public void LoadGame()
    {
        score = 0;
        SceneManager.LoadScene("PlayScene");
    }
    public void Quit()
    {
        Application.Quit();
    }
}
