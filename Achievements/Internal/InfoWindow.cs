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
	internal class InfoWindow {
#pragma warning disable 649
		internal Callback closeCallback;
		internal Callback guiCallback;
#pragma warning restore 649
		internal string text = "";

		private string title;
		private int id = new System.Random().Next(int.MaxValue);
		private Rect rect = new Rect(100, 100, 200, 0);
		private EditorLock editorLock = new EditorLock("Achievements_info");

		internal InfoWindow(string title) {
			this.title = title;
		}

		internal void draw() {
			rect = GUILayout.Window(id, rect, drawContents, title);

			editorLock.draw(rect.Contains(Utils.getMousePosition()));
		}

		private void drawContents(int id) {
			if (guiCallback != null) {
				GUILayout.BeginVertical();
			}

			GUILayout.Label(text, GUILayout.ExpandWidth(true));

			if (guiCallback != null) {
				GUILayout.Space(15);
				guiCallback.Invoke();
				GUILayout.EndVertical();
			}

			if (GUI.Button(new Rect(rect.width - 18, 2, 16, 16), "")) {
				close();
			}

			GUI.DragWindow();
		}

		private void close() {
			editorLock.draw(false);

			if (closeCallback != null) {
				closeCallback();
			}
		}
	}
}
