/// [EntityFuC#](https://github.com/bwoogie/EntityFu)
/// [EntityFu](https://github.com/NatWeiss/EntityFu)
/// A simple, fast entity component system written in C++.
/// Under the MIT license.
///

using System;
using System.Collections.Generic;
using Eid = System.UInt16;

namespace EntityFu {
    class EntitySystem
    {

        //An example entity.
        public class Ent {
            public Eid id = new Eid();
            public EntityComponent.HealthComponent health;
            /// Add more components your systems will use frequently

            public Ent(Eid _id) {
                health = (EntityComponent.HealthComponent)EntityFu.getComponent(EntityComponent.HealthComponent.getCid(), _id);
                id = _id;
            }
        }

        //An example system.
        public class HealthSystem : EntitySystem {

            public static void tick(double fixedDelta) {
                List<Eid> entitiesToDestroy = new List<Eid>(); //Required to keep from getting 'Collection Was Modified' exception

                var all = EntityFu.getAll(EntityComponent.HealthComponent.getCid());

                // for this example, just decrement all health components each tick
                foreach (var eid in all) {
                    Ent e = new Ent(eid);

                    // this is overly pragmatic, but you get the drift of how to check if a component is valid
                    if (e.health == null || e.health.isEmpty())
                        continue;

                    // decrement
                    e.health.hp--;
                    if (e.health.hp < 0) {
                        e.health.hp = 0;
                    }
                    Console.Write("Entity ");
                    Console.Write((int)e.id);
                    Console.Write(" has ");
                    Console.Write(e.health.hp);
                    Console.Write("/");
                    Console.Write(e.health.maxHP);
                    Console.Write(" hit points.");
                    Console.Write("\n");

                    // destroy entity if zero health
                    if (e.health.hp <= 0) {
                        //EntityFu.destroyNow(eid);
                        entitiesToDestroy.Add(eid);
                    }
                }

                foreach(Eid eid in entitiesToDestroy) {
                    EntityFu.destroyNow(eid);
                }
            }
        }



    }
}
