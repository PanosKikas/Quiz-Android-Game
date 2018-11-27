using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CategoryManager : MonoBehaviour
{
    [SerializeField]
    private const int minimumSelectedCategories = 3;

    [SerializeField]
    GameObject categoryButtonPrefab;

    [SerializeField]
    Transform categoryParent;

    [SerializeField]
    GameObject anyCategoryPrefab;

    public Sprite[] btnSprites;

    
    public Toggle anyCategoryToggle;

   
    // Use this for initialization
    void Start ()
    {
        SetUpCategoryButtons();
	}

    // A method that retrieves all the availiable categories from 
    // the trivia api and uses them to initialize a dictionary <name, id>
    private void SetUpCategoryButtons()
    {
        foreach (var entry in GameManager.Instance.AllCategoriesDictionary)
        { 
            Toggle obj = ObjectPooler.GetInstance(categoryButtonPrefab,categoryParent).GetComponent<Toggle>();

            if (obj != null)
            {
                Text buttText = obj.GetComponentInChildren<Text>();
                Image img = obj.GetComponent<Image>();
                img.sprite = btnSprites[Random.Range(0, btnSprites.Length)];
                // Remove Header for example Entertainment: Something
                buttText.text = entry.Key;
            }
        }
        //StartCoroutine(GetQuestions(testQuestUrl));
    }

    public void StartGame()
    {
        Toggle[] categoryToggles = GetComponentsInChildren<Toggle>();
        List<Category> selectedCategories = new List<Category>();
        if(anyCategoryToggle.isOn)
        {
            selectedCategories = GameManager.Instance.AllCategoriesDictionary.Values.ToList();
        }
        else
        {
            foreach (Toggle toggle in categoryToggles)
            {
                if (toggle.isOn)
                {
                    Text catText = toggle.gameObject.GetComponentInChildren<Text>();
                    if (catText != null)
                    {
                        Category category = GameManager.Instance.AllCategoriesDictionary[catText.text];
                        //Debug.Log("Selected Category: ID: " + idSelected + " NAME: " + catText.text);
                        
                        selectedCategories.Add(category);
                    }
                }
            }
        }
        
        if (selectedCategories.Count >= minimumSelectedCategories)
        {
            foreach (Category category in selectedCategories)
            {
                Debug.Log("ID: " + category.id + " NAME: " + category.name + "\n");
            }
            Debug.Log("Starting Game");
        }
        else
        {
            Debug.Log("You have to select at least 3 categories");
        }
    }


}
