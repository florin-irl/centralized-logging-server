import { Component } from '@angular/core';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule, HttpClientModule],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title(title: any) {
      throw new Error('Method not implemented.');
  }
  alpha = { phoneId: 0, phoneNr: '', entries: 1 };
  charlie = { phoneId: 0, phoneNr: '', entries: 1 };

  constructor(private http: HttpClient) {}

  clearForm(service: 'alpha' | 'charlie') {
    if (service === 'alpha') {
      this.alpha = { phoneId: 0, phoneNr: '', entries: 1 };
    } else {
      this.charlie = { phoneId: 0, phoneNr: '', entries: 1 };
    }
  }

  sendData(service: 'alpha' | 'charlie') {
    const form = service === 'alpha' ? this.alpha : this.charlie;
    const entries = this.generateEntries(form);

    const url =
      service === 'alpha'
        ? 'http://localhost:8081/phones' // Change to actual Alpha endpoint
        : 'http://localhost:8082/phones'; // Change to actual Charlie endpoint

    this.http.post(url, entries).subscribe({
      next: () => alert(`${service.toUpperCase()} data sent successfully!`),
      error: (err) => alert(`Error sending to ${service}: ${err.message}`)
    });
  }

  private generateEntries(form: { phoneId: number; phoneNr: string; entries: number }) {
    const list = [];
    list.push({ phoneId: form.phoneId, phoneNr: form.phoneNr });

    for (let i = 1; i < form.entries; i++) {
      list.push({
        phoneId: Math.floor(Math.random() * 100000),
        phoneNr: `07${Math.floor(10000000 + Math.random() * 89999999)}`
      });
    }

    return list;
  }
}
