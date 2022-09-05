using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeToggler : MonoBehaviour
{
    [SerializeField]
    GameObject[] answerPanels = new GameObject[2];

    public void Toggle(int i)
    {
        if (!answerPanels[i].activeSelf)
        {
            answerPanels[i].SetActive(true);
            answerPanels[(i + 1) % 2].SetActive(false);
        }
    }
}
