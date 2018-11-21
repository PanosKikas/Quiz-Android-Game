using System.Net;

[System.Serializable]
public class Question
{
    public Category questionCategory { get; private set; }
    public string category;

    public int categoryId
    {
        get
        {
            return GameManager.AllCategories[category];
        }
    }
    
    public string type;

    public QuestionType TypeOfQuestion
    {
        get
        {
            return (QuestionType)System.Enum.Parse(typeof(QuestionType), type.ToString());
        }
    }
   
    public string difficulty;
    
    public Difficulty QuestionDifficulty
    {
        get
        {
            return (Difficulty)System.Enum.Parse(typeof(Difficulty), difficulty);
        }
    }
    
    public string question;

    public string correct_answer;
    public string[] incorrect_answers;

    public override string ToString()
    {
        return "Category: " + category + " Type: " + type +
            " Difficulty: " + difficulty + " Question: " + WebUtility.HtmlDecode(question)
            + " Correct Answer: " + correct_answer 
            + " Incorrect answers: " + string.Join(" ", incorrect_answers) + "\n";       
    }

}

public enum QuestionType
{
    multiple,
    boolean
    
};

public enum Difficulty
{
     easy,
     medium,
     hard
};

