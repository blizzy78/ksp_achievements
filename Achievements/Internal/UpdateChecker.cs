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
	internal class UpdateChecker {
		private const string VERSION_URL = "http://blizzy.de/achievements/version2.txt";

		internal bool Done;
		internal string[] KspVersions = null;
		internal bool? UpdateAvailable;

		private WWW www;

		internal UpdateChecker() {
		}

		internal void update() {
			Log.trace("UpdateChecker.update()");

			if (!Done) {
				if (www == null) {
					Log.debug("getting version from {0}", VERSION_URL);
					www = new WWW(VERSION_URL);
				}

				if (www.isDone) {
					try {
						bool updateAvailable = false;
						if (String.IsNullOrEmpty(www.error)) {
							string text = www.text.Replace("\r", string.Empty);
							Log.debug("version text: {0}", text);
							string[] lines = text.Split(new char[] { '\n' }, StringSplitOptions.None);
							try {
								int version = int.Parse(lines[0]);
								updateAvailable = version > Achievements.VERSION;
							} catch (Exception) {
								// ignore
							}
							KspVersions = lines[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
						}

						if (updateAvailable) {
							Log.info("update found: {0} vs {1}", www.text, Achievements.VERSION);
						} else {
							Log.info("no update found: {0} vs {1}", www.text, Achievements.VERSION);
						}
						UpdateAvailable = updateAvailable;
					} finally {
						www = null;
						Done = true;
					}
				}
			}
		}
	}
}
