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


    private Button trueBooleanButton;

    private Button falseBooleanButton;
    [SerializeField]
    private GameObject trueButtonParent;
    [SerializeField]
    private GameObject falseButtonParent;

    QuestionUI questionUI;

    // an array of all the multiple answer buttons
    Button[] multipleAnswerButtons;

    // helping gui
    [SerializeField]
    GameObject PopupGUI;

    // Use this for initialization
    void Start ()
    {     
        gameManager = GameManager.Instance;
        questionUI = GetComponent<QuestionUI>();
        playerStats = gameManager.playerStats;

        playerStats.NewRoundInit();
        // Updates the gui such as lives, score, streak
        questionUI.UpdateGUI(playerStats);

        trueBooleanButton = trueButtonParent.GetComponentInChildren<Button>();
        falseBooleanButton = falseButtonParent.GetComponentInChildren<Button>();
           
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
        // disables all the wrong answer buttons and the correct answer button
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

    // a function that sets up the answer buttons 
    void SetAnswerButtons(QuestionType type)
    {        
        // Different functionality for true/false and multiple answer buttons
        switch (type)
        {
            case QuestionType.multiple:
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
                    
                    Text _text = answerButton.GetComponentInChildren<Text>();
                    string currentAnswer = answers.ElementAt(randIndex);
                    // add it to wrong button or correct button depending on whether it is correct/wrong answer
                    if (currentAnswer.Equals(currentQuestion.correct_answer))
                    {
                        correctAnswerButton = answerButton;
                    }
                    else
                    {
                        wrongAnswers.Add(answerButton);
                    }
                    // transforms the answer text's html special characters to readable form
                    _text.text = WebUtility.HtmlDecode(answers.ElementAt(randIndex));
                    answers.RemoveAt(randIndex);
                    // enable it
                    answerButton.interactable = true;
                    answerButton.gameObject.SetActive(true);
                }

                break;
                // in case of true/false buttons
            case QuestionType.boolean:       
                // Set the button's parent to true
                trueButtonParent.SetActive(true);
                falseButtonParent.SetActive(true);
               
                if(currentQuestion.correct_answer.Equals("True"))
                {
                    correctAnswerButton = trueBooleanButton;
                    wrongAnswers.Add(falseBooleanButton);
                }
                else
                {
                    correctAnswerButton = falseBooleanButton;
                    wrongAnswers.Add(trueBooleanButton);
                }
                break;
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
        AnimateAnswerButtons(); // play animations
        
       
        if(button != correctAnswerButton)
        {
            OnWrongAnswer();
        }
        else
        {
            OnCorrectAnswer();
        }

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
        // increment their streak
        playerStats.CurrentStreak++;
        // every x4-x8-x12 etc streak add a life
        if(playerStats.CurrentStreak%4 == 0)
        {
            // Add a live
            playerStats.RemainingLives++;
        }
        // score to be added depending on it's streak
        int scoreReceived = ((int)currentQuestion.QuestionDifficulty + 1) * (int) Mathf.Floor((float)playerStats.CurrentStreak* 0.6f* 200);
        // increment the total correct questions answered on this round
        playerStats.RoundCorrectAnswers++;
        // Add bonus points for asking quickly
        scoreReceived += 10 * questionUI.timer;
        playerStats.CurrentScore += scoreReceived;
        
    }
    // The player has answered incorrectly
    void OnWrongAnswer()
    {
        // decrement its live
        playerStats.RemainingLives--;

        // check if this is higher than highscore
        if(playerStats.CurrentStreak > playerStats.HighestStreak)
        {
            playerStats.HighestStreak = playerStats.CurrentStreak;
        }

        // Check if it is the best streak
        if(playerStats.CurrentStreak > playerStats.BestRoundStreak)
        {
            playerStats.BestRoundStreak = playerStats.CurrentStreak;
        }
        
        playerStats.CurrentStreak = 1;
        
        // If no more lives
        if (playerStats.RemainingLives <= 0)
        {
            // Clear all answers
            ClearPreviousAnswers();
            // end the game
            gameManager.EndGame();
        }
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
