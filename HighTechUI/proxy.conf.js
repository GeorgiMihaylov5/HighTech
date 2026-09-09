const { env } = require('process');

const target = "https://localhost:7140"

const PROXY_CONFIG = [
	{
		context: [
			"/Products",
			"/Categories",
			"/Clients",
			"/Employees",
			"/Fields",
			"/Orders",
			"/Reviews",
			"/Favorites",
			"/Configurator",
			"/Assistant",
			"/uploads",
			"/api"
		],
		proxyTimeout: 100000,
		target: target,
		secure: false,
		headers: {
			Connection: 'Keep-Alive'
		}
	}
]

module.exports = PROXY_CONFIG;
