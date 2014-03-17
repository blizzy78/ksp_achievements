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
using UnityEngine;

namespace Achievements {
	internal class LandingFactory : AchievementFactory {
		public IEnumerable<Achievement> getAchievements() {
			List<Achievement> achievements = new List<Achievement>();
			achievements.AddRange(new Achievement[] {
				new Landing(false, -1),

				new BodyLanding(Body.MOHO, false, "Hot Foot"),
				new BodyLanding(Body.EVE, false, "Good Luck Getting Back"),
				new BodyLanding(Body.KERBIN, false, "Safe and Sound"),
				new BodyLanding(Body.DUNA, false, "One Giant Leap"),
				new BodyLanding(Body.DRES, false, "Asteroid Miner"),
				new BodyLanding(Body.EELOO, false, "Far Out"),
				new BodyLanding(Body.GILLY, false, "Lightfoot"),
				new BodyLanding(Body.MUN, false, "One Small Step"),
				new BodyLanding(Body.MINMUS, false, "Little Green Moon"),
				new BodyLanding(Body.IKE, false, "We Like Ike"),
				new BodyLanding(Body.LAYTHE, false, "Almost Feels Like Home"),
				new BodyLanding(Body.VALL, false, "Cold Stop"),
				new BodyLanding(Body.TYLO, false, "Who Needs Aerobraking?"),
				new BodyLanding(Body.BOP, false, "Kraken Hunter"),
				new BodyLanding(Body.POL, false, "Allergy Season"),

				new BodyLanding(Body.KERBIN, true, "Taking a Bath"),
				new BodyLanding(Body.EVE, true, "Looks Like Water, Feels Like Water"),
				new BodyLanding(Body.LAYTHE, true, "Just As Wet As At Home")
			});

			foreach (Body body in Body.ALL_LANDABLE.Where(b => !b.isStock())) {
				achievements.Add(new BodyLanding(body, false, "One Small Step - " + body.name).addon());
			}
			foreach (Body body in Body.ALL_SPLASHABLE.Where(b => !b.isStock())) {
				achievements.Add(new BodyLanding(body, true, "Taking a Bath - " + body.name).addon());
			}

			achievements.AddRange(new Achievement[] {
				new AllCrewAliveLanding(false,
					"I Think We Made It", "Land on the surface of a celestial body with all crew members still alive.", "landing.allCrewAlive"),
				new AllCrewAliveLanding(true,
					"That Was Close", "Abort a launch and land with all crew members still alive.", "landing.allCrewAlive.abort"),
				new EnginesDestroyedLanding(),

				new BodyLanding(Body.MUN, false, true, -1, -1, new Location[] {
					new Location(Body.MUN, 10.886.south(), 81.182.east(), 44000),
					new Location(Body.MUN, 11.260.north(), 22.229.east(), 54000),
					new Location(Body.MUN, 38.988.south(), 4.574.east(), 42000),
					new Location(Body.MUN, 61.936.north(), 32.985.west(), 48000),
					new Location(Body.MUN, 2.063.north(), 56.534.west(), 39000),
					new Location(Body.MUN, 5.672.north(), 151.283.west(), 37000)
				}, "Deep Impact", "Land inside one of the big craters on the Mun.", "landing.mun.crater"),
				new BodyLanding(Body.KERBIN, false, true, -1, -1, new Location[] { Location.KSC }, "Home Sweet Home", "Land at the Kerbal Space Center.", "landing.ksc"),
				new BodyLanding(Body.KERBIN, false, true, -1, -1, new Location[] { Location.KERBIN_NORTH_POLE, Location.KERBIN_SOUTH_POLE },
					"I'm Freezing Out Here", "Land on the north or south pole of Kerbin.", "landing.kerbin.pole"),
				new BodyLanding(Body.KERBIN, false, false, 10000, -1, new Location[] { Location.KSC_LAUNCH_PAD },
					"Grasshopper", "Land on the Kerbal Space Center launch pad from an altitude of at least 10000 m.", "landing.kscLaunchPad"),
				new BodyLanding(Body.KERBIN, false, false, 10000, -1, new Location[] { Location.KSC_HELICOPTER_PAD },
					"Chopper License", "Land on the Kerbal Space Center helicopter pad from an altitude of at least 10000 m.", "landing.kscHelicopterPad"),
				new BodyLanding(Body.KERBIN, false, false, 10000, -1, new Location[] { Location.KSC_RUNWAY },
					"Pilot License", "Land on the Kerbal Space Center runway from an altitude of at least 10000 m.", "landing.kscRunway"),
				new BodyLanding(Body.KERBIN, false, false, 10000, -1, new Location[] { Location.ISLAND_RUNWAY },
					"Not As Bad As It Looks", "Land on the island runway from an altitude of at least 10000 m.", "landing.islandRunway"),
				new BodyLanding(Body.MUN, false, true, -1, -1, new Location[] { Location.ARMSTRONG_MEMORIAL },
					"First... Not", "Land at the Armstrong Memorial.", "landing.armstrongMemorial").hide(),

				new BodyLanding(Body.INACCESSABLE, false, true, -1, 5, new Location[0], "I See What You Did There",
					"Land near the equator on Inaccessable.", "landing.inaccessable.equator").addon()
			});

			return achievements;
		}

		public Category getCategory() {
			return Category.LANDING;
		}
	}

	internal class Landing : AchievementBase {
		private bool stableOrbit;
		private double minAltitude;
		private bool flyingStep;
		private bool stableOrbitStep;
		private bool minAltitudeStep;

		internal Landing(bool stableOrbit, double minAltitude) {
			this.stableOrbit = stableOrbit;
			this.minAltitude = minAltitude;

			registerOnVesselChange(reset);
		}

		private void reset(Vessel vessel) {
			flyingStep = false;
			stableOrbitStep = false;
			minAltitudeStep = false;
		}

		public override bool check(Vessel vessel) {
			if ((vessel != null) && !vessel.isEVA()) {
				if (!flyingStep) {
					flyingStep = !vessel.isOnSurface();
				}

				if (!stableOrbitStep) {
					stableOrbitStep = !stableOrbit || vessel.isInStableOrbit();
				}

				if (!minAltitudeStep) {
					minAltitudeStep = (minAltitude < 0) || (vessel.altitude >= minAltitude);
				}

				return flyingStep && stableOrbitStep && minAltitudeStep && vessel.isOnSurface() && (vessel.horizontalSrfSpeed < 1d);
			} else {
				return false;
			}
		}

		public override string getTitle() {
			return "It's One Small Step";
		}

		public override string getText() {
			return "Land on the surface of a celestial body.";
		}

		public override string getKey() {
			return "landing";
		}
	}

	internal class BodyLanding : Landing {
		private Body body;
		private bool splash;
		private double maxDegreesLatitudeFromEquator;
		private IEnumerable<Location> locations;
		private string title;
		private string text;
		private string key;

		internal BodyLanding(Body body, bool splash, string title)
			: this(body, splash, false, -1, -1, new Location[0], title,
				splash ? "Splash into an ocean on the surface of " + body.theName + "." : "Land on the surface of " + body.theName + ".",
				splash ? "landing.splash." + body.name : "landing." + body.name) {

			this.body = body;
			this.splash = splash;
			this.title = title;
		}

		internal BodyLanding(Body body, bool splash, bool stableOrbit, double minAltitude, double maxDegreesLatitudeFromEquator,
			IEnumerable<Location> locations, string title, string text, string key)
			: base(stableOrbit, minAltitude) {

			this.body = body;
			this.splash = splash;
			this.maxDegreesLatitudeFromEquator = maxDegreesLatitudeFromEquator;
			this.locations = locations;
			this.title = title;
			this.text = text;
			this.key = key;
		}

		public override bool check(Vessel vessel) {
			return base.check(vessel) &&
				vessel.getCurrentBody().Equals(body) &&
				(splash ? vessel.isSplashed() : vessel.isLanded()) &&
				((maxDegreesLatitudeFromEquator < 0) || (Math.Abs(vessel.latitude) <= maxDegreesLatitudeFromEquator)) &&
				isAtLocation(vessel);
		}

		private bool isAtLocation(Vessel vessel) {
			if (locations.Count() > 0) {
				foreach (Location location in locations) {
					if (location.isAtLocation(vessel)) {
						return true;
					}
				}
				return false;
			} else {
				return true;
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

	internal class AllCrewAliveLanding : AchievementBase {
		private bool mustUseAbort;
		private string title;
		private string text;
		private string key;
		private bool flyingStep;
		private int crewCount;
		private bool abortStep;

		internal AllCrewAliveLanding(bool mustUseAbort, string title, string text, string key) {
			this.mustUseAbort = mustUseAbort;
			this.title = title;
			this.text = text;
			this.key = key;

			registerOnVesselChange(reset);
		}

		private void reset(Vessel vessel) {
			flyingStep = false;
		}

		public override bool check(Vessel vessel) {
			if ((vessel != null) && !vessel.isEVA()) {
				if (!flyingStep) {
					crewCount = vessel.GetCrewCount();
					flyingStep = !vessel.isOnSurface() && (crewCount > 0);
				}

				if (!abortStep) {
					abortStep = !mustUseAbort || vessel.ActionGroups[KSPActionGroup.Abort];
				}

				return flyingStep && abortStep && vessel.isOnSurface() && (vessel.horizontalSrfSpeed < 1d) && (vessel.GetCrewCount() == crewCount);
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

	internal class EnginesDestroyedLanding : Landing {
		private bool enginesStep;

		internal EnginesDestroyedLanding()
			: base(true, -1) {
			registerOnVesselChange(onVesselChange);
			registerOnPartRemove(onPartRemove);
		}

		private void onVesselChange(Vessel vessel) {
			reset();
		}

		private void onPartRemove(GameEvents.HostTargetAction<Part, Part> action) {
			if (!action.target.isEngine()) {
				reset();
			}
		}

		private void reset() {
			enginesStep = false;
		}

		public override bool check(Vessel vessel) {
			if (vessel != null) {
				if (!enginesStep) {
					enginesStep = vessel.getEnginesCount() > 0;
				}

				return base.check(vessel) && enginesStep && (vessel.getEnginesCount() == 0) && !vessel.getCurrentBody().Equals(Body.KERBIN);
			} else {
				return false;
			}
		}

		public override string getTitle() {
			return "I Live Here Now";
		}

		public override string getText() {
			return "Land on the surface of another celestial body with all engines destroyed.";
		}

		public override string getKey() {
			return "landing.enginesDestroyed";
		}
	}
}
