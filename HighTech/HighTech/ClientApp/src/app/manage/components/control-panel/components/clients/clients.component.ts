import { Component } from '@angular/core';
import { Observable } from 'rxjs';
import { ClientService } from 'src/app/manage/services/client.service';
import { Client } from 'src/app/manage/models/client.model';

@Component({
  selector: 'app-clients',
  templateUrl: './clients.component.html',
  styleUrls: ['./clients.component.css']
})
export class ClientsComponent {
  public clients: Observable<Client[]>;

  constructor(clientApi: ClientService) {
    this.clients = clientApi.getClients();
  }
}
