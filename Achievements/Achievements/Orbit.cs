﻿/*
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
using System.Numeric;
using UnityEngine;

class OrbitFactory : AchievementFactory {
	public IEnumerable<Achievement> getAchievements() {
		List<Achievement> achievements = new List<Achievement>();
		foreach (Body body in Body.ALL) {
			achievements.Add(new BodyOrbit(body));
		}
		achievements.AddRange(new Achievement[] {
			new OrbitAchievement(),
			new ExtraKerbalPlanetOrbit(),
			new MoonOrbit(),
			new KesslerSyndrome(),
			new SSTO()
		});
		return achievements;
	}

	public Category getCategory() {
		return Category.SPACEFLIGHT;
	}
}

class OrbitAchievement : AchievementBase {
	public override bool check(Vessel vessel) {
		return (vessel != null) && vessel.isInStableOrbit();
	}

	public override string getTitle() {
		return "Going Around and Around";
	}

	public override string getText() {
		return "Get into a stable orbit around a celestial body.";
	}

	public override string getKey() {
		return "stableOrbit";
	}
}

class BodyOrbit : OrbitAchievement {
	private Body body;

	public BodyOrbit(Body body) {
		this.body = body;
	}

	public override bool check(Vessel vessel) {
		return base.check(vessel) && vessel.getCurrentBody().Equals(body);
	}

	public override string getTitle() {
		return "Going Around and Around - " + body.name;
	}

	public override string getText() {
		return "Get into a stable orbit around " + body.theName + ".";
	}

	public override string getKey() {
		return "stableOrbit." + body.name;
	}
}

abstract class AnyBodyOrbit : OrbitAchievement {
	private IEnumerable<Body> bodies;

	public AnyBodyOrbit(IEnumerable<Body> bodies) {
		this.bodies = bodies;
	}

	public override bool check(Vessel vessel) {
		return base.check(vessel) && bodies.Contains(vessel.getCurrentBody());
	}
}

class ExtraKerbalPlanetOrbit : AnyBodyOrbit {
	public ExtraKerbalPlanetOrbit() : base(Body.PLANETS_WITHOUT_KERBIN) {
	}

	public override string getTitle() {
		return "It's Round, Too";
	}

	public override string getText() {
		return "Get into a stable orbit around another planet.";
	}

	public override string getKey() {
		return "stableOrbit.extraKerbalPlanet";
	}
}

class MoonOrbit : AnyBodyOrbit {
	public MoonOrbit() : base(Body.MOONS) {
	}

	public override string getTitle() {
		return "Little Round Rock";
	}

	public override string getText() {
		return "Get into a stable orbit around a moon.";
	}

	public override string getKey() {
		return "stableOrbit.moon";
	}
}

class KesslerSyndrome : CountingAchievement {
	private bool counted;

	public KesslerSyndrome() : base(100) {
		registerOnVesselCreate(onVesselCreate);
	}

	private void onVesselCreate(Vessel vessel) {
		counted = false;
	}

	public override bool check(Vessel vessel) {
		if (FlightGlobals.fetch != null) {
			if (!counted) {
				resetCounter();
				foreach (Vessel v in FlightGlobals.Vessels) {
					if ((v.vesselType == VesselType.Debris) && v.getCurrentBody().Equals(Body.KERBIN) && v.isInStableOrbit()) {
						increaseCounter();
					}
				}
				counted = true;
			}

			return base.check(vessel);
		} else {
			return false;
		}
	}

	public override string getTitle() {
		return "Kessler Syndrome";
	}

	public override string getText() {
		return "Have 100 debris objects in orbit around Kerbin.";
	}

	public override string getKey() {
		return "kesslerSyndrome";
	}
}

class SSTO : OrbitAchievement {
	private bool preLaunchStep;
	private int numParts;

	public SSTO() {
		registerOnVesselChange(onVesselChange);
	}

	private void onVesselChange(Vessel vessel) {
	}

	public override bool check(Vessel vessel) {
		if (vessel != null) {
			if (!preLaunchStep) {
				preLaunchStep = vessel.isPreLaunched();

				if (preLaunchStep) {
					numParts = getNumParts(vessel);
				}
			}

			return base.check(vessel) && preLaunchStep && (getNumParts(vessel) == numParts) && vessel.getCurrentBody().Equals(Body.KERBIN);
		} else {
			return false;
		}
	}

	private int getNumParts(Vessel vessel) {
		IEnumerable<Part> launchClampParts = vessel.FindPartModulesImplementing<LaunchClamp>().ConvertAll(lc => lc.part);
		return vessel.parts.Count() - launchClampParts.Count();
	}

	public override string getTitle() {
		return "Step 1: Orbit";
	}

	public override string getText() {
		return "Get into a stable orbit without staging or losing any parts.";
	}

	public override string getKey() {
		return "stableOrbit.ssto";
	}
}