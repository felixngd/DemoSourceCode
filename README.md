This repo is only used for the purpose of sharing personal source code in an interview.
In this project, there are 2 parts of code that I developed:

#### 1. UIManager is a series of common UI management codes that I have developed and used effectively in 4 recent projects.
Some types of managed uGUI elements are:
+ Screens: fullscreen UI screens cannot overlap each other, switch between each other and have the option to save history.
+ Modals: are popup or similar UI elements, they can overlap each other.
+ Sheets: is tabs-like UI.
The three types of UI elements mentioned above all have detailed lifecycle controls. UI animations are separated into ScriptableObject or Monobehaviour so that animations can be reused.
+ There are also some other types of UI elements such as: Yes/No/Cancel Dialogs, Tooltip, Toast.

#### 2. The second part is an inventory system that I have been developing recently. This part of the code is not complete and has bugs. And I will continue to develop this inventory system even if I do not pass the interview round.
Why do I develop features through such packages?
+ Because my colleagues and I can reuse code easily, even in different projects.
+ I can easily manage those code versions without affecting the source code of the game project I am working on.
+ Easier to maintain without much impact on the running project, especially in cases where the project has many developers working together.
