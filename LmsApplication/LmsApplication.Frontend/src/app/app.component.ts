import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import {OidcSecurityService} from "angular-auth-oidc-client";
import {HttpClient, HttpHeaders} from "@angular/common/http";

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'LmsApplication.Frontend';

  constructor(public oidcSecurityService: OidcSecurityService, private http: HttpClient) {
    this.oidcSecurityService.checkAuth().subscribe((isAuthenticated) => {
      console.log("isAuthenticated: ", isAuthenticated);
    });
  }

  login() {
    this.oidcSecurityService.authorize();
  }

  sendrequest() {
    this.oidcSecurityService.getIdToken().subscribe((token) => {
      const headers = new HttpHeaders().set("Authorization", "Bearer " + token).set("X-Tenant-Id", "1");
      this.http.get("http://localhost:5120/api/auth", {headers}).subscribe((data) => {
        console.log("data: ", data);
      });
    });
  }
}
