# BoxSorting

Box Sorting Game
Developed by Mitch Currie

Unity Editor Version: 6000.1.3f1


------ Overview ------

Boxes fall randomly from above and are sorted by the NPC according to their colour. 

The coloured flags indicate where the boxes should be taken to.

The UI panel in the top left corner displays the NPC's current state and previous state.

The button in the top right corner restarts the game.

When opened in the Unity Editor, you can tweak settings on the NPC (e.g. movement speed), Object Spawner (e.g. spawn rate) and the Boxes.

Enjoy!


------ Details ------

The NPC has the following states:
- Search For Boxes
- Walk To Box
- Pick Up Box
- Walk With Box
- Drop Box
- Throw Box

The normal flow is Search For Boxes -> Walk To Box -> Pick Up Box -> Walk With Box -> Drop Box.

Throw box is used when the path ahead is blocked by another box.

The AI behaviour logic is contained in the NPC Controller and various state scripts.

The spawner uses a simple object pooling system for managing the boxes.

The art assets are from free sites such as kenney.nl/assets, and I edited and created some myself.