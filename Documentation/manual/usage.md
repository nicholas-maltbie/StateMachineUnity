# Usage

The State Machine Unity project implements synchronous
[Finite State Machines](https://en.wikipedia.org/wiki/Finite-state_machine)
in unity.

To use this project, you simply need to create a class or MonoBehaviour
which implements one of the defined state machine types
(such as
@nickmaltbie.StateMachineUnity.Fixed.FixedSM ,
@nickmaltbie.StateMachineUnity.Fixed.FixedSMBehaviour , or
@nickmaltbie.StateMachineUnity.Fixed.FixedSMAnim
see [FSM Design](fsm-design.md)for more details.)
and then define the relevant transitions, actions, events, and attributes
of the state machine.

## What is a FSM

A Finite State Machine (FSM) is an abstraction of how to organize a computation.
A state machine is defined by a set of states and transitions and a given instance
of said state machine can be in exactly one of the defined states.
This can be a useful concept in game design as it allows for abstracting
out some code patterns to be more easily organized and managed without
having to create many nested if statements.

A simple example of a state machine is a Character Controller. The
character controller could be defined as having a set of given states,
possibly grounded and not grounded, depending on external input (such as a
game world), the character would transition to not grounded upon leaving the
ground and grounded upon being in the ground.

## Why Use This Library?

I found myself re-creating a state machine over and over again
in unity code or making many nested if statements.
I could not find a simple, good solution to manage lots of
states for many objects on the screen at once.

To solve this issue and cleanup some of my spaghetti code,
I created this library to simply creation and management of state
machines in C# for unity based off a similar architecture
to the [Coyote Framework](https://www.microsoft.com/en-us/research/project/coyote/).
That framework is designed for large scale asynchronous communication projects
of which most games are not. As they have different requirements,
I defined this state machine project as a simplified version meant
to run synchronously in games.

## Example FSM

There is an example FSM explained in further detail in the
[Example FSM](example-fsm.md) document.
