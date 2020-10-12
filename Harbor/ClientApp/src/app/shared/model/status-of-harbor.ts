import { InitialParameter } from "./initial-parameter";

export class StatusOfHarbor {
    initialParameter: InitialParameter = new InitialParameter;
    boatStatus: BoatStatus[] = [];
}

export class BoatStatus {
    boats: Boats = Boats.CargoShip;
    boatPosition: BoatPosition = BoatPosition.AtPerimeter;
    estimateToReachNextPosition?: Date;
}

export enum Boats {
    SailBoat,
    CargoShip,
    SpeedBoat
}

export enum SpeedOfBoats {
    SailBoat = 15,
    CargoShip = 5,
    SpeedBoat = 30
}

export enum BoatPosition {
    AtPerimeter,
    InComing,
    Anchored,
    OutGoing,
    Shipped
}