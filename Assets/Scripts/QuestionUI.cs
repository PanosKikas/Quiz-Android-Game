using UnityEngine.UI;
using UnityEngine;
using System.Net;
using System.Collections;

// A class that sets the gui of the questions such as 
// category, difficulty and updates the timer, lives, streak
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
    [SerializeField]
    Text currentStreak;

    public int timer { get; private set; }
    
    
    QuestionManager questionManager;

    private void Start()
    {
        questionManager = GetComponent<QuestionManager>();
    }

    // Takes the info for the current question and updates the gui
    public void SetQuestionUI(Question currentQuestion)
    {
        // set the timer (depends on difficulty)
        SetTimer(currentQuestion.QuestionDifficulty, currentQuestion.TypeOfQuestion);
        questionText.text = WebUtility.HtmlDecode(currentQuestion.question);
        this.difficultyText.text = "Difficulty: " + currentQuestion.difficulty.ToUpper();
        this.categoryText.text = currentQuestion.category;            
    }

    // a function that takes a reference to the player stats and updates 
    // the helper gui (score, lives streak)
    public void UpdateGUI(PlayerStats stats)
    {
        livesText.text = stats.RemainingLives.ToString();
        currentStreak.text = "STREAK: x" + stats.CurrentStreak.ToString();
        scoreText.text = "Score: " + stats.CurrentScore.ToString();
    }
    
    // Decreases the timer each second
    IEnumerator DecreaseTimer()
    {        
        timer--;
        timer = Mathf.Clamp(timer, 0, int.MaxValue); // dont let it fall below 0
        timerText.text = timer.ToString(); 
        yield return new WaitForSeconds(1f);
        // no more time left
        if (timer <= 0)
        {
            Debug.Log("Time's up!");
            questionManager.ButtonClicked(null); // the user clicked no button (wrong)
        }
        else
        {
            StartCoroutine(DecreaseTimer()); // loop
        }        
    }

    // Gets the question time and the difficulty and adjusts the value of timer
    // accordinly
    void SetTimer(Difficulty difficulty, QuestionType questionType)
    {
        timer = 0;

        
        if (questionType == QuestionType.boolean)
        {
            // decrease by 5s if question true/false
            timer -= 5;
            
        }

        // more time depending on difficulty
        switch (difficulty)
        {
            case Difficulty.easy:
                timer += 16;
                break;
            case Difficulty.medium:
                timer += 16;
                break;
            case Difficulty.hard:
                timer += 20;
                break;
        }
        // Decrease timer
        StartCoroutine(DecreaseTimer());
    }
    
}
