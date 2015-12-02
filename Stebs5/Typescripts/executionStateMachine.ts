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

    abstract class StateAdapter implements IState {
        assemble() { }
        assembled() { }
        start() { }
        debug() { }
        startOrPause() { }
        stop() { }
        singleStep(stepSize: SimulationStepSize) { }
    }

    /**
     * Current state of the state machine.
     */
    export var state: IState

    class InitialState extends StateAdapter {
        assemble() {
            serverHub.assemble();
        }
        assembled() {
            state = new AssembledState();
        }
    }

    class AssembledState extends StateAdapter {
        start() {
            state = new RunningState();
        }
        debug() {
            state = new PausedState();
        }
    }

    class RunningState extends StateAdapter {
        startOrPause() {
            state = new PausedState();
        }
        stop() {
            //TODO
        }
    }

    class PausedState extends StateAdapter {
        startOrPause() {
            state = new RunningState();
        }
        stop() {
            //TODO
        }
        singleStep(stepSize: SimulationStepSize) {
            //TODO: execute
        }
    }
}