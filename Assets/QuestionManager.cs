using UnityEngine.UI;
using UnityEngine;
using System.Net;

public class QuestionManager : MonoBehaviour {

    [SerializeField]
    Text questionText;
    [SerializeField]
    Text difficultyText;
    [SerializeField]
    Text categoryText;

	// Use this for initialization
	void Start () {
        Question questionEntry = GameManager.Instance.questionList[0];
        this.questionText.text = WebUtility.HtmlDecode(questionEntry.question);
        this.difficultyText.text = "Difficulty: " + questionEntry.difficulty.ToUpper();
        this.categoryText.text = questionEntry.category;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
