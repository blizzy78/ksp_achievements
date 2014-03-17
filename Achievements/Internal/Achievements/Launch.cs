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
	internal class LaunchFactory : AchievementFactory {
		public IEnumerable<Achievement> getAchievements() {
			return new Achievement[] {
				new Launch(1, "Up, Up, and Away"),
				new Launch(10, "10 Launches"),
				new Launch(100, "100 Launches"),
				new Launch(1000, "1000 Launches")
			};
		}

		public Category getCategory() {
			return Category.GENERAL_FLIGHT;
		}
	}

	internal class Launch : CountingAchievement {
		private string title;

		internal Launch(int minRequired, string title)
			: base(minRequired) {
			this.title = title;

			registerOnLaunch(onLaunch);
		}

		private void onLaunch(EventReport report) {
			increaseCounter();
		}

		public override string getTitle() {
			return title;
		}

		public override string getText() {
			return (minRequired == 1) ? "Launch a vessel." : "Launch " + minRequired.ToString("D0") + " vessels.";
		}

		public override string getKey() {
			return "launch." + minRequired;
		}
	}
}
