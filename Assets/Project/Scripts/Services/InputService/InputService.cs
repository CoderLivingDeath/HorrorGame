using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputService : IDisposable
{
    private readonly InputActionAsset _asset;
    private readonly InputSubscriberContainer _inputSubscribers;

    public InputService(InputActionAsset asset)
    {
        _asset = asset ?? throw new ArgumentNullException(nameof(asset));
        _inputSubscribers = new InputSubscriberContainer();
    }

    public InputActionMap FindMap(string key)
    {
        InputActionMap actionMap = _asset.FindActionMap(key) ?? throw new ArgumentException($"Action map '{key}' not found.");

        return actionMap;
    }

    public InputAction FindAction(string map, string action)
    {
        InputActionMap actionMap = _asset.FindActionMap(map) ?? throw new ArgumentException($"Action map '{map}' not found.");
        InputAction inputAction = actionMap.FindAction(action) ?? throw new ArgumentException($"Action '{action}' not found in map '{actionMap.name}'.");

        return inputAction;
    }

    public InputAction FindAction(string key, InputActionMap actionMap)
    {
        InputAction inputAction = actionMap.FindAction(key) ?? throw new ArgumentException($"Action '{key}' not found in map '{actionMap.name}'.");

        return inputAction;
    }

    public void Subscribe(ActionPath path, Action<InputAction.CallbackContext> action, InputActionType type = InputActionType.Performed)
    {
        if (action == null) throw new ArgumentNullException(nameof(action));

        var inputActionMap = FindMap(path.Map);
        var inputAction = FindAction(path.Action, inputActionMap);

        var subscriber = new InputSubscriber(inputAction, action, type);
        _inputSubscribers.Add(path, subscriber);
        subscriber.Enable();
    }

    public void Unsubscribe(ActionPath path)
    {
        _inputSubscribers.Remove(path);
    }

    public void Unsubscribe(ActionPath path, InputActionType type)
    {
        _inputSubscribers.Remove(path, type);
    }

    public void DisableAll()
    {
        _asset.Disable();
    }

    public void EnableAll()
    {
        _asset.Enable();
    }

    public void DisableMap(string map)
    {
        if (string.IsNullOrEmpty(map)) throw new ArgumentNullException(nameof(map));

        var actionMap = _asset.FindActionMap(map) ?? throw new ArgumentException($"Action map '{map}' not found.");
        actionMap.Disable();
    }

    public void EnableMap(string map)
    {
        if (string.IsNullOrEmpty(map)) throw new ArgumentNullException(nameof(map));

        var actionMap = _asset.FindActionMap(map) ?? throw new ArgumentException($"Action map '{map}' not found.");
        actionMap.Enable();
    }

    public void DisableAction(string map, string action)
    {
        if (string.IsNullOrEmpty(map)) throw new ArgumentNullException(nameof(map));
        if (string.IsNullOrEmpty(action)) throw new ArgumentNullException(nameof(action));

        var actionMap = _asset.FindActionMap(map) ?? throw new ArgumentException($"Action map '{map}' not found.");
        var inputAction = actionMap.FindAction(action) ?? throw new ArgumentException($"Action '{action}' not found in map '{map}'.");

        inputAction.Disable();
    }

    public void EnableAction(string map, string action)
    {
        if (string.IsNullOrEmpty(map)) throw new ArgumentNullException(nameof(map));
        if (string.IsNullOrEmpty(action)) throw new ArgumentNullException(nameof(action));

        var actionMap = _asset.FindActionMap(map) ?? throw new ArgumentException($"Action map '{map}' not found.");
        var inputAction = actionMap.FindAction(action) ?? throw new ArgumentException($"Action '{action}' not found in map '{map}'.");

        inputAction.Enable();
    }

    public void Dispose()
    {
        _inputSubscribers.Dispose();
    }
}

public class InputSubscriberContainer : IDisposable
{
    private Dictionary<ActionPath, InputSubscriber> _subscribers
        = new Dictionary<ActionPath, InputSubscriber>();

    public IReadOnlyDictionary<ActionPath, InputSubscriber> Subscribers => _subscribers;

    public void Add(ActionPath path, InputSubscriber subscriber)
    {
        if (subscriber == null) throw new ArgumentNullException(nameof(subscriber));
        var key = path;

        if (_subscribers.ContainsKey(key))
        {
            throw new InvalidOperationException($"Subscriber for {path} and {subscriber.Type} already exists.");
        }

        _subscribers[key] = subscriber;
    }

    public void Remove(ActionPath path)
    {
        foreach (var key in _subscribers.Keys)
        {
            _subscribers[key].Dispose();
            _subscribers.Remove(key);
        }
    }

    public void Remove(ActionPath path, InputActionType type)
    {
        var key = path;
        if (_subscribers.TryGetValue(key, out var subscriber))
        {
            subscriber.Dispose();
            _subscribers.Remove(key);
        }
    }

    public IEnumerable<InputSubscriber> GetAllSubscribers()
    {
        return _subscribers.Values;
    }

    public IEnumerable<KeyValuePair<ActionPath, InputSubscriber>> GetAllSubscribersMap()
    {
        return _subscribers.Select(kvp => KeyValuePair.Create(kvp.Key, kvp.Value));
    }

    public InputSubscriber FindSubscriber(ActionPath path, InputActionType? type = null)
    {
        if (type.HasValue)
        {
            return _subscribers.GetValueOrDefault(path);
        }
        else
        {
            return _subscribers.FirstOrDefault(kvp => kvp.Key == path).Value;
        }
    }

    public void Dispose()
    {
        foreach (var subscriber in _subscribers.Values)
        {
            subscriber.Dispose();
        }
        _subscribers.Clear();
    }
}

[Flags]
public enum InputActionType
{
    Started = 1,
    Performed = 2,
    Canceled = 4
}


public sealed class InputSubscriber : IDisposable
{
    public readonly InputAction Action;
    public readonly Action<InputAction.CallbackContext> Callback;
    public readonly InputActionType Type;

    private bool disposed = false;

    public InputSubscriber(InputAction action, Action<InputAction.CallbackContext> callback, InputActionType type)
    {
        Action = action ?? throw new ArgumentNullException(nameof(action));
        Callback = callback ?? throw new ArgumentNullException(nameof(callback));
        Type = type;

        if (Type.HasFlag(InputActionType.Started))
            Action.started += Callback;

        if (Type.HasFlag(InputActionType.Performed))
            Action.performed += Callback;

        if (Type.HasFlag(InputActionType.Canceled))
            Action.canceled += Callback;
    }

    public void Enable()
    {
        if (disposed) throw new ObjectDisposedException(nameof(InputSubscriber));
        Action?.Enable();
    }

    public void Disable()
    {
        if (disposed) throw new ObjectDisposedException(nameof(InputSubscriber));
        Action?.Disable();
    }

    public void Dispose()
    {
        Dispose(true);
    }

    public void Dispose(bool disposing)
    {
        if (disposed) return;

        if (disposing)
        {
            if (Type.HasFlag(InputActionType.Started))
                Action.started -= Callback;

            if (Type.HasFlag(InputActionType.Performed))
                Action.performed -= Callback;

            if (Type.HasFlag(InputActionType.Canceled))
                Action.canceled -= Callback;
        }

        disposed = true;
    }

}

public readonly struct ActionPath
{
    public readonly string Map;
    public readonly string Action;

    public ActionPath(string map, string action)
    {
        Map = map;
        Action = action;
    }

    public override string ToString()
    {
        return $"{Map}/{Action}";
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Map, Action);
    }

    public override bool Equals(object obj)
    {
        return obj is ActionPath other &&
               Map == other.Map &&
               Action == other.Action;
    }

    public static bool operator ==(ActionPath left, ActionPath right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ActionPath left, ActionPath right)
    {
        return !(left == right);
    }
}