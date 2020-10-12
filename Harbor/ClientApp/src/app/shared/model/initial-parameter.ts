export class InitialParameter {
    boatCount: number = 0;
    anchorSize: number = 0;
    anchorTime: number = 0;
    windSpeed: number = 0;
    autoGenerateBoatTime: number = 1;
    oneHourPerSecond: number = 10;
    perimeterLineDistance: number = 10;
    nextAuotGeneratedBoatTime?: Date;
    token: string = '';
}