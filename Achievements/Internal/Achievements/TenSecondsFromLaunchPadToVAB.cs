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
	internal class TenSecondsFromLaunchPadToVABFactory : AchievementFactory {
		public IEnumerable<Achievement> getAchievements() {
			return new Achievement[] {
				new TenSecondsFromLaunchPadToVAB()
			};
		}

		public Category getCategory() {
			return Category.GENERAL_FLIGHT;
		}
	}

	internal class TenSecondsFromLaunchPadToVAB : AchievementBase {
		private bool preLaunchStep;
		private bool landedStep;
		private double landedMissionTime;

		internal TenSecondsFromLaunchPadToVAB() {
			registerOnVesselChange(onVesselChange);

			// must react to situation change immediately because the usual achievements checking interval is too coarse
			registerOnVesselSituationChange(onVesselSituationChange);
		}

		private void onVesselChange(Vessel vessel) {
			preLaunchStep = false;
			landedStep = false;
			landedMissionTime = 0;
		}

		private void onVesselSituationChange(GameEvents.HostedFromToAction<Vessel, Vessel.Situations> action) {
			if (FlightGlobals.fetch != null) {
				Vessel vessel = FlightGlobals.ActiveVessel;
				landedStep = vessel.isLanded();
				if (vessel.isLanded()) {
					landedMissionTime = vessel.missionTime;
				}
			}
		}

		public override bool check(Vessel vessel) {
			if (vessel != null) {
				if (!preLaunchStep) {
					preLaunchStep = vessel.isPreLaunched() && Location.KSC_LAUNCH_PAD.isAtLocation(vessel) && (vessel.GetCrewCount() > 0);
				}

				return preLaunchStep && landedStep &&
					Location.KSC_HELICOPTER_PAD.isAtLocation(vessel) &&
					(vessel.horizontalSrfSpeed < 0.2d) && (vessel.GetCrewCount() > 0) &&
					(landedMissionTime < 10d);
			} else {
				return false;
			}
		}

		public override string getTitle() {
			return "Rapid Relocation";
		}

		public override string getText() {
			return "Launch a Kerbal from the launch pad and land him safely on the helicopter pad in under 10 s.";
		}

		public override string getKey() {
			return "landing.kscHelicopterPad.underTenSeconds";
		}
	}
}
