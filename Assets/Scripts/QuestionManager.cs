using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Linq;

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
    Text questionText;
    [SerializeField]
    Text difficultyText;
    [SerializeField]
    Text categoryText;

    [SerializeField]
    Transform MultipleAnswersPanel;

    [SerializeField]
    GameObject multipleAnswerButton;

    [SerializeField]
    Transform trueFalsePanel;

    [SerializeField]
    GameObject booleanButtonPrefab;

    GameManager gameManager;
    List<Question> QuestionList;

    Question currentQuestion;

    private List<Button> wrongAnswers;
    private Button correctAnswerButton;


    
	// Use this for initialization
	void Start ()
    {
        gameManager = GameManager.Instance;
        QuestionList = gameManager.questionList;
        wrongAnswers = new List<Button>();
        GetNextQuestion();
    }
	
    void GetNextQuestion()
    {
        // Remove Previous Answer Buttons
        ClearPreviousAnswers();

        // Get random question and remove it from list
        int randomQuestionIndex = Random.Range(0, QuestionList.Count);
        currentQuestion = QuestionList[randomQuestionIndex];
        QuestionList.RemoveAt(randomQuestionIndex);
        
        List<string> answers = currentQuestion.incorrect_answers.ToList<string>();
        
        answers.Add(currentQuestion.correct_answer);
        
        for (int i = 0; i < currentQuestion.incorrect_answers.Length +1; i++)
        {
            int randIndex = Random.Range(0, answers.Count);
            Button answerButton;
            if(currentQuestion.TypeOfQuestion == QuestionType.boolean )
            {
                answerButton = Instantiate(booleanButtonPrefab, trueFalsePanel).GetComponent<Button>();
            }
            else
            {
                answerButton = Instantiate(multipleAnswerButton, MultipleAnswersPanel).GetComponent<Button>();
            }
            
            
            Text _text = answerButton.GetComponentInChildren<Text>();
            string currentAnswer = answers.ElementAt(randIndex);
            if(currentAnswer.Equals(currentQuestion.correct_answer))
            {
                correctAnswerButton = answerButton;
            }
            else
            {
                wrongAnswers.Add(answerButton);
            }

            _text.text = WebUtility.HtmlDecode(answers.ElementAt(randIndex));
            answers.RemoveAt(randIndex);
        }

        Debug.Log(currentQuestion.ToString());
        
        SetQuestionUI();
        
    }

    void ClearPreviousAnswers()
    {      
        foreach (Button answer in wrongAnswers)
        {
            Destroy(answer.gameObject);
        }
            

        if(correctAnswerButton != null)
        {
            Destroy(correctAnswerButton.gameObject);
        }
        wrongAnswers.Clear();
    }
        

    void SetQuestionUI()
    {
        // Set the UI
        this.questionText.text = WebUtility.HtmlDecode(currentQuestion.question);
        this.difficultyText.text = "Difficulty: " + currentQuestion.difficulty.ToUpper();
        this.categoryText.text = currentQuestion.category;
    }

    public void OnAnswerClicked(Button button)
    {
       // Text text = button.GetComponentInChildren<Text>();
        //string correctAnswer = WebUtility.HtmlDecode(currentQuestion.correct_answer);
        if(button != correctAnswerButton)
        {
            Debug.Log("Wrong Answer...");
        }
        else
        {
            Debug.Log("Correct Answer!");
        }

        GetNextQuestion();
    }

    

}
