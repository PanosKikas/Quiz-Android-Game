using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintManager : MonoBehaviour
{
    [SerializeField]
    string[] HintsArray;

    [SerializeField]
    Text HintText;

    private void Start()
    {
        var hint = PickRandomHint();
        DisplayHint(hint);
    }

    string PickRandomHint()
    {
        var randomInt = Random.Range(0, HintsArray.Length);
        return HintsArray[randomInt];
    }

    void DisplayHint(string hint)
    {
        HintText.text = hint;
    }
}
