var assemblerInstruction = {
    END: 'variable-2',
    ORG: 'variable-2',
    DB: 'variable-2'
};
CodeMirror.defineMode('assembler', function (_config) {
    'use strict';

    // If an architecture is specified, its initialization function may
    // populate this array with custom parsing functions which will be
    // tried in the event that the standard functions do not find a match.
    var custom = [];

    var registers = {
        AL: 'keyword',
        BL: 'keyword',
        CL: 'keyword',
        DL: 'keyword'
    };

    function nextUntilUnescaped(stream, end) {
        var escaped = false, next;
        while ((next = stream.next()) != null) {
            if (next === end && !escaped) {
                return false;
            }
            escaped = !escaped && next === '\\';
        }
        return escaped;
    }

    function clikeComment(stream, state) {
        var maybeEnd = false, ch;
        while ((ch = stream.next()) != null) {
            if (ch === '/' && maybeEnd) {
                state.tokenize = null;
                break;
            }
            maybeEnd = (ch === '*');
        }
        return 'comment';
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

            if (ch === '/') {
                if (stream.eat('*')) {
                    state.tokenize = clikeComment;
                    return clikeComment(stream, state);
                }
            }
            //Comment
            if (ch === ';') {
                stream.skipToEnd();
                return 'comment';
            }
            if (ch === '"') {
                nextUntilUnescaped(stream, '"');
                return 'string';
            }
            if (ch === '=') {
                stream.eatWhile(/\w/);
                return 'tag';
            }
            if (ch === '[' || ch === ']') {
                return 'braket';
            }
            if (/\d/.test(ch)) {
                    stream.eatWhile(/[0-9a-fA-F]/);
                    return 'number';
            }
            if (/\w/.test(ch)) {
                stream.eatWhile(/\w/);
                //Tag (Main:)
                if (stream.eat(':')) {
                    return 'tag';
                }
                cur = stream.current();
                //Check registers for Registername
                style = registers[cur];
                if (style != null) {
                    return style;
                }
                //Check SpecialNames
                style = assemblerInstruction[cur];
                if (style != null) {
                    return style;
                } else if (/[0-9a-fA-F]/.test(ch)) {
                    //TODO: Also 'AX' will be colored atm
                    return 'number';
                }
                return null;
            }
        },

        lineComment: ';',
    };
});
