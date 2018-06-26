# Project Team
* CROQ Nicolas
* SAMMANI Axel
* MERRIEN Maxime
* VARIS Florian
* AL HUSSEIN Ryan
* BOURY Arthur
* ESNAULT Alexis

# Project Management
During this project, our 7-member group worked using SCRUM agil method, thanks to the utilization of Trello.

We also planified weekly meetings, each Monday, to discuss about what has been done the week before, and also what will be done the current week.

# Main Supported Features
* World Generation
* Character's Actions - *running, jumping, picking up items, fighting*
* Inventories Management - *consuming items, crafting items, equip items, split items*

### 1. World Generation - *CROQ Nicolas & VARIS Florian*

First we wanted the player to travel on a large number of planets. Generating the planets one by one would have taken too much time,
that's why we decided to generate them in a procedural way, that means in a large scale (huge quantity of planets),
completely automatic and following a set of rules defined by an algorithm.

Advantages of procedural generation include smaller file sizes, larger amounts of content, and randomness for less predictable gameplay.

The generation process works in two steps :

Step 1 : an algorithm will define the main parameters of the planets (horizontal and vertical size, available resources, climate...)
Then the player can choose which planet to visit if the characteristics of the planet suit him.

Step 2 : When the player chooses to go on a planet, this one will be generated according to the parameters assigned to it.

Note: The state of the planet is saved when the player decides to leave it.

  - **Main Generation** - *CROQ Nicolas & VARIS Florian*

       **_Find below a non-detailed & non-exhaustive list of tasks_**
	   
<table>
  <th align="center"> Sprint Number </th>
  <th align="center"> Functionalities added </th>
  <tr>
    <td rowspan="1" align="center">1</td>
    <td align="left">Chunk based Generation</td>
  </tr>
  <tr>
    <td rowspan="1" align="center">2</td>
    <td align="left">Tilemap based Generation including caves, ores and seamless map</td>
  </tr>
   <tr>
    <td rowspan="1" align="center">3</td>
    <td align="left">Background, sun light, day / night cycle, weather system</td>
  </tr>
  <tr>
    <td rowspan="1" align="center">4</td>
    <td align="left">Ruletiles / sprites, Planet loading / saving</td>
  </tr>
   <tr>
    <td rowspan="1" align="center">6</td>
    <td align="left">Fauna / Flora generation</td>
  </tr>
</table>

  - **Nicolas’ world generation functionalities** - *CROQ Nicolas*

       **_Find below a non-detailed & non-exhaustive list of tasks_**

<table>
  <th align="center"> Sprint Number </th>
  <th align="center"> Functionalities added </th>
  <tr>
    <td rowspan="1" align="center">1</td>
    <td align="left">Non optimized generation with chunks</td>
  </tr>
  <tr>
    <td rowspan="1" align="center">2</td>
    <td align="left">Tilemap based, ore deposit and cave generation</td>
  </tr>
   <tr>
    <td rowspan="1" align="center">3</td>
    <td align="left">Game data managing tool, weather system (rain, snow, fog, lighting, stars), world light</td>
  </tr>
  <tr>
    <td rowspan="1" align="center">4</td>
    <td align="left">Optimized planet loading / saving</td>
  </tr>
  <tr>
    <td rowspan="1" align="center">5</td>
    <td align="left">Dumb (Move towards the player) and Smart (A* pathfinding) Artificial Intelligence</td>
  </tr>
   <tr>
    <td rowspan="1" align="center">6</td>
    <td align="left">Fauna generation with spawner depending on the time, flora generation</td>
  </tr>
</table>

  - **HUB creation (spaceship)** - *VARIS Florian*

       **_Find below a non-detailed & non-exhaustive list of tasks_**
	   
<table>
  <th align="center"> Sprint Number </th>
  <th align="center"> Functionalities added </th>
  <tr>
    <td rowspan="1" align="center">5</td>
    <td align="left">Creation of the sprites</td>
  </tr>
   <tr>
    <td rowspan="1" align="center">6</td>
    <td align="left">Creation of the scene and dynamic objects (doors, dashboard and teleportation pad)</td>
  </tr>
</table>
	   
### 2. Character's Actions

### 3. Inventories Management - *SAMMANI Axel & MERRIEN Maxime*
  
One of the most essential features in a 2D role-playing based adventure game is indeed the managment of several items, weather it is equipments, consumables, quest rewards, and so on.
  The way we are managing our system is fairly simple.
  
  On the first side, we have a main inventory, useful for the storage of various items.
    
  - **Main Inventory** - *SAMMANI Axel & MERRIEN Maxime*

       **_Find below a non-detailed & non-exhaustive list of tasks_**

<table>
  <th align="center"> Sprint Number </th>
  <th align="center"> Functionalities added </th>
  <tr>
    <td rowspan="2" align="center">1</td>
    <td align="left">Importing assets - Inventory Master</td>
  </tr>
  <tr>
    <td align="left">Allowing player to destroy resources and collect them afterwards</td>
  </tr>
  <tr>
    <td rowspan="3" align="center">2</td>
    <td align="left">Allowing split in main inventory</td>
  </tr>
  <tr>
    <td align="left">Allowing drag and drop of items from main inventory to world</td>
  </tr>
  <tr>
    <td align="left">Adding FullInventory function</td>
  </tr>
  <tr>
    <td rowspan="1" align="center">4</td>
    <td align="left">Display of different recipes linked to the item and the recipe to create it</td>
  </tr>
</table>
  
On another part, we found interesting to implement a craft system, where the player can create new items, thanks to many recipes.
    
  - **Craft System** - *MERRIEN Maxime*
  
  **_Find below a non-detailed & non-exhaustive list of tasks_**

<table>
  <th align="center"> Sprint Number </th>
  <th align="center"> Functionalities added </th>
  <tr>
    <td rowspan="2" align="center">1</td>
    <td align="left">Fixing minor bug - ResultSlot always coming back to 1st item</td>
  </tr>
  <tr>
    <td align="left">Prohibiting consumming items in the Craft System</td>
  </tr>
  <tr>
    <td rowspan="2" align="center">2</td>
    <td align="left">Allowing the player to craft the amount of item he wants thanks to a spiner</td>
  </tr>
  <tr>
    <td align="left">Adding input field to facilitate number selection</td>
  </tr>
  <tr>
    <td rowspan="2" align="center">3</td>
    <td align="left">Calculating the max amount of item the player can craft at a certain point in time</td>
  </tr>
  <tr>
    <td align="left">Managing the creation / addition of the item in the main inventory</td>
  </tr>
</table>
   
Last but not least, we added a hotbar, in order to allow the player to quickly select the item he wants to use.
    
  - **Hotbar** - *SAMMANI Axel*
  
  **_Find below a non-detailed & non-exhaustive list of tasks_**

<table>
  <th align="center"> Sprint Number </th>
  <th align="center"> Functionalities added </th>
  <tr>
    <td rowspan="3" align="center">3</td>
    <td align="left">Generation of a cursor associated with the Hotbar</td>
  </tr>
  <tr>
    <td align="left">Drag & drop an item from the main inventory towards the Hotbar</td>
  </tr>
  <tr>
    <td align="left">Build the item selected by the cursor (this item needs to be a texture) </td>
  </tr>
  <tr>
    <td align="center">5</td>
    <td align="left">Equip itself with weapons thanks to the cursor</td>
  </tr>
</table>
   
### 4. Character Stats

# Time and Context of the project
This project has been realized for the validation of the **_L3 Informatique_** of the **ISTIC**, on the *Campus de Beaulieu, Université de Rennes 1*.
Starting on Monday, 14th of May, it lasted 6 weeks.
