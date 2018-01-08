import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { UniversalModule } from 'angular2-universal';
import { FormsModule } from '@angular/forms';
import { AppComponent } from './components/app/app.component';
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { BypassComponent } from './components/bypass/bypass.component';

@NgModule({
    bootstrap: [ AppComponent ],
    declarations: [
        AppComponent,
        NavMenuComponent,
        BypassComponent
    ],
    imports: [
        UniversalModule,// Must be first import. This automatically imports BrowserModule, HttpModule, and JsonpModule too.
        FormsModule, 
        RouterModule.forRoot([
            { path: '', redirectTo: 'home', pathMatch: 'full' },
            { path: 'bypass', component: BypassComponent },
            { path: '**', redirectTo: 'home' }
        ])
    ]
})
export class AppModule {
}
