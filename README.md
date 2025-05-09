3D Supermarket Planner
-----------------------------------------------------------------------------------
WebGL Build: https://unknownpack.itch.io/3d-store-planning
Password: Planoverse
-----------------------------------------------------------------------------------

How to Use
- Select a component from the UI and click on where you want to place it in 'placement mode'
	- After placing an object down, you can click and drag it around to move it again.

- Can select a component in the environment to see and CHANGE it's transform properties (position, rotation and scale) in real-time with the UI inspector
	
	- After selecting a component you can:
		- Press 'Left Shift' after selecting a component to enter 'manipulation' mode, which allows you to use 'Q' or 'E' to rotate the component.
		- Hold 'Control' and Press 'V' to instantiate another copy of the selected component.
		- Press 'Space' to remove and destroy the selected component.

- Either click on the floor, a different object or press right click to de-select
  
- Use the camera to pan/zoom/rotate
	- Press 'A' or 'D' to pan camera left or right respectively
	- Press 'X' or 'C' to pan camera up or down respectively
	- Press 'E' or 'Q' to rotate camera left or right respectively
	- Press 'W' or 'S' to rotate camera up or down respectively
	- Use scroll wheel to zoom in/out

-----------------------------------------------------------------------------------

Code Structure

- `CaneraController.cs`: Handles camera movement and basic object moving, rotating, pasting and deleting
- `UIManager.cs`: Handles listView and inspector UI as well as component manipulation of it's transform

-- incomplete --
- `SaveManager.cs`: contains semi-implemented, unused functions for saving and spawning saved data
- `Prefabs_MetaData.cs`: Stores info about each prefab component item, including name and transform data

-----------------------------------------------------------------------------------

Original Feature: Object Inspector

Helps users manipulate a selected component in more granular detail, allowing them to the components:
- position
- rotation
- scale

-----------------------------------------------------------------------------------

Limitations 
- Saving is not yet fully implemented 
- No system to import and assign 3d models to certain prefabs
- No ability to create new reusable prefabs

-----------------------------------------------------------------------------------

Future Ideas:
- Better sleek UI styling
- Grid-based lock system that move and places components in and around a grid for more precise and concise design/ placement

