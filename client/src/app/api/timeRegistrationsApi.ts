import { ITimeRegistration } from "../types/ITimeRegistration";

const BASE_URL = "http://localhost:3001/api";

export interface Query {
    projectId: string
}

export async function get(query: Query) {
    var url = `${BASE_URL}/timeRegistrations`;

    if (query) {
        url += "?";
    }

    if (query.projectId) {
        url += "projectId=" + encodeURIComponent(query.projectId);
    }
    const response = await fetch(url);
    return response.json();
}

export async function save(data: ITimeRegistration) {
    var url = `${BASE_URL}/timeRegistrations`;

    const requestOptions = {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data)
    };

 
    const response = await fetch(url, requestOptions);
    return response.json();
}
