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
using System.Reflection;
using System.Text;
using UnityEngine;

public class Achievements : MonoBehaviour {
	private const long VERSION = 3;
	private const long CHECK_INTERVAL = 1500;
	private const long SAVE_INTERVAL = 300000;
	private const float SCIENCE_REWARD = 5;

	// debugging
	private const bool SHOW_ACHIEVEMENTS_IN_MENU = false;
	private const bool SHOW_LOCATION_PICKER_BUTTON = false;

	private Dictionary<Category, IEnumerable<Achievement>> achievements;
	private long lastCheck = 0;
	private Dictionary<Achievement, AchievementEarn> earnedAchievements;
	private AudioClip achievementEarnedClip;
	private AudioSource achievementEarnedAudioSource;
	private Toast toast;
	private HashSet<Achievement> queuedEarnedAchievements = new HashSet<Achievement>();
	private string settingsFile = KSPUtil.ApplicationRootPath + "GameData/blizzy/Achievements/achievements.dat";
	private AchievementsWindow achievementsWindow;
	private WWW versionWWW;
	private bool? newVersionAvailable = null;
	private LocationPicker locationPicker;

	protected void Start() {
		Debug.LogWarning("Achievements.Start");

		versionWWW = new WWW("http://blizzy.de/achievements/version.txt");

		achievements = getAchievements();
		earnedAchievements = loadEarnedAchievements();

		achievementEarnedClip = GameDatabase.Instance.GetAudioClip("blizzy/Achievements/achievement");
		achievementEarnedAudioSource = gameObject.AddComponent<AudioSource>();
		achievementEarnedAudioSource.clip = achievementEarnedClip;
		achievementEarnedAudioSource.panLevel = 0;
		achievementEarnedAudioSource.playOnAwake = false;
		achievementEarnedAudioSource.loop = false;
		achievementEarnedAudioSource.Stop();

		GameEvents.onGameStateSaved.Add(new EventData<Game>.OnEvent(onAutoSave));
	}

	public void Update() {
		long now = DateTime.UtcNow.Ticks / 10000;
		if ((now - lastCheck) >= CHECK_INTERVAL) {
			bool forceSave = false;

			foreach (Achievement achievement in getAchievementsList()) {
				if (!earnedAchievements.ContainsKey(achievement)) {
					Vessel vessel = (FlightGlobals.fetch != null) ? FlightGlobals.ActiveVessel : null;
					if (achievement.check(vessel)) {
						string key = achievement.getKey();
						Debug.Log("achievement earned: " + key);
						AchievementEarn earn = new AchievementEarn(now, (vessel != null) ? vessel.vesselName : "unknown");
						earnedAchievements.Add(achievement, earn);

						forceSave = true;

						// queue for later display
						queuedEarnedAchievements.Add(achievement);
					}
				}
			}

			//long done = DateTime.UtcNow.Ticks / 10000;
			//Debug.LogWarning("checking achievements took " + (done - now) + " ms");

			if ((queuedEarnedAchievements.Count() > 0) && (toast == null)) {
				Achievement achievement = queuedEarnedAchievements.First<Achievement>();
				queuedEarnedAchievements.Remove(achievement);

				toast = new Toast(achievement, earnedAchievements[achievement]);
				playAchievementEarnedClip();
				awardScience(achievement);
			}

			if (forceSave) {
				saveEarnedAchievements(earnedAchievements);
			}

			checkForNewVersion();

			lastCheck = now;
		}
	}

	public void OnGUI() {
		if ((HighLogic.LoadedScene == GameScenes.SPACECENTER) ||
			HighLogic.LoadedSceneHasPlanetarium ||
			((FlightGlobals.fetch != null) && (FlightGlobals.ActiveVessel != null)) ||
			SHOW_ACHIEVEMENTS_IN_MENU) {

			drawAchievementsWindowButton();
		} else {
			achievementsWindow = null;
		}

		if (SHOW_LOCATION_PICKER_BUTTON &&
			((FlightGlobals.fetch != null) && (FlightGlobals.ActiveVessel != null))) {

			drawLocationPickerButton();
		}

		if (toast != null) {
			if (!toast.isTimedOut()) {
				toast.draw();
			} else {
				toast = null;
			}
		}

		if (achievementsWindow != null) {
			achievementsWindow.draw();
		}

		if (locationPicker != null) {
			locationPicker.draw();
		}
	}

	private void drawAchievementsWindowButton() {
		GUI.depth = -100;
		GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(new Vector3(0, 0, -90)), Vector3.one);
		GUIStyle style = new GUIStyle(GUI.skin.button);
		if (newVersionAvailable == true) {
			style.normal.textColor = Color.yellow;
			style.onHover.textColor = Color.yellow;
			style.hover.textColor = Color.yellow;
			style.onActive.textColor = Color.yellow;
			style.active.textColor = Color.yellow;
			style.onFocused.textColor = Color.yellow;
			style.focused.textColor = Color.yellow;
		}
		if (GUI.Button(new Rect(-600, Screen.width - 25, 120, 25), "Achievements", style)) {
			toggleAchievementsWindow();
		}
		GUI.matrix = Matrix4x4.identity;
		GUI.depth = 0;
	}

	private void drawLocationPickerButton() {
		GUI.depth = -100;
		GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(new Vector3(0, 0, -90)), Vector3.one);
		if (GUI.Button(new Rect(-700, Screen.width - 25, 120, 25), "Location")) {
			toggleLocationPicker();
		}
		GUI.matrix = Matrix4x4.identity;
		GUI.depth = 0;
	}

	private void toggleAchievementsWindow() {
		if (achievementsWindow == null) {
			achievementsWindow = new AchievementsWindow(achievements, earnedAchievements, newVersionAvailable == true);
			achievementsWindow.closeCallback = () => {
				achievementsWindow = null;
			};
		} else {
			achievementsWindow = null;
		}
	}

	private void toggleLocationPicker() {
		if (locationPicker == null) {
			MapView.EnterMapView();
			locationPicker = new LocationPicker();
			locationPicker.closeCallback = () => {
				locationPicker.destroy();
				locationPicker = null;
			};
		} else {
			locationPicker.destroy();
			locationPicker = null;
		}
	}

	private void playAchievementEarnedClip() {
		if (!achievementEarnedAudioSource.isPlaying) {
			achievementEarnedAudioSource.volume = GameSettings.UI_VOLUME;
			achievementEarnedAudioSource.Play();
		}
	}

	private void awardScience(Achievement achievement) {
		if (ResearchAndDevelopment.Instance != null) {
			ScienceSubject subject = new ScienceSubject("achievement", "Achievement: " + achievement.getTitle(), 1, SCIENCE_REWARD, 10000);
			ResearchAndDevelopment.Instance.SubmitScienceData(1, subject);
		}
	}

	private void checkForNewVersion() {
		if ((newVersionAvailable == null) && String.IsNullOrEmpty(versionWWW.error) && versionWWW.isDone) {
			try {
				long ver = long.Parse(versionWWW.text);
				newVersionAvailable = ver > VERSION;
			} catch (Exception) {
				// ignore
			}
		}
	}

	private Dictionary<Achievement, AchievementEarn> loadEarnedAchievements() {
		Dictionary<Achievement, AchievementEarn> result = new Dictionary<Achievement, AchievementEarn>();
		ConfigNode node = ConfigNode.Load(settingsFile) ?? new ConfigNode();

		// old way of doing things
		List<ConfigNode> legacyNodes = new List<ConfigNode>();
		foreach (ConfigNode.Value value in node.values) {
			string key = value.name;

			// legacy
			if (key == "launch") {
				key = "launch.1";
			}

			string time = value.value;
			ConfigNode legacyNode = new ConfigNode(key);
			legacyNode.AddValue("time", time);
			legacyNode.AddValue("flight", "unknown");
			legacyNodes.Add(legacyNode);
		}
		foreach (ConfigNode legacyNode in legacyNodes) {
			node.RemoveValue(legacyNode.name);
			node.AddNode(legacyNode);
		}

		// new way
		foreach (Achievement achievement in getAchievementsList()) {
			string key = achievement.getKey();
			if (node.HasNode(key)) {
				ConfigNode achievementNode = node.GetNode(key);
				achievement.init(achievementNode);

				if (achievementNode.HasValue("time") && achievementNode.HasValue("flight")) {
					long time = long.Parse(achievementNode.GetValue("time"));
					string flightName = achievementNode.HasValue("flight") ? achievementNode.GetValue("flight") : null;
					AchievementEarn earn = new AchievementEarn(time, flightName);
					result.Add(achievement, earn);
				}
			}
		}

		Debug.Log("loaded " + result.Count() + " earned achievements");
		return result;
	}

	private void saveEarnedAchievements(Dictionary<Achievement, AchievementEarn> earnedAchievements) {
		Debug.Log("saving achievements (" + earnedAchievements.Count() + " earned)");
		ConfigNode node = new ConfigNode();
		foreach (Achievement achievement in getAchievementsList()) {
			ConfigNode achievementNode = node.AddNode(achievement.getKey());

			AchievementEarn earn = earnedAchievements.ContainsKey(achievement) ? earnedAchievements[achievement] : null;
			if (earn != null) {
				achievementNode.AddValue("time", earn.time);
				if (earn.flightName != null) {
					achievementNode.AddValue("flight", earn.flightName);
				}
			}

			achievement.save(achievementNode);
		}
		node.Save(settingsFile);
	}

	private void onAutoSave(Game game) {
		saveEarnedAchievements(earnedAchievements);
	}

	private Achievement getAchievementByKey(string key) {
		foreach (Achievement achievement in getAchievementsList()) {
			if (achievement.getKey() == key) {
				return achievement;
			}
		}
		throw new ArgumentException("unknown achievement: " + key);
	}

	private IEnumerable<Achievement> getAchievementsList() {
		List<Achievement> achievements = new List<Achievement>();
		foreach (IEnumerable<Achievement> categoryAchievements in this.achievements.Values.AsEnumerable()) {
			achievements.AddRange(categoryAchievements);
		}
		return achievements;
	}

	private Dictionary<Category, IEnumerable<Achievement>> getAchievements() {
		Type achievementFactoryType = typeof(AchievementFactory);
		Assembly assembly = achievementFactoryType.Assembly;
		IEnumerable<Type> types = assembly.GetTypes().Where<Type>(t => {
			Type interfaceType = t.GetInterface(achievementFactoryType.FullName);
			return (interfaceType != null) && interfaceType.Equals(achievementFactoryType);
		});
		Dictionary<Category, IEnumerable<Achievement>> achievements = new Dictionary<Category, IEnumerable<Achievement>>();
		foreach (Type type in types) {
			AchievementFactory factory = (AchievementFactory) type.GetConstructor(Type.EmptyTypes).Invoke(null);
			Category category = factory.getCategory();
			List<Achievement> categoryAchievements;
			if (achievements.ContainsKey(category)) {
				categoryAchievements = (List<Achievement>) achievements[category];
			} else {
				categoryAchievements = new List<Achievement>();
				achievements.Add(category, categoryAchievements);
			}
			categoryAchievements.AddRange(factory.getAchievements());
		}
		Debug.Log("number of achievements: " + achievements.getValuesCount() + " in " + achievements.Keys.Count() + " categories");
		return achievements;
	}
}
