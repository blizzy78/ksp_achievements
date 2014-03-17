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
	internal class KillFactory : AchievementFactory {
		public IEnumerable<Achievement> getAchievements() {
			return new Achievement[] {
				new Kill(1, "Ouch!", "Kill a crew member.", true),
				new Kill(10, "Bloodbath", "Kill 10 crew members in a single incident.", true),
				new Kill(10, "Careless", "Kill 10 crew members.", false),
				new Kill(100, "Death Trap", "Kill 100 crew members.", false),
				new KillJebediahAgain().hide()
			};
		}

		public Category getCategory() {
			return Category.CREW_OPERATIONS;
		}
	}

	internal class Kill : CountingAchievement {
		private string title;
		private string text;
		private bool resetOnVesselChange;
		private HashSet<string> killedCrewNames = new HashSet<string>();

		internal Kill(int minKilled, string title, string text, bool resetOnVesselChange)
			: base(minKilled) {
			this.title = title;
			this.text = text;
			this.resetOnVesselChange = resetOnVesselChange;

			if (resetOnVesselChange) {
				registerOnVesselChange(reset);
			}
			registerOnCrewKilled(onCrewKilled);
		}

		private void reset(Vessel vessel) {
			resetCounter();
			killedCrewNames.Clear();
		}

		private void onCrewKilled(EventReport report) {
			string crewName = report.sender;
			// make sure to not double-count
			if (!killedCrewNames.Contains(crewName)) {
				killedCrewNames.Add(crewName);

				increaseCounter();
			}
		}

		public override string getTitle() {
			return title;
		}

		public override string getText() {
			return text;
		}

		public override string getKey() {
			return resetOnVesselChange ? "kill.singleIncident." + minRequired : "kill." + minRequired;
		}
	}

	internal class KillJebediahAgain : CountingAchievement {
		private bool killed;

		internal KillJebediahAgain()
			: base(2) {
			registerOnVesselChange(onVesselChange);
			registerOnCrewKilled(onCrewKilled);
		}

		private void onVesselChange(Vessel vessel) {
			killed = false;
		}

		private void onCrewKilled(EventReport report) {
			string crewName = report.sender;
			if (crewName == "Jebediah Kerman") {
				// make sure to not double-count
				if (!killed) {
					increaseCounter();
					killed = true;
				}
			}
		}

		public override string getTitle() {
			return "What? Again?";
		}

		public override string getText() {
			return "Kill Jebediah Kerman more than once.";
		}

		public override string getKey() {
			return "kill.jebediahAgain";
		}
	}
}
