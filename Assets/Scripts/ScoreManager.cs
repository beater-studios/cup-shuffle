using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    [Header("UI Components")]
    [Tooltip("Text component to display the score.")]
    [SerializeField] private Text scoreText;

    private int score = 0;

    #region Unity Methods

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (scoreText == null)
        {
            Debug.LogError("Score Text is not assigned in the inspector");
        }
        UpdateScoreText();
    }

    #endregion

    #region Score Management

    /// <summary>
    /// Updates the score text to reflect the current score.
    /// </summary>
    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    /// <summary>
    /// Adds points to the current score and updates the score text.
    /// </summary>
    /// <param name="points">The number of points to add.</param>
    public void AddScore(int points)
    {
        score += points;
        UpdateScoreText();
    }

    #endregion

}