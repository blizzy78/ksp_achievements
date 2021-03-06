﻿/*
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
using Toolbar;

namespace Achievements {
	[KSPAddon(KSPAddon.Startup.EveryScene, false)]
	internal class Achievements : MonoBehaviour {
		internal const string UNKNOWN_VESSEL = "unknown";
		internal const long VERSION = 18;

		private const long CHECK_INTERVAL = 1500;
		private const float REPUTATION_REWARD = 10;

		internal static UpdateChecker UpdateChecker;

		private long lastCheck = 0;
		private AudioClip achievementEarnedClip;
		private AudioSource achievementEarnedAudioSource;
		private Toast toast;
		private HashSet<Achievement> queuedEarnedAchievements = new HashSet<Achievement>();
		private AchievementsWindow achievementsWindow;
#if LOCATION_PICKER
		private LocationPicker locationPicker;
#endif
		private IButton windowButton;
		private bool showGui = true;

		protected void Start() {
			if (UpdateChecker == null) {
				UpdateChecker = new UpdateChecker();
			}

			achievementEarnedClip = GameDatabase.Instance.GetAudioClip("blizzy/Achievements/achievement");
			achievementEarnedAudioSource = gameObject.AddComponent<AudioSource>();
			achievementEarnedAudioSource.clip = achievementEarnedClip;
			achievementEarnedAudioSource.panLevel = 0;
			achievementEarnedAudioSource.playOnAwake = false;
			achievementEarnedAudioSource.loop = false;
			achievementEarnedAudioSource.Stop();

			windowButton = ToolbarManager.Instance.add("achievements", "achievements");
			windowButton.TexturePath = "blizzy/Achievements/button-normal";
			windowButton.ToolTip = "Achievements";
			windowButton.Visibility = new GameScenesVisibility(GameScenes.FLIGHT, GameScenes.TRACKSTATION, GameScenes.EDITOR, GameScenes.SPH);
			windowButton.OnClick += (e) => toggleAchievementsWindow();

			GameEvents.onShowUI.Add(onShowUI);
			GameEvents.onHideUI.Add(onHideUI);
		}

		internal void OnDestroy() {
			windowButton.Destroy();

			GameEvents.onShowUI.Remove(onShowUI);
			GameEvents.onHideUI.Remove(onHideUI);
		}

		internal void Update() {
			updateAchievements();
			checkAchievements();

			if (achievementsWindow != null) {
				achievementsWindow.update();
			}

			UpdateChecker.update();
		}

		private void updateAchievements() {
			if (EarnedAchievements.instance != null) {
				foreach (Achievement achievement in EarnedAchievements.instance.achievementsList) {
					try {
						achievement.update();
					} catch (Exception e) {
						Debug.LogException(e);
					}
				}
			}
		}

		private void checkAchievements() {
			long now = DateTime.UtcNow.Ticks / 10000;
			if ((now - lastCheck) >= CHECK_INTERVAL) {
				if (EarnedAchievements.instance != null) {
					foreach (Achievement achievement in EarnedAchievements.instance.achievementsList) {
						if (!EarnedAchievements.instance.earnedAchievements.ContainsKey(achievement)) {
							Vessel vessel = (FlightGlobals.fetch != null) ? FlightGlobals.ActiveVessel : null;
							try {
								if (achievement.check(vessel)) {
									string key = achievement.getKey();
									Debug.Log("achievement earned: " + key);
									AchievementEarn earn = new AchievementEarn(now, (vessel != null) ? vessel.vesselName : Achievements.UNKNOWN_VESSEL);
									EarnedAchievements.instance.earnedAchievements.Add(achievement, earn);

									// queue for later display
									queuedEarnedAchievements.Add(achievement);
								}
							} catch (Exception e) {
								Debug.LogException(e);
							}
						}
					}

					//long done = DateTime.UtcNow.Ticks / 10000;
					//Debug.LogWarning("checking achievements took " + (done - now) + " ms");

					if ((queuedEarnedAchievements.Count() > 0) && (toast == null)) {
						Achievement achievement = queuedEarnedAchievements.First<Achievement>();
						queuedEarnedAchievements.Remove(achievement);

						toast = new Toast(achievement, EarnedAchievements.instance.earnedAchievements[achievement]);
						playAchievementEarnedClip();
						awardReputation(achievement);
						highlightButton();
					}
				}

				lastCheck = now;
			}
		}

		internal void OnGUI() {
			if (!showGui) {
				return;
			}

			// auto-close achievements list window
			if ((EarnedAchievements.instance == null) || !windowButton.Visibility.Visible) {
				achievementsWindow = null;
			}

#if LOCATION_PICKER
			if (SHOW_LOCATION_PICKER_BUTTON && (HighLogic.LoadedScene == GameScenes.FLIGHT) && MapView.MapIsEnabled) {
				drawLocationPickerButton();
			}
#endif

			if (toast != null) {
				if (!toast.isTimedOut()) {
					toast.draw();
				} else {
					// auto-close toast after timeout
					toast = null;
				}
			}

			if (achievementsWindow != null) {
				achievementsWindow.draw();
			}

#if LOCATION_PICKER
			if (locationPicker != null) {
				locationPicker.draw();
			}
#endif
		}

		private void onShowUI() {
			showGui = true;
		}

		private void onHideUI() {
			showGui = false;
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
				achievementsWindow = new AchievementsWindow(EarnedAchievements.instance.achievements, EarnedAchievements.instance.earnedAchievements,
					UpdateChecker.Done && (UpdateChecker.UpdateAvailable == true));
				achievementsWindow.closeCallback = () => {
					achievementsWindow = null;
				};

				// reset toolbar button
				windowButton.TexturePath = "blizzy/Achievements/button-normal";
			} else {
				achievementsWindow = null;
			}
		}

		private void toggleLocationPicker() {
#if LOCATION_PICKER
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
#endif
		}

		private void playAchievementEarnedClip() {
			if (!achievementEarnedAudioSource.isPlaying) {
				achievementEarnedAudioSource.volume = GameSettings.UI_VOLUME;
				achievementEarnedAudioSource.Play();
			}
		}

		private void awardReputation(Achievement achievement) {
			if (Reputation.Instance != null) {
				Reputation.Instance.AddReputation(REPUTATION_REWARD, "Achievement: " + achievement.getTitle());
			}
		}

		private void highlightButton() {
			if (achievementsWindow == null) {
				windowButton.TexturePath = "blizzy/Achievements/button-highlight";
			}
		}
	}
}
