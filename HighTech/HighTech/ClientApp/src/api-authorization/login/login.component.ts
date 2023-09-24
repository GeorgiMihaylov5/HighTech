import { Component, Input } from "@angular/core";

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.css']
  })
  export class LoginComponent {
    @Input() username: string = '';
    @Input() password: string = '';

    constructor(){

    }

    login(){
        console.log(`u: ${this.username} p: ${this.password}`)
    }
  }