# Changelog

All notable changes to this project will be documented in this file.

## In Progress

* Added backwards compatibility to version 2019.4.40f1 of unity.
## [1.3.0] - 2023-04-09

* Added `DynamicAnimationAttribute` to select an animation based on
    a function. And added support for this attribute to the
    `FixedSMAnim` behaviour.
* Added capability to pass the `IEvent` data when invoking an action
    or transition for a state machine to parse additional
    data.

## [1.2.3] - 2023-02-22

* Patched how OnAnimationComplete events are handled by using events within
    animation clips.

## [1.2.0] - 2023-02-16

* Combined the NetworkStateMachine and StateMachine project
    into one repo.
* Added support for `AnyState` and `TransitionFrom` and all
    required support for such events.
* Can create a new `State` that inherits from `AnyState`. This state can then
    be used as a way to add `TransitionAttribute` to transition to a given
    state from any other state on a given event.
  * This `Anystate` also supports events, on entry, on exit, etc... that are
    used by any other state for the `FixedSM` and `FixedSMBehaviou` by using
    adjustments made to the `FSMUtils` class.
* Created a new state called `AnyState` to handle this internally.
* Added a new attribute `TransitionFromAttribute` to allow transition from a
    target state to the labeled state.
* Added a new attribute `TransitionFromAnyAttribute` to allow to transition
    from any state to a given state
* Also, cannot transition to `AnyState` or to any state which inherits
    `AnyState`, this will throw an exception.

## [1.1.2] - 2022-11-20

* Updated AnimSMRequest to be Serializable.

## [1.1.1] - 2022-11-19

* Changed FSMUtils.SetupCache value to be public instead of internal.

## [1.1.0] - 2022-11-09

* Added locked animations to the IAnimStateMachine
* Changed IAnimStateMachine to use AnimSMRequest instead of directly
    calling the CrossFade api
* Added example locked animation in the form of Punching state.

## [1.0.1] - 2022-11-08

* Modified attached animator component of FixedSMAnim to allow to be
    overwritten and will default to the animator attached to the FixedSMAnim
    if none is provided.

## [1.0.0] - 2022-11-07

* Added support for IAnimStateMachine to work with an Animator
* Added implementation of IAnimStateMachine in FixedSMAnim.
* Added example state machine implementation of FixedSMAnim in the anim folder.

## [0.0.1] - 2022-11-02

* Imported existing Fixed State Machine code from the OpenKCC Project.
* Created project from template package.

Start of changelog.
