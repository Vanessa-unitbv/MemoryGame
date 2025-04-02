using System;
using System.Collections.Generic;
using System.Linq;

namespace MemoryGame.Services
{
    /// <summary>
    /// Sistem de mesagerie pentru comunicarea între ViewModels, fără a crea dependențe directe între ele
    /// </summary>
    public class Messenger
    {
        private static readonly Lazy<Messenger> _instance = new Lazy<Messenger>(() => new Messenger());

        // Dicționar care stochează abonații la mesaje specifice
        private readonly Dictionary<Type, List<Subscription>> _subscriptions = new Dictionary<Type, List<Subscription>>();

        public static Messenger Default => _instance.Value;

        private Messenger() { }

        /// <summary>
        /// Înregistrează un abonat pentru a primi mesaje de un anumit tip
        /// </summary>
        /// <typeparam name="TMessage">Tipul mesajului</typeparam>
        /// <param name="action">Acțiunea care va fi executată când este primit mesajul</param>
        /// <returns>Un token de înregistrare care poate fi folosit pentru dezabonare</returns>
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

        /// <summary>
        /// Dezînregistrează un abonat folosind token-ul de înregistrare
        /// </summary>
        /// <param name="token">Token-ul obținut la înregistrare</param>
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

        /// <summary>
        /// Trimite un mesaj tuturor abonaților care ascultă acest tip de mesaj
        /// </summary>
        /// <typeparam name="TMessage">Tipul mesajului</typeparam>
        /// <param name="message">Mesajul de trimis</param>
        public void Send<TMessage>(TMessage message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var messageType = typeof(TMessage);

            if (_subscriptions.TryGetValue(messageType, out var subscribers))
            {
                // Creăm o copie a listei de abonați pentru a evita probleme
                foreach (var subscription in subscribers.ToList())
                {
                    subscription.Action(message);
                }
            }
        }

        // Clasă internă pentru a stoca informații despre un abonament
        private class Subscription
        {
            public Action<object> Action { get; set; }
            public object Token { get; set; }
        }

        // Clasă internă folosită ca token pentru dezabonare
        private class SubscriptionToken { }
    }

    // Clase de mesaje utilizate în aplicație

    /// <summary>
    /// Mesaj trimis când un utilizator se autentifică
    /// </summary>
    public class UserLoggedInMessage
    {
        public Models.User User { get; set; }
    }

    /// <summary>
    /// Mesaj trimis când se dorește navigarea către o nouă fereastră
    /// </summary>
    public class NavigationMessage
    {
        public string Destination { get; set; }
        public object Parameter { get; set; }
    }
}