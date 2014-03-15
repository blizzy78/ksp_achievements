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
	internal static class VesselExtensions {
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
			return vessel.hasModule<KerbalEVA>() && vessel.Parts.Count() == 1;
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

		internal static int getEnginesCount(this Vessel vessel) {
			return vessel.FindPartModulesImplementing<ModuleEngines>().Count();
		}

		internal static bool hasModule<T>(this Vessel vessel) where T : PartModule {
			return vessel.FindPartModulesImplementing<T>().FirstOrDefault() != null;
		}
	}
}
