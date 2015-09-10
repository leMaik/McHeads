# McHeads
McHeads is a WPF element for displaying 3D Minecraft heads. It supports getting a skin either by player name or by UUID.

## Usage
You need to import the namespace into your XAML document. The following examples assume that you imported it as `mcheads`.

To add simple heads, use the `Head` element.
```xaml
<!-- my head -->
<mcheads:Head Playername="leMaik"/>

<!-- my head, using the UUID (recommended) -->
<mcheads:Head Uuid="94d67f2f-d039-419b-8958-abe6b25916b0"/>
```
Using a player's UUID to get the head is the recommended way as a UUID is unique per player and will never change (the nickname may be changed by the player).

The `Head` element has `RotationX`, `RotationY` and `RotationZ` properties to rotate the head around the corresponding axis. The value is in degrees.  
The `Scale` property can be used to slightly adjust the head size. The default value is `1.0`, so that the head will always fit into the bounding box.

## Bonus: Mouse watching head
The scenario I originally coded the element was to have a head that always looks at the user's cursor. The `MouseWatchingHead` is implemented as a subclass of `Head`.

```xaml
<mcheads:MouseWatchingHead Uuid="94d67f2f-d039-419b-8958-abe6b25916b0"/>
```
Two additional properties are provided by this element. `LookAtCursor` can be used to control if the head should look at the cursor or not, default value is `true`.  
The `ScreenDistance` property controls how far the head is "in" the screen when calculating the rotation for looking at the cursor. The default value of `350.0` works pretty well.

# License
To get the skin of a UUID from the Mojang API, McHeads uses [DynamicJson][dynjson] to work with JSON in an easy way. DynamicJson is licensed under the [MS-PL][mspl]. All other files in this repository are licensed under the MIT license, see [the license file][license] for more information.

[dynjson]: http://dynamicjson.codeplex.com/
[mspl]: https://github.com/leMaik/McHeads/blob/master/MS-PL
[license]: https://github.com/leMaik/McHeads/blob/master/LICENSE
