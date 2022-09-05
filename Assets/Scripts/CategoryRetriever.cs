using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class CategoryRetriever : MonoBehaviour
{
    /*TriviaCategories catData = null;
    // a url that retrieves all the categories in the database
    private const string categoriesRetrieveUrl = "https://opentdb.com/api_category.php";

    public IEnumerator Retrieve()
    {
        yield return RequestCategories();

        // has been initialized
        if (catData != null)
        {
            FormatCategoryData();
        }
    }

    IEnumerator RequestCategories()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(categoriesRetrieveUrl))
        {
            yield return request.SendWebRequest();

            // error
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError(request.error);

            }
            else
            {
                DeserilizeTriviaCategories(request);
            }

        }
    }

    private void DeserilizeTriviaCategories(UnityWebRequest request)
    {
        string retrievedData = request.downloadHandler.text;     // get the text as string
        catData = JsonUtility.FromJson<TriviaCategories>(retrievedData); // deserialize the json to a TriviaCategories object
    }


    private void FormatCategoryData()
    {
        foreach (Category category in catData.trivia_categories)
        {
            List<int> ids = 
            
            //string formattedName = RemoveCategoryPrefix(category.name);

            AllCategoriesData.AddCategory(formattedName, category.id);
        }
    }
    */
}
