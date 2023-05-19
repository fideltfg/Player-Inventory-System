<p class="western" align="center" style="margin-bottom: 0in; line-height: 100%">
# Player Inventory System 
<br/>
 <p>The Player Inventory System is designed to make implementing an inventory system in your game simple. <br/>
The Player Inventory System  was developed to replicate yet improve upon the functionality of the inventory system in an existing and very popular game.<br/> 
The Player Inventory System  will allow you to add this functionality to you game with ease.</p>
 
<p>This document explains how to set up the Player Inventory System in its default configuration as well as an overview of all public properties 
that can be found on components in the Untiy Editor.</p>

<p>Full documentation for all classes used, their public methods and properties can be found via the menus at the top of this page.</p>

 ## Setting Up the Player Inventory System
  
<p>The Player Inventory System is designed to work with best in conjunction with Unity's First or third person Character controllers.</P

<p>To add the Player Inventory System to you game Follow these steps.</p>


</p>
<ol type="1">

<li>Add the InventorySystem prefab to your scene. This can be found in the prefabs folder.</li>
<li>Add a FirstPersonController or ThirdPersonUserControl component to your player object if you don't already have one.</li>
<li>On your player object add a InventoryPlayerController component. This provides functionality to pick up items and is only required if your player needs to do so.<br />
Depending on how your player is set up you may need to change the settings on this component for it to function correctly within your game.</li>
<li>Drag your Player Object in to the Player value of the InventoryController component on the InventorySystem Object.<br />
If your player object is tagged 'Player' you can skip this step. The Player Inventory System will find and include your player at start up.</li>
</ol>

## How it Works..

**Inventory Items**

<p>An Inventory Item is any object that the inventory can hold. To simplify creation of your items for use in PIS a script-able object is provided to hold the Item Catalog.<br />
The Item Catalog is located by default in the root folder of the player inventory system.</p>
<p>In Unity, select the Item Catalog. This will display the full list of prepackaged items in the inspector. Each item has its own listing with a number of settings associated with it.<br /><br />
Below are details of these settings</p>


</p>
<ul>
<li><b>Name</b>: The name of the item. This will be used in game to refer to this item. This should be unique.</li>
<li><b>ID</b>: This is the item id and must be unique. This is used in code to reference this item. It is also used in crafting recipes.</li>
<li><b>Description</b>: 	A short description of the item.</li>
<li><b>Sprite</b>:	The sprite that will be used to display the item is slots.</li>
<li><b>World Prefab Single</b>: This is the prefab that will represent a single instance of the item in your game world.</li>
<li><b>World Prefab Multiple</b>: The is the prefab the will represent a stack of the item in your	game world. If the Item is not stack-able (stack count = 1) then you can ignore this.</li>
<li><b>Equipped Prefab</b>: The prefab that will represent the item when equipped on the player or placed in the world.</li>
<li><b>Item Type</b>: This defines what type of item this is. Item Types and how they perform are listed later in this text.</li>
<li><b>Slot Type</b>: This defines what slots this item can be placed in. Inventory allows the Item to be placed in any slot except all other slot types. Items marked as any other slot can be placed in inventory slots and the selected slot type ONLY.<br />
For example if you set an objects slot type to HEAD, that item can be placed in inventory slots (inventory panel slots, item bar slots, crafting slots and chest slots) but only the HEAD slot of the character panel.</li>
<li><b>Max Stack Size</b>: This defines how many of this item can be stacked in a single slot.</li>
<li><b>Recipe</b>: The crafting recipe for this item. See Crafting section below for more information.</li>
<li><b>Craft Count</b>: The number of items that the above recipe will create. See Crafting	section below for more information.</li>
<li><b>Max Durability</b>: The max durability if this item.</li>
<li><b>Damage</b>: The damage increase/decrease this item adds to the players stats.</li>
<li><b>Speed</b>: The speed increase/decrease this item adds to the players stats.</li>
<li><b>Health</b>: The health increase/decrease this item adds to the players stats.</li>
<li><b>Stamina</b>: The stamina increase/decrease this item adds to the players stats.</li>
<li><b>Mana</b>: The mana increase/decrease this item adds to the players stats.</li>
<li><b>Armor</b>: The armor increase/decrease this item adds to the players stats.</li>
<li><b>Intelligence</b>: The intelligence increase/decrease this item adds to the players stats.</li>
<li><b>Dexterity</b>: The dexterity increase/decrease this item adds to the players stats.</li>
<li><b>Luck</b>: The luck increase/decrease this item adds to the players stats.</li>
</ul>

**Crafting**
<p>Each item in the Player Inventory System can be assigned a crafting recipe that allows the player to craft the item in game using other items from the Player Inventory System.</p>

<p>Each item in the Item Catalog has a recipe section. The nine boxes there represent the nine slots on the crafting panel.</p>

<p>In game the player can place the ingredients in to the crafting slots in any location. As long as the pattern the ingredients create matches the recipe defined.</p>


**Craft Count**:
<p>Below the recipe there is the Craft Count property. Set this to the number of items the recipe will create. </p>

<p>Any items that a crafted will be placed in to the output slot on the crafting panel. The player can click the output slot to pickup the item(s) there and may continue to collect items.<br />
So long as there remains enough items to create the recipe in the crafting panel slots. SHIFT + Left click will create as many items as the provided ingredients allow.<br />
The generated items will be moved to the players item bar. Filling available slots there. Overflow will be moved to the players inventory. </p>
<p>The crafting panel will only generate as many items as will fit in the combined free space of the Item bar and inventory. </p>
 
**Panels** 
<p>There are 5 panels provided with the Player Inventory System. These are;
<ul>
	<li><b>Inventory Panel</b> E or Tab</li>
	<li><b>Character Panel</b> C</li>
	<li><b>Crafting Panel</b> V</li>
	<li><b>Chest Panel</b> Mouse 0</li> 
	<li><b>Item Bar</b></li>
</ul>
</p>
<p>With the exception of the Item Bar these panels can be toggled by the player using the keys noted in the list above.<br />
By default the Item Bar can not be toggled. Though a method to do so is provided in the InventoryController class.</p>
<p>The Chest Panel can only be displayed when the player moves a chest to the center his/her view and clicks the left mouse button.</p>
<p>Panel toggle keys can be set via the InventoryController competent on the InventorySystem object.</p>
<p>Opening any panel will disable player movement and display the cursor.</p>
<p>**ESC** will close all panels that are open, hide the cursor and enable player movement.</p>

**Moving Items**
<p>Items can be picked up and moved between slots on any panel by clicking on the item in the slot and then clicking on the destination slot.<br />
If the destination slot currently has an item in it then that item is swapped with the item the player is currently holding. Provided the item is compatible with the slot.</p>

 **Slots** 
<p>Each panel contains a number of slots that are capable of holding one item. Items can be stacked in a slot if they are the same.<br />
he number of items that can be stacked is defined in the Item Catalog. See the **Inventory Items** section above for more details.</p>
<p>By default the inventory is set up with 24 slots, the item bar and crafting panel with 10 and the character panel with 6. Chest panels differ depending on the number of slots assigned when the chest is created.<br />
Please refer to the appropriate section to find out more details on each panel and the slots they contain.</p>


<p>Slots display the sprite (as defined in the Item Catalog) of any item it contains. As well as the number of that item stacked there. Items that are single (or can not be stacked) will not display this number.<br .>
If the item has a MaxDurability value greater than zero, the slot will display a bar to indicate the items durability value.</p>


<p>If the player moves the mouse over a slot, the slot will be highlighted. The color of the highlight will vary depending on the following;</p>
<ul>
	<li>If the slot is empty and no item is being held: Yellow</li>
	<li>If the slot contains an item and no item is being held: Yellow</li>
	<li>If an item is being held and the held item can be placed in the slot: Green </li>
	<li>If an item is being held but the held item can NOT be placed in the	slot: Red</li>
</ul>
<p>The colors noted above are the default. These can be changed on the Slot Prefab located in the Prefabs/Slots folder.</p>


<p>A slot can only hold items that have a matching slot type set in the Item catalog. A head slot can only hold items with slot type head. Most slots have the default slot type setting of inventory.<br />
With the exception of those on the character panel and the crafting panel's output slot. Items can only be placed in slots that are marked as SLOTTYPE.Inventory or that match the items set slot type in the Item Catalog.<br />
For instance a stack of sticks can not be placed on the head slot of the character panel as that slot has a slot type of head and the sticks have a slot type of inventory.</p>

 **Slot Interactions**
<p>A slots reaction to interaction may differ depending on which panel it is on. Below are the details of what a slot will do be default.</p>
 
<ul>
	<li>Left click an inventory slot to pick up the item or stack of items it holds.</li>
	<li>Left click an empty inventory slot to place the held item(s) in to the slot.</li>
	<li>Left click on a slot that contains the same item as is being held, the held(s) will be combined with those already on the slot. Any excess will remain in hand. However, If the stack being clicked is already full, the two will be swapped.</li>
	<li>Right click an item to pick up half of the stack.</li>
	<li>Right click an inventory slot to place one of the held item(s) in to the slot.</li>
</ul>


<p>&lt;Shift&gt; Click will move the contents of the clicked slot depending on the following. Note that  'space' in the following includes that left in other stacks of the same item. Items will fill these stacks first.</p>

<ul>
	<li>If the slot clicked is on the Item Bar, Inventory Panel or chest panel and the slot contains a wearable item (clothing/weapon) and the character panel is open then the item will be moved on to the character (swapping with exiting item if there is one)</li>
	<li>If the slot is on the character panel and the slot contains an item and the Inventory panel is open the item will be moved to the inventory	if there is space. Else it will be placed on the item bar if there is space. If there is no space on the item bar or inventory nothing will happen.</li>
	<li>If the slot is on the item bar and is NOT a wearable item the item will be moved to the inventory panel if there is space. If there is no space in the inventory nothing will happen.</li>
	<li>If the slot is on the inventory panel and is NOT a wearable item it	will be moved to the item bar if there is space. If there is no	space on the item bar nothing will happen.</li>
	<li>If the slot is on the chest panel and the inventory panel is open then items are move to the inventory panel if there is space. Else they are sent to the item bar if space there. If neither have space nothing will happen.</li>
	<li>If the slot is on the crafting panel, items will be placed on the item bar if there is space else in the inventory if there is space. If neither have space nothing will happen.</li>
</ul>

<p>Hover the mouse over a slot and press 1 through 0 to move the item to the corresponding slot on the item bar. If the item bar slot already contains an item the two will swap places so long as the items are valid for the appropriate slot. If not this function will do nothing.</p>


**Dropping Items**
<p>There are a three of ways a player can drop and item. These are listed below. But no matter which method is used the Player Inventory System will spawn a prefab to represent the item in the game world. There are three world prefab values that can be set on the time in the Item Catalog. See the Inventory Items section above for details on these.</p>
<p>Item Drop Methods
<ul>
<li>Pick up an item from any slot and LEFT click out side any panel to drop the item or stack.</li>
<li>Pick up a stack of items from any slot and RIGHT click out side any panel to drop one item from the stack.</li>
<li>Select a slot on the Item Bar and press the drop key. Q by default.</li>
</ul>
</p>

**Picking up Items**
<p>Providing the prefab for the item to be picked up has an ItemPickup component and the player object has the InventoryPlayerController component, The player simply walks over the item to pick it up. The InventoryPlayerController will try and place the item in the ItemBar first. If there is no space there it will try the players inventory.</p>
