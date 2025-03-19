using System.Collections.Generic;
using System;
using UnityEngine;

namespace ECS
{
    /// <summary>
    /// Event system that enables communication between entities and systems.
    /// Implements the Singleton pattern.
    /// </summary>
    public class EventSystem
    {
        /// <summary>
        /// Singleton instance of the EventSystem.
        /// </summary>
        private static EventSystem instance;

        /// <summary>
        /// Gets the singleton instance of the EventSystem, creating it if it doesn't exist.
        /// </summary>
        public static EventSystem Instance
        {
            get
            {
                if (instance == null)
                    instance = new EventSystem();
                return instance;
            }
        }

        /// <summary>
        /// Dictionary storing event subscriptions by event type.
        /// </summary>
        private Dictionary<Type, List<Action<GameEvent>>> eventSubscriptions =
            new Dictionary<Type, List<Action<GameEvent>>>();

        /// <summary>
        /// Queue of events to process in the current tick.
        /// </summary>
        private Queue<GameEvent> eventQueue = new Queue<GameEvent>();

        /// <summary>
        /// Queue of events to process in the next tick.
        /// </summary>
        private Queue<GameEvent> nextTickQueue = new Queue<GameEvent>();

        /// <summary>
        /// Subscribes a handler to a specific event type.
        /// </summary>
        /// <typeparam name="T">The type of event to subscribe to.</typeparam>
        /// <param name="handler">The event handler to call when the event is raised.</param>
        public void Subscribe<T>(Action<GameEvent> handler) where T : GameEvent
        {
            Type eventType = typeof(T);

            // Create a new list if this event type doesn't exist yet
            if (!eventSubscriptions.ContainsKey(eventType))
                eventSubscriptions[eventType] = new List<Action<GameEvent>>();

            // Add the handler to the subscription list
            eventSubscriptions[eventType].Add(handler);
        }

        /// <summary>
        /// Unsubscribes a handler from a specific event type.
        /// </summary>
        /// <typeparam name="T">The type of event to unsubscribe from.</typeparam>
        /// <param name="handler">The event handler to remove.</param>
        public void Unsubscribe<T>(Action<GameEvent> handler) where T : GameEvent
        {
            Type eventType = typeof(T);

            // Remove the handler if the event type exists
            if (eventSubscriptions.ContainsKey(eventType))
                eventSubscriptions[eventType].Remove(handler);
        }

        /// <summary>
        /// Raises an event immediately, notifying all subscribed handlers.
        /// </summary>
        /// <param name="gameEvent">The event to raise.</param>
        public void RaiseEvent(GameEvent gameEvent)
        {
            Type eventType = gameEvent.GetType();

            // Notify all handlers subscribed to this event type
            if (eventSubscriptions.ContainsKey(eventType))
            {
                foreach (var handler in eventSubscriptions[eventType])
                {
                    handler(gameEvent);
                }
            }
        }

        /// <summary>
        /// Queues an event to be processed in the current tick.
        /// </summary>
        /// <param name="gameEvent">The event to queue.</param>
        public void QueueEvent(GameEvent gameEvent)
        {
            eventQueue.Enqueue(gameEvent);
        }

        /// <summary>
        /// Queues an event to be processed in the next tick.
        /// </summary>
        /// <param name="gameEvent">The event to queue for the next tick.</param>
        public void QueueEventNextTick(GameEvent gameEvent)
        {
            nextTickQueue.Enqueue(gameEvent);
        }

        /// <summary>
        /// Processes all queued events.
        /// Called by the World at the end of each tick.
        /// </summary>
        public void ProcessEvents()
        {
            // Process all events in the current queue
            while (eventQueue.Count > 0)
            {
                GameEvent gameEvent = eventQueue.Dequeue();
                RaiseEvent(gameEvent);
            }

            // Move events from next tick queue to current queue
            while (nextTickQueue.Count > 0)
            {
                eventQueue.Enqueue(nextTickQueue.Dequeue());
            }
        }
    }
}