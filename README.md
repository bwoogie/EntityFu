# EntityFu C#
C# port of [EntityFu by Nat Weiss](https://github.com/NatWeiss/EntityFu)

A simple, fast entity component system originally written in C++ and ported to C#. Originally built for the game [Songbringer](http://songbringer.com) and adapted for release.

Only three files need to be included in your game project: EntityFu.cs, EntityComponent.cs, and EntitySystem.cs.

See Program.cs for example usage.

Basically:

- An entity is simply just an integer ID.
- Components are pure data representing aspects of an entity.
- Systems are where the code goes to operate on components.

Here's an [intro to entity component systems](http://www.raywenderlich.com/24878/introduction-to-component-based-architecture-in-games).

License
-------

EntityFu is licensed under the MIT license.
