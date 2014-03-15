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
	internal class Toast {
		private const long SHOW_TIMEOUT = 10000;
		private const int NAV_BALL_HEIGHT = 189;

		private Achievement achievement;
		private AchievementEarn earn;
		private Rect rect;
		private long firstShown;

		internal Toast(Achievement achievement, AchievementEarn earn) {
			this.achievement = achievement;
			this.earn = earn;

			int height = AchievementGUI.TEX_HEIGHT + AchievementGUI.TEX_BORDER * 2;
			float top = (Screen.height / 2 - height) / 2 + Screen.height / 2;
			// make sure it doesn't block the nav ball
			top = Math.Min(top, Screen.height - NAV_BALL_HEIGHT - height - 20);
			rect = new Rect((Screen.width - AchievementGUI.TEX_WIDTH) / 2, top, AchievementGUI.TEX_WIDTH, height);
		}

		internal void draw() {
			GUILayout.BeginArea(rect);
			new AchievementGUI(achievement, earn).draw(false, false, false);
			GUILayout.EndArea();

			if (firstShown == 0L) {
				firstShown = DateTime.UtcNow.Ticks / 10000;
			}
		}

		internal bool isTimedOut() {
			if (firstShown > 0) {
				long now = DateTime.UtcNow.Ticks / 10000;
				return (now - firstShown) >= SHOW_TIMEOUT;
			} else {
				return false;
			}
		}
	}
}
