# FSM Design

This project implements an example of a Finite State Machine (FSM). In the
code, the example implementation of this is called a Fixed State Machine
because each definition of the state machine is 'fixed' and statically
defined in its possible transitions and states.

## Finite State Machine

The main design of these classes is managed
by a set of [C# Attributes](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/attributes/)
to configure and manage controls for the state machine
directly from the C# code.

### Interfaces

* @nickmaltbie.StateMachineUnity.IStateMachine`1 -
    interface to manage a set of states and transitions.
* @nickmaltbie.StateMachineUnity.IAnimStateMachine`1 -
    extends @nickmaltbie.StateMachineUnity.IStateMachine`1
    and supports extra features such as an attached @UnityEngine.Animator
    and a currently selected animation.

### Implementations

implementations of the state machines.

* @nickmaltbie.StateMachineUnity.Fixed.FixedSM
  \- abstract implementation of @nickmaltbie.StateMachineUnity.IStateMachine`1
  with cached transitions and events from decorators from
  @nickmaltbie.StateMachineUnity.Utils.FSMUtils.
* @nickmaltbie.StateMachineUnity.Fixed.FixedSMBehaviour
  \- abstract implementation of @nickmaltbie.StateMachineUnity.IStateMachine`1
  with cached transitions and events from decorators from
  @nickmaltbie.StateMachineUnity.Utils.FSMUtils
  in addition to firing off events for Unity Messages and supports
  attributes such as
  @nickmaltbie.StateMachineUnity.Attributes.OnUpdateAttribute
* @nickmaltbie.StateMachineUnity.Fixed.FixedSMAnim
  \- abstract extension of the @nickmaltbie.StateMachineUnity.Fixed.FixedSM
  that supports updating an animation state of an @UnityEngine.Animator
  component of a state machine and fully implements the
  @nickmaltbie.StateMachineUnity.IAnimStateMachine`1

### Components

* @nickmaltbie.StateMachineUnity.State - A state for a given FSM.
* @nickmaltbie.StateMachineUnity.Attributes.TransitionAttribute
  \- Attribute to define and manage the transitions for a given state.
* @nickmaltbie.StateMachineUnity.Attributes.AnimationAttribute
  \- Attribute to select an animation state for the given @UnityEngine.Animator
  of a state machine
* @nickmaltbie.StateMachineUnity.Event.IEvent
  \- Event for managing transitions or executing actions in state machines.

* Entry and exit behaviors defined via the attributes:
  * @nickmaltbie.StateMachineUnity.Attributes.OnEnterStateAttribute
    \- Called when state is entered
  * @nickmaltbie.StateMachineUnity.Attributes.OnExitStateAttribute
    \- Called when the state is exited

* @nickmaltbie.StateMachineUnity.Attributes.OnEventDoAction
  \- Whenever an @nickmaltbie.StateMachineUnity.Event.IEvent
  is thrown invoke a given action.

* Update Attributes to be triggered on various @UnityEngine.MonoBehaviour
  functions including the following subset. There are other
  messages defined for the unity MonoBehaviour but these
  are the only planned ones as of now, feel free to extend
  the code or add your own events if you wish.

  * @nickmaltbie.StateMachineUnity.Attributes.OnUpdateAttribute
    \- Called each frame.
  * @nickmaltbie.StateMachineUnity.Attributes.OnFixedUpdateAttribute
    \- Called each fixed update.
  * @nickmaltbie.StateMachineUnity.Attributes.OnLateUpdateAttribute
    \- Called at the end of each frame.
  * @nickmaltbie.StateMachineUnity.Attributes.OnGUIAttribute
    \- Called each GUI update.
  * @nickmaltbie.StateMachineUnity.Attributes.OnEnableAttribute
    \- Called when object is enabled.
  * @nickmaltbie.StateMachineUnity.Attributes.OnDisableAttribute
    \- Called when object is disabled.
  * @nickmaltbie.StateMachineUnity.Attributes.OnAnimatorIKAttribute
    \- Callback for setting up animation IK (inverse kinematics).

* @nickmaltbie.StateMachineUnity.Attributes.AnimationTransitionAttribute
  \- Extension of @nickmaltbie.StateMachineUnity.Attributes.TransitionAttribute
  that will trigger an animation upon transition and can configure settings for
  the @nickmaltbie.StateMachineUnity.IAnimStateMachine`1.CrossFade* call
  for the animation transition between states.

## Customization

Some examples of custom state machines are added
in the test code under

* @nickmaltbie.StateMachineUnity.Tests.EditMode.Fixed.DemoFixedStateMachine
    \- Example implementation of a @nickmaltbie.StateMachineUnity.Fixed.FixedSM.
* @nickmaltbie.StateMachineUnity.Tests.EditMode.Fixed.DemoFixedStateMachineMonoBehaviour
    \- Example implementation of a @nickmaltbie.StateMachineUnity.Fixed.FixedSMBehaviour
* @nickmaltbie.StateMachineUnity.Tests.EditMode.Fixed.DemoFixedSMAim
    \- Example implementation of a @nickmaltbie.StateMachineUnity.Fixed.FixedSMAnim

In addition, the example FSM is explained in further detail in the
[Example FSM](example-fsm.md) document.
