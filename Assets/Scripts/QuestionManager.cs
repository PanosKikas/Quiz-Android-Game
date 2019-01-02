using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Linq;
using System.Collections;

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
    
    Question currentQuestion;
    PlayerStats playerStats;

    private List<Button> wrongAnswers;
    private Button correctAnswerButton;

    [SerializeField]
    private Button trueBooleanButton;
    [SerializeField]
    private Button falseBooleanButton;

    private GameObject trueButtonParent;
    private GameObject falseButtonParent;

    QuestionUI questionUI;

    Button[] multipleAnswerButtons;

	// Use this for initialization
	void Start ()
    {
        
        gameManager = GameManager.Instance;
        questionUI = GetComponent<QuestionUI>();
        playerStats = gameManager.playerStats;

        questionUI.UpdateLivesText(playerStats.RemainingLives);
        questionUI.UpdateScoreText(playerStats.CurrentScore);

        trueButtonParent = trueBooleanButton.transform.parent.gameObject;
        falseButtonParent = falseBooleanButton.transform.parent.gameObject;
        
     
        wrongAnswers = new List<Button>();

        multipleAnswerButtons = MultipleAnswersPanel.GetComponentsInChildren<Button>(true);

      

        GetNextQuestion();
    }
	
    void GetNextQuestion()
    {
        // Remove Previous Answer Buttons
        ClearPreviousAnswers();
      
        // Get random question and remove it from list
        int randomQuestionIndex = Random.Range(0, gameManager.questionList.Count);
        currentQuestion = gameManager.questionList[randomQuestionIndex];
        gameManager.questionList.RemoveAt(randomQuestionIndex);
        
        Debug.Log(currentQuestion);
        // Set the UI elements of the current question
        questionUI.SetQuestionUI(currentQuestion);

        // Set the answer buttons
        SetAnswerButtons(currentQuestion.TypeOfQuestion);       
    }

    void SetAnswerButtons(QuestionType type)
    {        
        switch (type)
        {
            case QuestionType.multiple:

                List<string> answers = currentQuestion.incorrect_answers.ToList<string>();
                answers.Add(currentQuestion.correct_answer);

                for (int i = 0; i < currentQuestion.incorrect_answers.Length + 1; i++)
                {
                    int randIndex = Random.Range(0, answers.Count);
                    
                    Button answerButton = multipleAnswerButtons[i];
                    
                    Text _text = answerButton.GetComponentInChildren<Text>();
                    string currentAnswer = answers.ElementAt(randIndex);
                    if (currentAnswer.Equals(currentQuestion.correct_answer))
                    {
                        correctAnswerButton = answerButton;
                    }
                    else
                    {
                        wrongAnswers.Add(answerButton);
                    }

                    _text.text = WebUtility.HtmlDecode(answers.ElementAt(randIndex));
                    answers.RemoveAt(randIndex);
                    answerButton.interactable = true;
                    answerButton.gameObject.SetActive(true);
                }

                break;

            case QuestionType.boolean:       
                
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
       
    void ClearPreviousAnswers()
    {       
        if (trueButtonParent.activeInHierarchy)
        {
            trueButtonParent.SetActive(false);
            trueBooleanButton.interactable = true;
        }

        if(falseButtonParent.activeInHierarchy)
        {
            falseButtonParent.SetActive(false);
            falseBooleanButton.interactable = true;
            wrongAnswers.Clear();
            correctAnswerButton = null;
        }
        
        foreach (Button answer in wrongAnswers)
        {
            if (answer.gameObject.activeSelf)
            {
                answer.gameObject.SetActive(false);
            }
        }
        
        if(correctAnswerButton != null && correctAnswerButton.gameObject.activeSelf)
        {
            correctAnswerButton.gameObject.SetActive(false);
        }
        wrongAnswers.Clear();        
    }
    
    public void ButtonClicked(Button button)
    {
        StartCoroutine(OnAnswerClicked(button));
    }
    
    public IEnumerator OnAnswerClicked(Button button)
    {
        AnimateAnswerButtons();
        
        if(button != correctAnswerButton)
        {
            //playerStats.RemainingLives--;
            questionUI.UpdateLivesText(playerStats.RemainingLives);
            if (playerStats.RemainingLives <= 0)
            {
                ClearPreviousAnswers();
                gameManager.EndGame();
            }                     
        }
        else
        {
            int scoreReceived = ((int)currentQuestion.QuestionDifficulty + 1) * 200;
            
            scoreReceived += 20 * questionUI.timer;
            playerStats.CurrentScore += scoreReceived;
            playerStats.TotalCorrectQuestionsAnswered++;
            questionUI.UpdateScoreText(playerStats.CurrentScore);

        }

        yield return new WaitForSeconds(1.5f);

        if (gameManager.questionList.Count <= 0)
        {
            yield return StartCoroutine(gameManager.GetQuestions());
        }

        GetNextQuestion();
    }

    void AnimateAnswerButtons()
    {
        questionUI.StopAllCoroutines();

        foreach (Button _button in wrongAnswers)
        {
            Animator anim = _button.GetComponentInParent<Animator>();
            _button.interactable = false;
            anim.SetTrigger("Incorrect");
        }

        correctAnswerButton.interactable = false;
        correctAnswerButton.GetComponentInParent<Animator>().SetTrigger("Correct");
    }
    
}
