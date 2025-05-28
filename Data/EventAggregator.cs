using System;
using System.Collections.Generic;

namespace Travel_Company.WPF.Data;

public class EventAggregator
{
    private readonly Dictionary<Type, List<Action<object>>> _eventSubscribers;
    private readonly Dictionary<Type, object> _lastPublishedMessage;

    public EventAggregator()
    {
        _eventSubscribers = new Dictionary<Type, List<Action<object>>>();
        _lastPublishedMessage = new Dictionary<Type, object>();
    }

    public void Publish<TMessage>(TMessage message)
    {
        var messageType = typeof(TMessage);

        if (_eventSubscribers.ContainsKey(messageType))
        {
            var allSubscribers = _eventSubscribers[messageType];
            foreach (var subscriber in allSubscribers)
            {
                subscriber(message);
            }
        }

        _lastPublishedMessage[messageType] = message;
    }

    public void Subscribe<TMessage>(Action<TMessage> action)
    {
        var messageType = typeof(TMessage);

        if (!_eventSubscribers.ContainsKey(messageType))
        {
            _eventSubscribers[messageType] = new List<Action<object>>();
        }

        _eventSubscribers[messageType].Add(m => action((TMessage)m));

        if (_lastPublishedMessage.ContainsKey(messageType))
        {
            action((TMessage)_lastPublishedMessage[messageType]);
        }
    }

    public void RemoveMessage<TMessage>()
    {
        var messageType = typeof(TMessage);

        if (_lastPublishedMessage.ContainsKey(messageType))
        {
            _lastPublishedMessage.Remove(messageType);
        }
    }

}