using System;
using System.Collections.Generic;
using System.Linq;

namespace MemoryGame.Services
{
    public class Messenger
    {
        private static readonly Lazy<Messenger> _instance = new Lazy<Messenger>(() => new Messenger());
        private readonly Dictionary<Type, List<Subscription>> _subscriptions = new Dictionary<Type, List<Subscription>>();
        public static Messenger Default => _instance.Value;
        private Messenger() { }
        public object Register<TMessage>(Action<TMessage> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            var messageType = typeof(TMessage);
            var token = new SubscriptionToken();

            if (!_subscriptions.TryGetValue(messageType, out var subscribers))
            {
                subscribers = new List<Subscription>();
                _subscriptions[messageType] = subscribers;
            }

            var subscription = new Subscription
            {
                Action = message => action((TMessage)message),
                Token = token
            };

            subscribers.Add(subscription);
            return token;
        }
        public void Unregister(object token)
        {
            if (token == null)
                throw new ArgumentNullException(nameof(token));

            foreach (var messageType in _subscriptions.Keys.ToList())
            {
                var subscribers = _subscriptions[messageType];
                var subscription = subscribers.FirstOrDefault(s => s.Token == token);

                if (subscription != null)
                {
                    subscribers.Remove(subscription);

                    if (subscribers.Count == 0)
                        _subscriptions.Remove(messageType);

                    break;
                }
            }
        }
        public void Send<TMessage>(TMessage message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var messageType = typeof(TMessage);

            if (_subscriptions.TryGetValue(messageType, out var subscribers))
            {
                foreach (var subscription in subscribers.ToList())
                {
                    subscription.Action(message);
                }
            }
        }
        private class Subscription
        {
            public Action<object> Action { get; set; }
            public object Token { get; set; }
        }
        private class SubscriptionToken { }
    }
    public class UserLoggedInMessage
    {
        public Models.User User { get; set; }
    }
    public class NavigationMessage
    {
        public string Destination { get; set; }
        public object Parameter { get; set; }
    }
}