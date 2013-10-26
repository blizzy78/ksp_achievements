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
using UnityEngine;

class AltitudeFactory : AchievementFactory {
	public IEnumerable<Achievement> getAchievements() {
		return new Achievement[] {
			new Altitude(Body.KERBIN, 10000d, 11000d,
				"I'm Flying!", "Fly up to an altitude of 10000 m above the surface of Kerbin.", "flight.altitude." + Body.KERBIN.name + ".10000"),
			new Altitude(Body.SUN, 0d, 1000000d, "Sunburn", "Get within 1000 km to the Sun.", "flight.altitude." + Body.SUN.name + ".1000000"),
			new AboveAtmosphere(Body.EVE, 96708d, 97708d),
			new AboveAtmosphere(Body.KERBIN, 69077d, 70077d),
			new AboveAtmosphere(Body.DUNA, 41446d, 42446d),
			new AboveAtmosphere(Body.LAYTHE, 55262d, 56262d)
		};
	}

	public Category getCategory() {
		return Category.GENERAL_FLIGHT;
	}
}

class Altitude : AchievementBase {
	private double minAltitude;
	private double maxAltitude;
	private bool onSurfaceStep;
	private string title;
	private string text;
	private string key;

	protected Body body;

	public Altitude(Body body, double minAltitude, double maxAltitude, string title, string text, string key) {
		this.body = body;
		this.minAltitude = minAltitude;
		this.maxAltitude = maxAltitude;
		this.title = title;
		this.text = text;
		this.key = key;

		registerOnVesselChange(reset);
	}

	private void reset(Vessel vessel) {
		onSurfaceStep = false;
	}

	public override bool check(Vessel vessel) {
		if (vessel != null) {
			Body currentBody = vessel.getCurrentBody();

			if (!onSurfaceStep) {
				onSurfaceStep = vessel.isOnSurface() && currentBody.Equals(body);
			}

			return onSurfaceStep && currentBody.Equals(body) && vessel.altitude.between(minAltitude, maxAltitude);
		} else {
			return false;
		}
	}

	public override string getTitle() {
		return title;
	}

	public override string getText() {
		return text;
	}

	public override string getKey() {
		return key;
	}
}

class AboveAtmosphere : Altitude {
	public AboveAtmosphere(Body body, double minAltitude, double maxAltitude)
		: base(body, minAltitude, maxAltitude, "The Air Is Getting Thin Up Here - " + body.name,
			"Fly above the atmosphere of " + body.theName, "flight.aboveAtmosphere." + body.name) {
	}
}
