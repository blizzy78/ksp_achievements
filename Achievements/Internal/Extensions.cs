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
	internal static class Extensions {
		internal static bool between(this double d, double bound1, double bound2) {
			return ((d >= bound1) && (d <= bound2)) || ((d >= bound2) && (d <= bound1));
		}

		internal static bool isOnSurface(this Vessel vessel) {
			return (vessel.situation == Vessel.Situations.PRELAUNCH) || (vessel.situation == Vessel.Situations.LANDED) ||
				(vessel.situation == Vessel.Situations.SPLASHED);
		}

		internal static bool isLanded(this Vessel vessel) {
			return (vessel.situation == Vessel.Situations.LANDED) || (vessel.situation == Vessel.Situations.SPLASHED);
		}

		internal static bool isSplashed(this Vessel vessel) {
			return (vessel.situation == Vessel.Situations.SPLASHED);
		}

		internal static bool isPreLaunched(this Vessel vessel) {
			return (vessel.situation == Vessel.Situations.PRELAUNCH);
		}

		internal static Body getCurrentBody(this Vessel vessel) {
			return Body.get(vessel.mainBody);
		}

		internal static bool isEVA(this Vessel vessel) {
			return vessel.FindPartModulesImplementing<KerbalEVA>().Count() > 0;
		}

		internal static bool isInStableOrbit(this Vessel vessel) {
			Orbit orbit = vessel.orbit;
			float atmosphereAltitude = Math.Max(vessel.mainBody.maxAtmosphereAltitude, 0f);
			return (orbit.ApA > 0) && (orbit.PeA > 0) && (orbit.ApA > atmosphereAltitude) && (orbit.PeA > atmosphereAltitude);
		}

		internal static bool hasSurfaceSample(this Vessel vessel) {
			return vessel.FindPartModulesImplementing<ModuleScienceExperiment>().Any(e =>
					(e.experimentID == "surfaceSample") &&
					e.GetData().Any(d => (d != null) && (d.dataAmount > 0)));
		}

		internal static int getValuesCount(this Dictionary<Category, IEnumerable<Achievement>> achievements) {
			int count = 0;
			foreach (IEnumerable<Achievement> categoryAchievements in achievements.Values) {
				count += categoryAchievements.Count();
			}
			return count;
		}

		internal static double south(this double d) {
			return -d;
		}

		internal static double north(this double d) {
			return d;
		}

		internal static double west(this double d) {
			return -d;
		}

		internal static double east(this double d) {
			return d;
		}

		internal static bool isEngine(this Part part) {
			return part.Modules.OfType<ModuleEngines>().FirstOrDefault() != null;
		}
	}
}
