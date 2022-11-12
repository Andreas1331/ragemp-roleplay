<a name="readme-top"></a>
<!-- PROJECT LOGO -->
<div align="center">
<h3 align="center">Roleplay Gamemode for RageMP</h3>

  <p align="center">
    The gamemode contains various features and a core foundation in place for expansion. 
    <br />
    <a href="https://github.com/Andreas1331/ragemp-roleplay/tree/main/GTARoleplay/GTARoleplay"><strong>Explore the code</strong></a>
    <br />
    <p>Disclaimer: This is not affiliated nor endorsed by Take2 Entertainment.</p>
  </p>
</div>

<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li><a href="#about-the-project">About The Project</a></li>
    <li><a href="#prerequisites">Prerequisites</a></li>
    <li><a href="#getting-started">Getting Started</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
    <li><a href="#acknowledgments">Acknowledgments</a></li>
  </ol>
</details>


<!-- ABOUT THE PROJECT -->
## About The Project

After being affiliated with other gaming communities as a developer I decided to try and create my own gamemode from scratch. The plan was however never to start up my own server, but rather release my take on a roleplay gamemode where I could try out things my way. The project is written in C# server-side and Javascript client-side. 

At the current state the following is implemented:
* Account system with CEF login screen
* Dynamic animation system with proper player positioning
* Character system that allows for as many characters as you wish linked to an account
* Advanced inventory & item system with item bases saved in the database
* Inventory menu using CEF
* Database system utilizing Entity Framework for ORM querying
* Clothing customizer using CEF
* Vehicle tuning using CEF
* System for interacting with any object in the gameworld
* Interactionwheel to interact with objects or perform actions
* Binding server-side actions to keystrokes from clients
* Playable roulette with multiple players
* and more ...

I originally began development in 2021 and abandoned the project again due to time constraints. So do not expect more updates to be released. Also note that some references to SVG (specifically for item icons) files will be null pointers as the files are removed. This is due to copyright, even though they are free for commercial use I've removed them and you'll have to include your own. 

The setup in Visual Studio abstracts away some of the tedious tasks, such as moving client files written in Javascript into the server folder everytime you wish to test something. This is achieved using post build commands inside Visual Studio to copy the files into wherever you have your server folder located. For the .DLL Visual Studio will output it to your server folder as well. You will need to edit the paths for your projects, but more on that in ```Getting Started```.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## Prerequisites

Make sure to have the following installed. Newer versions might be supported as well, but this has not been tested. It is important you have the correct versions of Entity Framework Core and MySQL Server, otherwise you'll get errors. Newer MySQL servers releases seems to have issues with older versions of Entity Framework Core.
* Visual Studio 2019+
* .NET Core v3.1
* BCrypt.Net-Next v4.0.2
* Microsoft.EntityFrameworkCore.Tools v3.1.10
* MySQL.Data.EntityFrameworkCore v8.0.22
* Newtonsoft.Json v13.0.1
* MySQL Server v8.0.20
* RageMP server
* Optional: MySQL Workbench v8.0.20

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- GETTING STARTED -->
## Getting Started

To get your server up and running there's a few things we need to prepare first. This involves ensuring the database is ready, and the RageMP server contains the compiled gamemode.
* Setup the database
* Download an instance of RageMP server
* Change output paths in Visual Studio
* Compile the project and start your server

Let us start with the database. This is thankfully very easy to set up as we will use Entity Framework and rely on it to create our database schema and all related tables. Start by downloading an MySQL server v8.0.20 and install it. With the MySQL server running, open the Visual Studio solution. Now navigate to ```GTARoleplay/Database/DbConn.cs``` and alter the connection string so username, password, port, and password is correct. MySQL will default use port 3306 so you can probably leave that. If you want a different schema name for your database just change the database value in the string.

Now from within Visual Studio we will open the Package Manager Console:
> View -> Other Windows -> Package Manager Console

Run the following commands to create our initial migration and invoke it to have Entity Framework create our schema and tables.

```
add-migration InitialCreate
update-database
```
Note this will create a Migrations folder to keep your database state consistent across the board if you are multiple developers with each their local database running. If everything ran smoothly you can use any schema viewer such as MySQL Workbench to open up your newly created schema and see that all of the tables have been successfully created according to the code.

Next step, is to get our output paths in order inside Visual Studio so our client files and compiled C# project will go to the proper folder. So get yourself a copy of an RageMP server and place it wherever you like. I have mine at ```C:\GTARoleplay\server-files``` so with that done we will go back to Visual Studio. 
Right click the GTARoleplay project and change the output path to your server location:
> Properties -> Build -> Output path:

For me this will be ```C:\GTARoleplay\server-files\dotnet\resources\GTARoleplay\```. Now everytime you build the solution inside Visual Studio the .DLL is automatically placed in your server folder. We will now ensure all client files are also moved to the server folder. So once more right click GTARoleplay in case you closed the window:
> Properties -> Build Events -> Post-build event command line

These commands will copy the entire folder of client files to your server folder. So change each of them to match your path. This is also where you will add new copy commands when you add your own client folders in the future. The reason for setting up the project this way was to have the server files and client files stored near their relevance when developing.

Last step is to build the entire solution in Visual Studio. So hit CTRL + SHIFT + B and watch how it moves everything to your server folder. 

If you wish to attach the debugger to the server you can use F5 instead to build and start the server automatically. For this however you must also edit the path for the debugger. So oncemore right click the GTARoleplay project:
> Properties -> Debug

Edit the Executable and Working Directory to match your paths. For me they will be
```C:\GTARoleplay\server-files\ragemp-server.exe``` and ```C:\GTARoleplay\server-files``` respectively.


<p align="right">(<a href="#readme-top">back to top</a>)</p>


<!-- LICENSE -->
## License

Distributed under the MIT License. See `LICENSE.txt` for more information.

<p align="right">(<a href="#readme-top">back to top</a>)</p>


<!-- CONTACT -->
## Contact

Andreas  - adch18@student.aau.dk

<p align="right">(<a href="#readme-top">back to top</a>)</p>


<!-- ACKNOWLEDGMENTS -->
## Acknowledgments

* [Captien for providing a nice script to show native instructional buttons](https://rage.mp/files/file/148-instructional-buttons/)
* [The wheelnav.js by softwaretailoring.net](http://wheelnavjs.softwaretailoring.net/)
* [Timerbars by root-cause to easily show native timer bars](https://github.com/root-cause/ragemp-timerbars)
* [Logic for getting the minimap anchoring points by glitchdetector](https://github.com/glitchdetector/fivem-minimap-anchor)

<p align="right">(<a href="#readme-top">back to top</a>)</p>
