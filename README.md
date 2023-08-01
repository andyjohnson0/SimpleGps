# Simp[leGps


## Motivation

This is the source code for a microcontroller-based project developed 
in around 2015. Its purpose was to create a simple aid for paper map-based
navigation: a small battery-powered device that simply gets its current
location from a GPS receiver and displays it as a UK Ordnance Survey National
Grid reference.

The project never got further that the breadboard stage, but it worked and it
fun and simple to build. I'm putting it on GitHub for (perhaps) historical
interest.


## Description

The project used a (now obsolete) [Netduino](https://www.wildernesslabs.co/Netduino)
microcontroller that was programmed using C# and the (now also obsolete)
[.NET Micro Framework](https://netmf.github.io/). The power source was
a 9v PP3 battery behind a 5v voltage regulator. The GPS sensor used was a
GlobalSat EM-406 and the display was a generic 2x16 4-wire LCD. The prototype
had a power switch (essential for something that would live in a rucksack),
a button to get and display the current position, and another button to activate
the LCD's backlight for low-light navigation.

Unfortunately I have no photos or schematics of the prototype. It is very unlikely
that the code will build with present development tools due to its obsolete
dependencies.

The main project is in the SimpleGps folder. The Drivers and Utilities projects
were intended to be reusable code.

One interesting part of the project was that computing the grid reference from the
latitude/longitude returned by the GPS sensor requires trigonometric and
trancendental functions that were omitted from the .NET Micro Framework's System.Math
implementation, so I had to write my own using Taylor series etc. These are in 
Utils\Math.cs. Another complication was that the platform had limited support for time
zone information, which I needed to convert UTC to local time, so a simple
implementation of System.TimeZone is in Utils\Time.cs.


## Author

Andrew Johnson | [github.com/andyjohnson0](https://github.com/andyjohnson0) | https://andyjohnson.uk

