using R3;
using System;
using System.Collections.Generic;
using System.Text;

public class ComputerModel
{
    public enum Disk
    {
        None, Print, EmailSend, DataMatch, Shreder, FactChecking
    }

    public Disk CurrentDisk { get => CurrentDiskRP.Value; set => CurrentDiskRP.Value = value; }

    public ReactiveProperty<Disk> CurrentDiskRP { get;} = new(Disk.None);
}
