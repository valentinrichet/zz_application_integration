import { ApiCompany } from './api/api-company';

export class Company {
    public readonly id: bigint;
    public readonly mail: string;
    public readonly name: string;
    public readonly address: string;
    public readonly description: string;

    constructor(json: ApiCompany) {
        this.id = json.id;
        this.mail = json.mail;
        this.name = json.name;
        this.address = json.address;
        this.description = json.description;
    }
}