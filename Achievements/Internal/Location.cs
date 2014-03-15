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
	internal class Location {
		internal static readonly Location KSC = new Location(Body.KERBIN, 0.097.south(), 74.557.west(), 2000);
		internal static readonly Location KSC_LAUNCH_PAD = new Location(Body.KERBIN, 0.09722.south(), 74.5575.west(), 15);
		internal static readonly Location KSC_HELICOPTER_PAD = new Location(Body.KERBIN, 0.09583.south(), 74.62111.west(), 0.0975.south(), 74.61611.west());
		internal static readonly Location KERBIN_NORTH_POLE = new Location(Body.KERBIN, 90d.north(), 0d.east(), 10000);
		internal static readonly Location KERBIN_SOUTH_POLE = new Location(Body.KERBIN, 90d.south(), 0d.east(), 10000);

		// this is compensated for the runway not being exactly parallel to the equator, leading to small areas outside of the actual runway
		internal static readonly Location KSC_RUNWAY = new Location(Body.KERBIN, 0.05277.south(), 74.72833.west(), 0.04555.south(), 74.48833.west());

		// encloses all the area with gravel on the ground
		internal static readonly Location ISLAND_RUNWAY = new Location(Body.KERBIN, 1.51111.south(), 71.96888.west(), 1.53583.south(), 71.85055.west());

		internal static readonly Location ARMSTRONG_MEMORIAL = new Location(Body.MUN, 0.7025.north(), 22.74694.east(), 100);

		private Body body;
		private double latitude;
		private double longitude;
		private double latitude2;
		private double longitude2;
		private double radius;
		private bool circle;

		internal Location(Body body, double latitude, double longitude, double radius) {
			this.body = body;
			this.latitude = latitude;
			this.longitude = longitude;
			this.radius = radius;

			circle = true;
		}

		// FIXME: does only work for rectangles where a side is exactly parallel to the equator
		internal Location(Body body, double latitude, double longitude, double latitude2, double longitude2) {
			this.body = body;
			this.latitude = latitude;
			this.longitude = longitude;
			this.latitude2 = latitude2;
			this.longitude2 = longitude2;

			circle = false;
		}

		internal bool isAtLocation(Vessel vessel) {
			if (vessel.getCurrentBody().Equals(body)) {
				if (circle) {
					CelestialBody celestialBody = body.getCelestialBody();
					double altitude = celestialBody.TerrainAltitude(latitude, longitude);
					Vector3d pos = celestialBody.GetWorldSurfacePosition(latitude, longitude, altitude);

					celestialBody = vessel.getCurrentBody().getCelestialBody();
					Vector3d vesselPos = celestialBody.GetWorldSurfacePosition(vessel.latitude, vessel.longitude, vessel.altitude);

					double distance = Vector3d.Distance(pos, vesselPos);

					return distance <= radius;
				} else {
					// FIXME: does only work for rectangles where a side is exactly parallel to the equator
					return MuUtils.ClampDegrees180(vessel.latitude).between(latitude, latitude2) &&
						MuUtils.ClampDegrees180(vessel.longitude).between(longitude, longitude2);
				}
			} else {
				return false;
			}
		}
	}
}
