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

namespace Achievements {
	internal class Body {
		internal static readonly Body SUN = new Body("Sun", "the Sun");

		internal static readonly Body MOHO = new Body("Moho");
		internal static readonly Body EVE = new Body("Eve");
		internal static readonly Body KERBIN = new Body("Kerbin");
		internal static readonly Body DUNA = new Body("Duna");
		internal static readonly Body DRES = new Body("Dres");
		internal static readonly Body JOOL = new Body("Jool");
		internal static readonly Body EELOO = new Body("Eeloo");

		internal static readonly Body GILLY = new Body("Gilly");
		internal static readonly Body MUN = new Body("Mun", "the Mun");
		internal static readonly Body MINMUS = new Body("Minmus");
		internal static readonly Body IKE = new Body("Ike");
		internal static readonly Body LAYTHE = new Body("Laythe");
		internal static readonly Body VALL = new Body("Vall");
		internal static readonly Body TYLO = new Body("Tylo");
		internal static readonly Body BOP = new Body("Bop");
		internal static readonly Body POL = new Body("Pol");

		internal static readonly Body ABLATE = new Body("Ablate");
		internal static readonly Body ASCENSION = new Body("Ascension");
		internal static readonly Body ERIN = new Body("Erin");
		internal static readonly Body INACCESSABLE = new Body("Inaccessable");
		internal static readonly Body POCK = new Body("Pock");
		internal static readonly Body RINGLE = new Body("Ringle");
		internal static readonly Body SENTAR = new Body("Sentar");
		internal static readonly Body SKELTON = new Body("Skelton");
		internal static readonly Body THUD = new Body("Thud");
		internal static readonly Body SERIOUS = new Body("Serious");
		internal static readonly Body JOKER = new Body("Joker");

		internal static readonly IEnumerable<Body> STOCK_ALL = new Body[] { SUN, MOHO, EVE, KERBIN, DUNA, DRES, JOOL, EELOO, GILLY, MUN, MINMUS, IKE, LAYTHE, VALL, TYLO, BOP, POL };
		internal static readonly IEnumerable<Body> STOCK_PLANETS = new Body[] { MOHO, EVE, KERBIN, DUNA, DRES, JOOL, EELOO };
		internal static readonly IEnumerable<Body> STOCK_PLANETS_WITHOUT_KERBIN = new Body[] { MOHO, EVE, DUNA, DRES, JOOL, EELOO };
		internal static readonly IEnumerable<Body> STOCK_MOONS = new Body[] { GILLY, MUN, MINMUS, IKE, LAYTHE, VALL, TYLO, BOP, POL };
		internal static readonly IEnumerable<Body> STOCK_LANDABLE = new Body[] { MOHO, EVE, KERBIN, DUNA, DRES, EELOO, GILLY, MUN, MINMUS, IKE, LAYTHE, VALL, TYLO, BOP, POL };
		internal static readonly IEnumerable<Body> STOCK_SPLASHABLE = new Body[] { EVE, KERBIN, LAYTHE };
		internal static readonly IEnumerable<Body> STOCK_WITH_ATMOSPHERE = new Body[] { EVE, KERBIN, DUNA, JOOL, LAYTHE };

		internal static readonly IEnumerable<Body> SENTAR_ALL = new Body[] { ABLATE, ASCENSION, ERIN, INACCESSABLE, POCK, RINGLE, SENTAR, SKELTON, THUD, SERIOUS, JOKER };
		internal static readonly IEnumerable<Body> SENTAR_PLANETS = new Body[] { ABLATE, ASCENSION, ERIN, INACCESSABLE, RINGLE, SENTAR, SKELTON, THUD, JOKER };
		internal static readonly IEnumerable<Body> SENTAR_MOONS = new Body[] { POCK };
		internal static readonly IEnumerable<Body> SENTAR_LANDABLE = new Body[] { ABLATE, ASCENSION, ERIN, INACCESSABLE, POCK, RINGLE, SKELTON, THUD, JOKER };
		internal static readonly IEnumerable<Body> SENTAR_SPLASHABLE = new Body[] { ERIN };
		internal static readonly IEnumerable<Body> SENTAR_WITH_ATMOSPHERE = new Body[] { ERIN, SENTAR, SKELTON };

		internal static readonly IEnumerable<Body> ALL = flatten(STOCK_ALL, SENTAR_ALL);
		internal static readonly IEnumerable<Body> ALL_PLANETS = flatten(STOCK_PLANETS, SENTAR_PLANETS);
		internal static readonly IEnumerable<Body> ALL_PLANETS_WITHOUT_KERBIN = flatten(STOCK_PLANETS_WITHOUT_KERBIN, SENTAR_PLANETS);
		internal static readonly IEnumerable<Body> ALL_MOONS = flatten(STOCK_MOONS, SENTAR_MOONS);
		internal static readonly IEnumerable<Body> ALL_LANDABLE = flatten(STOCK_LANDABLE, SENTAR_LANDABLE);
		internal static readonly IEnumerable<Body> ALL_SPLASHABLE = flatten(STOCK_SPLASHABLE, SENTAR_SPLASHABLE);
		internal static readonly IEnumerable<Body> ALL_WITH_ATMOSPHERE = flatten(STOCK_WITH_ATMOSPHERE, SENTAR_WITH_ATMOSPHERE);

		internal readonly string name;
		internal readonly string theName;

		private Body(string name)
			: this(name, name) {
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

		internal bool isSame(CelestialBody b) {
			return name == b.name;
		}

		internal bool isStock() {
			return STOCK_ALL.Contains(this);
		}

		internal CelestialBody getCelestialBody() {
			return FlightGlobals.Bodies.First(b => b.name == name);
		}

		internal static Body get(CelestialBody b) {
			return ALL.Single(body => body.isSame(b));
		}
	}
}
