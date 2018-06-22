# Architecture of our project

## Development tools and Editors
In order to develop our game, we used the Game Development Platform <img src="https://zupimages.net/up/18/25/0rhh.png" width="70" height="53">, on the 2017.3.1 version.

In terms of editors, for the code, we chose both <img src="https://zupimages.net/up/18/25/ljh9.png" width="150" height="26"> and <img src="https://zupimages.net/up/18/25/c637.png" width="150" height="42">

## Principal classes
When it comes to the main classes, we have 5 major parts, as follows :
* World/Planet Generation
* Game Interfaces
* Character Design and Animation
* Inventories Management
* AI

#### 1. World/Planet Generation
The first part, the **World/Planet Generation** consists of the generation of procedural planets, with all its aspects, such as :
* Intelligent ground generation
* Day/Night cycle
* Caves generation
* Addition of ores

#### 2. Game Interfaces
For the **Game Interfaces**, the player can move between galaxies and planets in his spaceship.
He can see information about those there.

#### 3. Character Design and Animation
Moving on to the **Character Design and Animation**. Here is how the character looks like : ![Penny](https://zupimages.net/up/18/25/4jlm.png)
The character can make several actions, like :
* Running
* Jumping
* Fighting with knifes or guns

#### 4. Inventories Management
**Inventories Management** makes sure that the player can store the objects he picks up, craft new items, and equip himself.
It manages things such as the main inventory being full or not, in different situations, like picking up item, or crafting ones.

#### 5. AI
Coming into the last part, the **AI**. It takes care of the movement of potential ennemies that our player will encounter.
Ennemies basically run towards our player, taking the optimal path for that purpose.
