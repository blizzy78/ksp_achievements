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

class DockingFactory : AchievementFactory {
	public IEnumerable<Achievement> getAchievements() {
		return new Achievement[] {
			new Docking()
		};
	}

	public Category getCategory() {
		return Category.GENERAL_FLIGHT;
	}
}

class Docking : AchievementBase {
	private bool dockStep;

	public Docking() {
		registerOnPartCouple(onPartCouple);
	}

	public override bool check(Vessel vessel) {
		return dockStep;
	}

	public void onPartCouple(GameEvents.FromToAction<Part, Part> action) {
		dockStep = true;
	}

	public override string getTitle() {
		return "We're Meant to Be Together";
	}

	public override string getText() {
		return "Perform a docking maneuver.";
	}

	public override string getKey() {
		return "docking";
	}
}
