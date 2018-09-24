using UnityEngine;
using UnityEngine.UI;

// this is a UI component that can show a message, icon and button
[RequireComponent(typeof(SliderUI))]
public class MessageWindowUI : MonoBehaviour 
{

	[SerializeField] private Image _messageIcon;
	[SerializeField] private Text _messageText;
	[SerializeField] private Text _buttonText;

    ///<summary>Shows text and icon in message widnow</summary>
    public void ShowMessage(Sprite sprite = null, string message = "", string buttonMsg = "START")
	{
		if (_messageIcon != null) 
		{
			_messageIcon.sprite = sprite;
		}

        if (_messageText != null)
        {
            _messageText.text = message;
        }
			
        if (_buttonText != null)
        {
            _buttonText.text = buttonMsg;
        }
	}
}
