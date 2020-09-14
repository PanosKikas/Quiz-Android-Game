using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraAdTest : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("Main Interstitial ad test");
        AdManager.Instance.ShowInterstial();
    }
}
