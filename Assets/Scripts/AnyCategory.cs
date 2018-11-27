using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnyCategory : MonoBehaviour
{
    
	public void OnValueChanged()
    {
        Toggle toggle = GetComponent<Toggle>();
        
        if(toggle.isOn)
        {
            Toggle[] toggles = gameObject.transform.parent.GetComponentsInChildren<Toggle>();
            for(int i = 0; i<toggles.Length;++i)
            {
                if(toggles[i] != null && toggles[i] != toggle)
                {
                    toggles[i].isOn = true;
                }
            }
        }
    }
}
