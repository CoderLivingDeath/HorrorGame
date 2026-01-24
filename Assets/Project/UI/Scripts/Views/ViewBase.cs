using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public abstract class ViewBase : MonoBehaviour
{
    public RectTransform RectTransfrom => (RectTransform)transform;

    public Rect Rect => this.RectTransfrom.rect;
    public virtual bool Visible
    {
        get
        {
            return this.gameObject.activeSelf;
        }
        set
        {
            if (this.gameObject.activeSelf == value) return;

            this.gameObject.SetActive(value);
        }
    }

    public event Func<UniTask> VisibleChange;
    public event Func<UniTask> VisibleChanged;

    public event Func<UniTask> OnShow;
    public event Func<UniTask> OnShowed;

    public event Func<UniTask> OnHide;
    public event Func<UniTask> OnHided;

    public event Func<UniTask> OnDestroy;
    public event Func<UniTask> OnDestroyed;

    public async UniTask Show()
    {
        if (Visible) return;

        if (OnShow != null) await OnShow.Invoke();
        if (VisibleChange != null) await VisibleChange.Invoke();

        Visible = true;

        if (VisibleChanged != null) await VisibleChanged.Invoke();
        if (OnShowed != null) await OnShowed.Invoke();
    }

    public async UniTask Hide()
    {
        if (!Visible) return;

        if (OnHide != null) await OnHide.Invoke();
        if (VisibleChange != null) await VisibleChange.Invoke();
        Visible = false;
        if (VisibleChanged != null) await VisibleChanged.Invoke();
        if (OnHided != null) await OnHided.Invoke();
    }

    public async UniTask Destroy()
    {
        await OnDestroy.Invoke();
        GameObject.Destroy(this);
        await OnDestroyed.Invoke();
    }
}
