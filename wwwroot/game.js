const connection = new signalR.HubConnectionBuilder().withUrl('/gameHub').build();
let playerId = 0;

connection.on('State', state => {});

connection.start().then(() => {
	connection.invoke('Join').then(id => {
		playerId = id;
		console.log('Joined as player', id);
	}).catch(console.error);
}).catch(console.error);

const keys = {};
window.addEventListener('keydown', e => {
	if (['ArrowLeft','ArrowRight','ArrowUp','ArrowDown'].includes(e.key)) {
		keys[e.key] = true;
		sendInput();
		e.preventDefault();
	}
});
window.addEventListener('keyup', e => {
	if (['ArrowLeft','ArrowRight','ArrowUp','ArrowDown'].includes(e.key)) {
		keys[e.key] = false;
		sendInput();
		e.preventDefault();
	}
});

function sendInput(){
	let vx = 0, vy = 0;
	if (keys['ArrowLeft']) vx -= 1;
	if (keys['ArrowRight']) vx += 1;
	if (keys['ArrowUp']) vy -= 1;
	if (keys['ArrowDown']) vy += 1;
	connection.invoke('SendInput', vx, vy).catch(console.error);
}

function refreshFrame(){
	const img = document.getElementById('gameFrame');
	img.src = `/frame?ts=${Date.now()}`;
}

setInterval(refreshFrame, 100);
refreshFrame();
