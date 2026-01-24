using R3;
using System;

namespace Assets.Project.UI.Scripts.ViewModels
{
    public class PrintSecondViewModel : ViewModelBase, IDisposable
    {
        public ReactiveProperty<string> IdTitle { get; } = new("ID");
        public ReactiveProperty<string> IdContent { get; } = new("Content");

        public ReactiveProperty<string> MustBeTitle { get; } = new("Must be");
        public ReactiveProperty<string> MustBeContent { get; } = new("Content");

        public ReactiveProperty<string> MustBeExtraTitle { get; } = new("Extra");
        public ReactiveProperty<string> MustBeExtraContent { get; } = new("Content");


        public ReactiveCommand<DocumentModel> SetModelCommand { get; private set; }

        private DocumentModel _CurrentDocumentModel;

        private readonly PlayerService _playerService;

        public PrintSecondViewModel(PlayerService playerService)
        {
            _playerService = playerService;

            SetModelCommand = new(SetModel);
        }

        private void SetModel(DocumentModel model)
        {
            IdTitle.Value = "ID";
            IdContent.Value = model.DocumentId;

            MustBeTitle.Value = "Must Be";
            MustBeContent.Value = model.DocumentMustBe;

            MustBeExtraTitle.Value = "Content";
            MustBeExtraContent.Value = model.DocumentMustBeContent;
        }

        public void Dispose()
        {
            IdTitle.Dispose();
            IdContent.Dispose();
            MustBeTitle.Dispose();
            MustBeContent.Dispose();
            MustBeExtraTitle.Dispose();
            MustBeExtraContent.Dispose();
            SetModelCommand.Dispose();
        }
    }
}
