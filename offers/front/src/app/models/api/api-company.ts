export interface ApiCompany {
    id: bigint;
    mail: string;
    name: string;
    address: string;
    description: string;
}

export interface ApiSignInCompany {
    mail: string;
    password: string;
}