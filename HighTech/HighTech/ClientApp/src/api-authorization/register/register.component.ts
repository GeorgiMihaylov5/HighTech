import { Component, Input } from "@angular/core";

@Component({
    selector: 'app-register',
    templateUrl: './register.component.html',
    styleUrls: ['./register.component.css']
  })
  export class RegisterComponent {
    @Input() username: string = '';
    @Input() password: string = '';
    @Input() confirmPassword: string = '';

    constructor(){

    }

    login(){
        console.log(`u: ${this.username} p: ${this.password} cp: ${this.confirmPassword}`)
    }
  }