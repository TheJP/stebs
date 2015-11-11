var Stebs;
(function (Stebs) {
    var Ram = (function () {
        function Ram(size) {
            this.ramContent = Array(size);
            for (var i = 0; i < size; i++) {
                this.ramContent[i] = 0;
            }
        }
        Ram.prototype.setContent = function (exising) {
            if (exising == null) {
                return false;
            }
            return true;
        };
        Ram.prototype.setRamAt = function (pos, val) {
            if (pos < 0 || pos >= this.ramContent.length || val < 0 || val > 255) {
                return false;
            }
            this.ramContent[pos] = val;
            return true;
        };
        Ram.prototype.getAsString = function (lineBreak) {
            var asString = "";
            for (var i = 0; i < this.ramContent.length; i++) {
                if (i % lineBreak == 0) {
                    asString += "\n";
                }
                if (i % 2 == 0) {
                    asString += " ";
                }
                asString += this.ramContent[i].toString(16);
            }
            return asString;
        };
        Ram.prototype.getAsTable = function (lineLengh) {
            var table;
            var thead;
            var tbody;
            table = document.createElement('table');
            //table.setAttribute("id", "ramTable");
            thead = table.createTHead();
            tbody = table.createTBody();
            var hrow = table.tHead.insertRow(0);
            hrow.insertCell(0).innerHTML = "";
            for (var i = 0; i < lineLengh / 2; i++) {
                var cell = hrow.insertCell(i + 1);
                cell.innerHTML = i.toString(16);
            }
            var newWith = (this.ramContent.length / (lineLengh));
            for (var i = 0; i < newWith / 2; i++) {
                var row = table.tHead.insertRow(i + 1);
                for (var j = 0; j < lineLengh / 2; j++) {
                    if (j == 0) {
                        var cell = row.insertCell(0);
                        cell.innerHTML = i.toString(16);
                    }
                    var cell = row.insertCell(j + 1);
                    cell.innerHTML = this.ramContent[(i * newWith) + (j * 2)].toString(16) + this.ramContent[(i * newWith) + (j * 2) + 1].toString(16);
                }
            }
            return table;
        };
        return Ram;
    })();
    Stebs.Ram = Ram;
    ;
})(Stebs || (Stebs = {}));
