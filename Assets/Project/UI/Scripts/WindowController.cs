using Cysharp.Threading.Tasks;
using ObservableCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WindowController : MonoBehaviour
{
    [Serializable]
    public struct ViewWrap
    {
        public string Key;
        public ViewBase View;
    }

    [SerializeField]
    private List<ViewWrap> _Views;

    private Dictionary<string, ViewBase> _viewMap;

    private void Start()
    {
        _viewMap = _Views.ToDictionary((k) => k.Key, (v) => v.View);
    }

    public async UniTask<ViewBase> ShowView(string key)
    {
        if (_viewMap.TryGetValue(key, out ViewBase view))
        {

            var tasks = _viewMap.Values
                .Where(item => item != view)
                .Select(item => item.Hide());

            await UniTask.WhenAll(tasks);
            await view.Show();

            return view;
        }
        else
        {
            Debug.LogError($"Not exist {key}");
            throw new ArgumentException();
        }
    }
}