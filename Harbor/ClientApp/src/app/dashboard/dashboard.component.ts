import { Component, AfterViewInit } from '@angular/core';
import { MatDialog } from '@angular/material';

import { ConfigDialogComponent } from './config-dialog/config-dialog.component';
import { InitialParameter } from "./../shared/model/initial-parameter";
import { Boats, StatusOfHarbor, BoatPosition, BoatStatus } from "./../shared/model/status-of-harbor";
import { Response } from "./../shared/model/response";
import { APICallService } from "./../shared/service/apicall.service";

export interface DemoColor {
  name: string;
  color: string;
  modes: string;
}

@Component({
	selector: 'app-dashboard',
	templateUrl: './dashboard.component.html',
	styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements AfterViewInit {
	controllerName: string = 'Harbor';
	initialParameter: InitialParameter = new InitialParameter();
	harbor: StatusOfHarbor = new StatusOfHarbor();
	availableTotalAnchor: any[] = [];
	attempt: number = 0;

	constructor(public dialog: MatDialog,
		private api: APICallService){
			this.FirstAttempt();
		}
	
	ngAfterViewInit() {}

	private FirstAttempt() {
		this.initialParameter.token = this.api.getCookies();
		this.api.PostMethod(this.controllerName, 'FirstAttemt', this.initialParameter).subscribe(
			data => {
				let FirstAttemptResponse = data as Response;
				if (FirstAttemptResponse.statusCode == 100) {
					this.initialParameter = FirstAttemptResponse.data as InitialParameter;
					this.api.setCookies(this.initialParameter.token);
					document.getElementById('myConfigButton')?.click();
					this.attempt = 1;
				}
				else if (FirstAttemptResponse.statusCode == 200) {
					this.harbor = FirstAttemptResponse.data as StatusOfHarbor;
					this.setResponse(FirstAttemptResponse);
					setInterval(() => this.UpdateStatus(), 2000);
				}
			},
			error => {
			}
		);
	}

	openDialog(): void {
	  const dialogRef = this.dialog.open(ConfigDialogComponent, {
		width: '65%',
		data: this.initialParameter,
		disableClose: true
	  });
  
	  dialogRef.afterClosed().subscribe(result => {
		if (result != undefined) {
			this.initialParameter = result;
			if (this.attempt == 1) {
				this.attempt = 0;
				this.api.PostMethod(this.controllerName, 'SetInitial', this.initialParameter).subscribe(
					data => {
						let response: Response = data as Response;
						if (response.statusCode == 200) {
							this.setResponse(response);
							setInterval(() => this.UpdateStatus(), 2000);
						}
					}
				);
			}
		}
	  });
	}

	GetAvailableTotalBoatCount() {
		if (this.harbor == undefined){
			return 0;
		}
		return this.harbor.boatStatus.filter(b => b.boatPosition == BoatPosition.AtPerimeter).length;
	}

	GetBoatCount(boat: Boats) {
		if (this.harbor == undefined){
			return 0;
		}
		return this.harbor.boatStatus.filter(b => b.boats == boat && b.boatPosition == BoatPosition.AtPerimeter).length;
	}

	GetShippedTotalBoatCount() {
		if (this.harbor == undefined){
			return 0;
		}
		return this.harbor.boatStatus.filter(b => b.boatPosition == BoatPosition.Shipped).length;
	}

	GetShippedBoatCount(boat: Boats) {
		if (this.harbor == undefined){
			return 0;
		}
		return this.harbor.boatStatus.filter(b => b.boats == boat && b.boatPosition == BoatPosition.Shipped).length;
	}

	GetBoatOnAnchor() {
		if (this.harbor == undefined){
			return [];
		}
		return this.harbor.boatStatus.filter(b => b.boatPosition == BoatPosition.InComing || b.boatPosition == BoatPosition.OutGoing || b.boatPosition == BoatPosition.Anchored);
	}

	GetBoatName(boats: Boats) {
		return Boats[boats];
	}

	GetBoatPositionName(boatsPosition: BoatPosition) {
		return BoatPosition[boatsPosition];
	}

	UpdateStatus() {
		this.harbor.initialParameter = this.initialParameter;
		this.api.PostMethod(this.controllerName, 'UpdateStatus', this.harbor).subscribe(
			data => {
				let response: Response = data as Response;
				if (response.statusCode == 200) {
					this.setResponse(response);
				}
			}
		)
	}

	setResponse(response: Response) {
		if (response.statusCode == 200) {
			this.harbor = response.data as StatusOfHarbor;
			let boatOnAnchor: BoatStatus[] = this.GetBoatOnAnchor() as BoatStatus[];
			this.initialParameter = this.harbor.initialParameter;
			this.availableTotalAnchor = [];
			for (let i = 1; i < this.initialParameter.anchorSize + 1; i++) {
				if (boatOnAnchor.length >= i) {
					this.availableTotalAnchor.push({
						SrNo: i,
						BoatStatus: boatOnAnchor[i-1]
					});
				}
				else {
					this.availableTotalAnchor.push({
						SrNo: i,
						BoatStatus: null
					});
				}
			}
		}
	}
}