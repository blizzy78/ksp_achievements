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
using System.Numeric;
using UnityEngine;

namespace Achievements {
	internal class OrbitFactory : AchievementFactory {
		public IEnumerable<Achievement> getAchievements() {
			List<Achievement> achievements = new List<Achievement>();
			foreach (Body body in Body.ALL) {
				achievements.Add(new BodyOrbit(body).addon(!body.isStock()));
			}
			achievements.AddRange(new Achievement[] {
				new OrbitAchievement(),
				new ExtraKerbalPlanetOrbit(),
				new MoonOrbit(),
				new KesslerSyndrome(),
				new SSTO(),
				new JetPackOrbit(),
				new SunEscapeTrajectory(),
				new PreventAsteroidImpact(),
				new MoveAsteroidIntoStableOrbit()
			});
			return achievements;
		}

		public Category getCategory() {
			return Category.SPACEFLIGHT;
		}
	}

	internal class OrbitAchievement : AchievementBase {
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

	internal class BodyOrbit : OrbitAchievement {
		private Body body;

		internal BodyOrbit(Body body) {
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

	internal abstract class AnyBodyOrbit : OrbitAchievement {
		private IEnumerable<Body> bodies;

		internal AnyBodyOrbit(IEnumerable<Body> bodies) {
			this.bodies = bodies;
		}

		public override bool check(Vessel vessel) {
			return base.check(vessel) && bodies.Contains(vessel.getCurrentBody());
		}
	}

	internal class ExtraKerbalPlanetOrbit : AnyBodyOrbit {
		internal ExtraKerbalPlanetOrbit()
			: base(Body.ALL_PLANETS_WITHOUT_KERBIN) {
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

	internal class MoonOrbit : AnyBodyOrbit {
		internal MoonOrbit()
			: base(Body.ALL_MOONS) {
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

	internal class KesslerSyndrome : CountingAchievement {
		private bool counted;

		internal KesslerSyndrome()
			: base(100) {
			registerOnVesselCreate(onVesselCreate);
		}

		private void onVesselCreate(Vessel vessel) {
			counted = false;
		}

		public override bool check(Vessel vessel) {
			if (FlightGlobals.fetch != null) {
				if (!counted) {
					resetCounter();
					foreach (Vessel v in FlightGlobals.Vessels.Where((v =>
							(v.vesselType == VesselType.Debris) && v.getCurrentBody().Equals(Body.KERBIN) && v.isInStableOrbit()))) {

						increaseCounter();
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

	internal class SSTO : OrbitAchievement {
		private bool preLaunchStep;
		private int numParts;

		internal SSTO() {
			registerOnVesselChange(onVesselChange);
		}

		private void onVesselChange(Vessel vessel) {
		}

		public override bool check(Vessel vessel) {
			if (vessel != null) {
				if (!preLaunchStep) {
					preLaunchStep = vessel.isPreLaunched();
				}

				if (vessel.isPreLaunched()) {
					numParts = getNumParts(vessel);
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

	internal class JetPackOrbit : OrbitAchievement {
		private bool surfaceStep;

		internal JetPackOrbit() {
			registerOnVesselChange(onVesselChange);
		}

		private void onVesselChange(Vessel vessel) {
			surfaceStep = false;
		}

		public override bool check(Vessel vessel) {
			if ((vessel != null) && vessel.isEVA()) {
				if (!surfaceStep) {
					surfaceStep = vessel.isOnSurface();
				}

				return surfaceStep && base.check(vessel);
			} else {
				return false;
			}
		}

		public override string getTitle() {
			return "Who Needs Spaceships, Anyway?";
		}

		public override string getText() {
			return "Get into a stable orbit by only using the EVA jet pack.";
		}

		public override string getKey() {
			return "stableOrbit.jetPack";
		}
	}

	internal class SunEscapeTrajectory : AchievementBase {
		public override bool check(Vessel vessel) {
			return (vessel != null) && vessel.getCurrentBody().Equals(Body.SUN) &&
				(vessel.orbit.eccentricity >= 1d);
		}

		public override string getTitle() {
			return "One Way Trip";
		}

		public override string getText() {
			return "Get into an escape trajectory out of the Sun's sphere of influence.";
		}

		public override string getKey() {
			return "orbit.escapeSun";
		}
	}

	internal class PreventAsteroidImpact : AchievementBase {
		private bool impactStep;

		internal PreventAsteroidImpact() {
			registerOnVesselChange(onVesselChange);
		}

		private void onVesselChange(Vessel vessel) {
			impactStep = false;
		}

		public override bool check(Vessel vessel) {
			if ((vessel != null) && vessel.hasGrabbedAsteroid() && !vessel.isAsteroid()) {
				bool impact = vessel.isOnImpactTrajectory();

				if (!impactStep) {
					impactStep = impact;
				}

				return impactStep && !impact;
			} else {
				return false;
			}
		}

		public override string getTitle() {
			return "Is it a Plane? Is it a Bird?";
		}

		public override string getText() {
			return "Prevent an asteroid impact.";
		}

		public override string getKey() {
			return "orbit.asteroid.preventImpact";
		}
	}

	internal class MoveAsteroidIntoStableOrbit : OrbitAchievement {
		private bool escapeStep;

		internal MoveAsteroidIntoStableOrbit() {
			registerOnVesselChange(onVesselChange);
		}

		private void onVesselChange(Vessel vessel) {
			escapeStep = false;
		}

		public override bool check(Vessel vessel) {
			if ((vessel != null) && vessel.hasGrabbedAsteroid() && !vessel.isAsteroid() && !vessel.orbit.referenceBody.Equals(Body.SUN.getCelestialBody())) {
				bool escape = vessel.isOnEscapeTrajectory();

				if (!escapeStep) {
					escapeStep = escape;
				}

				return escapeStep && !escape && vessel.isInStableOrbit();
			} else {
				return false;
			}
		}

		public override string getTitle() {
			return "Once in a New Moon";
		}

		public override string getText() {
			return "Move an asteroid into a stable orbit around a celestial body.";
		}

		public override string getKey() {
			return "orbit.asteroid.moveIntoStableOrbit";
		}
	}
}
