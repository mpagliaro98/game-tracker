# Game Tracker
Game Tracker consists of a framework and user interface for tracking a selection of games, with options for tracking completion status and ratings.
The goal of the framework is to be easily-extendable.

The software is currently built with .NET version 4.8 using C#.
The user interface is built in WPF for Windows and Xamarin for Android and iOS (the iOS version is untested).

This is still currently a work in progress.

## Framework

Game Tracker is built on this framework, which you can find in the solution under the RatableTracker folder.
This framework can be imported into a project on its own, then extended.
All parts of the model that involve games specifically exist entirely in the GameTracker folder.
The framework can be extended using just inheritance.

More detailed documentation on parts of the framework and how it is extended will come in the future.
