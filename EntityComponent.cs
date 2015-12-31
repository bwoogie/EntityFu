/// [EntityFuC#](https://github.com/bwoogie/EntityFu)
/// [EntityFu](https://github.com/NatWeiss/EntityFu)
/// A simple, fast entity component system written in C++.
/// Under the MIT license.
///

using Cid = System.UInt16;

namespace EntityFu {
    class EntityComponent
    {
        //An example component.
        public class HealthComponent : EntityFu.Component {
            public int hp, maxHP;
            public HealthComponent(int _hp, int _maxHP)
            {
                hp = _hp;
                maxHP = _maxHP;
            }

            HealthComponent()
            {
                hp = 0;
                maxHP = 0;
            }

            public virtual bool isEmpty() { return maxHP == 0; }
            private static Cid myCid;
            public override ushort cid { get { return myCid; } set { myCid = value; } }

            public static void setCid(ushort c) { myCid = c; }
            public static Cid getCid() { return myCid; }
        }

        //Copy and paste this class to easily create a new Component and change the class name and "value" variable
        /*
        public class NewComponent : EntityFu.Component {
            public int value;
            public NewComponent(int _value) {
                value = _value;
            }

            NewComponent() {
                value = 0;
            }

            public virtual bool isEmpty() { return value == 0; }
            private static Cid myCid;
            public override ushort cid { get { return myCid; } set { myCid = value; } }

            public static void setCid(ushort c) { myCid = c; }
            public static Cid getCid() { return myCid; }
        }
        */

    }
}
