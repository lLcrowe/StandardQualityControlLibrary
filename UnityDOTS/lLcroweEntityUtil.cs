#if lLcroweDOTS

using Unity.Entities;
using UnityEngine;

namespace lLCroweTool.DOTS
{
    public static class lLcroweEntityUtil
    {

        public static Entity GetOrCreateEntity(Entity entity, string name)
        {
            if (entity != Entity.Null)
                return entity;

            var world = World.DefaultGameObjectInjectionWorld;
            var manager = world.EntityManager;

            entity = manager.CreateEntity();
            manager.SetName(entity, name);

            return entity;
        }

        public static Entity GetOrCreateEntity(Entity entity, MonoBehaviour monoBehaviour)
        {
            return GetOrCreateEntity(entity, monoBehaviour.name);
        }

        public static void DestroyEntity(Entity entity)
        {
            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null)
                return;

            var manager = world.EntityManager;
            manager.DestroyEntity(entity);
        }
        public static void OnEnable(Entity entity)
        {
            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null)
                return;

            var manager = world.EntityManager;
            manager.SetEnabled(entity, true);
        }

        public static void OnDisable(Entity entity)
        {
            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null)
                return;

            var manager = world.EntityManager;
            manager.SetEnabled(entity, false);
        }
    }
}
#endif
