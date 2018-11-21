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
   
    
    public Difficulty questionDifficulty { get; private set; }
    public string difficulty
    {
        get
        {
            switch (questionDifficulty)
            {
                case Difficulty.easy:
                    return "Easy";

                case Difficulty.medium:
                    return "Normal";
                case Difficulty.hard:
                    return "Hard";
                default:
                    return "";
            }
        }
        set
        {
            type = value;
            switch (type)
            {

                case "easy":
                    questionDifficulty = Difficulty.easy;
                    break;
                case "medium":
                    questionDifficulty = Difficulty.medium;
                    break;
                case "hard":
                    questionDifficulty = Difficulty.hard;
                    break;
                default:
                    break;

            }
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

