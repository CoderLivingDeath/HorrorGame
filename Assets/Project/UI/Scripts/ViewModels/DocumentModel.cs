using R3;
using System;
using System.Collections.Generic;
using System.Text;

public sealed class DocumentModel
{
    public string DocumentId { get => DocumentIdRP.Value; set => DocumentIdRP.Value = value; }
    public string DocumentMustBe { get => DocumentMustBeRP.Value; set => DocumentMustBeRP.Value = value; }
    public string DocumentMustBeContent { get => DocumentMustBeContentRP.Value; set => DocumentMustBeContentRP.Value = value; }

    public ReactiveProperty<string> DocumentIdRP { get; } = new(string.Empty);
    public ReactiveProperty<string> DocumentMustBeRP { get; } = new(string.Empty);
    public ReactiveProperty<string> DocumentMustBeContentRP { get; } = new(string.Empty);

    public DocumentModel()
    {
    }

    public DocumentModel(string documentId, string documentMustBe, string documentMustBeContent)
    {
        DocumentId = documentId;
        DocumentMustBe = documentMustBe;
        DocumentMustBeContent = documentMustBeContent;
    }
}
