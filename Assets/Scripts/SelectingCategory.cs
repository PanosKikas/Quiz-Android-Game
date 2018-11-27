using UnityEngine;
using UnityEngine.UI;

public class SelectingCategory : MonoBehaviour
{
    Toggle anyCategoryToggle;
    
    private void Start()
    {
        Transform parentGM = gameObject.transform.parent;
        anyCategoryToggle = 
            parentGM.gameObject.transform.parent.transform.parent.gameObject.GetComponent<CategoryManager>().anyCategoryToggle;

    }

    public void OnValueChanged()
    {
        if(!GetComponent<Toggle>().isOn)
        {
            anyCategoryToggle.isOn = false;
        }
    }
}
