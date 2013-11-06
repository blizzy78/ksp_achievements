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

class AchievementGUI {
	public static readonly int TEX_WIDTH = 340;
	public static readonly int TEX_HEIGHT = 95;

	private const int FONT_SIZE = 13;
	private const int TITLE_FONT_SIZE = 15;
	private const int TEXT_MARGIN = 20;

	private Achievement achievement;
	private AchievementEarn earn;
	private Texture2D toastTex;
	private Texture2D toastNotEarnedTex;
	private Texture2D toastAddonTex;
	private Texture2D toastAddonNotEarnedTex;
	private GUIStyle bgStyle;
	private GUIStyle bgNotEarnedStyle;
	private GUIStyle bgAddonStyle;
	private GUIStyle bgAddonNotEarnedStyle;
	private GUIStyle textStyle;
	private GUIStyle textNotEarnedStyle;
	private GUIStyle titleStyle;
	private GUIStyle titleNotEarnedStyle;

	public AchievementGUI(Achievement achievement, AchievementEarn earn) {
		this.achievement = achievement;
		this.earn = earn;

		toastTex = GameDatabase.Instance.GetTexture("blizzy/Achievements/toast", false);
		toastNotEarnedTex = GameDatabase.Instance.GetTexture("blizzy/Achievements/toast-not-earned", false);
		toastAddonTex = GameDatabase.Instance.GetTexture("blizzy/Achievements/toast-addon", false);
		toastAddonNotEarnedTex = GameDatabase.Instance.GetTexture("blizzy/Achievements/toast-addon-not-earned", false);

		int width = TEX_WIDTH + 300;
		int height = Screen.height / 2;

		bgStyle = new GUIStyle();
		bgStyle.normal.background = toastTex;
		bgStyle.fixedWidth = TEX_WIDTH;
		bgStyle.fixedHeight = TEX_HEIGHT;

		bgNotEarnedStyle = new GUIStyle();
		bgNotEarnedStyle.normal.background = toastNotEarnedTex;
		bgNotEarnedStyle.fixedWidth = TEX_WIDTH;
		bgNotEarnedStyle.fixedHeight = TEX_HEIGHT;

		bgAddonStyle = new GUIStyle(bgStyle);
		bgAddonStyle.normal.background = toastAddonTex;

		bgAddonNotEarnedStyle = new GUIStyle(bgNotEarnedStyle);
		bgAddonNotEarnedStyle.normal.background = toastAddonNotEarnedTex;

		textStyle = new GUIStyle();
		textStyle.alignment = TextAnchor.MiddleCenter;
		textStyle.fontSize = FONT_SIZE;
		textStyle.normal.textColor = Color.black;
		textStyle.padding = new RectOffset(TEXT_MARGIN, TEXT_MARGIN, 0, 0);
		textStyle.wordWrap = true;
		textStyle.fixedWidth = TEX_WIDTH;

		textNotEarnedStyle = new GUIStyle();
		textNotEarnedStyle.alignment = TextAnchor.MiddleCenter;
		textNotEarnedStyle.fontSize = FONT_SIZE;
		textNotEarnedStyle.normal.textColor = Color.grey;
		textNotEarnedStyle.padding = new RectOffset(TEXT_MARGIN, TEXT_MARGIN, 0, 0);
		textNotEarnedStyle.wordWrap = true;
		textNotEarnedStyle.fixedWidth = TEX_WIDTH;

		titleStyle = new GUIStyle(textStyle);
		titleStyle.fontSize = TITLE_FONT_SIZE;
		titleStyle.fontStyle = FontStyle.Bold;

		titleNotEarnedStyle = new GUIStyle(textNotEarnedStyle);
		titleNotEarnedStyle.fontSize = TITLE_FONT_SIZE;
		titleNotEarnedStyle.fontStyle = FontStyle.Bold;
	}

	public void draw(bool showCounter, bool showAddonIndicator) {
		GUIStyle bgStyle;
		if (earn != null) {
			bgStyle = (showAddonIndicator && achievement.isAddon()) ? bgAddonStyle : this.bgStyle;
		} else {
			bgStyle = (showAddonIndicator && achievement.isAddon()) ? bgAddonNotEarnedStyle : bgNotEarnedStyle;
		}
		GUILayout.BeginVertical(bgStyle, GUILayout.Width(TEX_WIDTH), GUILayout.Height(TEX_HEIGHT), GUILayout.ExpandWidth(true));

		GUILayout.FlexibleSpace();

		GUILayout.Label(achievement.getTitle(), (earn != null) ? titleStyle : titleNotEarnedStyle,
			GUILayout.Width(TEX_WIDTH), GUILayout.ExpandWidth(true));
		GUILayout.Space(5);
		string text = achievement.getText();
		if ((achievement is CountingAchievement) && showCounter && (earn == null)) {
			int minRequired = ((CountingAchievement) achievement).minRequired;
			if (minRequired > 1) {
				// clamp to minRequired
				int count = Math.Min(((CountingAchievement) achievement).getCount(), minRequired);
				text += " (" + count.ToString("D0") + "/" + minRequired.ToString("D0") + ")";
			}
		}
		GUILayout.Label(text, (earn != null) ? textStyle : textNotEarnedStyle,
			GUILayout.Width(TEX_WIDTH), GUILayout.ExpandWidth(true));

		GUILayout.FlexibleSpace();

		GUILayout.EndVertical();
	}
}
