using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class OnButtonSubmit : MonoBehaviour
{
    StartScreenButtons startScreen;
    InputField[] inputFields;
    Dropdown[] dropdowns;

    private void OnEnable()
    {
        if (startScreen == null)
        {
            startScreen = GetComponentInParent<StartScreenButtons>();
        }

        InitializeUI();
    }

    private void Start()
    {
        AdManager.Instance.ShowInterstial();
    }

    public void Submit()
    {
        bool success = CheckInputFields();
        if (success)
        {

            StartCoroutine(startScreen.DisplayPopup("Question was submitted successfully!"));
            InitializeUI();
            startScreen.ToggleQuestionAddPanel();
        }
        else
        {
            StartCoroutine(startScreen.DisplayPopup("Please fill all the fields and try again"));
        }
    }

    bool CheckInputFields()
    {
        FindInputFields();

        foreach (InputField inputField in inputFields)
        {
            if (!CheckField(inputField))
            {
                return false;
            }
        }

        return true;
    }

    void FindInputFields()
    {
        inputFields = GetComponentsInChildren<InputField>();
    }

    bool CheckField(InputField field)
    {
        if (string.IsNullOrEmpty(field.text))
        {
            
            return false;
        }
        return true;
    }

    void InitializeUI()
    {
        InitializeDropdowns();
        InitializeInputFields();
    }

    void InitializeDropdowns()
    {
        if (dropdowns == null)
        {
            dropdowns = GetComponentsInChildren<Dropdown>();
        }

        foreach (var dropdown in dropdowns)
        {
            dropdown.value = 0;
        }
    }

    void InitializeInputFields()
    {
        foreach (var inputField in GetComponentsInChildren<InputField>())
        {
            inputField.text = "";
        }
    }
}
