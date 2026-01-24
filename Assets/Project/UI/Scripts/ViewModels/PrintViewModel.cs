using ObservableCollections;
using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrintViewModel : ViewModelBase, IDisposable
{
    #region Properies

    public ObservableList<DocumentModel> Documents { get; private set; } = new();

    public ReactiveProperty<DocumentModel> SelectedDocument { get; set; } = new(default);
    public ReactiveProperty<string> PrintButtonText { get; } = new("Print ept");

    #endregion

    #region Commands


    public ReactiveCommand<IEnumerable<DocumentModel>> SetupDocumentsCommand { get; private set; }
    public ReactiveCommand<int> SelectDocumentCommand { get; private set; }
    public ReactiveCommand<int> GenerateRandomDocumentsCommand { get; private set; }

    #endregion

    private readonly PlayerService _playerService;

    public PrintViewModel(PlayerService playerService)
    {
        _playerService = playerService;

        SetupCommands();
    }

    private void SetupCommands()
    {
        SetupDocumentsCommand = new(SetupDocuments);
        SelectDocumentCommand = new(SelectDocument);
        GenerateRandomDocumentsCommand = new(GenerateRandomDocuments);
    }

    private void SetupDocuments(IEnumerable<DocumentModel> documents)
    {
        Documents.Clear();
        Documents.AddRange(documents);
    }

    private void SelectDocument(int documentId)
    {
        SelectedDocument.Value = Documents[documentId];
    }

    private void GenerateRandomDocuments(int length)
    {
        if (_playerService.TryGetDocumentInWork(out var inWork))
        {
            List<DocumentModel> newDocuments = new();
            for (int i = 0; i < length; i++)
            {
                var doc = GetRandomDocument();
                newDocuments.Add(doc);
            }

            int insertIndex = UnityEngine.Random.Range(0, Documents.Count + 1);
            newDocuments.Insert(insertIndex, inWork);

            Documents.Clear();

            Documents.AddRange(newDocuments);
        }
        else
        {
            Debug.LogWarning("In work document not exist");
        }
    }

    private DocumentModel GetRandomDocument()
    {
        return new DocumentModel
        {
            DocumentId = Guid.NewGuid().ToString(),
            DocumentMustBe = "Random Must Be " + Guid.NewGuid().ToString().Substring(0, 5),
            DocumentMustBeContent = "Random Content " + Guid.NewGuid().ToString().Substring(0, 10)
        };
    }

    public void Dispose()
    {

        PrintButtonText.Dispose();
    }
}
