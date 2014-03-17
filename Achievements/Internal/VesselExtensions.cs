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
			return (vessel.Parts.Count() == 1) && vessel.hasModule<KerbalEVA>();
		}

		internal static bool isAsteroid(this Vessel vessel) {
			return (vessel.Parts.Count() == 1) && vessel.hasModule<ModuleAsteroid>();
		}

		internal static bool hasGrabbedAsteroid(this Vessel vessel) {
			return vessel.hasModule<ModuleAsteroid>();
		}

		internal static bool isInStableOrbit(this Vessel vessel) {
			Orbit orbit = vessel.orbit;
			float atmosphereAltitude = Math.Max(vessel.mainBody.maxAtmosphereAltitude, 0f);
			return (orbit.patchStartTransition == Orbit.PatchTransitionType.INITIAL) &&
				(orbit.patchEndTransition == Orbit.PatchTransitionType.FINAL) &&
				(orbit.ApA > 0) && (orbit.PeA > 0) && (orbit.ApA > atmosphereAltitude) && (orbit.PeA > atmosphereAltitude);
		}

		internal static bool isOnImpactTrajectory(this Vessel vessel) {
			return vessel.getOrbitPatches().Any(o => {
				try {
					float atmosphereAltitude = Math.Max(o.referenceBody.maxAtmosphereAltitude, 0f);
					if (o.PeA <= atmosphereAltitude) {
						return true;
					}
				} catch (Exception) {
					// Orbit.PeA may throw NullReferenceException
				}
				return false;
			});
		}

		internal static bool isOnEscapeTrajectory(this Vessel vessel) {
			return vessel.orbit.patchEndTransition == Orbit.PatchTransitionType.ESCAPE;
		}

		private static IEnumerable<Orbit> getOrbitPatches(this Vessel vessel) {
			for (Orbit orbit = vessel.orbit; orbit != null; orbit = orbit.nextPatch) {
				yield return orbit;
			}
		}

		internal static bool hasSurfaceSample(this Vessel vessel) {
			return vessel.hasScienceExperiment("surfaceSample");
		}

		internal static bool hasAsteroidSample(this Vessel vessel) {
			return vessel.hasScienceExperiment("asteroidSample");
		}

		private static bool hasScienceExperiment(this Vessel vessel, string experimentId) {
			experimentId += "@";
			return vessel.getScienceDatas().Any(d => d.subjectID.StartsWith(experimentId) && (d.dataAmount > 0));
		}

		private static IEnumerable<ScienceData> getScienceDatas(this Vessel vessel) {
			return vessel.getScienceDatasFromExperiments().Union(vessel.getScienceDatasFromContainers());
		}

		private static IEnumerable<ScienceData> getScienceDatasFromExperiments(this Vessel vessel) {
			return vessel.FindPartModulesImplementing<ModuleScienceExperiment>().SelectMany(e => e.GetData()).Where(d => d != null);
		}

		private static IEnumerable<ScienceData> getScienceDatasFromContainers(this Vessel vessel) {
			return vessel.FindPartModulesImplementing<ModuleScienceContainer>().SelectMany(c => c.GetData()).Where(d => d != null);
		}

		internal static int getEnginesCount(this Vessel vessel) {
			return vessel.FindPartModulesImplementing<ModuleEngines>().Count();
		}

		internal static bool hasModule<T>(this Vessel vessel) where T : PartModule {
			return vessel.FindPartModulesImplementing<T>().FirstOrDefault() != null;
		}
	}
}
