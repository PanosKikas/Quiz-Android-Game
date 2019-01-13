// This is an object where the json category data is 
// stored when requesting categories from db

[System.Serializable]
public class TriviaCategories
{
    // a list of all fetched categories
    public Category[] trivia_categories;
}
