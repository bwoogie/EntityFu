using System;
using System.Threading;
using Cid = System.UInt16;

namespace EntityFu {
    class Program
    {

        static Cid _id = 0;
        
        static void Main(string[] args) {
            //Give Components a cId
            EntityComponent.HealthComponent.cid = _id++;
            EntityFu.Component.numCids = _id; //total number of cids

            //Create a new entity and add a HealthComponent to it
            EntityFu.create(
                new Cid[] {EntityComponent.HealthComponent.cid},
                new EntityComponent.HealthComponent(10, 100)
                );
            EntityFu.create(
                new Cid[] {EntityComponent.HealthComponent.cid},
                new EntityComponent.HealthComponent(5, 50)
                );
                
                /*
                Because of limitations of either C# or my own...
                to create a new entity you must specify an array of Cid which contains the cid's of each component.
                Obviously, `ids` and `args` must be in the same order.
                This is not required in the C++ version of EntityFu. If anyone knows how to achieve this without
                the array, let me know.
                
                Example code to create an entity with multiple components:
                
                EntityFu.create(
                    new Cid[] {EntityComponent.HealthComponent.cid, EntityComponent.PositionComponent.cid},
                    new EntityComponent.HealthComponent(5, 50),
                    new EntityComponent.PositionComponent(30, 60)
                );
                
                */

            //Example usage, take away 1 health point every second.
            while (EntityFu.count() > 0) {
                EntitySystem.HealthSystem.tick(0.1);
                Thread.Sleep(1000);
            }

            //We're done. Destroy it.
            EntityFu.dealloc();
            Console.ReadLine();
        }
    }
}
