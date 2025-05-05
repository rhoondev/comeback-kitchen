# The Comeback Kitchen

## A customizable virtual reality cooking simulation for physical, occupational, and cognitive rehabilitation of people suffering from brain injuries.

In this simulation, the user cooks a paella from start to finish. The instructions are given to the user through a floating cookbook, which provides verbal, auditory, and visual instruction on how to complete each step of the recipe. This process involves tasks like:
* Washing
* Cutting
* Blending
* Pouring
* Stirring
* Measuring
* Seasoning
* Boiling

## Known Issues

This project is still a work in process.

* Pinch pose for interacting with objects needs to be swapped out with a grab pose
* Cooking section is incomplete.
* Preparation section has functional cutting logic but the gameplay loop needs to be refactored and is currently not functional; knife prefab will require modification to correctly interact with sliceable objects
* Transparent materials such as the olive oil bottle, measuring cup, and jar, need to be converted to interactable materials (may require custom shader)
* Knob interaction does not work as intended
* Faucet interaction does not work as intended
* InfiniteGrabSpawner collider needs to be converted to trigger (while still being interactable) to prevent collisions with spawned objects (alternatively, it can be on a different layer that doesn't collide with the "Dynamic Object" layer)
* Circular washing progress bar does not face towards the player, which means it can sometimes be obscured
* The meshes and/or materials for mussels, onions, and shrimp are placeholders
* Tomato mesh is has visual artifacts when imported into Unity (issues are not visible in Blender)
* Interactable objects remain highlighted in white after the player stops interacting with them (they should return to their normal material with no rim alpha)

## Attributions

* "Kitchen 10012025" (https://skfb.ly/ptCL7) by baldoVReal is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).
* "Chicken breast" (https://skfb.ly/oKysq) by PaShok3D is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).
* "cutting board" (https://skfb.ly/oAMAV) by YouniqueĪdeaStudio is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).
* "'tomato'" (https://skfb.ly/6U8Cv) by B.F.C is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).
* "Wooden spoon" (https://skfb.ly/6RHXD) by quedlin is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).
* "Whiskey and Wine Low-Poly Bottles" (https://skfb.ly/oqEqR) by Helyeouka is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).
* "Cooked Shrimp Low poly" (https://skfb.ly/oQ7nC) by cptoey is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).
* "Salt Shaker" (https://skfb.ly/6zII7) by KatH10 is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).
* "Pepper Shaker" (https://skfb.ly/6zIIq) by KatH10 is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).
* "Limes should be free" (https://skfb.ly/pnsOz) by LordCinn is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).
* "Blender" (https://skfb.ly/o8YCz) by giga is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).