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

class BodyEVAFactory : AchievementFactory {
	public IEnumerable<Achievement> getAchievements() {
		return new Achievement[] {
			new AllBodiesEVA()
		};
	}

	public Category getCategory() {
		return Category.GROUND_OPERATIONS;
	}
}

class AllBodiesEVA : CountingAchievement {
	private Dictionary<Body, bool> bodies = new Dictionary<Body, bool>();

	public AllBodiesEVA() : base(Body.LANDABLE.Count()) {
	}

	public override void init(ConfigNode node) {
		foreach (Body body in Body.LANDABLE) {
			bool landed = false;
			if (node.HasValue(body.name)) {
				landed = bool.Parse(node.GetValue(body.name));
			}
			bodies.Add(body, landed);
			if (landed) {
				increaseCounter();
			}
		}
	}

	public override void save(ConfigNode node) {
		foreach (Body body in bodies.Keys) {
			if (bodies[body]) {
				if (node.HasValue(body.name)) {
					node.RemoveValue(body.name);
				}
				node.AddValue(body.name, bodies[body].ToString());
			}
		}
	}

	public override bool check(Vessel vessel) {
		if ((vessel != null) && vessel.isEVA() && vessel.isOnSurface()) {
			Body body = vessel.getCurrentBody();
			if (bodies.ContainsKey(body)) {
				bodies.Remove(body);
			}
			bodies.Add(body, true);

			resetCounter();
			foreach (var x in bodies.Where(kv => kv.Value)) {
				increaseCounter();
			}
		}
		return base.check(vessel);
	}

	public override string getTitle() {
		return "Steps in the Sand";
	}

	public override string getText() {
		return "Set foot on every planet and moon.";
	}

	public override string getKey() {
		return "landing.allBodiesEVA";
	}
}
