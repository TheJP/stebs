module Stebs {

    /**
     * The IState interface is the base of all States.
     * The defined methods describe possible state transitions.
     */
    export interface IState {
        assemble(): void;
        assembled(): void;
        start(): void;
        debug(): void;
        startOrPause(): void;
        stop(): void;
        halted(): void;
        singleStep(stepSize: SimulationStepSize): void;
    }

    /**
     * Current state of the state machine.
     */
    export var state: IState;

    /**
     * Sets the state to the initial value.
     */
    export function stateInit(first = true): void {
        state = new InitialState(first);
    };

    /**
     * Determines, if the radio buttons or buttons should be showed for the step size.
     */
    enum ContinuousOrSingleStep { Continuous, SingleStep }

    /**
     * Button ids, which are used the specify, which state transitions are supported.
     */
    var actions = {
        assemble: 'assemble',
        start: 'start',
        debug: 'debug',
        pause: 'pause',
        continue: 'continue',
        stop: 'stop',
        reset: 'reset',
        microStep: 'microStep',
        macroStep: 'macroStep',
        instructionStep: 'instructionStep'
    };

    /**
     * Adapter which allows that states don't have to implement the full interface IState.
     * The constructor has methods to enable and disable gui elements according to the specific state.
     */
    abstract class StateAdapter implements IState {
        constructor(allowedActions: string[], continuousOrSingleStep: ContinuousOrSingleStep = ContinuousOrSingleStep.Continuous) {
            for (var action in actions) {
                $('#' + action).prop('disabled', $.inArray(action, allowedActions) < 0);
            }
            if (continuousOrSingleStep == ContinuousOrSingleStep.Continuous) {
                $('.stepSizeButtons').hide();
                $('.stepSizeRadios').show();
            } else {
                $('.stepSizeButtons').show();
                $('.stepSizeRadios').hide();
            }
        }
        assemble() { }
        assembled() { }
        start() { }
        debug() { }
        startOrPause() { }
        stop() { }
        halted() { }
        singleStep(stepSize: SimulationStepSize) { }

        protected editMode(enable: boolean) {
            if (enable) {
                Stebs.codeEditor.setOption('readOnly', false);
                Stebs.codeEditor.setOption('cursorBlinkRate', 530);
                $('#codingFrame').removeClass('running');
                Stebs.codeEditor.off('mousedown', disableEvent);
            } else {
                Stebs.codeEditor.setOption('readOnly', 'nocursor');
                Stebs.codeEditor.setOption('cursorBlinkRate', -1);
                $('#codingFrame').addClass('running');
                Stebs.codeEditor.on('mousedown', disableEvent);
            }
        }
    }

    var disableEvent = function (instance: any, event: Event) {
        event.preventDefault();
    }

    /**
     * Starting state of the application.
     */
    class InitialState extends StateAdapter {
        constructor(first = false) {
            super([actions.assemble]);
            if (!first) { super.editMode(true); }
        }
        assemble() {
            serverHub.assemble();
        }
        assembled() {
            state = new AssembledState();
        }
    }

    /**
     * State after assembling, before execution of the simulation.
     */
    class AssembledState extends StateAdapter {
        constructor() {
            super([actions.assemble, actions.start, actions.debug, actions.continue]);
            super.editMode(true);
        }
        assemble() {
            state = new InitialState();
            state.assemble();
        }
        startOrPause() { this.start(); }
        start() {
            state = new RunningState();
            serverHub.run(Stebs.ui.getStepSize());
            Stebs.ui.highlightLine(0);
        }
        debug() {
            state = new PausedState();
            serverHub.pause();
            Stebs.ui.highlightLine(0);
        }
    }

    /**
     * Running simulation state.
     */
    class RunningState extends StateAdapter {
        constructor() {
            super([actions.pause, actions.stop, actions.assemble], ContinuousOrSingleStep.Continuous);
            super.editMode(false);
        }
        assemble() {
            state = new InitialState();
            serverHub.stop();
            state.assemble();
        }
        startOrPause() {
            state = new PausedState();
            serverHub.pause();
        }
        stop() {
            state = new AssembledState();
            serverHub.stop();
        }
        halted() {
            state = new InitialState();
        }
    }

    /**
     * Paused / Single step simulation state.
     */
    class PausedState extends StateAdapter {
        constructor() {
            super([actions.start, actions.continue, actions.stop, actions.assemble, actions.microStep, actions.macroStep, actions.instructionStep], ContinuousOrSingleStep.SingleStep);
            super.editMode(false);
        }
        assemble() {
            state = new InitialState();
            serverHub.stop();
            state.assemble();
        }
        start() {
            state = new RunningState();
            serverHub.run(Stebs.ui.getStepSize());
        }
        startOrPause() { this.start(); }
        stop() {
            state = new AssembledState();
            serverHub.stop();
        }
        singleStep(stepSize: SimulationStepSize) {
            serverHub.singleStep(stepSize);
        }
        halted() {
            state = new InitialState();
        }
    }
}