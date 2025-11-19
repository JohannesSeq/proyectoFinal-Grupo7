let replayBoard = [];

function initReplayBoard() {
    replayBoard = [];
    for (let r = 0; r < 5; r++) {
        let row = [];
        for (let c = 0; c < 5; c++) {
            row.push({ symbol: "N" });
        }
        replayBoard.push(row);
    }
}

function renderReplayBoard() {
    document.querySelectorAll(".cell-replay").forEach(el => {
        const r = parseInt(el.dataset.row);
        const c = parseInt(el.dataset.col);
        const cell = replayBoard[r][c];
        el.textContent = cell.symbol;
    });
}

function applyMove(move) {
    const fr = move.from[0];
    const fc = move.from[1];
    replayBoard[fr][fc] = { symbol: "N" };

    const dir = determineDirection(move.from, move.to);

    if (dir === "down") {
        for (let i = fr; i < 4; i++) replayBoard[i][fc] = replayBoard[i + 1][fc];
        replayBoard[4][fc] = { symbol: move.symbol };
    }

    if (dir === "up") {
        for (let i = fr; i > 0; i--) replayBoard[i][fc] = replayBoard[i - 1][fc];
        replayBoard[0][fc] = { symbol: move.symbol };
    }

    if (dir === "right") {
        for (let i = fc; i < 4; i++) replayBoard[fr][i] = replayBoard[fr][i + 1];
        replayBoard[fr][4] = { symbol: move.symbol };
    }

    if (dir === "left") {
        for (let i = fc; i > 0; i--) replayBoard[fr][i] = replayBoard[fr][i - 1];
        replayBoard[fr][0] = { symbol: move.symbol };
    }
}

function determineDirection(from, to) {
    const fr = from[0], fc = from[1];
    const tr = to[0], tc = to[1];

    if (fr === 0 && tr === 4 && fc === tc) return "down";
    if (fr === 4 && tr === 0 && fc === tc) return "up";
    if (fc === 0 && tc === 4 && fr === tr) return "right";
    if (fc === 4 && tc === 0 && fr === tr) return "left";

    return null;
}

function replayTo(moveNumber) {
    initReplayBoard();

    for (let i = 0; i < moveNumber; i++) {
        applyMove(MOVES[i]);
    }

    renderReplayBoard();

    document.getElementById("moveLabel").textContent =
        "Movimiento " + moveNumber + " de " + MOVES.length;
}

document.getElementById("moveSlider").addEventListener("input", function () {
    const v = parseInt(this.value);
    replayTo(v);
});

initReplayBoard();
renderReplayBoard();
