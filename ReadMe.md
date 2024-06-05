
# Player Inventory System

The Player Inventory System is designed to make implementing an inventory system in your game simple. I developed this system to replicate yet improve upon the functionality of the inventory system in an existing and very popular game.<br/> 
The Player Inventory System will allow you to add this functionality to you game with ease.

![Inventory Window](https://raw.githubusercontent.com/fideltfg/PlayerInventorySystem/master/Assets/PlayerInventorySystem/Docs/images/CS9raV.png)
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
- Item durability,
- Item Catalog for easy configuration and mapping of items and their properties.
- Custom editor scripts help organize items in your catalog.
- Simple design allows you to easily swap catalogs and themes at runtime.
- Encrypted object serialization and saving to help prevent cheating,
- Inventory/chest contents, placed and dropped items can be loaded and saved automatically,
- Keyboard and partial controller support using the new input system. (Full support coming soon!)
- Item recyceling/salvage allows players to reclaim the items used to craft an item.


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
Assuming you already have a Unity project open that contains either the contents of the Unity package or the repository, follow these steps to add the Player Inventory System to a new scene.

1. Drag the prefab **PlayerInventorySystem\Prefabs\InventorySystem** into your scene. 
2. Add an event system object by right clicking the hierarchy and selecting **UI->Event System**
3. Add your player object to your scene. 
4. Add a **PlayerInventoryControler** component to your player object.
5. Select the InventorySystem in the hierarchy and drag your player object in to the player property slot.

NOTE: While the demo in the package includes scripts to control the player, anything located in the Demo folder is not supported under this system. The scripts are used to provide simple gameplay so the demo can do its thing. They should NOT be considered part of the Player Inventory System and as such are not covered in the documentation except were to explain how to integrate with existing player controllers and the input system.

# Player Inventory System Guide

## Introduction
The Player Inventory System is designed to simplify implementing an inventory in your Unity game. This guide will help you set it up and provide tips on using it effectively.

## Getting Started
### Requirements:
- **Skill Level**: Intermediate
- **Coding**: Minimal

### Setup Steps:
1. **Add InventorySystem Prefab**:
   - Open the Unity Editor.
   - In the Project window, navigate to the `Prefabs` folder.
   - Drag the `InventorySystem` prefab into the `Hierarchy` window.
   - Ensure there is only one `InventorySystem` in each scene.

2. **Player Controller**:
   - Select your player object in the `Hierarchy`.
   - In the Inspector, click `Add Component`.
   - Type `FirstPersonController` or `ThirdPersonUserControl` in the search bar.
   - Click to add the chosen component to your player object.

3. **Inventory Player Controller**:
   - With the player object still selected, click `Add Component` again.
   - Type `InventoryPlayerController` and add it.
   - Adjust settings in the Inspector if necessary to fit your game’s needs.

4. **Link Player**:
   - Select the `InventorySystem` object in the `Hierarchy`.
   - In the Inspector, find the `InventoryController` component.
   - Drag your player object from the `Hierarchy` to the `Player` field of the `InventoryController`.
   - If your player object is tagged 'Player', you can skip this step.

## Using the System
### Inventory Items
- **Definition**: Any object that the inventory can hold.
- **Settings**: Manage in the Item Catalog with details like name, ID, description, sprite, and prefab configurations.

### Moving and Dropping Items
- **Move**: Click an item in a slot, then click the destination slot to move it.
- **Drop**: 
  - Left-click outside any panel to drop the entire stack.
  - Right-click outside any panel to drop one item from the stack.
  - Press the drop key (default Q) while selecting a slot in the Item Bar.

### Picking Up Items
- Ensure the item prefab has a `DroppedItem` component.
- Ensure the player object has the `InventoryPlayerController` component.
- To pick up items, the player simply walks over them.

### Equipping Items
- **Drag Items**: Drag items from the Inventory Panel to the appropriate slot on the Character Panel.
- **Shift-click**: Shift-click an item to automatically move it to the correct slot.

## Panels and Interactions
### Panels:
- **Inventory Panel (E or Tab)**: Main storage with 24 slots (modifiable).
- **Item Bar**: Quick access to items.
- **Character Panel (C)**: Equip items to character slots.
- **Crafting Panel (V)**: Craft new items from ingredients.
- **Chest Panel (Mouse 1)**: For storing items.

### Slot Interactions
- **Left-click**: Pick up or place items.
- **Right-click**: Split stacks or place one item.
- **Shift-click**: Move items between panels or equip them.

## Item Catalog
### Creating Items
1. **New Catalog**:
   - In the Project view, right-click the `InventorySystem` folder.
   - Select `Create` > `SO_ItemList`.
   - Rename the new list.
   - Drag the new list onto the `Item Catalog` property of the `InventoryController` component.

2. **Add Items**:
   - Select the `Item Catalog` in the Project view.
   - In the Inspector, add elemets to the catolog fill in details for each item: ``First item MUST BE NULL``
     - **Name**: The human-readable name of the item.
     - **BaseMaterial**: The base material of the item.
     - **MaterialVariant**: The variant of the material.
     - **ID**: Unique identifier for the item.
     - **ConsumeOnPickup**: If true, the item will be consumed/used when picked up and not placed in the inventory.
     - **Description**: Brief description of the item displayed when hovering over the slot.
     - **Sprite**: Inventory sprite used to display the item in slots.
     - **PrefabSingle**: Prefab representing a single instance of the item in the game world.
     - **PrefabMultiple**: Prefab representing a stack of the item in the game world.
     - **WorldPrefab**: Game object representing the item when equipped or placed in the game world.
     - **ItemType**: Defines the item’s type (e.g., Consumable, Usable, Wearable, Quest).
     - **SlotType**: Defines the type of slot this item requires.
     - **MaxStackSize**: Maximum number of this item that can be stacked in one slot.
     - **Recipe**: The crafting recipe for this item.
     - **CraftCount**: Number of items the crafting recipe will yield.
     - **Salvageable**: Indicates if this item can be salvaged from other items.
     - **Recyclable**: Indicates if this item can be recycled into other items via the salvage process.
     - **MaxDurability**: Durability of the item when new.
     - **Damage**: Damage bonus when equipped.
     - **Strength**: Strength bonus when equipped.
     - **Speed**: Speed bonus when equipped.
     - **Health**: Health bonus when equipped.
     - **Stamina**: Stamina bonus when equipped.
     - **Mana**: Mana bonus when equipped.
     - **Armor**: Armor bonus when equipped.
     - **IQ**: Intelligence bonus when equipped.
     - **Dexterity**: Dexterity bonus when equipped.
     - **Luck**: Luck bonus when equipped.


## Crafting
- **Define Recipes**:
  - In the Item Catalog, set the recipe by entering item IDs into the recipe boxes.
  - Specify the number of items produced with the `craftCount` property.

- **In-Game Crafting**:
  - Open the Crafting Panel (V).
  - Place ingredients in the nine input slots.
  - The crafted item will appear in the output slot.

## Saving and Loading Data
- **Automatic**: Saves when the game closes or the Inventory System object is destroyed.
- **Manual**: Call `Load()` and `Save()` methods from your scripts.
- **Save Location**: 
  - Set by the `UsePersistantDataPath` option on the `InventoryController`.
  - True: Saves at `Application.persistentDataPath + "/Data/data.dat"`.
  - False: Saves at `pathToYourProject + "/Data/data.dat"`.

## Additional Resources
For more detailed information and tutorials, refer to the full online documentation.

## Assets
While this project is in Alpha it makes use of a small number of 3rd party art assets. These are stored in the **ThridParty** folder. Remember to remove and replace these assets before you release your game.

## Demo Scene
Both the repo and Unity package contain a simple demo scene that allows you to test each of the systems functions. Depending on the state of the project at the last push to the git repo, the demo scene may or may not work correctly.
As the project progresses the demo scene will be expanded and features fixed in place.

