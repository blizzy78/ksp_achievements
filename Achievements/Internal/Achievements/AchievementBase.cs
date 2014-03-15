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
	internal abstract class AchievementBase : Achievement {
		private interface IEventHandlerEntry {
			void register();
			void unregister();
		}

		private class EventHandlerEntry<T> : IEventHandlerEntry {
			private EventData<T> evt;
			private EventData<T>.OnEvent callback;

			internal EventHandlerEntry(EventData<T> evt, EventData<T>.OnEvent callback) {
				this.evt = evt;
				this.callback = callback;
			}

			public void register() {
				evt.Add(callback);
			}

			public void unregister() {
				evt.Remove(callback);
			}
		}

		private bool hidden;
		private bool addonAchievement;
		private List<IEventHandlerEntry> eventHandlers = new List<IEventHandlerEntry>();

		public virtual void update() {
		}

		public abstract bool check(Vessel vessel);
		public abstract string getTitle();
		public abstract string getText();
		public abstract string getKey();

		public bool isHidden() {
			return hidden;
		}

		internal AchievementBase hide() {
			hidden = true;
			return this;
		}

		public virtual bool isAddon() {
			return addonAchievement;
		}

		internal AchievementBase addon() {
			this.addonAchievement = true;
			return this;
		}

		internal AchievementBase addon(bool addonAchievement) {
			this.addonAchievement = addonAchievement;
			return this;
		}

		public virtual void init(ConfigNode node) {
		}

		public virtual void save(ConfigNode node) {
		}

		public virtual void destroy() {
			foreach (IEventHandlerEntry entry in eventHandlers) {
				entry.unregister();
			}
		}

		protected void registerOnVesselChange(EventData<Vessel>.OnEvent callback) {
			registerEventHandler(GameEvents.onVesselChange, callback);
		}

		protected void registerOnVesselCreate(EventData<Vessel>.OnEvent callback) {
			registerEventHandler(GameEvents.onVesselCreate, callback);
		}

		protected void registerOnVesselSituationChange(EventData<GameEvents.HostedFromToAction<Vessel, Vessel.Situations>>.OnEvent callback) {
			registerEventHandler(GameEvents.onVesselSituationChange, callback);
		}

		protected void registerOnCrash(EventData<EventReport>.OnEvent callback) {
			registerEventHandler(GameEvents.onCrash, callback);
		}

		protected void registerOnCrewKilled(EventData<EventReport>.OnEvent callback) {
			registerEventHandler(GameEvents.onCrewKilled, callback);
		}

		protected void registerOnPartCouple(EventData<GameEvents.FromToAction<Part, Part>>.OnEvent callback) {
			registerEventHandler(GameEvents.onPartCouple, callback);
		}

		protected void registerOnPartRemove(EventData<GameEvents.HostTargetAction<Part, Part>>.OnEvent callback) {
			registerEventHandler(GameEvents.onPartRemove, callback);
		}

		protected void registerOnTechnologyResearched(EventData<GameEvents.HostTargetAction<RDTech, RDTech.OperationResult>>.OnEvent callback) {
			registerEventHandler(GameEvents.OnTechnologyResearched, callback);
		}

		protected void registerOnLaunch(EventData<EventReport>.OnEvent callback) {
			registerEventHandler(GameEvents.onLaunch, callback);
		}

		private void registerEventHandler<T>(EventData<T> evt, EventData<T>.OnEvent callback) {
			EventHandlerEntry<T> entry = new EventHandlerEntry<T>(evt, callback);
			entry.register();
			eventHandlers.Add(entry);
		}
	}
}
