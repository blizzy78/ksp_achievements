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
	internal class Category {
		internal static readonly Category CREW_OPERATIONS = new Category("Crew Operations");
		internal static readonly Category GENERAL_FLIGHT = new Category("General Flight");
		internal static readonly Category GROUND_OPERATIONS = new Category("Ground Operations");
		internal static readonly Category LANDING = new Category("Landing");
		internal static readonly Category RESEARCH_AND_DEVELOPMENT = new Category("Research and Development");
		internal static readonly Category SPACEFLIGHT = new Category("Spaceflight");

		internal readonly string title;

		private Category(string title) {
			this.title = title;
		}
	}
}
