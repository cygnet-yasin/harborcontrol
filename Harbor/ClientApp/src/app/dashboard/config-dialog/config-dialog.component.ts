import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';

@Component({
  selector: 'app-config-dialog',
  templateUrl: './config-dialog.component.html',
  styleUrls: ['./config-dialog.component.css']
})
export class ConfigDialogComponent implements OnInit {
  FirstAttempt: boolean = false;
  AllValid: boolean = true;

  constructor(
    public dialogRef: MatDialogRef<ConfigDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) { }

  ngOnInit() {
    this.FirstAttempt = this.data.boatCount == 0;
  }

  onNoClick(): void {
    this.dialogRef.close();
  }

  setFirstAttempt() {
    if (this.FirstAttempt) {
      this.updateData();
    }
  }

  updateData() {
    this.validate();
    if (this.AllValid) {
      this.dialogRef.close(this.data);
    }
  }

  validate() {
    if (this.data.boatCount == 0) {
      this.AllValid = false;
    }
    if (this.data.anchorSize == 0) {
      this.AllValid = false;
    }
    if (this.data.anchorTime == 0) {
      this.AllValid = false;
    }
    if (this.data.autoGenerateBoatTime == 0) {
      this.AllValid = false;
    }
    if (this.data.oneHourPerSecond == 0) {
      this.AllValid = false;
    }
    if (this.data.perimeterLineDistance == 0) {
      this.AllValid = false;
    }
  }
}

