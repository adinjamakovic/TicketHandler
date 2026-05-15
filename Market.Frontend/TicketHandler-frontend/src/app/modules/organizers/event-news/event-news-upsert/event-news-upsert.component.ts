import {Component, inject, OnInit} from '@angular/core';
import {EventNewsService} from '../services/event-news-form.service';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';
import {EventNewsApiService} from '../../../../api-services/event-news/event-news-api.service';
import {ToasterService} from '../../../../core/services/toaster.service';
import {FormGroup} from '@angular/forms';
import {CreateEventNewsCommand, UpdateEventNewsCommand} from '../../../../api-services/event-news/event-news-api.model';
import {CurrentUserService} from '../../../../core/services/auth/current-user.service';
import {OrganizerApiService} from '../../../../api-services/organizers/organizers-api.service';
import {EventsApiService} from '../../../../api-services/events/events-api.service';
import {ListEventsQueryDto} from '../../../../api-services/events/events-api.model';

export interface EventNewsDialogData {
  mode: 'create' | 'edit';
  eventNewsId?: number;
}

@Component({
  selector: 'app-event-news-upsert',
  standalone: false,
  templateUrl: './event-news-upsert.component.html',
  styleUrl: './event-news-upsert.component.scss',
  providers: [EventNewsService, EventsApiService],
})
export class EventNewsUpsertComponent implements OnInit{
    private dialogRef=inject(MatDialogRef<EventNewsUpsertComponent>);
    data = inject<EventNewsDialogData>(MAT_DIALOG_DATA);
    private api = inject(EventNewsApiService);
    private eventsApi = inject(EventsApiService);
    private formService = inject(EventNewsService);
    private toaster = inject(ToasterService);

    events: ListEventsQueryDto[] =[];
    form!: FormGroup;
    isLoading = false;
    isEditMode = false;
    title='';

    ngOnInit(): void {
      this.isEditMode = this.data.mode === 'edit';
      this.title = this.isEditMode ? 'Edit News' : "Add News";
      this.loadEvents();

      if(this.isEditMode && this.data.eventNewsId){
        this.loadNews(this.data.eventNewsId);
      } else {
        this.form = this.formService.createNewsForm();
      }
    }

  private loadEvents() : void {
        this.eventsApi.list().subscribe({
          next: (response)=>{
            this.events = response.items;
          },
          error: (err) => {
            this.toaster.error('Failed to load events.');
            console.error('Load events error', err);
          }
        });
    }

    private loadNews(id: number): void {
      this.isLoading = true;

      this.api.GetById(id).subscribe({
        next: (eventNews)=>{
          this.form = this.formService.createNewsForm(eventNews);
          this.isLoading = false;
        },
        error: (err)=>{
          this.toaster.error('Failed to load category');
          console.error('Load category error:', err);
          this.dialogRef.close(false);
        }
      });
    }

    onSubmit() : void {
      if(this.form.invalid || this.isLoading){
        this.form.markAllAsTouched();
        return;
      }

      this.isLoading = true;

      if(this.isEditMode && this.data.eventNewsId){
        this.updateEventNews();
      } else {
        this.createEventNews();
      }
    }

    private createEventNews(): void {
      const command: CreateEventNewsCommand = {
        eventId: this.form.value.event,
        header: this.form.value.header,
        body: this.form.value.body,
      }

      this.api.create(command).subscribe({
        next: () => {
          this.toaster.success('Event news added successfully');
          this.dialogRef.close(true); // Signal success
        },
        error: (err) => {
          this.isLoading = false;
          console.error('Create event news error:', err);
        }
      });
    }

  updateEventNews() {
      const command: UpdateEventNewsCommand = {
        header: this.form.value.header,
        body: this.form.value.body,
      };

      this.api.update(this.data.eventNewsId!, command).subscribe({
        next: () => {
          this.toaster.success('News updated successfully');
          this.dialogRef.close(true);
        },
        error: (err) => {
          this.isLoading = false;
          console.error('Update event news error:', err);
        }
      });
    }
  onCancel(): void {
    this.dialogRef.close(false);
  }

  getErrorMessage(controlName: string): string {
    return this.formService.getErrorMessages(this.form, controlName);
  }
}
