using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Linq;
using System.Collections;

// Question manager handles the main game functionality
// for displaying new questions, animating buttons etc
public class QuestionManager : MonoBehaviour
{   
    [SerializeField]
    Transform MultipleAnswersPanel;

    [SerializeField]
    Transform trueFalsePanel;
    
    GameManager gameManager;
    
    // The currently displaed question
    Question currentQuestion;
    PlayerStats playerStats;

    // List of all wrong answer buttons
    private List<Button> wrongAnswers;
    // reference to the currently correct answer button
    private Button correctAnswerButton;

    [SerializeField]
    private Button trueBooleanButton;
    [SerializeField]
    private Button falseBooleanButton;

    private AudioClip audioClipToPlay;

    QuestionUI questionUI;

    const int ScoreForCorrectQuestion = 20;
    // an array of all the multiple answer buttons
    Button[] multipleAnswerButtons;

    // helping gui
    [SerializeField]
    GameObject PopupGUI;

    [Header("AudioClips")]
    [SerializeField] AudioClip correctAnswerSfx;
    [SerializeField] AudioClip wrongAnswerSfx;


    #region Singleton
    public static QuestionManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    #endregion

    // Use this for initialization
    void Start ()
    {
        AudioManager.Instance.StopBGMusic();
        gameManager = GameManager.Instance;
        questionUI = GetComponent<QuestionUI>();
        playerStats = gameManager.playerStats;

        playerStats.NewRoundInit();
        // Updates the gui such as lives, score, streak
        questionUI.UpdateGUI(playerStats);
    
        wrongAnswers = new List<Button>();

        multipleAnswerButtons = MultipleAnswersPanel.GetComponentsInChildren<Button>(true);
        GetNextQuestion();
    }
    
    void GetNextQuestion()
    {
        ClearPreviousAnswers();
        currentQuestion = QuestionsRetriever.Instance.GetRandomQuestion();
        Debug.Log(currentQuestion.correct_answer);
        questionUI.SetQuestionUI(currentQuestion);
        SetAnswerButtons(currentQuestion.TypeOfQuestion);       
    }

    // A function that clear all the previous buttons
    void ClearPreviousAnswers()
    {
        foreach (Button wrongAnswerButton in wrongAnswers)
        {
            InitializeButton(wrongAnswerButton);
        }

        InitializeButton(correctAnswerButton);
        // clears the wrong answers list
        wrongAnswers.Clear();
    }
    
    
    void InitializeButton(Button button)
    {
        if (button != null && button.gameObject.activeSelf)
        {
            button.interactable = true;
            button.gameObject.SetActive(false);
        }
        
    }

    void SetAnswerButtons(QuestionType type)
    {        
        // Different functionality for true/false and multiple answer buttons
        switch (type)
        {
            case QuestionType.multiple:
                SetMultipleQuestionButtons();
                break;
            case QuestionType.boolean:
                SetBooleanButtons();
                break;
        }
    }
    
    void SetMultipleQuestionButtons()
    {
        // get a list of all the wrong answers
        List<string> answers = currentQuestion.incorrect_answers.ToList<string>();
        // add the correct answer to the list
        answers.Add(currentQuestion.correct_answer);
        
        // For every answer entry
        for (int i = 0; i < currentQuestion.incorrect_answers.Length + 1; i++)
        {
            // Get a random index
            int randIndex = Random.Range(0, answers.Count);
            // set the ith button with the text of the answer
            Button answerButton = multipleAnswerButtons[i];

            Text answerButtonText = answerButton.GetComponentInChildren<Text>();
            string currentAnswer = answers.ElementAt(randIndex);

            if (IsCorrectAnswer(currentAnswer))
            {
                correctAnswerButton = answerButton;
            }
            else
            {
                wrongAnswers.Add(answerButton);
            }

            // transforms the answer text's html special characters to readable form
            answerButtonText.text = WebUtility.HtmlDecode(currentAnswer);
            answers.RemoveAt(randIndex);
            answerButton.gameObject.SetActive(true);
        }
    }   

    bool IsCorrectAnswer(string answer)
    {
        return answer.Equals(currentQuestion.correct_answer);
    }
    
    private void SetBooleanButtons()
    {
        trueBooleanButton.gameObject.SetActive(true);
        falseBooleanButton.gameObject.SetActive(true);

        if (currentQuestion.correct_answer.Equals("True"))
        {
            correctAnswerButton = trueBooleanButton;
            wrongAnswers.Add(falseBooleanButton);
        }
        else
        {
            correctAnswerButton = falseBooleanButton;
            wrongAnswers.Add(trueBooleanButton);
        }
    }
    
    // This is called from OnAnswerButton whenever a button is clicked
    public void ButtonClicked(Button button)
    {
        StartCoroutine(OnAnswerClicked(button));
    }
    
    // Check which button was just clicked. Different functionality for correct/wrong answers
    public IEnumerator OnAnswerClicked(Button button)
    {
        AnimateAnswerButtons(); 
       
        if(button != correctAnswerButton)
        {
            OnWrongAnswer();
        }
        else
        {
            OnCorrectAnswer();
        }

        AudioManager.Instance.PlayAudioClip(audioClipToPlay);

        // Update the player's gui
        questionUI.UpdateGUI(playerStats);

        // wait before displaying the next question
        yield return new WaitForSeconds(1.5f);
        // Get next question
        GetNextQuestion();
    }

    // the player has answered correctly
    void OnCorrectAnswer()
    {
        audioClipToPlay = correctAnswerSfx;

        // increment their streak
        playerStats.CurrentStreak++;
        // every x5-x10-x15 etc streak add a life
        if(playerStats.CurrentStreak%5 == 0)
        {
            playerStats.RemainingLives++;
        }

        int scoreReceived = ComputeScoreForQuestion();
        // increment the total correct questions answered on this round
        playerStats.RoundCorrectAnswers++;
        playerStats.CurrentScore += scoreReceived;
        
    }

    int ComputeScoreForQuestion()
    {
        float difficultyMultipler = (int)(currentQuestion.QuestionDifficulty + 1);
        float streakMultipler = (float)(playerStats.CurrentStreak * 0.2f);
        int timeBonus = 2 * questionUI.timer;
        int finalScore = Mathf.FloorToInt((streakMultipler + difficultyMultipler) * ScoreForCorrectQuestion) + timeBonus;
        return finalScore;
    }

    // The player has answered incorrectly
    void OnWrongAnswer()
    {
        audioClipToPlay = wrongAnswerSfx;
        // decrement its live
        playerStats.RemainingLives--;

        UpdateStreakStats();
        
        // If no more lives
        if (HasNoLivesRemaining())
        {
            ClearPreviousAnswers();
            gameManager.EndGame();
        }
    }

    

    void UpdateStreakStats()
    {
        
        if (playerStats.CurrentStreak > playerStats.HighestStreak)
        {
            playerStats.HighestStreak = playerStats.CurrentStreak;
        }

        // Check if it is the best round streak
        if (playerStats.CurrentStreak > playerStats.BestRoundStreak)
        {
            playerStats.BestRoundStreak = playerStats.CurrentStreak;
        }

        playerStats.CurrentStreak = 0;

    }

    bool HasNoLivesRemaining()
    {
        return playerStats.RemainingLives <= 0;
    }

    // Animates the buttons
    void AnimateAnswerButtons()
    {
        // Stop the timer
        questionUI.StopAllCoroutines();

        // animate every wrong answer button
        foreach (Button _button in wrongAnswers)
        {
            Animator anim = _button.GetComponentInParent<Animator>();
            _button.interactable = false;
            anim.SetTrigger("Incorrect");
        }

        // set to note interactable 
        correctAnswerButton.interactable = false;
        // animate correct answer button
        correctAnswerButton.GetComponentInParent<Animator>().SetTrigger("Correct");
    }

}
