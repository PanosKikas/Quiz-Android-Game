using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScreenButtons : MonoBehaviour
{
    public void Play()
    {
        GameManager.Instance.ToCategorySelect();
    }
}
