using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupDisplayer : MonoBehaviour
{
    [SerializeField]
    GameObject PopupGUI;

    public void DisplayMessagePopup(string message)
    {
        StartCoroutine(DisplayPopup(message));
    }

    IEnumerator DisplayPopup(string popuptext)
    {
        PopupGUI.GetComponentInChildren<Text>().text = popuptext;
        PopupGUI.SetActive(true);
        yield return new WaitForSeconds(2f);
        PopupGUI.SetActive(false);
    }
}
