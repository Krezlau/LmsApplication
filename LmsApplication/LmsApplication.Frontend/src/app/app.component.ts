import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { AuthService } from './services/auth.service';
import { HeaderComponent } from './components/header/header.component';
import { AlertService } from './services/alert.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, HeaderComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent {
  title = 'LmsApplication.Frontend';

  authStateLoading = this.authService.authStateLoading;
  alert = this.alertService.alert;

  constructor(
    private authService: AuthService,
    private alertService: AlertService,
  ) {
    this.authService.loadAuthState();
    // this.alertService.show("Hello World", "success")
  }

  hideAlert() {
    this.alertService.hide();
  }
}
