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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

class EarnedAchievements : ScenarioModule {
	private static List<Type> achievementFactoryTypes;

	public Dictionary<Category, IEnumerable<Achievement>> achievements {
		get;
		private set;
	}

	public List<Achievement> achievementsList {
		get;
		private set;
	}

	public Dictionary<Achievement, AchievementEarn> earnedAchievements {
		get;
		private set;
	}

	public static EarnedAchievements getInstance() {
		Game game = HighLogic.CurrentGame;
		if (game == null) {
			return null;
		}

		if (!game.scenarios.Any(s => s.moduleName == typeof(EarnedAchievements).Name)) {
			ProtoScenarioModule scenario = game.AddProtoScenarioModule(typeof(EarnedAchievements),
				GameScenes.EDITOR, GameScenes.FLIGHT, GameScenes.SPACECENTER, GameScenes.SPH, GameScenes.TRACKSTATION);
			if (scenario.targetScenes.Contains(HighLogic.LoadedScene)) {
				scenario.Load(ScenarioRunner.fetch);
			}
		}

		return game.scenarios.Select(s => s.moduleRef).OfType<EarnedAchievements>().SingleOrDefault();
	}

	public override void OnLoad(ConfigNode node) {
		achievements = getAchievements();
		achievementsList = getAchievementsList();

		if (node.HasNode("achievements")) {
			node = node.GetNode("achievements");
		} else {
			string settingsFile = KSPUtil.ApplicationRootPath + "GameData/blizzy/Achievements/achievements.dat";
			if (System.IO.File.Exists(settingsFile)) {
				Debug.Log("converting earned achievements from global to current save");
			}
			node = ConfigNode.Load(settingsFile) ?? new ConfigNode();
		}
		earnedAchievements = loadEarnedAchievements(node);
	}

	public override void OnSave(ConfigNode node) {
		if (node.HasNode("achievements")) {
			node.RemoveNode("achievements");
		}
		node = node.AddNode("achievements");
		saveEarnedAchievements(node);
	}

	private Dictionary<Achievement, AchievementEarn> loadEarnedAchievements(ConfigNode node) {
		Dictionary<Achievement, AchievementEarn> result = new Dictionary<Achievement, AchievementEarn>();

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
			legacyNode.AddValue("flight", Achievements.UNKNOWN_VESSEL);
			legacyNodes.Add(legacyNode);
		}
		foreach (ConfigNode legacyNode in legacyNodes) {
			node.RemoveValue(legacyNode.name);
			node.AddNode(legacyNode);
		}

		// new way
		foreach (Achievement achievement in achievementsList) {
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

	private void saveEarnedAchievements(ConfigNode node) {
		Debug.Log("saving achievements (" + earnedAchievements.Count() + " earned)");
		foreach (Achievement achievement in achievementsList) {
			ConfigNode achievementNode = node.AddNode(achievement.getKey());

			achievement.save(achievementNode);

			AchievementEarn earn = earnedAchievements.ContainsKey(achievement) ? earnedAchievements[achievement] : null;
			if (earn != null) {
				achievementNode.AddValue("time", earn.time);
				if (earn.flightName != null) {
					achievementNode.AddValue("flight", earn.flightName);
				}
			}
		}
	}

	private Achievement getAchievementByKey(string key) {
		foreach (Achievement achievement in achievementsList) {
			if (achievement.getKey() == key) {
				return achievement;
			}
		}
		throw new ArgumentException("unknown achievement: " + key);
	}

	private List<Achievement> getAchievementsList() {
		List<Achievement> achievements = new List<Achievement>();
		foreach (IEnumerable<Achievement> categoryAchievements in this.achievements.Values.AsEnumerable()) {
			achievements.AddRange(categoryAchievements);
		}
		return achievements;
	}

	private Dictionary<Category, IEnumerable<Achievement>> getAchievements() {
		IEnumerable<Type> factoryTypes = getAchievementFactoryTypes();
		Dictionary<Category, IEnumerable<Achievement>> achievements = new Dictionary<Category, IEnumerable<Achievement>>();
		foreach (Type type in factoryTypes) {
			try {
				AchievementFactory factory = (AchievementFactory) type.GetConstructor(Type.EmptyTypes).Invoke(null);
				Category category = factory.getCategory();
				List<Achievement> categoryAchievements;
				if (achievements.ContainsKey(category)) {
					categoryAchievements = (List<Achievement>) achievements[category];
				} else {
					categoryAchievements = new List<Achievement>();
					achievements.Add(category, categoryAchievements);
				}
				IEnumerable<Achievement> factoryAchievements = factory.getAchievements();
				categoryAchievements.AddRange(factoryAchievements);
			} catch (Exception e) {
				Debug.LogException(e);
			}
		}
		Debug.Log("number of achievements: " + achievements.getValuesCount() + " in " + achievements.Keys.Count() + " categories");
		return achievements;
	}

	private static IEnumerable<Type> getAchievementFactoryTypes() {
		if (achievementFactoryTypes == null) {
			achievementFactoryTypes = new List<Type>();
			Type achievementFactoryType = typeof(AchievementFactory);
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
				try {
					achievementFactoryTypes.AddRange(assembly.GetTypes().Where<Type>(t => {
						if (t.IsClass) {
							Type interfaceType = t.GetInterface(achievementFactoryType.FullName);
							return (interfaceType != null) && interfaceType.Equals(achievementFactoryType);
						} else {
							return false;
						}
					}));
				} catch (Exception) {
					Debug.LogWarning("exception while loading types, ignoring assembly: " + assembly.FullName);
				}
			}
		}
		return achievementFactoryTypes;
	}
}
