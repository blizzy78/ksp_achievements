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
	internal class BodyEVAFactory : AchievementFactory {
		public IEnumerable<Achievement> getAchievements() {
			return new Achievement[] {
				new AllBodiesEVA(Body.STOCK_LANDABLE, "Steps in the Sand", "Set foot on every planet and moon.", "landing.allBodiesEVA"),
				new AllBodiesEVA(Body.SENTAR_LANDABLE, "More Steps in the Sand", "Set foot on every planet and moon in the Sentar and Serious systems.",
					"landing.allBodiesEVA.sentar").addon()
			};
		}

		public Category getCategory() {
			return Category.GROUND_OPERATIONS;
		}
	}

	internal class AllBodiesEVA : CountingAchievement {
		private IEnumerable<Body> bodies;
		private string title;
		private string text;
		private string key;
		private Dictionary<Body, bool> landedBodies = new Dictionary<Body, bool>();

		internal AllBodiesEVA(IEnumerable<Body> bodies, string title, string text, string key)
			: base(bodies.Count()) {
			this.bodies = bodies;
			this.title = title;
			this.text = text;
			this.key = key;
		}

		public override void init(ConfigNode node) {
			foreach (Body body in bodies) {
				bool landed = false;
				if (node.HasValue(body.name)) {
					landed = bool.Parse(node.GetValue(body.name));
				}
				landedBodies.Add(body, landed);
				if (landed) {
					increaseCounter();
				}
			}
		}

		public override void save(ConfigNode node) {
			foreach (Body body in landedBodies.Keys) {
				if (landedBodies[body]) {
					if (node.HasValue(body.name)) {
						node.RemoveValue(body.name);
					}
					node.AddValue(body.name, landedBodies[body].ToString());
				}
			}
		}

		public override bool check(Vessel vessel) {
			if ((vessel != null) && vessel.isEVA() && vessel.isOnSurface()) {
				Body body = vessel.getCurrentBody();
				if (bodies.Contains(body)) {
					if (landedBodies.ContainsKey(body)) {
						landedBodies.Remove(body);
					}
					landedBodies.Add(body, true);

					resetCounter();
					foreach (var x in landedBodies.Where(kv => kv.Value)) {
						increaseCounter();
					}
				}
			}
			return base.check(vessel);
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
