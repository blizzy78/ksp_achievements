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
	internal class FlagFactory : AchievementFactory {
		public IEnumerable<Achievement> getAchievements() {
			return new Achievement[] {
				new ExtraKerbalFlag(),
				new AllBodiesFlags(Body.STOCK_LANDABLE, "All Your Base Are Belong to Us", "Plant a flag on every planet and moon.", "landing.allBodiesFlags"),
				new AllBodiesFlags(Body.SENTAR_LANDABLE, "All Your Base Are Belong to Us - Sentar", "Plant a flag on every planet and moon in the Sentar and Serious systems.",
					"landing.allBodiesFlags.sentar").addon()
			};
		}

		public Category getCategory() {
			return Category.GROUND_OPERATIONS;
		}
	}

	internal class ExtraKerbalFlag : AchievementBase {
		public override bool check(Vessel vessel) {
			if (FlightGlobals.fetch != null) {
				HashSet<Body> bodies = new HashSet<Body>();
				foreach (Vessel v in FlightGlobals.Vessels) {
					if ((v.vesselType == VesselType.Flag) && Body.ALL_PLANETS_WITHOUT_KERBIN.Contains(v.getCurrentBody())) {
						return true;
					}
				}
			}
			return false;
		}

		public override string getTitle() {
			return "I Claim This World in the Name Of";
		}

		public override string getText() {
			return "Plant a flag on another planet.";
		}

		public override string getKey() {
			return "landing.extraKerbalFlag";
		}
	}

	internal class AllBodiesFlags : CountingAchievement {
		private IEnumerable<Body> bodies;
		private string title;
		private string text;
		private string key;

		internal AllBodiesFlags(IEnumerable<Body> bodies, string title, string text, string key)
			: base(bodies.Count()) {
			this.bodies = bodies;
			this.title = title;
			this.text = text;
			this.key = key;
		}

		public override bool check(Vessel vessel) {
			if (FlightGlobals.fetch != null) {
				resetCounter();
				HashSet<Body> bodies = new HashSet<Body>();
				foreach (Vessel v in FlightGlobals.Vessels) {
					if (v.vesselType == VesselType.Flag) {
						Body body = v.getCurrentBody();
						if (this.bodies.Contains(body) && !bodies.Contains(body)) {
							increaseCounter();
							bodies.Add(body);
						}
					}
				}
				return base.check(vessel);
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
}
