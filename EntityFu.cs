using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

/// An `Eid` is an entity ID.
using Eid = System.UInt16;

/// A `Cid` is a component ID.
using Cid = System.UInt16;

namespace EntityFu {
    public class EntityFu
    {
        
        //Component
        //New components need to inherit this class.
        public class Component
        {
            ~Component() {}
	        const bool isEmpty = false;
            private static Cid _numCid;
	        public static Cid numCids { get { return _numCid; } set { _numCid = value; } }
            public Cid cid;
        }

        /// The maximum number of entities. Increase this if you need more.
        static int kMaxEntities = 4096;

        static int kTrustPointers = 0;

        static bool[] entities;
        static Component[][] components;
        static List<Eid>[] componentEids;

        /// Turn this to 0, 1 or 2 to debug the ECS.
        ///0 == no logging, 1 == log creation, 2 == log creation and deletion.
        static int verbosity = 1;

        ///Throw an exception on errors
        private static void Assert(bool condition, string e) {
            if (verbosity > 0) Log(e);
           if(!condition) throw new SystemException(e);
        }

        /// <summary>
        /// Allocate the memory for entities and components. Can call this manually or let it allocate automatically.
        /// </summary>
        public static void alloc()
        {
            if (components != null)
                return;
            
            if (verbosity > 0)
                Log("Allocing entities");

            //allocate entities
            entities = new bool[kMaxEntities];
            for (Eid eid = 0; eid < kMaxEntities; ++eid)
            {
                entities[eid] = false;
            }

            //allocate components
            var max = Component.numCids;
            components = new Component[max][];
            componentEids = new List<Eid>[Component.numCids];
            for (Cid cid = 0; cid < max; cid++)
            {
                //allocate component array
                components[cid] = new Component[kMaxEntities];

                //zero component pointers
                for (Eid eid = 0; eid < kMaxEntities; eid++)
                    components[cid][eid] = null;
            }

            for(Cid cid = 0; cid < Component.numCids; cid++) {
                componentEids[cid] = new List<Eid>();
            }
        }

        /// <summary>
        /// Deallocate the memory for entities and components. Only do this when you no longer need the ECS.
        /// </summary>
        public static void dealloc()
        {
            if (verbosity > 0)
                Log("Deallocing entities");

            entities = null;
            components = null;
            componentEids = null;
        }

        /// <summary>
        /// Create an entity with the given Component(s) and return the `Eid`.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Eid create(params Component[] args) {
            Eid eid = create();
            foreach (Component c in args) {
                addComponent(c.cid, eid, c);
            }
            return eid;
        }

        /// <summary>
        /// Create an entity and return the `Eid`.
        /// </summary>
        /// <returns></returns>
        public static Eid create()
        {
            alloc();
            Eid eid = 1;
            for (; eid < kMaxEntities && entities[eid]; ++eid)
            {

            }

            if (eid < 1 || eid >= kMaxEntities)
            {
                Assert(false, "Maximum number of entities reached!");
                eid = 0;
            }
            else
            {
                entities[eid] = true;

                if (verbosity > 0)
                {
                    Log("Entity " + eid + " created");
                }
            }
            return eid;
        }

        /// <summary>
        /// Destroy an entity and all its components right now.
        /// </summary>
        /// <param name="eid"></param>
        public static void destroyNow(Eid eid)
        {
            if (eid == 0)
                return;

            if (verbosity > 0)
                Log("Entity " + eid + " being destroyed");

            for (Cid cid = 0; cid < Component.numCids; cid++)
                removeComponent(cid, eid);

            entities[eid] = false;
        }

        /// <summary>
        /// Destroy all entities and components right now.
        /// </summary>
        public static void destroyAll() {
            for(Eid eid =1; eid < kMaxEntities; ++eid)
                if(entities[eid])
                    destroyNow(eid);
        }

        /// <summary>
        /// Add the given component to the given entity.
        /// Note that components must be allocated with new.
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="eid"></param>
        /// <param name="c"></param>
        public static void addComponent(Cid cid, Eid eid, Component c)
        {
            if (c == null)
            {
                return;
            }

            if (eid >= kMaxEntities || !entities[eid] || cid >= Component.numCids)
            {
                Assert(false, "Invalid eid " + eid + " or cid " + cid);
                return;
            }
            if (verbosity > 0)
            {
                Log("");
                log(cid);
                Log("Adding component cid " + cid + " eid " + eid);
            }

            //if component already added, delete old one
            if (components[cid][eid] != null)
                removeComponent(cid, eid);

            //pointers to components are stored in the map
            //(components must be allocated with new, not stack objects)
            components[cid][eid] = c;

            //store component eids
            componentEids[cid].Add(eid);

            if (verbosity > 0)
                log(cid);
        }

        /// <summary>
        /// Remove a component from an entity.
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="eid"></param>
        public static void removeComponent(Cid cid, Eid eid)
        {
            if (eid >= kMaxEntities || !entities[eid] || cid >= Component.numCids)
            {
                Assert(false, "Invalid eid " + eid + " cid " + cid);
                return;
            }


            /* var ptr = components[cid][eid];
             if (ptr == null)
                 return;*/

            if (verbosity > 1) {
                Log("");
                log(cid);
                Log("Removing component cid " + cid + " eid " + eid);
            }
                components[cid][eid] = null;

                //update component eids
               // var eids = componentEids[cid];
                var it = find(0, componentEids[cid].Count, eid);
                if (it != componentEids[cid].Count)
                    // eids.Remove((Cid)it);
                    componentEids[cid].Remove((Cid)it);

                if (verbosity > 1)
                    log(cid);
            
        }

        /// <summary>
        /// Get a component from an entity
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="eid"></param>
        /// <returns></returns>
        public static Component getComponent(Cid cid, Eid eid)
        {
            if (kTrustPointers == 0)
            {
                if (eid < kMaxEntities && cid < Component.numCids)
                {
                    return components[cid][eid];
                }
            }
            return null;
        }

        /// <summary>
        /// Get a List of the entities that have the given component.
        /// </summary>
        /// <param name="cid"></param>
        /// <returns></returns>
        public static List<Eid> getAll(Cid cid)
        {
            if (cid < Component.numCids)
                return componentEids[cid];
            List<Eid> blankEids = new List<Cid>();
            return blankEids;
        }

        /// <summary>
        /// The total number of entities.
        /// </summary>
        /// <returns></returns>
        public static uint count()
        {
            uint ret = 0;
            if (entities != null)
            {
                for (Eid eid = 1; eid < kMaxEntities; ++eid)
                    if (entities[eid])
                        ++ret;
            }
            return ret;
        }

        /// <summary>
        /// The number of entities with the given component.
        /// </summary>
        /// <param name="cid"></param>
        /// <returns></returns>
        public static uint count(Cid cid)
        {
            return (uint)getAll(cid).Count();
        }

        /// <summary>
        /// Check if an entity exists.
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>
        public static bool exists(Eid eid)
        {
            return entities != null && entities[eid];
        }

        private static void log(Cid cid)
        {
            var n = count(cid);
            var eids = getAll(cid);
            if (eids.Count > 0)
            {
                Log("Cid " + cid + " has " + n + " entities ranging from " + eids.First() + " to " + eids.Last());
            }
        }

        private static void logAll()
        {
            for(Cid cid = 0, max = Component.numCids; cid < max; cid++)
            log(cid);
        }

        private static void Log(string str)
        {
            Debug.Print(str);
        }

        private static int find(int first, int last, int val)
        {
            while (first != last)
            {
                if (first == val) return first;
                ++first;
            }
            return last;
        }

    }
}
