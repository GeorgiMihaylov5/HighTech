import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ConfiguratorComponent } from './components/configurator/configurator.component';
import { PartPickerComponent } from './components/part-picker/part-picker.component';
import { BuildSummaryComponent } from './components/build-summary/build-summary.component';
import { ConfiguratorApiService } from './services/configurator-api.service';
import { ConfiguratorFacade } from './services/configurator-facade.service';
import { AppRoutingModule } from '../app.routing.module';

@NgModule({
  declarations: [
    ConfiguratorComponent,
    PartPickerComponent,
    BuildSummaryComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    AppRoutingModule
  ],
  providers: [
    ConfiguratorApiService,
    ConfiguratorFacade
  ]
})
export class ConfiguratorModule {}
