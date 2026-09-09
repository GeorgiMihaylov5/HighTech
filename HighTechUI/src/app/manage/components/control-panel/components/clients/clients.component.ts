import { Component } from '@angular/core';
import { Observable } from 'rxjs';
import { ClientService } from 'src/app/manage/services/client.service';
import { Client } from 'src/app/manage/models/client.model';

@Component({
	selector: 'app-clients',
	templateUrl: './clients.component.html',
	styleUrls: ['./clients.component.css'],
	standalone: false
})
export class ClientsComponent {
	public clients: Observable<Client[]>;

	constructor(clientApi: ClientService) {
		this.clients = clientApi.getClients();
	}

	public getClientsWithPhone(clients: Client[]): number {
		return clients.filter(client => Boolean(client.phoneNumber?.trim())).length;
	}

	public getClientsWithAddress(clients: Client[]): number {
		return clients.filter(client => Boolean(client.address?.trim())).length;
	}
}
