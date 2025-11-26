const { env } = require('process');

const target = "https://localhost:7140"

const PROXY_CONFIG = [
	{
		//   context: [
		//     "/weatherforecast",
		//     "/_configuration",
		//     "/.well-known",
		//     "/Identity",
		//     "/connect",
		//     "/ApplyDatabaseMigrations",
		//     "/_framework"
		//  ],
		context: (path, req) => true,
		proxyTimeout: 10000,
		target: target,
		secure: false,
		headers: {
			Connection: 'Keep-Alive'
		}
	}
]

module.exports = PROXY_CONFIG;
