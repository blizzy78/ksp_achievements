﻿/*
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
	internal class DockingFactory : AchievementFactory {
		public IEnumerable<Achievement> getAchievements() {
				return new Achievement[] {
				new Docking(Docking.Mode.ORBIT, "We're Meant to Be Together", "Perform a docking maneuver in orbit.", "docking")
			};
		}

		public Category getCategory() {
			return Category.GENERAL_FLIGHT;
		}
	}

	internal class SurfaceDockingFactory : AchievementFactory {
		public IEnumerable<Achievement> getAchievements() {
				return new Achievement[] {
				new Docking(Docking.Mode.SURFACE, "Base Builder", "Perform a docking maneuver on the surface of another planet or moon.", "docking.surface")
			};
		}

		public Category getCategory() {
			return Category.GROUND_OPERATIONS;
		}
	}

	internal class Docking : AchievementBase {
		internal enum Mode {
			SURFACE, ORBIT
		}

		private Mode mode;
		private string title;
		private string text;
		private string key;
		private bool dockStep;

		internal Docking(Mode mode, string title, string text, string key) {
			this.mode = mode;
			this.title = title;
			this.text = text;
			this.key = key;

			registerOnPartCouple(onPartCouple);
		}

		public override bool check(Vessel vessel) {
			return dockStep;
		}

		public void onPartCouple(GameEvents.FromToAction<Part, Part> action) {
			Vessel vessel = action.from.vessel;
			if (action.from.isDockingPort() && action.to.isDockingPort()) {
				switch (mode) {
					case Mode.SURFACE:
						dockStep = vessel.isOnSurface() && !vessel.getCurrentBody().Equals(Body.KERBIN);
						break;
					case Mode.ORBIT:
						dockStep = vessel.isInStableOrbit();
						break;
				}
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
