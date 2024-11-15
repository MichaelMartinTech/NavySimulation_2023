# NavySimulation_2023
### Author: ### 
` Michael R. Martin `
### Creation Date: ### 
` 11/9/2023 `

### Program Purpose: ### 
This program is an algorithmic AI demo showcasing A* AI Pathfinding techniques for ship movement. 
The code is designed, developed, and implemented utilizing A* waypoint navigation with potential fields. Navigation modes can be switched through the menu interface,
along with setting randomly generated obstacles through the scene. Video demos of these features are available in this repository.

NOTE - ``Code requires Unity asset implementation for functionality.``

### For demonstration of code, please contact the author. ###

### Preview Screenshot ###
![alt text](https://github.com/MichaelRNia/NavySimulation_2023/blob/main/image_2024-01-13_161453116.png?raw=true)

## Video demos: ##

### Waypoints Demo ###
https://github.com/MichaelRNia/NavySimulation_2023/assets/70539262/4c54c12e-8b7e-410f-ae80-e2bd73f6a8bd

### A* Demo ###
https://github.com/MichaelRNia/NavySimulation_2023/assets/70539262/e6130b8e-f7f3-49bd-8403-c3a99412b041

### Potential Fields Demo ###
https://github.com/MichaelRNia/NavySimulation_2023/assets/70539262/364dbc6c-203b-4e55-8a32-3e4d9f936086

### A* Potential Fields Demo ###
https://github.com/MichaelRNia/NavySimulation_2023/assets/70539262/c840c34e-c90c-493b-9240-0a8c77595cd0

## Info
`Engine:` Unity 2023

`Language:` CSharp

## Core Components

#### NOTE: The components described are not the sole dependencies of the program, but rather some of the more major codes for the program. ####

### `UnitAI.cs` ### 
Manages commands like moves and intercepts, executing them during frame updates. The script handles initialization, removal, and adjustment of entity speed based on command execution. With methods for adding, setting, and stopping commands, it also incorporates UI decoration logic, enhancing the visual representation in the simulation. Overall, `UnitAI.cs` is a key component for orchestrating AI behavior and refining the user interface in the naval simulation.

### `SelectionMgr.cs` ###
Allows for box selection of multiple entities and provides functionality for selecting the next entity using the Tab key. It includes features such as determining the quadrant of the selection box, updating the visual representation of the selection box, and selecting entities within the box. The script also manages the selected entities, allowing for single or multiple selections and clearing the selection when needed.

### `PathManager.cs` ###
`PathManager.cs` is a Unity script responsible for managing the generation and visualization of paths in a navy simulation. It interacts with the camera and a Line Manager (`lineMgr`) to create LineRenderers representing the paths. The script captures the start and end points of the path based on mouse input and triggers the generation of the path using the A* algorithm implemented in the `AIMgr` class. The generated path is then visualized by creating LineRenderers with specified widths. The script ensures that the previous path lines are cleared before updating with a new one.

### `AIMgr.cs` ###
Includes functionalities for handling A* pathfinding, potential fields movement, and user input. The script allows ships to follow, intercept, and move to specified positions. It features parameters for potential fields behavior, A* settings, and grid size. Notable functions include `PerformAStar` for asynchronous A* pathfinding and methods for handling ship movements based on user input. The script demonstrates the coordination of ship behaviors and pathfinding within the naval simulation.

### `LineMgr.cs` ###

Handles the creation and destruction of LineRenderers for various purposes, such as representing movement, potential fields, following, and intercepting. The script provides methods to instantiate these LineRenderers with specified positions. Additionally, it manages a list of active LineRenderers, allowing for their removal when necessary. Overall, `LineMgr.cs` contributes to the visual representation of ship behaviors and interactions within the naval simulation.

## Update Log ##
#### 11/10/2024 ####
`4:49 PM PST` - Updated ReadMe for Clarity, including Program Purpose
#### 1/13/2024 ####
`4:11 PM PST` - Uploaded code and video for viewing.

`4:16 PM PST` - AStarPF Preview Image Provided

`5:16 PM PST` - Video Previews Embedded
