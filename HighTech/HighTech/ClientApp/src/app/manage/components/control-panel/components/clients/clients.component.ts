import { Component } from '@angular/core';
import { Observable } from 'rxjs';
import { ClientService } from 'src/app/manage/services/client.service';
import { IClient } from 'src/app/models/client.model';

@Component({
  selector: 'app-clients',
  templateUrl: './clients.component.html',
  styleUrls: ['./clients.component.css']
})
export class ClientsComponent {
  public clients: Observable<IClient[]>;

  constructor(clientApi: ClientService) {
    this.clients = clientApi.getClients();
  }
}
