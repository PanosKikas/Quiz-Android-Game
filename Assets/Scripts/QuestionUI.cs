using UnityEngine.UI;
using UnityEngine;
using System.Net;
using System.Collections;

public class QuestionUI : MonoBehaviour
{
    [SerializeField]
    Text questionText;
    [SerializeField]
    Text difficultyText;
    [SerializeField]
    Text categoryText;

    [SerializeField]
    Text livesText;
    [SerializeField]
    Text scoreText;
    [SerializeField]
    Text timerText;

    public int timer { get; private set; }

    [SerializeField]
    GameObject PauseUI;
    
    QuestionManager questionManager;

    private void Start()
    {
        questionManager = GetComponent<QuestionManager>();
    }

    public void SetQuestionUI(Question currentQuestion)
    {
        SetTimer(currentQuestion.QuestionDifficulty, currentQuestion.TypeOfQuestion);
        questionText.text = WebUtility.HtmlDecode(currentQuestion.question);
        this.difficultyText.text = "Difficulty: " + currentQuestion.difficulty.ToUpper();
        this.categoryText.text = currentQuestion.category;      
       
    }

    public void UpdateLivesText(int lives)
    {
        livesText.text = lives.ToString();
    }

    public void UpdateScoreText(int score)
    {
        scoreText.text = "Score: " + score.ToString();
    }
    
    IEnumerator DecreaseTimer()
    {        
        timer--;
        timer = Mathf.Clamp(timer, 0, int.MaxValue);
        timerText.text = timer.ToString();
        yield return new WaitForSeconds(1f);

        if (timer <= 0)
        {
            Debug.Log("Time's up!");
            questionManager.ButtonClicked(null);
        }
        else
        {
            StartCoroutine(DecreaseTimer());
        }        
    }

    void SetTimer(Difficulty difficulty, QuestionType questionType)
    {
        timer = 0;

        if (questionType == QuestionType.boolean)
        {
            // decrease by 15s if question true/false
            timer -= 5;
            
        }

        switch (difficulty)
        {
            case Difficulty.easy:
                timer += 13;
                break;
            case Difficulty.medium:
                timer += 13;
                break;
            case Difficulty.hard:
                timer += 15;
                break;
        }
        StartCoroutine(DecreaseTimer());
    }

    public void TogglePause()
    {
        if(PauseUI.activeSelf)
        {
            PauseUI.SetActive(false);
            Time.timeScale = 1f;
        }
        else
        {
            PauseUI.SetActive(true);
            Time.timeScale = 0f;
        }

    }
}
