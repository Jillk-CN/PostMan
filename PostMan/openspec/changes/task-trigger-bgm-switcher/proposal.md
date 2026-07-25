# Proposal: Task-Triggered BGM Switcher

## Problem

The game currently has no mechanism to switch the background music in response to narrative events. The `BGMPlayer` script only handles playback on Awake; there is no runtime hook that lets a Task transition trigger a BGM change. We need a clean way to say "when Task X starts, fade out the current BGM and fade in a new track."

## Solution

Create a new `MonoBehaviour` script — `TaskBGMSwitcher` — placed inside `Assets/Scripts/BGM/`. It subscribes to `TaskEventBus.OnTaskStarted`, checks whether the started task matches the one configured in the Inspector, then calls `AudioManager.Instance.Stop(BGM, ...)` followed by `AudioManager.Instance.Play(BGM, ...)` using the existing AudioManager API. Every parameter exposed by those two methods (fade toggles, durations, loop, volume) is serialized as Inspector-visible fields so designers can tune each transition without touching code.

## Key Decisions

- **BGM folder** — consistent with the existing `BGMPlayer.cs` location; designers find both scripts in the same place.
- **TaskEventBus.OnTaskStarted only** — the most common trigger point; handling Completed/Advanced/Failed is out of scope unless a future change requests it.
- **Reuse AudioManager methods** — `Stop(trackId, fadeOut, fadeOutDuration)` + `Play(trackId, clip, loop, fadeIn, fadeInDuration, volume)` already exist. No new audio logic is needed.
- **One component per transition** — if multiple tasks should each trigger a different BGM switch, designers add multiple `TaskBGMSwitcher` components or GameObjects. Simple and composable.

## Non-goals

- Does not handle `OnTaskCompleted`, `OnTaskAdvanced`, or `OnTaskFailed`.
- Does not implement a queue or crossfade between more than two tracks.
- Does not add any new AudioManager methods or modify the existing audio pipeline.
- Does not create any UI or visual feedback for the transition.
