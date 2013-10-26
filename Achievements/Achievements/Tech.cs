/*
Achievements - Brings achievements to Kerbal Space Program.
Copyright (C) 2013 Maik Schreiber

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

class TechFactory : AchievementFactory {
	public IEnumerable<Achievement> getAchievements() {
		return new Achievement[] {
			new AllTechsResearched()
		};
	}

	public Category getCategory() {
		return Category.RESEARCH_AND_DEVELOPMENT;
	}
}

class AllTechsResearched : AchievementBase {
	private static readonly string[] STOCK_TECH_IDS = new string[] {
		"advAerodynamics",
		"advancedMotors",
		"advConstruction",
		"advElectrics",
		"advExploration",
		"advFlightControl",
		"advLanding",
		"advMetalworks",
		"advRocketry",
		"advScienceTech",
		"advUnmanned",
		"aerodynamicSystems",
		//"aerospaceTech",
		//"automation",
		"basicRocketry",
		"composites",
		"electrics",
		"electronics",
		//"experimentalAerodynamics",
		//"experimentalElectrics",
		//"experimentalMotors",
		//"experimentalRocketry",
		//"experimentalScience",
		"fieldScience",
		"flightControl",
		"fuelSystems",
		"generalConstruction",
		"generalRocketry",
		"heavierRocketry",
		"heavyAerodynamics",
		"heavyRocketry",
		"highAltitudeFlight",
		"hypersonicFlight",
		"ionPropulsion",
		"landing",
		"largeControl",
		"largeElectrics",
		"largeProbes",
		"metaMaterials",
		//"nanolathing",
		"nuclearPropulsion",
		"precisionEngineering",
		//"robotics",
		"scienceTech",
		"spaceExploration",
		"specializedConstruction",
		"specializedControl",
		"specializedElectrics",
		"stability",
		"start",
		"supersonicFlight",
		"survivability",
		"unmannedTech",
		"veryHeavyRocketry"
	};

	private bool initialCheck;
	private bool allTechsResearched;

	public AllTechsResearched() {
		registerOnTechnologyResearched(onTechResearched);
	}

	public override bool check(Vessel vessel) {
		if (!initialCheck) {
			initialCheck = checkTechs();
		}

		return allTechsResearched;
	}

	private bool checkTechs() {
		if (ResearchAndDevelopment.Instance != null) {
			bool allTechsResearched = true;
			foreach (string techId in STOCK_TECH_IDS) {
				ProtoTechNode node = ResearchAndDevelopment.Instance.GetTechState(techId);
				if ((node == null) || (node.state != RDTech.State.Available)) {
					allTechsResearched = false;
					break;
				}
			}
			this.allTechsResearched = allTechsResearched;
			return true;
		} else {
			return false;
		}
	}

	private void onTechResearched(GameEvents.HostTargetAction<RDTech, RDTech.OperationResult> action) {
		checkTechs();
	}

	public override string getTitle() {
		return "Apes Can't Do That";
	}

	public override string getText() {
		return "Unlock all nodes of the technology tree.";
	}

	public override string getKey() {
		return "allTechsResearched";
	}
}
