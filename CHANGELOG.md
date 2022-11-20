# Changelog

All notable changes to this project will be documented in this file.

## In Progress

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
