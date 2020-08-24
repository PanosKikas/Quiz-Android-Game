using System.Net;
// A question object
[System.Serializable]
public class Question
{
    // the category of the question
    public Category questionCategory { get; private set; }
    public string category; // the category name

    // the category id
    public int categoryId
    {
        get
        {
            return AllCategoriesData.AllCategories[category];
        }
    }
    
    // the type of question
    public string type;

    public QuestionType TypeOfQuestion
    {
        get
        {
            return (QuestionType)System.Enum.Parse(typeof(QuestionType), type.ToString());
        }
    }
   
    // the difficulty of the question
    public string difficulty;
    
    public Difficulty QuestionDifficulty
    {
        get
        {
            return (Difficulty)System.Enum.Parse(typeof(Difficulty), difficulty);
        }
    }
    
    // The question itself
    public string question;

    // The correct answer
    public string correct_answer;
    // a list of all the incorrect answers
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

