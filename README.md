# GoblinParty

### Game
![MenuImage](https://raw.githubusercontent.com/MonteFloyd/GoblinParty/master/images/menu.png)

Goblin Party is a multiplayer old W3 Custom map style collections of minigames. Group of players go through different minigames through coordination or skill.

#### Maze

Maze level generates a random perfect maze through recursive backtrack algorithm and stores it in boolean array form. Later it draws it slowly so that player has a small timeframe to take advantage and familirize with it. Maze is randomly generated everytime so no point memorizing it for every game.

After done with generation and drawing the maze, **mazeGenerator** makes it completely dark so that group of players can use their limited skills and communication to solve it within the timeframe. Here is how it looks at the start:

![DeployingMaze](https://raw.githubusercontent.com/MonteFloyd/GoblinParty/master/images/maze.gif)


#### Ice Rink

Ice level is quite simple, player "skates" on the ice and needs to avoid dropping in the sea after ice cracks. Ice cracks after first drop of stone and shatters after second. 

![Ice](https://github.com/MonteFloyd/GoblinParty/blob/master/images/ice.gif)
