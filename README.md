# UnityGears
Recreated for fun the famous glxgears in Unity

When accessing the gears thru webgl, parameters are read from url. 

There are 3 possible scenes: UnityGear, UnityGears2, Viewer

If scene is set to "Viewer", the following parameters can be set to test out gear configurations:

| Param           | Type  |
|:----------------|:-----:|
| TeethCount      | int   |
| InnerRadius     | float | 
| InnerToothDepth | float |
| OuterRadius     | float |
| OuterToothDepth | float |
| Width           | float |

Ex: http://uberyoji.github.io/UnityGears?scene=viewer&width=1&TeethCount=100&InnertoothDepth=0.02&outertoothdepth=0.05&innerradius=0.75
