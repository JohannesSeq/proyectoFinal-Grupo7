let board = [];
let moves = [];
let startTime = Date.now();
let timerInterval = null;

let currentTurnIndex = 0;
let currentPlayer = null;

let MODE;
let GAME_ID;
let PLAYERS;

function initBoard() {
    board = [];
    for (let r = 0; r < 5; r++) {
        let row = [];
        for (let c = 0; c < 5; c++) {
            row.push({ symbol: "N", point: null });
        }
        board.push(row);
    }
}

function renderBoard() {
    document.querySelectorAll(".cube").forEach(c => {
        const r = parseInt(c.dataset.row);
        const col = parseInt(c.dataset.col);
        const data = board[r][col];

        c.innerHTML = data.symbol;
    });
}

function isEdge(r, c) {
    return r === 0 || r === 4 || c === 0 || c === 4;
}

function canTakeCube(player, r, c) {
    const cube = board[r][c];

    if (cube.symbol !== "N" && cube.symbol !== playerSymbol(player)) return false;

    if (MODE === 1) return true;

    if (cube.symbol === "N") return true;

    return cube.point === playerOrderToPoint(player.order);
}

function playerSymbol(player) {
    return player.team === "A" ? "O" : "X";
}

function playerOrderToPoint(order) {
    if (order === 1) return "Top";
    if (order === 2) return "Right";
    if (order === 3) return "Bottom";
    if (order === 4) return "Left";
    return null;
}

function nextTurn() {
    currentTurnIndex = (currentTurnIndex + 1) % PLAYERS.length;
    currentPlayer = PLAYERS[currentTurnIndex];
}

function setupClicks() {
    document.querySelectorAll(".cube").forEach(div => {
        div.addEventListener("click", () => handleClick(div));
    });
}

let selected = null;

function handleClick(div) {
    const r = parseInt(div.dataset.row);
    const c = parseInt(div.dataset.col);

    if (!isEdge(r, c)) return;

    if (!selected) {
        if (!canTakeCube(currentPlayer, r, c)) return;

        selected = { r, c };
        div.style.background = "#ccc";
        return;
    }

    if (selected.r === r && selected.c === c) {
        div.style.background = "#eee";
        selected = null;
        return;
    }

    const dir = getPushDirection(selected.r, selected.c, r, c);
    if (!dir) return;

    pushCube(selected, dir);

    selected = null;
    document.querySelectorAll(".cube").forEach(x => x.style.background = "#eee");

    renderBoard();

    if (checkWinner(currentPlayer)) {
        endGame(currentPlayer);
        return;
    }

    if (MODE === 2) {
        if (checkOpponentLine(currentPlayer)) {
            endGameOpponentLine();
            return;
        }
    }

    nextTurn();
}

function getPushDirection(fr, fc, tr, tc) {
    if (fr === 0 && tr === 4 && fc === tc) return "down";
    if (fr === 4 && tr === 0 && fc === tc) return "up";
    if (fc === 0 && tc === 4 && fr === tr) return "right";
    if (fc === 4 && tc === 0 && fr === tr) return "left";
    return null;
}

function pushCube(sel, dir) {
    let removed = board[sel.r][sel.c];
    removed = { symbol: "N", point: null };

    const sym = playerSymbol(currentPlayer);
    const pt = MODE === 2 ? decidePointOrientation(currentPlayer) : null;

    if (dir === "down") {
        for (let i = sel.r; i < 4; i++) board[i][sel.c] = board[i + 1][sel.c];
        board[4][sel.c] = { symbol: sym, point: pt };
    }

    if (dir === "up") {
        for (let i = sel.r; i > 0; i--) board[i][sel.c] = board[i - 1][sel.c];
        board[0][sel.c] = { symbol: sym, point: pt };
    }

    if (dir === "right") {
        for (let i = sel.c; i < 4; i++) board[sel.r][i] = board[sel.r][i + 1];
        board[sel.r][4] = { symbol: sym, point: pt };
    }

    if (dir === "left") {
        for (let i = sel.c; i > 0; i--) board[sel.r][i] = board[sel.r][i - 1];
        board[sel.r][0] = { symbol: sym, point: pt };
    }

    moves.push({
        MoveNumber: moves.length + 1,
        PlayerId: currentPlayer.id,
        FromRow: sel.r,
        FromCol: sel.c,
        ToRow: (dir === "down" ? 4 : dir === "up" ? 0 : sel.r),
        ToCol: (dir === "right" ? 4 : dir === "left" ? 0 : sel.c),
        Symbol: sym,
        PointOrientation: pt
    });
}

function decidePointOrientation(player) {
    return playerOrderToPoint(player.order);
}

function checkWinner(player) {
    const sym = playerSymbol(player);

    for (let r = 0; r < 5; r++) {
        if (board[r].every(c => c.symbol === sym)) return true;
    }

    for (let c = 0; c < 5; c++) {
        let ok = true;
        for (let r = 0; r < 5; r++) if (board[r][c].symbol !== sym) ok = false;
        if (ok) return true;
    }

    let ok1 = true, ok2 = true;
    for (let i = 0; i < 5; i++) if (board[i][i].symbol !== sym) ok1 = false;
    for (let i = 0; i < 5; i++) if (board[i][4 - i].symbol !== sym) ok2 = false;

    return ok1 || ok2;
}

function checkOpponentLine(player) {
    const opponentSymbol = playerSymbol(player) === "O" ? "X" : "O";

    for (let r = 0; r < 5; r++)
        if (board[r].every(c => c.symbol === opponentSymbol))
            return true;

    for (let c = 0; c < 5; c++) {
        let ok = true;
        for (let r = 0; r < 5; r++) if (board[r][c].symbol !== opponentSymbol) ok = false;
        if (ok) return true;
    }

    let ok1 = true, ok2 = true;
    for (let i = 0; i < 5; i++) if (board[i][i].symbol !== opponentSymbol) ok1 = false;
    for (let i = 0; i < 5; i++) if (board[i][4 - i].symbol !== opponentSymbol) ok2 = false;

    return ok1 || ok2;
}

function endGame(player) {
    clearInterval(timerInterval);
    saveGame({
        winnerPlayerId: player.id,
        winnerTeam: null
    });
}

function endGameOpponentLine() {
    clearInterval(timerInterval);
    const winningTeam = currentPlayer.team === "A" ? "B" : "A";
    saveGame({
        winnerPlayerId: null,
        winnerTeam: winningTeam
    });
}

function saveGame(result) {
    const duration = Math.floor((Date.now() - startTime) / 1000);

    const finalState = JSON.stringify(board);

    const payload = {
        GameId: GAME_ID,
        DurationSeconds: duration,
        FinalState: finalState,
        WinnerPlayerId: result.winnerPlayerId,
        WinnerTeam: result.winnerTeam,
        Moves: moves
    };

    fetch("/Games/SaveResult", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload)
    }).then(() => location.href = "/Games/Finished");
}

function startTimer() {
    timerInterval = setInterval(() => {
        const s = Math.floor((Date.now() - startTime) / 1000);
        const h = String(Math.floor(s / 3600)).padStart(2, "0");
        const m = String(Math.floor((s % 3600) / 60)).padStart(2, "0");
        const sec = String(s % 60).padStart(2, "0");
        document.getElementById("gameTimer").textContent = `${h}:${m}:${sec}`;
    }, 1000);
}

function resetGame() {
    location.reload();
}

document.addEventListener("DOMContentLoaded", () => {
    MODE = window.MODE;
    GAME_ID = window.GAME_ID;
    PLAYERS = window.PLAYERS || [];

    document.getElementById("resetGame").addEventListener("click", resetGame);

    initBoard();
    renderBoard();
    setupClicks();

    currentPlayer = PLAYERS[0];
    startTimer();
});
