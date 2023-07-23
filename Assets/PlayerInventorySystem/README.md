# Player Inventory System >> 0.0.5 Alpha!

The Player Inventory System is designed to make implementing an inventory system in your game simple. <br/>
The Player Inventory System  was developed to replicate yet improve upon the functionality of the inventory system in an existing and very popular game.<br/> 
The Player Inventory System  will allow you to add this functionality to you game with ease.

![Inventory Window](Assets/PlayerInventorySystem/Docs/images/CS9raV.png)

## Features
- Item Bar to store items for quick access,
- Inventory Panel to allow player to carry more objects while they move around,
- Chests to allow leaving items long term,
- Character Panel allows equipping gear and weapons and displays player stats,
- Crafting Panel allows player to create new items from others,
- Item info Panel allows player to view information about items in his/her inventory,
- Drag and drop items between inventories and slots,
- Place-able items,
- Consumable items,
- Stackable items,
- Items can be used to impart buffs on player,
- Item Catalog for easy configuration and mapping of items and their properties.
- Custom editor scripts help organize items in your catalog.
- Simple design allows you to easily swap catalogs and themes at runtime.
- Encrypted object serialization and saving to help prevent cheating,
- Inventory/chest contents, placed and dropped items can be loaded and saved automatically,
- Keyboard and partial controller support(Full support coming soon!) using the new input system.


## Getting Started

### Requirements
1. Unity 2022.3 or higher
2. Unity Input system Package 1.6.3 or higher


### Git Repo
If you have downloaded or cloned the git repository you can open the project through Unity Hub.

### Unity Package
If you have downloaded the Unity package either..
+ Open an existing project or create a new one,
or
+ Double click the downloaded package and import the Player Inventory System folder.

### From Scratch
Assuming you already have a unity project open that contains either the contents of the unity package or the repository, follow these steps to add the Player Inventory System to a new scene.

1. Drag the prefab **PlayerInventorySystem\Prefabs\InventorySystem** into your scene. 
2. Add an event system object by right clicking the hierarchy and selecting **UI->Event System**
3. Add your player object to your scene. 
4. Add a **PlayerInventoryControler** component to your player object.
5. Select the InventorySystem in the hierarchy and drag your player object in to the player property slot.

NOTE: While the demo in the package includes scripts to control the player, anything located in the Demo folder is not supported under this system. The scripts are used to provide simple gameplay so the demo can do its thing. They should NOT be considered part of the Player Inventory System and as such are not covered in the documentation except where to explain how to integrate with existing player controllers and the input system.


## How it works
The Inventory controller is the hub of the system. It has methods to create and interact with inventories. It contains a number of lists of inventories relating to the player, chests and placed or spawned items and the methods to interact with them. Please read the code docs to find out about all the methods available and how to use them.






## Documentation

As this system is still in alpha you can find most of the documentation in the code.

To get started open the file **PlayerInventorySystem\Scripts\Controller\InventoryController.cs** and read the comments at the start of the file.

Full documentation will be added once the player inventory system has moved to beta.




## Assets
While this project is in Alpha it makes use of a small number of 3rd party art assets. These are stored in the **ThridParty** folder. Remember to remove and replace these assets before you release your game.

## Demo Scene
Both the repo and Unity package contain a simple demo scene that allows you to test each of the systems functions. Depending on the state of the project at the last push to the git repo, the demo scene may or may not work correctly.
As the project progresses the demo scene will be expanded and features fixed in place.