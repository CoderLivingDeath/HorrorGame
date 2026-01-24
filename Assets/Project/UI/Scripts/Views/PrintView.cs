using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrintView : ViewBase
{
    [SerializeField]
    private Button[] _DocumentsButton;

    [SerializeField]
    private Button _PrintButton;

    [SerializeField]
    private Button _SelectedDocumentButton;

    [Inject]
    private PrintViewModel ViewModel;

    // TODO: сделать отдельные методы для биндинга VM
    void Start()
    {
        if (ViewModel != null)
        {
            ViewModel.GenerateRandomDocumentsCommand.Execute(_DocumentsButton.Length - 1);
            ViewModel.PrintButtonText.Subscribe(text => _PrintButton.GetComponentInChildren<TextMeshProUGUI>().text = text).AddTo(this);
            ViewModel.Documents.CollectionChanged += Documents_CollectionChanged;
        }

        int length = Mathf.Min(ViewModel.Documents.Count, _DocumentsButton.Length);

        for (int i = 0; i < length; i++)
        {
            var button = _DocumentsButton[i];
            button.GetComponentInChildren<TextMeshProUGUI>().text = ViewModel.Documents[i].DocumentId;

            int index = i;
            button.onClick.AddListener(() =>
            {
                ViewModel.SelectDocumentCommand.Execute(index);
            });
        }

        ViewModel.SelectedDocument.Subscribe((doc) => _SelectedDocumentButton.GetComponentInChildren<TextMeshProUGUI>().text = doc?.DocumentId);
    }

    private void Documents_CollectionChanged(in NotifyCollectionChangedEventArgs<DocumentModel> e)
    {
        for (int i = 0; i < ViewModel.Documents.Count && i < _DocumentsButton.Length; i++)
        {
            _DocumentsButton[i].GetComponentInChildren<TextMeshProUGUI>().text = ViewModel.Documents[i].DocumentId;
        }
    }
}