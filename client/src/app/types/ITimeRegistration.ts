import { IProject } from "./IProject";

export interface ITimeRegistration {
    id: number,
    project: IProject | undefined,
    minutes: number,
    valueDate:Date
}