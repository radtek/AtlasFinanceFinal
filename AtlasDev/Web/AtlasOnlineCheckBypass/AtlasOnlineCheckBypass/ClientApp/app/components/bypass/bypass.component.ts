import { Component } from '@angular/core';
import { Headers, Http } from '@angular/http';

@Component({
    selector: 'bypass',
    template: require('./bypass.component.html')
})
export class BypassComponent {
    public bypassSettings: BypassSettings;
    private headers = new Headers({ 'Content-Type': 'application/json' });

    constructor(private http: Http) {
        http.get('api/bypass/currentsettings').subscribe(result => {
            this.bypassSettings = result.json();
        });
    }

    settingChanged(): void {
        this.http.put('api/bypass/setbypasssettings/', JSON.stringify(this.bypassSettings), { headers: this.headers }).subscribe(result => {

        });
    }
}

interface BypassSettings {
    creditCheck: boolean;
    avsCheck: boolean;
    affordabilityCheck: boolean;
    fraudCheck: boolean;
    xdsCheck: boolean;
}