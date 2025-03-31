using UnityEngine;
using DFDev.UI;

[ExecuteAlways]
public class ButtonClickPlayAudio : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private string _audioName = "ButtonClick";
    
    private void Awake()
    {
        if (!_button) 
            _button = GetComponent<Button>();
        _button.AddClick(ClickPlayAudio);
    }

    private void ClickPlayAudio()
    {
        AudioManager.PlaySound(_audioName);
    }
}
