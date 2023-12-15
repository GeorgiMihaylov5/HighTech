import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent {
  title = 'app';
  /**
   *
   */
  constructor() {
    const event = new Event('localStorageChanged');
    window.dispatchEvent(event);

    window.addEventListener('localStorageChanged', () => {
      // Update the UI or perform any other necessary actions
      console.log('localStorage changed!');
    });
  }
    
}
