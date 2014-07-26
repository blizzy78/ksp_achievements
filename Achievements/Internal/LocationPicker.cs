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

#if LOCATION_PICKER
namespace Achievements {
	internal class LocationPicker {
		internal Callback closeCallback;

		private bool pickingActive = true;
		private InfoWindow infoWindow;
		private Coordinates coords;
		private float radius = 50000;

		internal LocationPicker() {
			RenderingManager.AddToPostDrawQueue(1, doCoordinatePicking);

			infoWindow = new InfoWindow("Location");
			infoWindow.text = "Pick location on map.";
			infoWindow.closeCallback = () => {
				close();
			};
			infoWindow.guiCallback = () => {
				infoWindowGUI();
			};
		}

		internal void destroy() {
			RenderingManager.RemoveFromPostDrawQueue(1, doCoordinatePicking);
		}

		private void close() {
			if (closeCallback != null) {
				closeCallback.Invoke();
			}
		}

		internal void draw() {
			infoWindow.draw();
		}

		private void infoWindowGUI() {
			GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
			radius = GUILayout.HorizontalSlider(radius, 1000, 100000, GUILayout.MinWidth(200), GUILayout.ExpandWidth(true));
			GUILayout.Label((radius / 1000).ToString("F0") + " km", GUILayout.ExpandWidth(true));
			GUILayout.EndVertical();
		}

		private void doCoordinatePicking() {
			if (pickingActive && !MapView.MapIsEnabled) {
				pickingActive = false;
				close();
			}

			CelestialBody body = PlanetariumCamera.fetch.target.celestialBody;
			if (body == null) {
				Vessel vessel = PlanetariumCamera.fetch.target.vessel;
				if (vessel != null) {
					body = vessel.getCurrentBody().getCelestialBody();
				}
			}

			if (body == null) {
				pickingActive = false;
			}

			if (!pickingActive) {
				return;
			}

			Coordinates mouseCoords = GuiUtils.GetMouseCoordinates(body);

			if (this.coords != null) {
				GLUtils.DrawMapViewGroundMarker(body, this.coords.latitude, this.coords.longitude, radius, Color.red);
				double distance = getDistance(this.coords, mouseCoords, body);
				infoWindow.text = this.coords.ToStringDecimal() + "\nDistance: " + distance.ToString("F1") + " m";
			}

			if (mouseCoords != null) {
				GLUtils.DrawMapViewGroundMarker(body, mouseCoords.latitude, mouseCoords.longitude, body.Radius / 15, Color.yellow);

				if (Input.GetMouseButtonDown(0)) {
					this.coords = mouseCoords;
				}
			}
		}

		private double getDistance(Coordinates coords1, Coordinates coords2, CelestialBody body) {
			double altitude = body.TerrainAltitude(coords1.latitude, coords1.longitude);
			Vector3d pos1 = body.GetWorldSurfacePosition(coords1.latitude, coords1.longitude, altitude);
			altitude = body.TerrainAltitude(coords2.latitude, coords2.longitude);
			Vector3d pos2 = body.GetWorldSurfacePosition(coords2.latitude, coords2.longitude, altitude);
			return Vector3d.Distance(pos1, pos2);
		}
	}
}
#endif
