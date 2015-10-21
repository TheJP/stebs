class Ram {
    ramContent: Number[][];

    constructor() {
        this.ramContent = [];

        for (var i: number = 0; i < 10; i++) {
            this.ramContent[i] = [];
            for (var j: number = 0; j < 10; j++) {
                this.ramContent[i][j] = new Number(0);
            }
        }
    }
}