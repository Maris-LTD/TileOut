namespace GameModules.Systems.Events
{
    using System;
    using System.Collections.Generic;
    using MessagePipe;
    using VContainer;
    using UnityEngine;

    public interface IEventBus    
    {
        void Subscribe<T>(Action<T> message);
        void Unsubscribe<T>(Action<T> handler);
        void Publish<T>(T eventData);
        void ClearAll();
    }

    public interface IGlobalEventBus : IEventBus
    {
    }

    internal readonly struct SubscriptionKey : IEquatable<SubscriptionKey>
    {
        private readonly Type _type;
        private readonly int _handlerHashCode;

        public SubscriptionKey(Type type, Delegate handler)
        {
            _type = type;
            _handlerHashCode = handler?.GetHashCode() ?? 0;
        }

        public bool Equals(SubscriptionKey other)
        {
            return _type == other._type && _handlerHashCode == other._handlerHashCode;
        }

        public override bool Equals(object obj)
        {
            return obj is SubscriptionKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_type, _handlerHashCode);
        }
    }

    internal static class HandlerPool<T>
    {
        private static readonly Stack<CachedMessageHandler<T>> _pool = new Stack<CachedMessageHandler<T>>();
        private static readonly Dictionary<Action<T>, CachedMessageHandler<T>> _activeHandlers = new Dictionary<Action<T>, CachedMessageHandler<T>>();

        public static CachedMessageHandler<T> Rent(Action<T> handler)
        {
            if (_activeHandlers.TryGetValue(handler, out var existing))
                return existing;

            CachedMessageHandler<T> wrapper;
            if (_pool.Count > 0)
            {
                wrapper = _pool.Pop();
                wrapper.Reset(handler);
            }
            else
            {
                wrapper = new CachedMessageHandler<T>(handler);
            }

            _activeHandlers[handler] = wrapper;
            return wrapper;
        }

        public static void Return(Action<T> handler)
        {
            if (_activeHandlers.Remove(handler, out var wrapper))
            {
                wrapper.Reset(null);
                _pool.Push(wrapper);
            }
        }
    }

    internal class CachedMessageHandler<T> : IMessageHandler<T>
    {
        private Action<T> _handler;

        public CachedMessageHandler(Action<T> handler)
        {
            _handler = handler;
        }

        public void Reset(Action<T> handler)
        {
            _handler = handler;
        }

        public void Handle(T message)
        {
            _handler?.Invoke(message);
        }
    }
        
    public class EventBus : IEventBus, IGlobalEventBus
    {
        private readonly IObjectResolver _container;
        private readonly Dictionary<SubscriptionKey, IDisposable> _subscriptions = new();
        private readonly Dictionary<Type, object> _subscriberCache = new();
        private readonly Dictionary<Type, object> _publisherCache = new();

        [Inject]
        public EventBus(IObjectResolver container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }
        
        public void Publish<T>(T eventData)
        {
            try
            {
                if (_container == null)
                {
                    Debug.LogError("[EventBus] Container is null, cannot publish event");
                    return;
                }

                if (!_publisherCache.TryGetValue(typeof(T), out var cachedPublisher))
                {
                    try
                    {
                        cachedPublisher = _container.Resolve<IPublisher<T>>();
                        if (cachedPublisher != null)
                        {
                            _publisherCache[typeof(T)] = cachedPublisher;
                        }
                    }
                    catch (VContainerException ex)
                    {
                        Debug.LogError($"[EventBus] Failed to resolve publisher for {typeof(T).Name}: {ex.Message}");
                        return;
                    }
                }

                var publisher = (IPublisher<T>)cachedPublisher;
                if (publisher == null)
                {
                    Debug.LogWarning($"[EventBus] No publisher found for type {typeof(T).Name}");
                    return;
                }
                
                publisher.Publish(eventData);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[EventBus] Failed to publish event of type {typeof(T).Name}: {ex.Message}");
            }
        }
        
        public void Subscribe<T>(Action<T> message)
        {
            try
            {
                if (message == null)
                {
                    Debug.LogError("[EventBus] Cannot subscribe with null message handler");
                    return;
                }

                if (_container == null)
                {
                    Debug.LogError("[EventBus] Container is null, cannot subscribe to event");
                    return;
                }

                var key = new SubscriptionKey(typeof(T), message);

                if (_subscriptions.ContainsKey(key))
                {
                    Debug.LogWarning($"[EventBus] Already subscribed to {typeof(T).Name} with this handler");
                    return;
                }
                
                if (!_subscriberCache.TryGetValue(typeof(T), out var cachedSubscriber))
                {
                    try
                    {
                        cachedSubscriber = _container.Resolve<ISubscriber<T>>();
                        if (cachedSubscriber != null)
                        {
                            _subscriberCache[typeof(T)] = cachedSubscriber;
                        }
                    }
                    catch (VContainerException ex)
                    {
                        Debug.LogError($"[EventBus] Failed to resolve subscriber for {typeof(T).Name}: {ex.Message}");
                        return;
                    }
                }

                var subscriber = (ISubscriber<T>)cachedSubscriber;
                if (subscriber == null)
                {
                    Debug.LogError($"[EventBus] No subscriber found for type {typeof(T).Name}");
                    return;
                }
                
                var handlerWrapper = HandlerPool<T>.Rent(message);
                var disposable = subscriber.Subscribe(handlerWrapper);
                
                if (disposable != null)
                {
                    _subscriptions.Add(key, disposable);
                }
                else
                {
                    Debug.LogWarning($"[EventBus] Subscription returned null disposable for {typeof(T).Name}");
                    HandlerPool<T>.Return(message);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[EventBus] Failed to subscribe to event of type {typeof(T).Name}: {ex.Message}");
            }
        }
        
        public void Unsubscribe<T>(Action<T> handler)
        {
            try
            {
                if (handler == null)
                {
                    Debug.LogError("[EventBus] Cannot unsubscribe with null handler");
                    return;
                }

                var key = new SubscriptionKey(typeof(T), handler);

                if (_subscriptions.TryGetValue(key, out var disposable))
                {
                    disposable?.Dispose();
                    _subscriptions.Remove(key);
                    
                    HandlerPool<T>.Return(handler);
                }
                else
                {
                    Debug.LogWarning($"[EventBus] No subscription found for {typeof(T).Name} with this handler");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[EventBus] Failed to unsubscribe from event of type {typeof(T).Name}: {ex.Message}");
            }
        }

        public void ClearAll()
        {
            try
            {
                foreach (var subscription in _subscriptions.Values)
                {
                    subscription?.Dispose();
                }
                
                _subscriptions.Clear();
                _subscriberCache.Clear();
                _publisherCache.Clear();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[EventBus] Failed to clear subscriptions: {ex.Message}");
            }
        }
    }
}
