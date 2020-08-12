using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile;

public class EasyMobileInitializer : MonoBehaviour
{
    private void Awake()
    {
        if (!RuntimeManager.IsInitialized())
        {
            RuntimeManager.Init();
        }
    }
}
