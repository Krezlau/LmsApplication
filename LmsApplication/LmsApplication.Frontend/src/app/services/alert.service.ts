import {Injectable, signal} from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AlertService {

  private _alert: Alert = {
    type: 'success',
    message: '',
    isVisible: false
  };

  public alert = signal(this._alert);

  constructor() { }

  public show(message: string, type: 'success' | 'info' | 'warning' | 'error') {
    if (this.alert().isVisible) {
      this.hide();
      setTimeout(() => {
        this.alert.set({
          type,
          message,
          isVisible: true
        });
      }, 400);
    }
    else {
      this.alert.set({
        type,
        message,
        isVisible: true
      });
    }
  }

  public hide() {
    this.alert.set({
      ...this.alert(),
      isVisible: false
    });
  }
}

export interface Alert {
  type : 'success' | 'info' | 'warning' | 'error'
  message: string;
  isVisible: boolean;
}
