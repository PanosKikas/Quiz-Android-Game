using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.CodeDom.Compiler;
using System;
// Manages the functionality of the category select menu
// Selecting/ Deselecting categories and starting the game
public class CategorySelect : MonoBehaviour
{
    

    [Header("AudiClips")]  
    [SerializeField]
    AudioClip ToggleCategoryClip;

    [Space]
    // The gameobject to which the category buttons 
    // will be stored
    [SerializeField]
    Transform categoryParent;
    

    
    // A list with all the category toggles
    Toggle[] categoryToggles;


    List<int> selectedCategories;   
    
    void Start ()
    {
        Initialize();
        AudioManager.Instance.EnableBGMusic();
        DeselectAllCategories();
    }

    void Initialize()
    {
        GatherCategories();
    }
    
    
    void GatherCategories()
    {
        categoryToggles = categoryParent.GetComponentsInChildren<Toggle>();
    }

        

    public List<int> FindSelectedCategories()
    {
        selectedCategories = new List<int>();

        foreach (Toggle toggle in categoryToggles)
        {
            if (toggle.isOn) 
            {
                AddCategoryToSelectedList(toggle);
            }
        }
        return selectedCategories;
    }

    
    void AddCategoryToSelectedList(Toggle categoryToggle)
    {
            CategoryName categoryName = (CategoryName)Enum.Parse(typeof(CategoryName), categoryToggle.gameObject.name, true);
            List<int> categoryIds = AllCategoriesData.AllCategories[categoryName];
            foreach (int id in categoryIds)
            {
                selectedCategories.Add(id);
            }
    
    }

    public void SelectAllCategories()
    {
        foreach (Toggle toggle in categoryToggles)
        {
            if(!toggle.isOn)
            {
                toggle.isOn = true;
            }
        }
    }

    public void DeselectAllCategories()
    {
        foreach (Toggle toggle in categoryToggles)
        {
            if (toggle.isOn)
            {
                toggle.isOn = false;
            }
        }
    }

    public void OnToggleClicked(bool val)
    {
        AudioManager.Instance.PlayAudioClip(ToggleCategoryClip);
        
    }
}
