module Stebs {
    export interface IState {
        assemble(): void;
        assembled(): void;
        start(): void;
        debug(): void;
        startOrPause(): void;
        stop(): void;
        singleStep(stepSize: SimulationStepSize): void;
    }

    /**
     * Current state of the state machine.
     */
    export var state: IState;
    export function stateInit(): void {
        state = new InitialState();
    };

    enum ContinuousOrSingleStep { Continuous, SingleStep }
    var actions = {
        assemble: 'assemble',
        start: 'start',
        debug: 'debug',
        startOrPause: 'startOrPause',
        stop: 'stop',
        microStep: 'microStep',
        macroStep: 'macroStep',
        instructionStep: 'instructionStep'
    };

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
        singleStep(stepSize: SimulationStepSize) { }
    }

    class InitialState extends StateAdapter {
        constructor() { super([actions.assemble]); }
        assemble() {
            serverHub.assemble();
        }
        assembled() {
            state = new AssembledState();
        }
    }

    class AssembledState extends StateAdapter {
        constructor() { super([actions.assemble, actions.start, actions.debug, actions.startOrPause]); }
        startOrPause() { this.start(); }
        start() {
            state = new RunningState();
        }
        debug() {
            state = new PausedState();
        }
    }

    class RunningState extends StateAdapter {
        constructor() { super([actions.startOrPause, actions.stop], ContinuousOrSingleStep.Continuous); }
        startOrPause() {
            state = new PausedState();
        }
        stop() {
            state = new AssembledState();
        }
    }

    class PausedState extends StateAdapter {
        constructor() { super([actions.start, actions.startOrPause, actions.stop, actions.microStep, actions.macroStep, actions.instructionStep], ContinuousOrSingleStep.SingleStep);}
        start() {
            state = new RunningState();
        }
        startOrPause() { this.start(); }
        stop() {
            state = new AssembledState();
        }
        singleStep(stepSize: SimulationStepSize) {
            Stebs.serverHub.singleStep(stepSize);
        }
    }
}