﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Ecs;

namespace Tiles.EntitySystems
{
    public abstract class BaseSystem : ISystem
    {
        public IEnumerable<int> ComponentIds { get; private set; }

        protected BaseSystem(params int[] requiredComponentIds)
        {
            ComponentIds = requiredComponentIds;
        }

        public void Update(IEntityManager entityManager, IGame game)
        {
            foreach (var entity in entityManager.GetEntities(ComponentIds))
            {
                UpdateEntity(entity, game);
            }
        }

        protected abstract void UpdateEntity(IEntity entity, IGame game);
    }
}
