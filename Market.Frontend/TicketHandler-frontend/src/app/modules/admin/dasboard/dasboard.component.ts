import { Component, inject, OnInit } from '@angular/core';
import { BaseComponent } from '../../../core/components/base-classes/base-component';
import { DashboardApiService } from '../../../api-services/dashboard/dashboard-api.service';
import { GetDashboardQueryDto } from '../../../api-services/dashboard/dashboard-api.model';

interface DashboardStat {
  key: keyof GetDashboardQueryDto;
  label: string;
  icon: string;
  variant: 'users' | 'organizers' | 'events' | 'performers' | 'sales' | 'revenue';
  isCurrency?: boolean;
}

@Component({
  selector: 'app-dasboard',
  standalone: false,
  templateUrl: './dasboard.component.html',
  styleUrl: './dasboard.component.scss',
})
export class DasboardComponent extends BaseComponent implements OnInit {
  private api = inject(DashboardApiService);

  dashboard: GetDashboardQueryDto | null = null;

  readonly stats: DashboardStat[] = [
    { key: 'userCount', label: 'Users', icon: 'group', variant: 'users' },
    { key: 'organizerCount', label: 'Organizers', icon: 'business', variant: 'organizers' },
    { key: 'eventCount', label: 'Events', icon: 'event', variant: 'events' },
    { key: 'performerCount', label: 'Performers', icon: 'mic', variant: 'performers' },
    { key: 'ticketSales', label: 'Tickets sold', icon: 'confirmation_number', variant: 'sales' },
    { key: 'revenue', label: 'Revenue', icon: 'payments', variant: 'revenue', isCurrency: true },
  ];

  ngOnInit(): void {
    this.loadDashboard();
  }

  loadDashboard(): void {
    this.startLoading();

    this.api.get().subscribe({
      next: (response) => {
        this.dashboard = response;
        this.stopLoading();
      },
      error: (err) => {
        this.dashboard = null;
        this.stopLoading('Failed to load dashboard');
        console.error('Load dashboard error:', err);
      },
    });
  }

  statValue(stat: DashboardStat): number {
    return this.dashboard ? this.dashboard[stat.key] : 0;
  }
}
