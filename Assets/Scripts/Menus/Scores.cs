/*
 * Authors: Edison and Tony
 */
using UnityEngine;
using TMPro;

/// <summary>
/// Controls the score
/// </summary>
public class Scores : MonoBehaviour
{
    /// <summary>
    /// Text object to modify
    /// </summary>
    public TextMeshProUGUI scoreText;
    /// <summary>
    /// Current score
    /// </summary>
    public float score;

    /// <summary>
    /// Current high score
    /// </summary>
    public float highscore; 

    private void Start()
    {
        // check if we have a score saved
        if (PlayerPrefs.HasKey("score"))
        {
            // load said score
            score = PlayerPrefs.GetFloat("score");
        }
        
        if (PlayerPrefs.HasKey("highscore"))
        {
            highscore = PlayerPrefs.GetFloat("highscore");
        }
        else
        {
            highscore = 0f;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        score -= Time.deltaTime;
        
        // Checks if score is below 0
        if (score < 0)
        {
            score = 0;
        }
        scoreText.text = "Score: " + Mathf.Round(score).ToString() + "\n" + "Highscore: " + Mathf.Round(highscore).ToString();
    }
}

// Citations:
// �Unity documentation,� Unity Documentation, https://docs.unity.com/ 