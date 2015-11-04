CodeMirror.defineMode("assembler", function (_config) {
    'use strict';

    // If an architecture is specified, its initialization function may
    // populate this array with custom parsing functions which will be
    // tried in the event that the standard functions do not find a match.
    var custom = [];

    var directives = {
        mov: "variable-2",
        shl: "variable-2",
        dec: "variable-2",
        jnz: "variable-2",
        end: "variable-2"
    };

    var registers = {
        al: "keyword",
        bl: "keyword",
        cl: "keyword",
        dl: "keyword"
    };

    function setx86Registers() {
        
    }
    setx86Registers();

    function nextUntilUnescaped(stream, end) {
        var escaped = false, next;
        while ((next = stream.next()) != null) {
            if (next === end && !escaped) {
                return false;
            }
            escaped = !escaped && next === "\\";
        }
        return escaped;
    }

    function clikeComment(stream, state) {
        var maybeEnd = false, ch;
        while ((ch = stream.next()) != null) {
            if (ch === "/" && maybeEnd) {
                state.tokenize = null;
                break;
            }
            maybeEnd = (ch === "*");
        }
        return "comment";
    }

    return {
        startState: function () {
            return {
                tokenize: null
            };
        },

        token: function (stream, state) {
            if (state.tokenize) {
                return state.tokenize(stream, state);
            }

            if (stream.eatSpace()) {
                return null;
            }

            var style, cur, ch = stream.next();

            if (ch === "/") {
                if (stream.eat("*")) {
                    state.tokenize = clikeComment;
                    return clikeComment(stream, state);
                }
            }
            //Comment
            if (ch === ';') {
                stream.skipToEnd();
                return "comment";
            }
            if (ch === '"') {
                nextUntilUnescaped(stream, '"');
                return "string";
            }
            if (ch === '=') {
                stream.eatWhile(/\w/);
                return "tag";
            }
            if (/\d/.test(ch)) {
                if (ch === "0" && stream.eat("x")) {
                    stream.eatWhile(/[0-9a-fA-F]/);
                    return "number";
                }
                stream.eatWhile(/\d/);
                return "number";
            }

            if (/\w/.test(ch)) {
                stream.eatWhile(/\w/);
                //Tag (Main:)
                if (stream.eat(":")) {
                    return 'tag';
                }
                cur = stream.current().toLowerCase();
                //Check registers for Registername
                style = registers[cur];
                if (style != null) {
                    return style;
                } else {
                    //Check SpecialNames
                    style = directives[cur];
                }
                return style || null;
            }
            /*
            if (ch === '{' || ch === '}') {
                return "braket";
            }
            */
        },

        lineComment: ";",
    };
});
