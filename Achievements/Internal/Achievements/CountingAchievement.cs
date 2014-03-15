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

namespace Achievements {
	internal abstract class CountingAchievement : AchievementBase {
		public readonly int minRequired;

		private int count;

		protected CountingAchievement(int minRequired) {
			this.minRequired = minRequired;
		}

		public override void init(ConfigNode node) {
			if (node.HasValue("count")) {
				count = int.Parse(node.GetValue("count"));
			}
		}

		public override void save(ConfigNode node) {
			if (node.HasValue("count")) {
				node.RemoveValue("count");
			}
			node.AddValue("count", count.ToString());
		}

		protected void increaseCounter() {
			count++;
		}

		protected void resetCounter() {
			count = 0;
		}

		public int getCount() {
			return count;
		}

		public override bool check(Vessel vessel) {
			return count >= minRequired;
		}
	}
}
