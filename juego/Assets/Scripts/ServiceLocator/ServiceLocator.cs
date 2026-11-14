using System;
using System.Collections.Generic;
using Patterns.ServiceLocator.Interfaces;
using UnityEngine;

namespace Patterns.ServiceLocator
{
    public class ServiceLocator : MonoBehaviour
    {
        public static ServiceLocator Instance; //Acceso global al ServiceLocator

        private Dictionary<Type, object> services = new Dictionary<Type, object>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
                return;
            }

            // Registro de servicios
            this.Register<DialogManager>(GetComponent<DialogManager>());
            InventoryService inventory = GetComponent<InventoryService>();
            if (inventory == null)
                this.Register<InventoryService>();
            else
                this.Register(inventory);
        }

        void Start() { }

        // Register for services that live out IN this object
        public void Register<T>()
            where T : MonoBehaviour, IService
        {
            var type = typeof(T);
            IService service = this.gameObject.AddComponent<T>();
            if (!services.ContainsKey(type))
            {
                service.Initialize();
                services.Add(type, service);
                Debug.Log($"Service {type} registered successfully");
            }
            else
            {
                Debug.LogWarning($"Service {type} is already registered");
            }
        }

        // Register for services that live OUT of this object
        public void Register<T>(T service)
            where T : MonoBehaviour, IService
        {
            var type = typeof(T);
            if (!services.ContainsKey(type))
            {
                service.Initialize();
                services.Add(type, service);
                Debug.Log($"Service {type} registered successfully");
            }
            else
            {
                Debug.LogWarning($"Service {type} is already registered");
            }
        }

        public T GetService<T>()
            where T : IService
        {
            var type = typeof(T);
            if (!services.TryGetValue(type, out object service))
            {
                throw new Exception($"The service{type} has not been found");
            }
            return (T)service;
        }

        public void Unregister<T>(T service)
            where T : MonoBehaviour, IService
        {
            Type type = typeof(T);
            if (services.ContainsKey(type))
            {
                services.Remove(type);
                Destroy((T)service);
                Debug.Log($"Service {type} unregistered successfully");
            }
        }

        public void StopAllServiceCorrutines()
        {
            foreach (var service in services)
            {
                if (service.Value is MonoBehaviour monoBehaviourService)
                {
                    monoBehaviourService.StopAllCoroutines();
                }
            }
        }
    }
}
