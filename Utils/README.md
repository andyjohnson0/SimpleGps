# Netduino Utilities Library

## Contents

1. [Synopsis](#Synopsis)
2. [Motivation](#Motivation)
3. [Installation and Use](#Installation and Use)
  * [uk.andyjohnson0.Netduino.Utils.Math Class](uk.andyjohnson0.Netduino.Utils.Math Class)
  * [uk.andyjohnson0.Netduino.Utils.TimeZoneFactory Class](uk.andyjohnson0.Netduino.Utils.TimeZoneFactory Class)
4. [Author](#Author)


## Synopsis

This is a C# class library for .net Micro Framework and the Netduino class of microcontrollers.
It provides various utility classes not related to hardware.


## Motivation

I wrote these drivers for use in various projects using a Netduino 1 and Netduino Minis. I intend to add more classes to this library.


# Installation and Use

This library has no dependencies except the .net Micro Framework.

Build the required configuration and add a reference in your project to uk.andyjohnson0.Netduino.Utils.dll in the appropriate
build directory. Configurations are:
* Debug
* Release

Namespace names and class names (in **bold**) are:

* uk.andyjohnson0.Netduino.Utils

  * uk.andyjohnson0.Netduino.Utils.**Math**

  * uk.andyjohnson0.Netduino.Utils.**TimeZoneFactory**


## uk.andyjohnson0.Netduino.Utils.Math Class

This class provides trancendental and other functions that are missing from the .net Micro Fraemwork's
standard System.Math class.

First add a using alias directive to your code:

	using NdMath = uk.andyjohnson0.Netduino.Utils.Math;

The alias prevents name clashes with the standard System.Math class. 

Then call the various static functions in the Math class. For example:

	var s = NdMath.Sin(NetduinoMath.PI / 2);
	var x = NdMath.Sqrt(2);


## uk.andyjohnson0.Netduino.Utils.TimeZoneFactory Class

The .net Micro Framework includes the abstract System.TimeZone class, but provides no time zone impementations.
The TimeZoneFactory. This class provides access to such imlementations. Currently only the TimeZoneId.London
(that is, UK) time zone ID is supported.

First, add a using directive to your code:

    using uk.andyjohnson0.Netduino.Utils;

The create a TimeZone object:

	var tz = TimeZoneFactory.Create(TimeZoneId.London);
	var dt = tz.GetDaylightChanges(2015);
	var dt = tz.ToLocalTime(DateTime.UtcNow);
	var b = tz.IsDaylightSavingTime(dt);


## Author

Andrew Johnson | [github.com/andyjohnson0](https://github.com/andyjohnson0) | https://andyjohnson.uk

