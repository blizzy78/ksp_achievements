/*
Achievements - Brings achievements to Kerbal Space Program.
Copyright (C) 2013 Maik Schreiber

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Body {
	public static readonly Body SUN = new Body("Sun", "the Sun");

	public static readonly Body MOHO = new Body("Moho");
	public static readonly Body EVE = new Body("Eve");
	public static readonly Body KERBIN = new Body("Kerbin");
	public static readonly Body DUNA = new Body("Duna");
	public static readonly Body DRES = new Body("Dres");
	public static readonly Body JOOL = new Body("Jool");
	public static readonly Body EELOO = new Body("Eeloo");

	public static readonly Body GILLY = new Body("Gilly");
	public static readonly Body MUN = new Body("Mun", "the Mun");
	public static readonly Body MINMUS = new Body("Minmus");
	public static readonly Body IKE = new Body("Ike");
	public static readonly Body LAYTHE = new Body("Laythe");
	public static readonly Body VALL = new Body("Vall");
	public static readonly Body TYLO = new Body("Tylo");
	public static readonly Body BOP = new Body("Bop");
	public static readonly Body POL = new Body("Pol");

	public static readonly IEnumerable<Body> ALL = new Body[] { SUN, MOHO, EVE, KERBIN, DUNA, DRES, JOOL, EELOO, GILLY, MUN, MINMUS, IKE, LAYTHE, VALL, TYLO, BOP, POL };
	public static readonly IEnumerable<Body> PLANETS = new Body[] { MOHO, EVE, KERBIN, DUNA, DRES, JOOL, EELOO };
	public static readonly IEnumerable<Body> PLANETS_WITHOUT_KERBIN = new Body[] { MOHO, EVE, DUNA, DRES, JOOL, EELOO };
	public static readonly IEnumerable<Body> MOONS = new Body[] { GILLY, MUN, MINMUS, IKE, LAYTHE, VALL, TYLO, BOP, POL };
	public static readonly IEnumerable<Body> LANDABLE = new Body[] { MOHO, EVE, KERBIN, DUNA, DRES, EELOO, GILLY, MUN, MINMUS, IKE, LAYTHE, VALL, TYLO, BOP, POL };
	public static readonly IEnumerable<Body> SPLASHABLE = new Body[] { EVE, KERBIN, LAYTHE };
	public static readonly IEnumerable<Body> WITH_ATMOSPHERE = new Body[] { EVE, KERBIN, DUNA, JOOL, LAYTHE };

	public readonly string name;
	public readonly string theName;

	private Body(string name) : this(name, name) {
	}

	private Body(string name, string theName) {
		this.name = name;
		this.theName = theName;
	}

	public bool isSame(CelestialBody b) {
		return name == b.name;
	}

	public CelestialBody getCelestialBody() {
		return FlightGlobals.Bodies.First(b => b.name == name);
	}

	public static Body get(CelestialBody b) {
		foreach (Body body in ALL) {
			if (body.isSame(b)) {
				return body;
			}
		}
		throw new ArgumentException("unknown body: " + b.name);
	}
}
