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
                new EntityComponent.HealthComponent(10, 100)
                );
                
            EntityFu.create(
                new EntityComponent.HealthComponent(5, 50)
                );
                

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
