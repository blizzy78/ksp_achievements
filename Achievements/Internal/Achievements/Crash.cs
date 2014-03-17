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
	internal class CrashFactory : AchievementFactory {
		public IEnumerable<Achievement> getAchievements() {
			return new Achievement[] {
				new KSCBuildingCrash()
			};
		}

		public Category getCategory() {
			return Category.LANDING;
		}
	}

	internal class KSCBuildingCrash : AchievementBase {
		private static IEnumerable<string> BUILDING_NAMES = new string[] {
			"Space Plane Hangar",
			"Vehicle Assembly Building",
			"TrackingStationOffice",
			"Satellite Dish",
			"Mission Control",
			"Astronaunt Complex Building", // sic 0.21.1
			"Astronaut Complex Building" // anticipate forward compatibility
		};

		private bool crashStep;

		internal KSCBuildingCrash() {
			registerOnVesselChange(reset);
			registerOnCrash(onCrash);
		}

		private void reset(Vessel vessel) {
			crashStep = false;
		}

		public override bool check(Vessel vessel) {
			return crashStep;
		}

		public void onCrash(EventReport report) {
			if ((report.other != null) && BUILDING_NAMES.Contains(report.other)) {
				crashStep = true;
			}
		}

		public override string getTitle() {
			return "Buzzing the Tower";
		}

		public override string getText() {
			return "Crash into a building at the Kerbal Space Center.";
		}

		public override string getKey() {
			return "crash.kscBuilding";
		}
	}
}
