using UnityEngine.UI;
using UnityEngine;

// Each answer button has this component 
// which when clicked sends an event to question manager
public class OnAnswerButton : MonoBehaviour
{

	public void OnClickEvent()
    {
        Button thisButton = GetComponent<Button>();
        if (thisButton != null)
        {
            QuestionManager.Instance.ButtonClicked(thisButton);
        }
    }
}
