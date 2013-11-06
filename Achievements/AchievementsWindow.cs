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

class AchievementsWindow {
	private const string FORUM_THREAD_URL = "http://forum.kerbalspaceprogram.com/threads/52535";

	public Callback closeCallback;

	private Dictionary<Category, IEnumerable<Achievement>> achievements;
	private Dictionary<Achievement, AchievementEarn> earnedAchievements;
	private bool newVersionAvailable;
	private int id = new System.Random().Next(int.MaxValue);
	private Rect rect;
	private Category selectedCategory;
	private Vector2 achievementsScrollPos;

	public AchievementsWindow(Dictionary<Category, IEnumerable<Achievement>> achievements,
		Dictionary<Achievement, AchievementEarn> earnedAchievements, bool newVersionAvailable) {

		this.achievements = achievements;
		this.earnedAchievements = earnedAchievements;
		this.newVersionAvailable = newVersionAvailable;

		int width = AchievementGUI.TEX_WIDTH + 300;
		int height = Screen.height / 2;
		rect = new Rect((Screen.width - width) / 2, (Screen.height - height) / 2, width, height);

		selectedCategory = achievements.Keys.OrderBy(c => c.title, StringComparer.CurrentCultureIgnoreCase).First();
	}

	public void draw() {
		rect = GUILayout.Window(id, rect, drawContents, "Achievements (earned " + earnedAchievements.Count() + " of " + achievements.getValuesCount() + ")");
	}

	private void drawContents(int id) {
		GUILayout.BeginVertical();

		GUILayout.BeginHorizontal();
		drawCategoriesList(achievements.Keys.OrderBy(c => c.title, StringComparer.CurrentCultureIgnoreCase));
		GUILayout.Space(15);
		drawAchievementsList(achievements[selectedCategory]);
		GUILayout.EndHorizontal();

		GUILayout.Space(20);

		GUILayout.BeginHorizontal();
		if (newVersionAvailable) {
			Color oldColor = GUI.color;
			GUI.color = Color.yellow;
			GUILayout.Label("A newer version of this plugin is available.");
			GUI.color = oldColor;
			GUILayout.Space(10);
			if (GUILayout.Button("Download Page")) {
				Application.OpenURL(FORUM_THREAD_URL);
			}
		}
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Forums Feedback Thread")) {
			Application.OpenURL(FORUM_THREAD_URL);
		}
		if (!newVersionAvailable) {
			GUILayout.FlexibleSpace();
		}
		GUILayout.EndHorizontal();

		GUILayout.EndVertical();

		if (GUI.Button(new Rect(rect.width - 18, 2, 16, 16), "")) {
			close();
		}

		GUI.DragWindow();
	}

	private void close() {
		if (closeCallback != null) {
			closeCallback.Invoke();
		}
	}

	private void drawCategoriesList(IEnumerable<Category> categories) {
		GUILayout.BeginVertical(GUILayout.Width(220), GUILayout.ExpandWidth(true));
	
		// adjust for texture drop shadow
		GUILayout.Space(5);

		foreach (Category category in categories) {
			GUILayout.Space(5);

			GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
			if (category.Equals(selectedCategory)) {
				buttonStyle.fontStyle = FontStyle.Bold;
			}
			if (GUILayout.Button(category.title, buttonStyle)) {
				selectedCategory = category;
				achievementsScrollPos = Vector2.zero;
			}
		}
		GUILayout.EndVertical();
	}

	private void drawAchievementsList(IEnumerable<Achievement> achievements) {
		achievementsScrollPos = GUILayout.BeginScrollView(achievementsScrollPos);
		bool first = true;
		foreach (Achievement achievement in achievements) {
			if (first) {
				first = false;
			} else {
				GUILayout.Space(5);
			}

			AchievementEarn earn = earnedAchievements.ContainsKey(achievement) ? earnedAchievements[achievement] : null;
			if ((earn != null) || !achievement.isHidden()) {
				drawAchievement(achievement, earn);
			}
		}
		GUILayout.EndScrollView();
	}

	private void drawAchievement(Achievement achievement, AchievementEarn earn) {
		new AchievementGUI(achievement, earn).draw(true, true);
	}
}
