using Cysharp.Threading.Tasks;
using R3;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MacWelcomeView : ViewBase
{
    [SerializeField]
    private Button _MailButton;

    [SerializeField]
    private Button _DiskButton;

    [SerializeField]
    private Button _EjectButton;

    [Inject]
    private MacWelcomeViewModel ViewModel;

    void Start()
    {
        if (ViewModel != null)
        {
            ViewModel.MailButtonText.Subscribe(text => _MailButton.GetComponentInChildren<TextMeshProUGUI>().text = text).AddTo(this);
            ViewModel.DiskButtonText.Subscribe(text => _DiskButton.GetComponentInChildren<TextMeshProUGUI>().text = text).AddTo(this);
            ViewModel.EjectButtonText.Subscribe(text => _EjectButton.GetComponentInChildren<TextMeshProUGUI>().text = text).AddTo(this);

            _DiskButton.onClick.AddListener(() => ViewModel.NavigateToDiskCommand.Execute(default));
            _MailButton.onClick.AddListener(() => ViewModel.NavigateToEmailCommand.Execute(default));
            _EjectButton.onClick.AddListener(() => ViewModel.EjectDiskCommand.Execute(default));
        }
    }
}
