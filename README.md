# Game Tracker
Game Tracker consists of a framework and user interface for tracking a selection of games, with options for tracking completion status and ratings.
The goal of the framework is to be as easily-extendable as possible.

The software is currently built with .NET version 4.8 using C#.
The user interface is built in WPF currently, with plans for an Android app using the same framework and model.

Currently used external libraries:
 * Json.NET (https://www.newtonsoft.com/json)

## Ratable Tracker Framework

Game Tracker is built on this framework, which you can find in the solution under the Framework folder.
This framework can be imported into a project on its own, then extended.
All parts of the model that involve games specifically exist entirely in the Model folder.
The framework can be extended using just inheritance and partial classes.

More detailed documentation on parts of the framework and how it is extended will come in the future.
