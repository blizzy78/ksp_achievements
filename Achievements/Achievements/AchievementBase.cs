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

public abstract class AchievementBase : Achievement {
	private bool addonAchievement;

	public abstract bool check(Vessel vessel);
	public abstract string getTitle();
	public abstract string getText();
	public abstract string getKey();

	public virtual bool isHidden() {
		return false;
	}

	public virtual bool isAddon() {
		return addonAchievement;
	}

	public AchievementBase addon() {
		this.addonAchievement = true;
		return this;
	}

	public AchievementBase addon(bool addonAchievement) {
		this.addonAchievement = addonAchievement;
		return this;
	}

	public virtual void init(ConfigNode node) {
	}

	public virtual void save(ConfigNode node) {
	}

	protected void registerOnVesselChange(EventData<Vessel>.OnEvent callback) {
		GameEvents.onVesselChange.Add(new EventData<Vessel>.OnEvent(callback));
	}

	protected void registerOnVesselCreate(EventData<Vessel>.OnEvent callback) {
		GameEvents.onVesselCreate.Add(new EventData<Vessel>.OnEvent(callback));
	}

	protected void registerOnVesselSituationChange(EventData<GameEvents.HostedFromToAction<Vessel, Vessel.Situations>>.OnEvent callback) {
		GameEvents.onVesselSituationChange.Add(new EventData<GameEvents.HostedFromToAction<Vessel, Vessel.Situations>>.OnEvent(callback));
	}

	protected void registerOnCrash(EventData<EventReport>.OnEvent callback) {
		GameEvents.onCrash.Add(new EventData<EventReport>.OnEvent(callback));
	}

	protected void registerOnCrewKilled(EventData<EventReport>.OnEvent callback) {
		GameEvents.onCrewKilled.Add(new EventData<EventReport>.OnEvent(callback));
	}

	protected void registerOnPartCouple(EventData<GameEvents.FromToAction<Part, Part>>.OnEvent callback) {
		GameEvents.onPartCouple.Add(new EventData<GameEvents.FromToAction<Part, Part>>.OnEvent(callback));
	}

	protected void registerOnPartRemove(EventData<GameEvents.HostTargetAction<Part, Part>>.OnEvent callback) {
		GameEvents.onPartRemove.Add(new EventData<GameEvents.HostTargetAction<Part, Part>>.OnEvent(callback));
	}

	protected void registerOnTechnologyResearched(EventData<GameEvents.HostTargetAction<RDTech, RDTech.OperationResult>>.OnEvent callback) {
		GameEvents.OnTechnologyResearched.Add(new EventData<GameEvents.HostTargetAction<RDTech, RDTech.OperationResult>>.OnEvent(callback));
	}

	protected void registerOnLaunch(EventData<EventReport>.OnEvent callback) {
		GameEvents.onLaunch.Add(new EventData<EventReport>.OnEvent(callback));
	}
}
