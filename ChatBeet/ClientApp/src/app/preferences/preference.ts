import { User } from "../common/user";

export class Preference {
    public preference: PreferenceType;
    public value: string;
    public user?: User;

    constructor(data: any) {
        this.preference = data.preference;
        this.value = data.value;
        if (data.user)
            this.user = data.user;
    }
}
export enum PreferenceType {
    DateOfBirth,
    SubjectPronoun,
    ObjectPronoun,
    WorkHoursStart,
    WorkHoursEnd,
    PossessivePronoun,
    ReflexivePronoun,
    WeatherLocation,
    WeatherTempUnit,
    WeatherWindUnit,
    WeatherPrecipUnit,
    GearColor
}