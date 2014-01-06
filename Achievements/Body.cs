/*
Achievements - Brings achievements to Kerbal Space Program.
Copyright (C) 2013-2014 Maik Schreiber

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

	public static readonly Body ABLATE = new Body("Ablate");
	public static readonly Body ASCENSION = new Body("Ascension");
	public static readonly Body ERIN = new Body("Erin");
	public static readonly Body INACCESSABLE = new Body("Inaccessable");
	public static readonly Body POCK = new Body("Pock");
	public static readonly Body RINGLE = new Body("Ringle");
	public static readonly Body SENTAR = new Body("Sentar");
	public static readonly Body SKELTON = new Body("Skelton");
	public static readonly Body THUD = new Body("Thud");
	public static readonly Body SERIOUS = new Body("Serious");
	public static readonly Body JOKER = new Body("Joker");

	public static readonly IEnumerable<Body> STOCK_ALL = new Body[] { SUN, MOHO, EVE, KERBIN, DUNA, DRES, JOOL, EELOO, GILLY, MUN, MINMUS, IKE, LAYTHE, VALL, TYLO, BOP, POL };
	public static readonly IEnumerable<Body> STOCK_PLANETS = new Body[] { MOHO, EVE, KERBIN, DUNA, DRES, JOOL, EELOO };
	public static readonly IEnumerable<Body> STOCK_PLANETS_WITHOUT_KERBIN = new Body[] { MOHO, EVE, DUNA, DRES, JOOL, EELOO };
	public static readonly IEnumerable<Body> STOCK_MOONS = new Body[] { GILLY, MUN, MINMUS, IKE, LAYTHE, VALL, TYLO, BOP, POL };
	public static readonly IEnumerable<Body> STOCK_LANDABLE = new Body[] { MOHO, EVE, KERBIN, DUNA, DRES, EELOO, GILLY, MUN, MINMUS, IKE, LAYTHE, VALL, TYLO, BOP, POL };
	public static readonly IEnumerable<Body> STOCK_SPLASHABLE = new Body[] { EVE, KERBIN, LAYTHE };
	public static readonly IEnumerable<Body> STOCK_WITH_ATMOSPHERE = new Body[] { EVE, KERBIN, DUNA, JOOL, LAYTHE };

	public static readonly IEnumerable<Body> SENTAR_ALL = new Body[] { ABLATE, ASCENSION, ERIN, INACCESSABLE, POCK, RINGLE, SENTAR, SKELTON, THUD, SERIOUS, JOKER };
	public static readonly IEnumerable<Body> SENTAR_PLANETS = new Body[] { ABLATE, ASCENSION, ERIN, INACCESSABLE, RINGLE, SENTAR, SKELTON, THUD, JOKER };
	public static readonly IEnumerable<Body> SENTAR_MOONS = new Body[] { POCK };
	public static readonly IEnumerable<Body> SENTAR_LANDABLE = new Body[] { ABLATE, ASCENSION, ERIN, INACCESSABLE, POCK, RINGLE, SKELTON, THUD, JOKER };
	public static readonly IEnumerable<Body> SENTAR_SPLASHABLE = new Body[] { ERIN };
	public static readonly IEnumerable<Body> SENTAR_WITH_ATMOSPHERE = new Body[] { ERIN, SENTAR, SKELTON };

	public static readonly IEnumerable<Body> ALL = flatten(STOCK_ALL, SENTAR_ALL);
	public static readonly IEnumerable<Body> ALL_PLANETS = flatten(STOCK_PLANETS, SENTAR_PLANETS);
	public static readonly IEnumerable<Body> ALL_PLANETS_WITHOUT_KERBIN = flatten(STOCK_PLANETS_WITHOUT_KERBIN, SENTAR_PLANETS);
	public static readonly IEnumerable<Body> ALL_MOONS = flatten(STOCK_MOONS, SENTAR_MOONS);
	public static readonly IEnumerable<Body> ALL_LANDABLE = flatten(STOCK_LANDABLE, SENTAR_LANDABLE);
	public static readonly IEnumerable<Body> ALL_SPLASHABLE = flatten(STOCK_SPLASHABLE, SENTAR_SPLASHABLE);
	public static readonly IEnumerable<Body> ALL_WITH_ATMOSPHERE = flatten(STOCK_WITH_ATMOSPHERE, SENTAR_WITH_ATMOSPHERE);

	public readonly string name;
	public readonly string theName;

	private Body(string name) : this(name, name) {
	}

	private Body(string name, string theName) {
		this.name = name;
		this.theName = theName;
	}

	private static IEnumerable<Body> flatten(params IEnumerable<Body>[] bodies) {
		List<Body> result = new List<Body>();
		foreach (IEnumerable<Body> bs in bodies) {
			result.AddRange(bs);
		}
		return result;
	}

	public bool isSame(CelestialBody b) {
		return name == b.name;
	}

	public bool isStock() {
		return STOCK_ALL.Contains(this);
	}

	public CelestialBody getCelestialBody() {
		return FlightGlobals.Bodies.First(b => b.name == name);
	}

	public static Body get(CelestialBody b) {
		return ALL.Single(body => body.isSame(b));
	}
}
