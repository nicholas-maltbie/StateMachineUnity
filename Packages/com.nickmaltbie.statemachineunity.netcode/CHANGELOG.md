# Changelog

All notable changes to this project will be documented in this file.

## In Progress

## [1.4.0] - 2023-05-29

* Added backwards compatibility and automated testing for versions 2021.3 of unity.

## [1.3.0] - 2023-04-09

* Added support for `DynamicAnimationAttribute` to `NetworkSMAnim`

## [1.2.5] - 2023-03-18

* Small patch to ensure animation cross fades aren't triggered
    when the object is not spawned.

## [1.2.4] - 2023-02-22

* Small patch to network sm initialization for server owned objects.

## [1.2.3] - 2023-02-22

* Patched how OnAnimationComplete events are handled by using events within
    animation clips.

## [1.2.2] - 2023-02-18

* Added edge case coverage for initializing state machines owned
    by the server and not just client owned.

## [1.2.0] - 2023-02-16

* Imported project into state machine unity repo.
* Setup and imported basic project.
