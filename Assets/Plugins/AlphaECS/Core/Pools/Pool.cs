﻿using System;
using System.Collections.Generic;
using UniRx;

namespace AlphaECS
{
    public class Pool : IPool
    {
        private readonly IList<IEntity> _entities;

        public string Name { get; private set; }
        public IEnumerable<IEntity> Entities { get { return _entities;} }
        public IIdentityGenerator IdentityGenerator { get; private set; }
        public IEventSystem EventSystem { get; private set; }

        public Pool(string name, IIdentityGenerator identityGenerator, IEventSystem eventSystem)
        {
            _entities = new List<IEntity>();
            Name = name;
            IdentityGenerator = identityGenerator;
            EventSystem = eventSystem;
        }

        public IEntity CreateEntity(IBlueprint blueprint = null)
        {
            var newId = IdentityGenerator.GenerateId();
            var entity = new Entity(newId, EventSystem);

            _entities.Add(entity);

            EventSystem.Publish(new EntityAddedEvent(entity, this));

            if (blueprint != null)
            { blueprint.Apply(entity); }

            return entity;
        }

        public void RemoveEntity(IEntity entity)
        {
            _entities.Remove(entity);

            foreach (var component in entity.Components)
            {
                if(component is IDisposable)
                { (component as IDisposable).Dispose(); }
            }

            EventSystem.Publish(new EntityRemovedEvent(entity, this));
        }
    }
}
