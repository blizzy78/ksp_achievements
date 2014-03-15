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
	internal class EditorLock {
		private string lockId;
		private bool editorLocked;

		internal EditorLock(string lockId) {
			this.lockId = lockId;
		}

		internal void draw(bool lockEditor) {
			if (HighLogic.LoadedSceneIsEditor) {
				if (lockEditor && !editorLocked) {
					EditorLogic.fetch.Lock(true, true, true, lockId);
					editorLocked = true;
				} else if (!lockEditor && editorLocked) {
					EditorLogic.fetch.Unlock(lockId);
					editorLocked = false;
				}
			}
		}
	}
}
