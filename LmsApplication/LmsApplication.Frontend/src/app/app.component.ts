import {Component, effect} from '@angular/core';
import { CommonModule } from '@angular/common';
import {ActivatedRoute, RouterOutlet} from '@angular/router';
import {AuthService} from "./services/auth.service";
import {HeaderComponent} from "./components/header/header.component";

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, HeaderComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'LmsApplication.Frontend';

  isLoading = this.authService.isLoading;

  constructor(private authService: AuthService, private route: ActivatedRoute) {
    // this.authService.checkLoading();
    // effect(() => {
    //   if (!this.isLoading()) {
    //     this.authService.checkAuth();
    //   }
    //   console.log('lmao')
    // })
    this.authService.checkAuth()
  }
}
