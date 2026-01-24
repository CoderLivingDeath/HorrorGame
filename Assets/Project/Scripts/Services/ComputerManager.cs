using Cysharp.Threading.Tasks;
using UnityEngine;

public class ComputerManager : MonoBehaviour
{
    [SerializeField]
    private WindowController CoreComputer;
    [SerializeField]
    private WindowController SecondComputer;

    public async UniTaskVoid ShowWelcomeView()
    {
        await UniTask.WhenAll(CoreComputer.ShowView("Welcome"), SecondComputer.ShowView("Welcome"));
    }

    public async UniTaskVoid ShowPrint()
    {
        await UniTask.WhenAll(CoreComputer.ShowView("Print"), SecondComputer.ShowView("Print"));
    }

    [ContextMenu("TestShowWelcome")]
    public void TestShowWelcome()
    {
        ShowWelcomeView().Forget();
    }

    [ContextMenu("TestShowPrint")]
    public void TestShowPrint()
    {
        ShowPrint().Forget();
    }
}