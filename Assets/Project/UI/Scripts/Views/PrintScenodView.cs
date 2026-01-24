using Assets.Project.UI.Scripts.ViewModels;
using Reflex.Attributes;
using R3;
using TMPro;
using UnityEngine;

public class PrintScenodView : ViewBase
{
    public TextMeshProUGUI IdTitle;
    public TextMeshProUGUI IdContent;

    public TextMeshProUGUI MustBeTitle;
    public TextMeshProUGUI MustBeContent;

    public TextMeshProUGUI MustBeExtraTitle;
    public TextMeshProUGUI MustBeExtraContent;

    [Inject]
    private PrintSecondViewModel ViewModel;

    void Start()
    {
        if (ViewModel != null)
        {
            ViewModel.IdTitle.Subscribe(text => IdTitle.text = text).AddTo(this);
            ViewModel.IdContent.Subscribe(text => IdContent.text = text).AddTo(this);

            ViewModel.MustBeTitle.Subscribe(text => MustBeTitle.text = text).AddTo(this);
            ViewModel.MustBeContent.Subscribe(text => MustBeContent.text = text).AddTo(this);

            ViewModel.MustBeExtraTitle.Subscribe(text => MustBeExtraTitle.text = text).AddTo(this);
            ViewModel.MustBeExtraContent.Subscribe(text => MustBeExtraContent.text = text).AddTo(this);
        }
    }
}

