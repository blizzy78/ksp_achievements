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

class SurfaceSampleFactory : AchievementFactory {
	public IEnumerable<Achievement> getAchievements() {
		List<Achievement> achievements = new List<Achievement>();
		foreach (Body body in Body.LANDABLE) {
			achievements.Add(new BodySurfaceSample(body));
		}
		achievements.AddRange(new Achievement[] {
			new SurfaceSample(),
			new AllBodiesSurfaceSample()
		});
		return achievements;
	}

	public Category getCategory() {
		return Category.RESEARCH_AND_DEVELOPMENT;
	}
}

class SurfaceSample : AchievementBase {
	public override bool check(Vessel vessel) {
		return (vessel != null) && vessel.isEVA() && vessel.hasSurfaceSample();
	}

	public override string getTitle() {
		return "Yep, Looks Like Dirt";
	}

	public override string getText() {
		return "Take a surface sample.";
	}

	public override string getKey() {
		return "surfaceSample";
	}
}

class BodySurfaceSample : SurfaceSample {
	private Body body;

	public BodySurfaceSample(Body body) {
		this.body = body;
	}

	public override bool check(Vessel vessel) {
		return base.check(vessel) && vessel.getCurrentBody().Equals(body);
	}

	public override string getTitle() {
		return "Yep, Looks Like Dirt - " + body.name;
	}

	public override string getText() {
		return "Take a surface sample on " + body.theName + ".";
	}

	public override string getKey() {
		return "surfaceSample." + body.name;
	}
}

class AllBodiesSurfaceSample : CountingAchievement {
	private Dictionary<Body, bool> bodies = new Dictionary<Body, bool>();

	public AllBodiesSurfaceSample() : base(Body.LANDABLE.Count()) {
	}

	public override void init(ConfigNode node) {
		foreach (Body body in Body.LANDABLE) {
			bool surfaceSample = false;
			if (node.HasValue(body.name)) {
				surfaceSample = bool.Parse(node.GetValue(body.name));
			}
			bodies.Add(body, surfaceSample);
			if (surfaceSample) {
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
		if ((vessel != null) && vessel.isEVA() && vessel.hasSurfaceSample()) {
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
		return "Pile of Dirt";
	}

	public override string getText() {
		return "Take surface samples on all planets and moons.";
	}

	public override string getKey() {
		return "surfaceSample.allBodies";
	}
}
