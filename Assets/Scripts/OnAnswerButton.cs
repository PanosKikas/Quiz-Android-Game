using UnityEngine.UI;
using UnityEngine;

public class OnAnswerButton : MonoBehaviour {

	public void OnClickEvent()
    {
        Button thisButton = GetComponent<Button>();
        if (thisButton != null)
        {
            QuestionManager.Instance.OnAnswerClicked(thisButton);
        }
    }
}
