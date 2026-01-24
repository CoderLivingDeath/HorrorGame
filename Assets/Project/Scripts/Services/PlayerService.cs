public class PlayerService
{
    private DocumentModel inWork = null;

    public void SetDocumentToWork(DocumentModel document)
    {
        inWork = document;
    }

    public bool TryGetDocumentInWork(out DocumentModel document)
    {
        //document = null;

        //if (inWork == null)
        //    return false;
        //else
        //    document = inWork;
        //return true;

        document = new()
        {
            DocumentId = "ID",
            DocumentMustBe = "Must be",
            DocumentMustBeContent = "Content"
        };

        return true;
    }
}
