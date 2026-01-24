using R3;
using System;
using UnityEngine;
using System.Collections.Generic;

public class MacWelcomeViewModel : ViewModelBase, IDisposable
{
    public ReactiveProperty<string> MailButtonText { get; } = new("Mail");
    public ReactiveProperty<string> DiskButtonText { get; } = new("Disk");
    public ReactiveProperty<string> EjectButtonText { get; } = new("Eject");

    public ReactiveProperty<ComputerModel.Disk> CurrentDisk => ComputerModel.CurrentDiskRP;

    public ReactiveCommand NavigateToDiskCommand { get; private set; }
    public ReactiveCommand NavigateToEmailCommand { get; private set; }
    public ReactiveCommand EjectDiskCommand { get; private set; }

    private ComputerModel ComputerModel;

    private ComputerManager _ComputerManager;

    public MacWelcomeViewModel(ComputerManager computerManager, ComputerModel computerModel)
    {
        _ComputerManager = computerManager;

        NavigateToDiskCommand = new ReactiveCommand(OnNavigateToDisk);
        NavigateToEmailCommand = new ReactiveCommand(OnNavigateToEmail);
        EjectDiskCommand = new ReactiveCommand(OnEjectDisk);
        ComputerModel = computerModel;
    }

    private void OnNavigateToDisk(R3.Unit unit)
    {
        // TODO: Implement navigation to disk view
        Debug.Log("Navigate to disk - placeholder");
    }

    private void OnNavigateToEmail(R3.Unit unit)
    {
        // TODO: Implement navigation to email view
        Debug.Log("Navigate to email - placeholder");
    }

    private void OnEjectDisk(R3.Unit unit)
    {
        // TODO: Implement disk ejection logic
        Debug.Log("Eject disk - placeholder");
    }

    public void Dispose()
    {
        MailButtonText.Dispose();
        DiskButtonText.Dispose();
        EjectButtonText.Dispose();
        NavigateToDiskCommand.Dispose();
        NavigateToEmailCommand.Dispose();
        EjectDiskCommand.Dispose();
    }
}

